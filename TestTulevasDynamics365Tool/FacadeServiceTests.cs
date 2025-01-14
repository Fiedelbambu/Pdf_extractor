using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FiedelsDynamic365Tool.Interfaces;
using FiedelsDynamic365Tool.Models;
using FiedelsDynamic365Tool.Service;
using Moq;
using Xunit;

namespace FiedelsDynamic365Tool.Tests
{
    public class FacadeServiceTests
    {
        [Fact]
        public async Task ProcessPdfAndCreateRecordsAsync_ShouldLogInfo_WhenNoPdfFilesFound()
        {
            // Arrange
            var mockPdfExtractor = new Mock<IPdfExtractorService>();
            var mockDataverse = new Mock<IDataverseService>();
            var mockFileReader = new Mock<IFileReader>();
            var mockLogger = new Mock<ILoggerService>();

            mockFileReader
                .Setup(x => x.LoadPdfFilesAndMoveToTemp())
                .Returns(false);

            var facadeService = new FacadeService(mockPdfExtractor.Object, mockDataverse.Object, mockFileReader.Object, mockLogger.Object);

            // Act
            await facadeService.ProcessPdfAndCreateRecordsAsync();

            // Assert
            mockLogger.Verify(x => x.LogInfo("Keine PDF-Dateien gefunden oder Verzeichnis ist ungültig."), Times.Once);
        }

        [Fact]
        public async Task ProcessPdfAndCreateRecordsAsync_ShouldCreateRecords_WhenPdfFilesAreProcessed()
        {
            // Arrange
            var mockPdfExtractor = new Mock<IPdfExtractorService>();
            var mockDataverse = new Mock<IDataverseService>();
            var mockFileReader = new Mock<IFileReader>();
            var mockLogger = new Mock<ILoggerService>();

            var mockPdfData = new List<PdfDataExtractor>
            {
                new PdfDataExtractor { InvoiceNumber = "12345" },
                new PdfDataExtractor { InvoiceNumber = "67890" }
            };

            mockFileReader
                .Setup(x => x.LoadPdfFilesAndMoveToTemp())
                .Returns(true);

            mockFileReader
                .Setup(x => x.PdfFiles)
                .Returns(new List<string> { "file1.pdf", "file2.pdf" });

            mockPdfExtractor
                .Setup(x => x.ExtractDataFromPdfs(It.IsAny<List<string>>()))
                .Returns(mockPdfData);

            var facadeService = new FacadeService(mockPdfExtractor.Object, mockDataverse.Object, mockFileReader.Object, mockLogger.Object);

            // Act
            await facadeService.ProcessPdfAndCreateRecordsAsync();

            // Assert
            foreach (var pdfData in mockPdfData)
            {
                mockDataverse.Verify(x => x.CreateRecordAsync(pdfData), Times.Once);
                mockLogger.Verify(x => x.LogInfo($"Daten für Rechnung '{pdfData.InvoiceNumber}' erfolgreich in Dataverse gespeichert."), Times.Once);
            }
        }

        [Fact]
        public async Task ProcessPdfAndCreateRecordsAsync_ShouldLogWarning_WhenExceptionOccurs()
        {
            // Arrange
            var mockPdfExtractor = new Mock<IPdfExtractorService>();
            var mockDataverse = new Mock<IDataverseService>();
            var mockFileReader = new Mock<IFileReader>();
            var mockLogger = new Mock<ILoggerService>();

            var mockPdfData = new List<PdfDataExtractor>
            {
                new PdfDataExtractor { InvoiceNumber = "12345" }
            };

            mockFileReader
                .Setup(x => x.LoadPdfFilesAndMoveToTemp())
                .Returns(true);

            mockFileReader
                .Setup(x => x.PdfFiles)
                .Returns(new List<string> { "file1.pdf" });

            mockPdfExtractor
                .Setup(x => x.ExtractDataFromPdfs(It.IsAny<List<string>>()))
                .Returns(mockPdfData);

            mockDataverse
                .Setup(x => x.CreateRecordAsync(It.IsAny<PdfDataExtractor>()))
                .ThrowsAsync(new Exception("Test Exception"));

            var facadeService = new FacadeService(mockPdfExtractor.Object, mockDataverse.Object, mockFileReader.Object, mockLogger.Object);

            // Act
            await facadeService.ProcessPdfAndCreateRecordsAsync();

            // Assert
            mockLogger.Verify(x => x.LogWarn(It.Is<string>(msg => msg.Contains("Fehler beim Verarbeiten der Datei '12345': Test Exception"))), Times.Once);
        }
    }
}
