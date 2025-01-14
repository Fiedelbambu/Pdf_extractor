using System;
using System.Collections.Generic;

namespace ReadInvoice
{
    //Datenklasse (DataObjectClass) um Daten zwischen verschiedenen klassen zu transferier oder man kann auch sagen POCO" (Plain Old CLR Object) Common Language Runtime 
    public class ExtractedData
    {        
        public string Costcenter { get; set; }
        public string Invoicenumber { get; set; }
        public string Invoicedate { get; set; }
        public string Serviceperiod { get; set; }
        public string Account { get; set; } // init-only kann hier nicht verwendet werden
        public string StreetAddress { get; set; }
        public List<string> Statecode { get; init; } = new();
        public List<string> City { get; init; } = new();
        /*
         * init-only Properties: Diese können nur während der Objektinitialisierung
         * gesetzt werden, wodurch sichergestellt wird, dass sie nach der Initialisierung unveränderlich sind.
        */

        public ExtractedData()
        {
            Statecode = new List<string>();
            City = new List<string>();
        }
    }
}
