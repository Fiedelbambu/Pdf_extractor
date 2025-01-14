using FiedelsDynamic365Tool.Interfaces;
using FiedelsDynamic365Tool.Service;
using Moq;
using Xunit;
using System;
using System.Collections.Generic;

namespace TestTulevasDynamics365Tool
{
    public class PdfExtractorServiceTest
    {
        private readonly Mock<ILoggerService> _mockLogger;
        private readonly Mock<IFileReader> _mockFileReader;
        private readonly PdfExtractorService _pdfExtractorService;

        // Konstruktor als Setup
        public PdfExtractorServiceTest()
        {
            _mockLogger = new Mock<ILoggerService>();
            _mockFileReader = new Mock<IFileReader>();
            _pdfExtractorService = new PdfExtractorService(_mockLogger.Object, _mockFileReader.Object);
        }

        [Fact]
        public void LoadPdfContent_ShouldReturnContent_WhenPdfIsValid()
        {
            // Arrange
            var pdfFilePath = "test.pdf"; // Beispielpfad, kann durch tatsächlichen Test-PDF-Pfad ersetzt werden
            _mockFileReader.Setup(fr => fr.FailedFolder).Returns(@"C:\Failed");

            // Act
            var result = _pdfExtractorService.LoadPdfContent(pdfFilePath);

            // Assert
            Assert.NotNull(result); // Überprüfen, dass Inhalt zurückgegeben wird
        }

        //[Fact]
        //public void ExtractDataFromPdfs_ShouldLogInfo_WhenDuplicateFile()
        //{
        //    // Arrange
        //    var pdfFileList = new List<string> { "duplicateTest.pdf" };
        //    _mockFileReader.Setup(fr => fr.ArchiveFolder).Returns(@"C:\Archive");

        //    // Act
        //    _pdfExtractorService.ExtractDataFromPdfs(pdfFileList);

        //    // Assert
        //    _mockLogger.Verify(logger => logger.LogInfo(It.IsAny<string>()), Times.AtLeastOnce);
        //}

        [Fact]
        public void FailedProcessesFile_ShouldLogWarning_WhenFileMoveFails()
        {
            // Arrange
            var filePath = "nonExistentFile.pdf";
            var failedPath = @"C:\Failed";

            // Act
            _pdfExtractorService.FailedProcessesFile(filePath, failedPath);

            // Assert
            _mockLogger.Verify(logger => logger.LogWarn(It.IsAny<string>()), Times.Once);
        }
    }
}
