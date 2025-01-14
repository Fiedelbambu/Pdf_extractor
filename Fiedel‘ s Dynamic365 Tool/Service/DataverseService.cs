using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk;
using Microsoft.PowerPlatform.Dataverse.Client;
using Microsoft.Xrm.Sdk.Query;
using Newtonsoft.Json;
using FiedelsDynamic365Tool.Interfaces;
using FiedelsDynamic365Tool.Models;

namespace FiedelsDynamic365Tool.Service
{
    /// <summary>
    /// Stellt eine Service-Klasse zur Verwaltung der Interaktion mit Microsoft Dataverse bereit.
    /// Diese Klasse unterstützt das Erstellen von Datensätzen und das Abrufen von Verbindungszeichenfolgen.
    /// </summary>
    public class DataverseService : IDataverseService
    {
        private readonly string _connectionString;
        private readonly ILoggerService _logger;
        private readonly IPdfExtractorService _pdfExtractor;

        /// <summary>
        /// Konstruktor, um das Singleton-Muster zu implementieren und die notwendigen Abhängigkeiten zu injizieren.
        /// </summary>
        public DataverseService(ILoggerService logger, IPdfExtractorService pdfExtractor, ConfigReader configReader)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _pdfExtractor = pdfExtractor ?? throw new ArgumentNullException(nameof(pdfExtractor));
            _connectionString = RetrieveAndDecryptConnectionString(configReader);
        }

        /// <summary>
        /// Liest die verschlüsselte Verbindungszeichenfolge aus einer Datei und entschlüsselt sie.
        /// </summary>
        private string RetrieveAndDecryptConnectionString(ConfigReader configReader)
        {
            try
            {
                string encryptedFilePath = configReader.EncryptionData;

                if (!File.Exists(encryptedFilePath))
                {
                    throw new FileNotFoundException("Die verschlüsselte Datei wurde nicht gefunden.", encryptedFilePath);
                }

                byte[] encryptedBytes = File.ReadAllBytes(encryptedFilePath);
                byte[] decryptedBytes = ProtectedData.Unprotect(encryptedBytes, null, DataProtectionScope.CurrentUser);
                return Encoding.UTF8.GetString(decryptedBytes);
            }
            catch (IOException ex)
            {
                throw new Exception("Fehler beim Lesen der Datei.", ex);
            }
            catch (CryptographicException ex)
            {
                throw new Exception("Fehler bei der Entschlüsselung der Daten.", ex);
            }
        }
        /// <summary>
        /// Erstellt einen neuen Datensatz in Dataverse basierend auf den extrahierten PDF-Daten.
        /// </summary>
        public async Task CreateRecordAsync(PdfDataExtractor extractedData)
        {
            var record = new RecordToSend
            {
                Rechnungsdatum = extractedData.InvoiceDate,
                Servicezeitraum = extractedData.ServicePeriod,
                Stadt = extractedData.City,
                Rechnungsnummer = extractedData.InvoiceNumber,
                Kostenstelle = extractedData.CostCenter,
                Strasse = extractedData.StreetAddress,
                Postleitzahl = extractedData.StateCode,
                Firmenname = extractedData.Account
            };

            using (var serviceClient = new ServiceClient(_connectionString))
            {
                if (!serviceClient.IsReady)
                {
                    throw new InvalidOperationException("Die Verbindung zu Dataverse konnte nicht hergestellt werden.");
                }

                string tableName = "tu_briefskopf";

                try
                {
                    var entity = new Entity(tableName)
                    {
                        ["tu_firmenname"] = record.Firmenname,
                        ["tu_rechnungsdatum"] = record.Rechnungsdatum,
                        ["tu_servicezeitraum"] = record.Servicezeitraum,
                        ["tu_stadt"] = record.Stadt,
                        ["tu_rechnungsnummer"] = record.Rechnungsnummer,
                        ["tu_kostenstelle"] = record.Kostenstelle,
                        ["tu_strasse"] = record.Strasse,
                        ["tu_postleitzahl"] = record.Postleitzahl
                    };

                    Guid recordId = await serviceClient.CreateAsync(entity);
                   Console.WriteLine($"Datensatz erfolgreich erstellt. ID: {recordId}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Fehler beim Erstellen des Datensatzes: {ex.Message}");
                    _logger.LogWarn($"Fehler beim Erstellen des Datensatzes: {ex.Message}");
                    throw;
                }
            }
        }
    }
}
