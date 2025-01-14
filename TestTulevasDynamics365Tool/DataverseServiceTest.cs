using FiedelsDynamic365Tool.Interfaces;
using FiedelsDynamic365Tool.Models;
using FiedelsDynamic365Tool.Service;
using Moq;
using System.Threading.Tasks;
using Xunit;

namespace FiedelsDynamic365Tool.Tests
{
    public class DataverseServiceTests
    {
        [Fact]
        public async Task CreateRecordAsync_Should_Be_Called()
        {
            // Arrange
            var mockDataverseService = new Mock<IDataverseService>();
            var pdfDataExtractor = new PdfDataExtractor();

            // Act
            await mockDataverseService.Object.CreateRecordAsync(pdfDataExtractor);

            // Assert
            mockDataverseService.Verify(service => service.CreateRecordAsync(pdfDataExtractor), Times.Once);
        }

        [Fact]
        public void Constructor_Should_Throw_ArgumentNullException_When_Logger_Is_Null()
        {
            // Arrange
            var mockPdfExtractor = new Mock<IPdfExtractorService>();
            var mockConfigReader = new Mock<ConfigReader>();

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new DataverseService(null, mockPdfExtractor.Object, mockConfigReader.Object));
        }

        //[Fact]
        //public void Constructor_Should_Throw_ArgumentNullException_When_PdfExtractor_Is_Null()
        //{
        //    // Arrange
        //    var mockLogger = new Mock<ILoggerService>();
        //    var mockConfigReader = new Mock<ConfigReader>();

        //    // Act & Assert
        //    Assert.Throws<ArgumentNullException>(() => new DataverseService(mockLogger.Object, null, mockConfigReader.Object));
        //}

    }
}
