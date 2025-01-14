using System.Collections.Generic;
using FiedelsDynamic365Tool.Models;

namespace FiedelsDynamic365Tool.Interfaces
{
    public interface IPdfExtractorService
    {
        string LoadPdfContent(string pdfFile);
        List<PdfDataExtractor> ExtractDataFromPdfs(List<string> pdfFiles);
        void FailedProcessesFile(string filePath, string failedPath);
        void ArchiveProcessedFile(string filePath, string archivePath);
    }
}
