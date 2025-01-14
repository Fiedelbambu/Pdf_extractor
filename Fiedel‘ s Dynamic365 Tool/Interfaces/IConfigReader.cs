using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiedelsDynamic365Tool.Interfaces
{
    public interface IConfigReader
    {
        string? DefaultPdfPath { get; }
        string? TempFolder { get; }
        string? FailedFolder { get; }
        string? ArchiveFolder { get; }
        string? ApplicationLog { get; }
        string? EncryptionData { get; }
    }
}
