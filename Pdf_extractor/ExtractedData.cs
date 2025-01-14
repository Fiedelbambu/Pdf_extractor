using System;
using System.Collections.Generic;

namespace PdfExtractor
{
    public class ExtractedData
    {
        public List<string> City { get; set; } // Anpassung von string auf List<string>
        public string Nummer { get; set; }
        public string Datum { get; set; }
        public string Kunde { get; set; }
        public string Konto { get; set; }
        public string Rechnung { get; set; }
        public string KostenstelleIntern { get; set; }
        public string Rechnungsnummer { get; set; }
        public string Rechnungsdatum { get; set; }
        public string Leistungszeitraum { get; set; }
        public string FirmaMitStrasse { get; set; }
        public List<string> PostleitzahlMitOrt { get; set; }

        // Optionale Methode zum Anzeigen der Daten
        public override string ToString()
        {
            return $"Nummer: {Nummer}\nDatum: {Datum}\nKunde: {Kunde}\nKonto: {Konto}\nRechnung: {Rechnung}\nKostenstelleIntern: {KostenstelleIntern}\nRechnungsnummer: {Rechnungsnummer}\nRechnungsdatum: {Rechnungsdatum}\nLeistungszeitraum: {Leistungszeitraum}\nFirmaMitStrasse: {FirmaMitStrasse}\nPostleitzahlMitOrt: {string.Join(", ", PostleitzahlMitOrt)}\nCity: {string.Join(", ", City)}";
        }
    }
}
