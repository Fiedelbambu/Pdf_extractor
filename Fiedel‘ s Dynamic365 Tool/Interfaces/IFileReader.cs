using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiedelsDynamic365Tool.Interfaces
{
    public interface IFileReader
    {
        bool LoadPdfFilesAndMoveToTemp(); 
        List<string> PdfFiles { get; } 
        string PdfPath { get; } 
        string TempFolder { get; } 
        string ArchiveFolder { get; } 
        string FailedFolder { get; } 
    }
}
