using FiedelsDynamic365Tool.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiedelsDynamic365Tool.Interfaces
{
    public interface IDataverseService
    {        
        Task CreateRecordAsync(PdfDataExtractor extractedData);
    }

}
