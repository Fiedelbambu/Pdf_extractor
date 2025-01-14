using System;
using System.Collections.Generic;
using System.IO;

namespace PdfExtractor
{
    public class FolderPath
    {
        private const string DefaultPdfPath = @"C:\dev_christian\";  // Standardpfad für PDFs
        private string _pdfPath;

        public string PdfPath  // Property für den PDF-Pfad
        {
            get { return _pdfPath; }
            set { _pdfPath = value; }
        }

        public List<string> PdfFiles { get; private set; } = new List<string>();

        public FolderPath(string pdfPath = null)
        {
            _pdfPath = pdfPath ?? DefaultPdfPath;
        }

        public bool LoadPdfFiles()  
        {
            if (Directory.Exists(PdfPath))
            {
                var files = Directory.GetFiles(PdfPath, "*.pdf", SearchOption.AllDirectories);
                PdfFiles.Clear(); 
                PdfFiles.AddRange(files);
                return PdfFiles.Count > 0;
            }
            else
            {
                Console.WriteLine("Das angegebene Verzeichnis existiert nicht.");
                return false;
            }
        }
    }
}
