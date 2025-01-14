namespace ReadInvoice
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

        public List<string> PdfFiles { get; private set; }

        public FolderPath(string pdfPath = null)
        {
            PdfPath = pdfPath ?? DefaultPdfPath;
            PdfFiles = new List<string>();  // Initialisiere die Liste hier
        }

        public bool LoadPdfFilesAndMoveToTemp()
        {
            if (Directory.Exists(PdfPath))
            {
                var files = Directory.GetFiles(PdfPath, "*.pdf", SearchOption.AllDirectories);

                string tempFolder = @"C:\Temp";

                var movedFiles = new List<string>();  // Temporäre Liste für verschobene Dateien

                foreach (var file in files)
                {
                    string tempFilePath = Path.Combine(tempFolder, Path.GetFileName(file));

                    try
                    {
                        File.Move(file, tempFilePath);
                        movedFiles.Add(tempFilePath);  // Datei in die temporäre Liste hinzufügen
                        Console.WriteLine($"Datei verschoben: {file} -> {tempFilePath}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Fehler beim Verschieben der Datei '{file}': {ex.Message}");
                    }
                }

                if (movedFiles.Count > 0)
                {
                    PdfFiles = movedFiles;  // Liste erst nach dem Verschieben zuweisen
                    return true;
                }
                else
                {
                   Console.WriteLine("Keine PDF-Dateien im angegebenen Verzeichnis gefunden.");
                    return false;
                }
            }
            else
            {
                Console.WriteLine($"Das angegebene Verzeichnis '{PdfPath}' existiert nicht.");
                return false;
            }
        }
    }
}
