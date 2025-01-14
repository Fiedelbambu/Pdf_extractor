using System;
using System.IO;
using System.Xml;

namespace FiedelsDynamic365Tool.Models
{
    public class ConfigReader
    {       

        public string? DefaultPdfPath { get; private set; }
        public string? TempFolder { get; private set; }
        public string? FailedFolder { get; private set; }
        public string? ArchiveFolder { get; private set; }
        public string? ApplicationLog { get; private set; }
        public string? EncryptionData { get; private set; }

        private readonly string[] defaultPaths =
        {
            @"C:\InProgress\",
            @"C:\Temp",
            @"C:\Failed",
            @"C:\Archive"
        };

        public ConfigReader(string configFilePath) => LoadPathsFromXmlConfig(configFilePath);

        // Lädt die Pfade aus der XML-Konfigurationsdatei
        private void LoadPathsFromXmlConfig(string configFilePath)
        {
            if (File.Exists(configFilePath))
            {
                var doc = new XmlDocument();
                doc.Load(configFilePath);

                DefaultPdfPath = GetXmlElementValue(doc, "DefaultPdfPath");
                TempFolder = GetXmlElementValue(doc, "TempFolder");
                FailedFolder = GetXmlElementValue(doc, "FailedFolder");
                ArchiveFolder = GetXmlElementValue(doc, "ArchiveFolder");
                ApplicationLog = GetXmlElementValue(doc, "ApplicationLog");
                EncryptionData = GetXmlElementValue(doc, "EncryptionData");
            }

            SetDefaultPathsIfNeeded();
        }

        private string? GetXmlElementValue(XmlDocument doc, string elementName)
        {
            var element = doc.SelectSingleNode($"/Config/{elementName}");
            return element?.InnerText.Trim();
        }

        // Setzt Standardpfade, wenn sie nicht festgelegt sind oder nicht existieren
        private void SetDefaultPathsIfNeeded()
        {
            DefaultPdfPath ??= defaultPaths[0];
            TempFolder ??= defaultPaths[1];
            FailedFolder ??= defaultPaths[2];
            ArchiveFolder ??= defaultPaths[3];

            DefaultPdfPath = Directory.Exists(DefaultPdfPath) ? DefaultPdfPath : defaultPaths[0];
            TempFolder = Directory.Exists(TempFolder) ? TempFolder : defaultPaths[1];
            FailedFolder = Directory.Exists(FailedFolder) ? FailedFolder : defaultPaths[2];
            ArchiveFolder = Directory.Exists(ArchiveFolder) ? ArchiveFolder : defaultPaths[3];
        }
    }
}
