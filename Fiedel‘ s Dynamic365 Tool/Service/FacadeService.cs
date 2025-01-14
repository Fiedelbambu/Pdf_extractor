using FiedelsDynamic365Tool.Models;
using FiedelsDynamic365Tool.Interfaces;
using System;
using System.Threading.Tasks;

namespace FiedelsDynamic365Tool.Service
{
    public class FacadeService : IFacadeService
    {
        // Schnittstellen für PDF-Datenextraktion, Dataverse, Dateioperationen und Logging
        private readonly IPdfExtractorService pdfDataExtractor;
        private readonly IDataverseService dataverseService;
        private readonly IFileReader folderPath;
        private readonly ILoggerService logger;

        // Initialisiert den FacadeService mit den notwendigen Abhängigkeiten
        public FacadeService(
            IPdfExtractorService pdfDataExtractor,
            IDataverseService dataverseService,
            IFileReader folderPath,
            ILoggerService logger)
        {
            this.pdfDataExtractor = pdfDataExtractor;
            this.dataverseService = dataverseService;
            this.folderPath = folderPath;
            this.logger = logger;
        }

        // Lädt PDF-Dateien, extrahiert Daten und erstellt Dataverse-Datensätze
        public async Task ProcessPdfAndCreateRecordsAsync()
        {                  

            if (!folderPath.LoadPdfFilesAndMoveToTemp())
            {
                logger.LogInfo("Keine PDF-Dateien gefunden oder Verzeichnis ist ungültig.");
                return;
            }

            var extractedDataList = pdfDataExtractor.ExtractDataFromPdfs(folderPath.PdfFiles);

            // Erstellt Datensätze für jede Rechnung in Dataverse und protokolliert den Fortschritt
            foreach (var pdfData in extractedDataList)
            {
                try
                {                    
                    await dataverseService.CreateRecordAsync(pdfData);
                   
                    logger.LogInfo($"Daten für Rechnung '{pdfData.InvoiceNumber}' erfolgreich in Dataverse gespeichert.");
                }
                catch (Exception ex)
                {
                    logger.LogWarn($"Fehler beim Verarbeiten der Datei '{pdfData.InvoiceNumber}': {ex.Message}");
                }
            }
        }
    }
}
