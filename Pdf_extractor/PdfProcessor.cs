using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using PdfExtractor; // Namespace importieren

namespace PdfExtractor
{
    public class PdfProcessor
    {
        public ExtractedData ProcessPdf(string pdfPath)
        {
            var extractedData = new ExtractedData
            {
                Nummer = null,
                Datum = null,
                Kunde = null,
                Konto = null,
                Rechnung = null,
                KostenstelleIntern = null,
                Rechnungsnummer = null,
                Rechnungsdatum = null,
                Leistungszeitraum = null,
                FirmaMitStrasse = null,
                PostleitzahlMitOrt = new List<string>(),
                City = new List<string>() // Initialisierung von City
            };

            try
            {
                using (var pdfReader = new PdfReader(pdfPath))
                using (var pdfDocument = new PdfDocument(pdfReader))
                {
                    for (int i = 1; i <= pdfDocument.GetNumberOfPages(); i++)
                    {
                        var page = pdfDocument.GetPage(i);
                        var strategy = new LocationTextExtractionStrategy();
                        var text = PdfTextExtractor.GetTextFromPage(page, strategy);
                        extractedData = ExtractDataFromText(text, extractedData);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Fehler beim Verarbeiten der Datei {pdfPath}: {ex.Message}");
            }

            return extractedData;
        }

        public ExtractedData ExtractDataFromText(string text, ExtractedData extractedData)
        {
            var searchPatterns = new List<(string Label, string Pattern)>
        {
            ("Nummer", @"Nummer\s+(\d+)"),
            ("Datum", @"Datum\s+(\d{2}\.\d{2}\.\d{4})"),
            ("Kunde", @"Kunde\s+(\d+)"),
            ("Konto", @"Konto\s+(\d+)"),
            ("Rechnung", @"Rechnung\s*(\d*)"),
            ("KostenstelleIntern", @"Kostenstelle\s+intern:\s*(\d+)"),
            ("Rechnungsnummer", @"Rechnungsnummer:\s*(\d+)"),
            ("Rechnungsdatum", @"Rechnungsdatum:\s*(\d{2}\.\d{2}\.\d{4})"),
            ("Leistungszeitraum", @"Leistungszeitraum:\s*(\d{2}\.\d{2}\.\d{2,4})\s*–\s*(\d{2}\.\d{2}\.\d{2,4})")
        };

            foreach (var (label, pattern) in searchPatterns)
            {
                var match = Regex.Match(text, pattern);
                if (match.Success)
                {
                    if (label == "Leistungszeitraum")
                    {
                        string startDate = match.Groups[1].Value;
                        string endDate = match.Groups[2].Value;
                        extractedData.Leistungszeitraum = $"{startDate} bis {endDate}";
                    }
                    else
                    {
                        string extractedValue = match.Groups[1].Value;
                        extractedData = UpdateExtractedData(extractedData, label, extractedValue);
                    }
                }
            }

            string extractAfterCity = ExtractCityDetails(text);
            if (extractAfterCity != null)
            {
                extractedData.FirmaMitStrasse = extractAfterCity;
            }

            // Postleitzahlen extrahieren
            var postalCodes = ExtractPostalCodesOrCities(text, extractCity: false);
            if (postalCodes.Count > 0)
            {
                extractedData.PostleitzahlMitOrt.AddRange(postalCodes);
            }

            // Städte extrahieren
            var cities = ExtractPostalCodesOrCities(text, extractCity: true);
            if (cities.Count > 0)
            {
                extractedData.City.AddRange(cities);
            }

            return extractedData;
        }

        private ExtractedData UpdateExtractedData(ExtractedData data, string label, string value)
        {
            switch (label)
            {
                case "Nummer":
                    data.Nummer = value;
                    break;
                case "Datum":
                    data.Datum = value;
                    break;
                case "Kunde":
                    data.Kunde = value;
                    break;
                case "Konto":
                    data.Konto = value;
                    break;
                case "Rechnung":
                    data.Rechnung = value;
                    break;
                case "KostenstelleIntern":
                    data.KostenstelleIntern = value;
                    break;
                case "Rechnungsnummer":
                    data.Rechnungsnummer = value;
                    break;
                case "Rechnungsdatum":
                    data.Rechnungsdatum = value;
                    break;
            }

            return data;
        }

        private string ExtractCityDetails(string text)
        {
            var pattern = @"Hamburg\s*([\s\S]*?)(?=\s*Rechnung|\z)";
            var match = Regex.Match(text, pattern, RegexOptions.IgnoreCase);

            if (match.Success)
            {
                string extractedText = match.Groups[1].Value.Trim();
                return Regex.Replace(extractedText, @"\s*\n\s*", "\n").Trim();
            }
            return null;
        }

        private List<string> ExtractPostalCodesOrCities(string text, bool extractCity)
        {
            var pattern = @"\b(\d{5})\s+([A-Za-zÄÖÜäöüß\s]+)";
            List<string> results = new List<string>();
            string relevantSection = ExtractRelevantSection(text, "Kostenstelle intern", "Rechnungsnummer");

            var matches = Regex.Matches(relevantSection, pattern);

            foreach (Match match in matches)
            {
                if (match.Success)
                {
                    string extractedData = extractCity ? match.Groups[2].Value.Trim() : match.Groups[1].Value.Trim();
                    if (!string.IsNullOrEmpty(extractedData))
                    {
                        results.Add(extractedData);
                    }
                }
            }

            return results;
        }

        private string ExtractRelevantSection(string text, string startKeyword, string endKeyword)
        {
            int startIndex = text.IndexOf(startKeyword);
            if (startIndex == -1) return string.Empty;

            int endIndex = text.IndexOf(endKeyword, startIndex);
            if (endIndex == -1) return string.Empty;

            startIndex += startKeyword.Length;
            return text.Substring(startIndex, endIndex - startIndex).Trim();
        }
    }
}