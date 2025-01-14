using System;
using System.Collections.Generic;

namespace WindowsService1.Models
{
    public class ExtractedData
    {
        public string Costcenter { get; set; }
        public string Invoicenumber { get; set; }
        public string Invoicedate { get; set; }
        public string Serviceperiod { get; set; }
        public string Account { get; set; }
        public string StreetAddress { get; set; }
        public List<string> Statecode { get; set; }
        public List<string> City { get; set; }

        public ExtractedData()
        {
            Statecode = new List<string>();
            City = new List<string>();
        }
    }
}
