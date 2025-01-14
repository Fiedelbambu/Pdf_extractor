using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using FiedelsDynamic365Tool.Models;
using System.Text;
using FiedelsDynamic365Tool.Interfaces;

namespace FiedelsDynamic365Tool.Service
{
    public class PdfExtractorService : IPdfExtractorService
    {
        private readonly ILoggerService _logger;
        private readonly IFileReader _fileReader;

        // Privater Konstruktor, um externe Instanziierung zu verhindern
        public PdfExtractorService(ILoggerService logger, IFileReader fileReader)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _fileReader = fileReader ?? throw new ArgumentNullException(nameof(fileReader));
        }

        // Lädt den gesamten Textinhalt aus einer PDF-Datei
        public string LoadPdfContent(string pdfFile)
        {
            try
            {
                using (var pdfReader = new PdfReader(pdfFile))
                using (var pdfDocument = new PdfDocument(pdfReader))
                {
                    StringBuilder content = new StringBuilder();
                    for (int pageIndex = 1; pageIndex <= pdfDocument.GetNumberOfPages(); pageIndex++)
                    {
                        var page = pdfDocument.GetPage(pageIndex);
                        var textExtractionStrategy = new LocationTextExtractionStrategy();
                        var pageText = PdfTextExtractor.GetTextFromPage(page, textExtractionStrategy);
                        content.Append(pageText);
                    }
                    return content.ToString();
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarn($"Fehler beim Laden des PDF-Inhalts: {ex.Message}");
                return string.Empty;
            }
        }
        // Extrahiert relevante Daten aus einer Liste von PDF-Dateien
        public List<PdfDataExtractor> ExtractDataFromPdfs(List<string> pdfFiles)
        {
            var extractedDataList = new List<PdfDataExtractor>();
            var archivePath = CreateArchiveDirectory();
            var failedPath = _fileReader.FailedFolder;

            foreach (var pdfFilePath in pdfFiles)
            {
                try
                {                   
                    if (IsDuplicateFile(pdfFilePath, archivePath))
                    {
                        _logger.LogInfo($"Datei '{pdfFilePath}' ist ein Duplikat und wird in den Failed-Ordner verschoben.");
                        FailedProcessesFile(pdfFilePath, failedPath);
                        continue;
                    }

                    using (var pdfReader = new PdfReader(pdfFilePath))
                    using (var pdfDocument = new PdfDocument(pdfReader))
                    {
                        var extractedData = new PdfDataExtractor();

                        for (int pageIndex = 1; pageIndex <= pdfDocument.GetNumberOfPages(); pageIndex++)
                        {
                            var page = pdfDocument.GetPage(pageIndex);
                            var textExtractionStrategy = new LocationTextExtractionStrategy();
                            var pageText = PdfTextExtractor.GetTextFromPage(page, textExtractionStrategy);

                            ExtractDataFromPage(pageText, extractedData);
                            var (companyName, streetAddress) = ExtractCityDetails(pageText);

                            if (!string.IsNullOrEmpty(companyName))
                            {
                                extractedData.Account = companyName;
                            }
                            if (!string.IsNullOrEmpty(streetAddress))
                            {
                                extractedData.StreetAddress = streetAddress;
                            }

                            var (postalCodes, cities) = ExtractPostalCodesAndCities(pageText);

                            // Hier den ersten Eintrag oder eine Standardzuweisung verwenden
                            extractedData.StateCode = postalCodes.FirstOrDefault();
                            extractedData.City = cities.FirstOrDefault(); 
                        }

                        extractedDataList.Add(extractedData);
                    }

                    ArchiveProcessedFile(pdfFilePath, archivePath);
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Fehler beim Erstellen des Datensatzes: {ex.Message}");
                    FailedProcessesFile(pdfFilePath, failedPath);
                }
            }

            return extractedDataList;
        }

        // Erstellt ein Verzeichnis zum Archivieren verarbeiteter Dateien
        private string CreateArchiveDirectory()
        {
            string archiveRoot = _fileReader.ArchiveFolder;
            string dateFolder = DateTime.Now.ToString("yyyy-MM-dd");
            string archivePath = Path.Combine(archiveRoot, dateFolder);

            if (!Directory.Exists(archivePath))
            {
                Directory.CreateDirectory(archivePath);
            }
            return archivePath;
        }

        // Verschiebt eine fehlerhafte Datei in den "Failed"-Ordner und protokolliert dies
        public void FailedProcessesFile(string filePath, string failedPath)
        {
            string fileName = Path.GetFileName(filePath);
            string destinationPath = Path.Combine(failedPath, fileName);

            try
            {
                if (!Directory.Exists(failedPath))
                {
                    Directory.CreateDirectory(failedPath);
                }

                File.Move(filePath, destinationPath);
                _logger?.LogWarn($"Fehlerhafte Datei verschoben: {filePath} -> {destinationPath}");
            }
            catch (Exception ex)
            {
                _logger?.LogWarn($"Fehler beim Verschieben der Datei '{filePath}': {ex.Message}");
            }
        }
       
        // Prüft auf doppelte Dateien anhand des Dateipfads
        public void ArchiveProcessedFile(string filePath, string archivePath)
        {
            try
            {
                string fileName = Path.GetFileName(filePath);
                string destinationPath = Path.Combine(archivePath, fileName);
                File.Move(filePath, destinationPath);
            }
            catch (Exception e)
            {
                _logger?.LogWarn($"Fehler beim Verschieben der Datei '{filePath}': {e.Message}");
                FailedProcessesFile(filePath, _fileReader.FailedFolder);
            }
        }
        
        // Extrahiert PLZ und Stadt aus dem Textabschnitt
        private static void ExtractDataFromPage(string pageText, PdfDataExtractor extractedData)
        {
            foreach (var (label, pattern) in SearchPatterns)
            {
                var match = pattern.Match(pageText);
                if (match.Success)
                {
                    string extractedValue = label == "Leistungszeitraum"
                        ? $"{match.Groups[1].Value} bis {match.Groups[2].Value}"                        
                        : match.Groups[1].Value;

                    UpdateExtractedData(extractedData, label, extractedValue);
                }
            }
        }

        // Analysiert und speichert relevante Daten der PDF-Seite in `extractedData`
        private static void UpdateExtractedData(PdfDataExtractor data, string label, string value1)
        {
            switch (label)
            {
                // Die Textausgabe muss getrennt sein daher habe ich das hier heraus genommen und schreibe eine neue Methode dafür noch fertig
                case "KostenstelleIntern":                    
                    data.CostCenter = value1;
                    break;
                case "Rechnungsnummer":                                      
                    data.InvoiceNumber = value1;
                    break;
                case "Rechnungsdatum":                 
                    data.InvoiceDate = value1;
                    break;
                case "Leistungszeitraum":                   
                    data.ServicePeriod = value1;
                    break;
                default:                   
                    break;
            }
        }

        // Verschiebt verarbeitete Dateien ins Archiv
        private static (string CompanyName, string Address) ExtractCityDetails(string text)
        {
            var pattern = new Regex(@"Hamburg\s*([\s\S]*?)(?=\s*Rechnung|\z)", RegexOptions.IgnoreCase);
            var match = pattern.Match(text);

            if (match.Success)
            {
                var extractedText = match.Groups[1].Value.Trim();
                extractedText = Regex.Replace(extractedText, @"\s*\n\s*", "\n").Trim();

                var lines = extractedText.Split(new[] { '\n' }, 2);
                string? companyName = lines.Length > 0 ? lines[0].Trim() : null;
                string? address = lines.Length > 1 ? lines[1].Trim() : null;

                return (companyName, address);
            }

            return (null,null);
        }

        // Koordiniert die Datenextraktion für eine Liste von PDFs
        private static (List<string> PostalCodes, List<string> Cities) ExtractPostalCodesAndCities(string text)
        {
            var postalCodePattern = new Regex(@"\b(\d{5})\s+([A-Za-zÄÖÜäöüß\s]+)");
            var postalCodes = new List<string>();
            var cities = new List<string>();
            string relevantSection = ExtractRelevantSection(text, "Kostenstelle intern", "Rechnungsnummer");

            var matches = postalCodePattern.Matches(relevantSection);
            foreach (Match match in matches)
            {
                if (match.Success)
                {
                    string postalCode = match.Groups[1].Value.Trim();
                    string city = match.Groups[2].Value.Trim();

                    if (!string.IsNullOrEmpty(postalCode))
                    {
                        postalCodes.Add(postalCode);
                    }

                    if (!string.IsNullOrEmpty(city))
                    {
                        cities.Add(city);
                    }
                }
            }
            return (postalCodes, cities);
        }

        // Definiert eine Liste von Suchmustern für die Extraktion relevanter PDF-Daten
        private static readonly List<(string Label, Regex Pattern)> SearchPatterns = new List<(string Label, Regex Pattern)>
        {
            ("Nummer", new Regex(@"Nummer\s+(\d+)")),
            ("Datum", new Regex(@"Datum\s+(\d{2}\.\d{2}\.\d{4})")),
            ("Kunde", new Regex(@"Kunde\s+(\d+)")),
            ("Konto", new Regex(@"Konto\s+(\d+)")),
            ("Rechnung", new Regex(@"Rechnung\s*(\d*)")),
            ("KostenstelleIntern", new Regex(@"Kostenstelle\s+intern:\s*(\d+)")),
            ("Rechnungsnummer", new Regex(@"Rechnungsnummer:\s*(\d+)")),
            ("Rechnungsdatum", new Regex(@"Rechnungsdatum:\s*(\d{2}\.\d{2}\.\d{4})")),
            ("Leistungszeitraum", new Regex(@"Leistungszeitraum:\s*(\d{2}\.\d{2}\.\d{2,4})\s*–\s*(\d{2}\.\d{2}\.\d{2,4})"))
        };

        // Extrahiert einen bestimmten Abschnitt aus dem Text zwischen den gegebenen Schlüsselwörtern
        private static string ExtractRelevantSection(string text, string startKeyword, string endKeyword)
        {
            int startIndex = text.IndexOf(startKeyword, StringComparison.Ordinal);
            if (startIndex == -1) return string.Empty;

            int endIndex = text.IndexOf(endKeyword, startIndex, StringComparison.Ordinal);
            if (endIndex == -1) return string.Empty;

            startIndex += startKeyword.Length;
            return text.Substring(startIndex, endIndex - startIndex).Trim();
        }
        // Prüft, ob eine Datei bereits im Archivverzeichnis existiert, um Duplikate zu vermeiden
        private static bool IsDuplicateFile(string filePath, string archivePath)
        {
            string fileName = Path.GetFileName(filePath);
            string destinationPath = Path.Combine(archivePath, fileName);
            
            return File.Exists(destinationPath);
        }
    }
}
