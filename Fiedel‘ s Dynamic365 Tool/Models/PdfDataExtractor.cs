using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiedelsDynamic365Tool.Models
{

    public class PdfDataExtractor
    {
        public string? CostCenter { get; set; }
        public string? InvoiceNumber { get; set; }
        public string? InvoiceDate { get; set; }
        public string? ServicePeriod { get; set; }
        public string? Account { get; set; }
        public string? StreetAddress { get; set; }
        public string? City { get; set; }
        public string? StateCode { get; set; }
               
        public PdfDataExtractor()
        {
            City = string.Empty;  
            StateCode = string.Empty;
        }

    }
}
