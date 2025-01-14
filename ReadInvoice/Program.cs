using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace ReadInvoice
{
    class Program
    {
        static void Main()
        {
            var folderPath = new FolderPath();
            if (!folderPath.LoadPdfFilesAndMoveToTemp())
            {
                Console.WriteLine("Keine PDF-Dateien gefunden oder Verzeichnis ist ungültig.");
                return;
            }

            var extractedDataList = new List<ExtractedData>();
            var archivePath = CreateArchiveDirectory();

            foreach (var pdfFilePath in folderPath.PdfFiles)
            {
                try
                {
                    using (var pdfReader = new PdfReader(pdfFilePath))
                    using (var pdfDocument = new PdfDocument(pdfReader))
                    {
                        var extractedData = new ExtractedData();

                        for (int pageIndex = 1; pageIndex <= pdfDocument.GetNumberOfPages(); pageIndex++)
                        {
                            var page = pdfDocument.GetPage(pageIndex);
                            var textExtractionStrategy = new LocationTextExtractionStrategy();
                            var pageText = PdfTextExtractor.GetTextFromPage(page, textExtractionStrategy);

                            ExtractDataFromPage(pageText, extractedData);

                            // Firma- und Adressdetails extrahieren
                            var (companyName, streetAddress) = ExtractCityDetails(pageText);
                            if (!string.IsNullOrEmpty(companyName))
                            {
                                extractedData.Account = companyName;
                            }
                            if (!string.IsNullOrEmpty(streetAddress))
                            {
                                extractedData.StreetAddress = streetAddress;
                            }

                            // Postleitzahlen und Städte extrahieren
                            var (postalCodes, cities) = ExtractPostalCodesAndCities(pageText);

                            if (postalCodes.Count > 0)
                            {
                                extractedData.Statecode.AddRange(postalCodes);
                            }

                            if (cities.Count > 0)
                            {
                                extractedData.City.AddRange(cities);
                            }
                        }

                        extractedDataList.Add(extractedData);                       
                    }
                    
                    ArchiveProcessedFile(pdfFilePath, archivePath);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Fehler beim Verarbeiten der Datei '{pdfFilePath}': {ex.Message}");
                    // Datei bleibt im temporären Ordner, da ein Fehler aufgetreten ist
                }
            }

            PrintExtractedData(extractedDataList);

            Console.WriteLine("Drücken Sie eine beliebige Taste, um das Programm zu beenden...");
            Console.ReadKey();
        }

        private static string CreateArchiveDirectory()
        {
            string archiveRoot = @"C:\Archive";
        //    string archiveRoot = Path.Combine(Directory.GetCurrentDirectory(), "Archive");
            string dateFolder = DateTime.Now.ToString("yyyy-MM-dd");
            string archivePath = Path.Combine(archiveRoot, dateFolder);

            if (!Directory.Exists(archivePath))
            {
                Directory.CreateDirectory(archivePath);
            }
            return archivePath;
        }

        private static void ArchiveProcessedFile(string filePath, string archivePath)
        {
            try
            {
                string fileName = Path.GetFileName(filePath);
                string destinationPath = Path.Combine(archivePath, fileName);

                File.Move(filePath, destinationPath);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Fehler beim Verschieben der Datei '{filePath}': {e.Message}");
            }
        }

        // CustomPatternMatching
        private static void ExtractDataFromPage(string pageText, ExtractedData extractedData)
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

        private static void PrintExtractedData(IEnumerable<ExtractedData> extractedDataList)
        {
            foreach (var data in extractedDataList)
            {
                Console.WriteLine("-------------------------------------------------");
                Console.WriteLine($"Kostenstelle Intern: {data.Costcenter}");
                Console.WriteLine($"Rechnungsnummer: {data.Invoicenumber}");
                Console.WriteLine($"Rechnungsdatum: {data.Invoicedate}");
                Console.WriteLine($"Leistungszeitraum: {data.Serviceperiod}");
                Console.WriteLine($"Firma: {data.Account}");
                Console.WriteLine($"Straße: {data.StreetAddress}");
                Console.WriteLine($"Postleitzahl: {string.Join(", ", data.Statecode)}");
                Console.WriteLine($"Stadt: {string.Join(", ", data.City)}");
            }
        }

        private static void UpdateExtractedData(ExtractedData data, string label, string value)
        {
            switch (label)
            {
                case "KostenstelleIntern":
                    data.Costcenter = value;
                    break;
                case "Rechnungsnummer":
                    data.Invoicenumber = value;
                    break;
                case "Rechnungsdatum":
                    data.Invoicedate = value;
                    break;
                case "Leistungszeitraum":
                    data.Serviceperiod = value;
                    break;
            }
        }

        private static (string CompanyName, string Address) ExtractCityDetails(string text)
        {
            var pattern = new Regex(@"Hamburg\s*([\s\S]*?)(?=\s*Rechnung|\z)", RegexOptions.IgnoreCase);
            var match = pattern.Match(text);

            if (match.Success)
            {
                var extractedText = match.Groups[1].Value.Trim();
                extractedText = Regex.Replace(extractedText, @"\s*\n\s*", "\n").Trim();

                var lines = extractedText.Split(new[] { '\n' }, 2);

                string companyName = lines.Length > 0 ? lines[0].Trim() : null;
                string address = lines.Length > 1 ? lines[1].Trim() : null;

                return (companyName, address);
            }

            return (null, null);
        }

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

        private static string ExtractRelevantSection(string text, string startKeyword, string endKeyword)
        {
            int startIndex = text.IndexOf(startKeyword, StringComparison.Ordinal);
            if (startIndex == -1) return string.Empty;

            int endIndex = text.IndexOf(endKeyword, startIndex, StringComparison.Ordinal);
            if (endIndex == -1) return string.Empty;

            startIndex += startKeyword.Length;
            return text.Substring(startIndex, endIndex - startIndex).Trim();
        }
    }
}
