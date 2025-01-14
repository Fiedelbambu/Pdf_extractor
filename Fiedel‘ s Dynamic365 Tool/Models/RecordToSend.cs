namespace FiedelsDynamic365Tool.Models
{
    //Datenklasse oder DTO (Data Transfer Object).
    // string? Nullable Reference Types (NRT)
    public class RecordToSend
    {
        public string? Rechnungsdatum { get; set; }
        public string? Servicezeitraum { get; set; }
        public string? Stadt { get; set; }
        public string? Rechnungsnummer { get; set; }
        public string? Kostenstelle { get; set; }
        public string? Strasse { get; set; }
        public string? Postleitzahl { get; set; }
        public string? Firmenname { get; set; } 
    }
}
