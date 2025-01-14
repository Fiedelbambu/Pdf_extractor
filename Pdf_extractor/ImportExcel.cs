using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PdfExtractor
{
    public class ImportExcel
    {
        public void ExportToExcel(ExtractedData data, string filePath)
        {
            // Öffnen der Excel-Datei im Bearbeitungsmodus
            using (SpreadsheetDocument spreadsheet = SpreadsheetDocument.Open(filePath, true))
            {
                WorkbookPart workbookPart = spreadsheet.WorkbookPart;
                WorksheetPart worksheetPart;

                // Prüfen, ob bereits ein Worksheet existiert, wenn nicht, erstelle ein neues
                if (workbookPart.Workbook.Sheets.Count() == 0)
                {
                    worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
                    worksheetPart.Worksheet = new Worksheet(new SheetData());

                    Sheets sheets = workbookPart.Workbook.AppendChild(new Sheets());
                    Sheet sheet = new Sheet()
                    {
                        Id = workbookPart.GetIdOfPart(worksheetPart),
                        SheetId = 1,
                        Name = "Extracted Data"
                    };
                    sheets.Append(sheet);

                    // Hinzufügen der Header-Zeile
                    SheetData sheetData = worksheetPart.Worksheet.GetFirstChild<SheetData>();
                    Row headerRow = new Row();
                    sheetData.AppendChild(headerRow);

                    headerRow.AppendChild(CreateCell("Nummer"));
                    headerRow.AppendChild(CreateCell("Datum"));
                    headerRow.AppendChild(CreateCell("Kunde"));
                    headerRow.AppendChild(CreateCell("Konto"));
                    headerRow.AppendChild(CreateCell("Rechnung"));
                    headerRow.AppendChild(CreateCell("KostenstelleIntern"));
                    headerRow.AppendChild(CreateCell("Rechnungsnummer"));
                    headerRow.AppendChild(CreateCell("Rechnungsdatum"));
                    headerRow.AppendChild(CreateCell("Leistungszeitraum"));
                    headerRow.AppendChild(CreateCell("FirmaMitStrasse"));
                    headerRow.AppendChild(CreateCell("PostleitzahlMitOrt"));
                    headerRow.AppendChild(CreateCell("City")); 

                    workbookPart.Workbook.Save();
                }
                else
                {
                    worksheetPart = workbookPart.WorksheetParts.First();
                }

                SheetData existingSheetData = worksheetPart.Worksheet.GetFirstChild<SheetData>();

                // Bestimmen der nächsten leeren Zeile
                uint rowIndex = (uint)(existingSheetData.Elements<Row>().Count() + 1);

                // Hinzufügen der neuen Datenzeile
                Row dataRow = new Row() { RowIndex = rowIndex };
                existingSheetData.AppendChild(dataRow);

                dataRow.AppendChild(CreateCell(data.Nummer));
                dataRow.AppendChild(CreateCell(data.Datum));
                dataRow.AppendChild(CreateCell(data.Kunde));
                dataRow.AppendChild(CreateCell(data.Konto));
                dataRow.AppendChild(CreateCell(data.Rechnung));
                dataRow.AppendChild(CreateCell(data.KostenstelleIntern));
                dataRow.AppendChild(CreateCell(data.Rechnungsnummer));
                dataRow.AppendChild(CreateCell(data.Rechnungsdatum));
                dataRow.AppendChild(CreateCell(data.Leistungszeitraum));
                dataRow.AppendChild(CreateCell(data.FirmaMitStrasse));
                dataRow.AppendChild(CreateCell(string.Join(";", data.PostleitzahlMitOrt)));
                dataRow.AppendChild(CreateCell(string.Join(";", data.City))); 

                worksheetPart.Worksheet.Save();
            }
        }

        private Cell CreateCell(string value)
        {
            return new Cell()
            {
                CellValue = new CellValue(value),
                DataType = CellValues.String
            };
        }
    }
}
