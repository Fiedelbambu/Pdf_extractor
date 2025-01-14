using PdfExtractor;


namespace PdfExtractor { 
class Program
{
    static void Main()
    {
        var folderPath = new FolderPath();
        if (!folderPath.LoadPdfFiles())
        {
            Console.WriteLine("Keine PDF-Dateien gefunden oder Verzeichnis existiert nicht.");
            return;
        }

        var pdfProcessor = new PdfProcessor();
        var excelExporter = new ImportExcel();
        string excelFilePath = @"C:\dev_christian\Datenempfangen.xlsx";

        foreach (var pdfPath in folderPath.PdfFiles)
        {
            var extractedData = pdfProcessor.ProcessPdf(pdfPath);
            excelExporter.ExportToExcel(extractedData, excelFilePath);
            Console.WriteLine($"Extrahierte Daten für {pdfPath} wurden in die Excel-Datei geschrieben.");
        }

        Console.WriteLine("Drücken Sie eine beliebige Taste, um das Programm zu beenden...");
        Console.ReadKey();
    }
}
}