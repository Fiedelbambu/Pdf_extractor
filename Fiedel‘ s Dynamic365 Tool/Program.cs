using System;
using System.Threading.Tasks;
using FiedelsDynamic365Tool.Interfaces;
using FiedelsDynamic365Tool.Models;
using FiedelsDynamic365Tool.Service;
using Microsoft.Extensions.DependencyInjection;


namespace FiedelsDynamic365Tool
{   
    class Program
    {                 
        static async Task Main(string[] args)
        {
            var serviceProvider = new ServiceCollection()
    .AddSingleton(new ConfigReader("konfig/Config.xml"))
    .AddSingleton<ILoggerService, LoggerService>()
    .AddSingleton<IFileReader, FileReader>()
    .AddSingleton<IPdfExtractorService, PdfExtractorService>()
    .AddSingleton<IDataverseService>(provider =>
    {
        var logger = provider.GetRequiredService<ILoggerService>();
        var pdfExtractor = provider.GetRequiredService<IPdfExtractorService>();
        var configReader = provider.GetRequiredService<ConfigReader>();

        return new DataverseService(logger, pdfExtractor, configReader);
    })
    .AddSingleton<IFacadeService>(provider =>
    {
        var logger = provider.GetRequiredService<ILoggerService>();
        var pdfExtractor = provider.GetRequiredService<IPdfExtractorService>();
        var dataverseService = provider.GetRequiredService<IDataverseService>();

        return new FacadeService(pdfExtractor, dataverseService, provider.GetRequiredService<IFileReader>(), logger);
    })
    .BuildServiceProvider();

            var facadeService = serviceProvider.GetService<IFacadeService>();
                        
            if (facadeService == null)
            {
                Console.WriteLine("Failed to initialize the FacadeService.");
                return;
            }
            try
            {
                /// <summary>
                /// Führt die Verarbeitung der PDF-Dateien durch und erstellt Datensätze in Dataverse.
                /// </summary>
                await facadeService.ProcessPdfAndCreateRecordsAsync();
                Console.WriteLine("Processing completed successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }
    }
}
