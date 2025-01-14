using System;
using System.Collections.Generic;
using System.IO;
using FiedelsDynamic365Tool.Interfaces;

namespace FiedelsDynamic365Tool.Models
{
    public class FileReader : IFileReader
    {
        private readonly ConfigReader _configReader;
        private readonly ILoggerService _logger;

        public string PdfPath => _configReader.DefaultPdfPath;
        public string TempFolder => _configReader.TempFolder;
        public string ArchiveFolder => _configReader.ArchiveFolder;
        public string FailedFolder => _configReader.FailedFolder;
        public List<string> PdfFiles { get; private set; }

        public FileReader(ConfigReader configReader, ILoggerService logger)
        {
            _configReader = configReader ?? throw new ArgumentNullException(nameof(configReader));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            PdfFiles = new List<string>();
        }

        // Methode zum Laden von PDF-Dateien und Verschieben in den Temp-Ordner
        public bool LoadPdfFilesAndMoveToTemp()
        {
            if (Directory.Exists(PdfPath))
            {
                var files = Directory.GetFiles(PdfPath, "*.pdf", SearchOption.AllDirectories);
                var movedFiles = new List<string>();

                foreach (var file in files)
                {
                    string tempFilePath = Path.Combine(TempFolder, Path.GetFileName(file));

                    try
                    {
                        File.Move(file, tempFilePath);
                        movedFiles.Add(tempFilePath);
                        _logger.LogInfo($"Datei verschoben: {file} -> {tempFilePath}");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError($"Fehler beim Verschieben der Datei '{file}': {ex.Message}");
                        MoveToFailedFolder(file);
                    }
                }

                if (movedFiles.Count > 0)
                {
                    PdfFiles = movedFiles;
                    return true;
                }
                else
                {
                    _logger.LogInfo("Keine PDF-Dateien im angegebenen Verzeichnis gefunden.");
                    return false;
                }
            }
            else
            {
                _logger.LogInfo("Keine PDF-Dateien im angegebenen Verzeichnis gefunden.");
                return false;
            }
        }

        // Methode zum Verschieben einer Datei in den Failed-Ordner
        public void MoveToFailedFolder(string filePath)
        {
            try
            {
                if (!Directory.Exists(FailedFolder))
                {
                    Directory.CreateDirectory(FailedFolder);
                }

                string destinationPath = Path.Combine(FailedFolder, Path.GetFileName(filePath));
                File.Move(filePath, destinationPath);
                _logger.LogInfo($"Datei in den Failed-Ordner verschoben: {filePath} -> {destinationPath}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Fehler beim Verschieben der Datei '{filePath}' in den Failed-Ordner: {ex.Message}");
            }
        }
    }
}
