using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsService1.Models
{
    public class FolderPath
    {
        private const string DefaultPdfPath = @"C:\dev_christian\";
        private string _pdfPath;

        public string PdfPath
        {
            get { return _pdfPath; }
            private set { _pdfPath = value; }
        }

        public List<string> PdfFiles { get; private set; } = new List<string>();

        public FolderPath(string pdfPath = null)
        {
            PdfPath = pdfPath ?? DefaultPdfPath;
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
            return false;
        }
    }
}
