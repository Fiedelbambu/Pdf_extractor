using Microsoft.PowerPlatform.Dataverse.Client;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using System.ServiceModel;
using System.Security.Cryptography;
using System.Text;

class Program
{
    static async Task Main(string[] args)
    {
        // Entschlüssele den Connection-String aus der Datei
        string connectionString = RetrieveAndDecryptConnectionString();

        try
        {
            using (var serviceClient = new ServiceClient(connectionString))
            {
                if (!serviceClient.IsReady)
                {
                    throw new InvalidOperationException("Die Verbindung zu Dataverse konnte nicht hergestellt werden.");
                }

                Console.WriteLine("Verbindung zu Dataverse erfolgreich hergestellt.");
                Console.WriteLine("Möchtest du eine Tabelle auslesen oder einen neuen Datensatz erstellen? (lesen/erstellen)");

                string response = Console.ReadLine()?.Trim().ToLower();
                if (response == "l")
                {
                    await ReadTableAsync(serviceClient);
                }
                else if (response == "e")
                {
                    await CreateRecordAsync(serviceClient);
                }
                else
                {
                    Console.WriteLine("Ungültige Eingabe.");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Fehler: {ex.Message}");
        }
    }

    public static string RetrieveAndDecryptConnectionString()
    {
        
        byte[] encryptedBytes = System.IO.File.ReadAllBytes("encryptedConnectionString.dat");
        
        byte[] decryptedBytes = ProtectedData.Unprotect(encryptedBytes, null, DataProtectionScope.CurrentUser);

        return Encoding.UTF8.GetString(decryptedBytes);
    }


    private static async Task ReadTableAsync(ServiceClient serviceClient)
    {
        
        string tableName = "tu_briefskopf"; 

        
        QueryExpression query = new QueryExpression(tableName)
        {
            ColumnSet = new ColumnSet("tu_firmenname", "tu_rechnungsdatum", "tu_servicezeitraum", "tu_stadt", "tu_rechnungsnummer", "tu_kostenstelle", "tu_strasse", "tu_postleitzahl")
        };

        try
        {
            EntityCollection result = await serviceClient.RetrieveMultipleAsync(query);

            foreach (var entity in result.Entities)
            {
                // Firmenname (tu_firmenname)
                var companyName = entity.GetAttributeValue<string>("tu_firmenname");
                Console.WriteLine($"Firma: {companyName ?? "Kein Wert vorhanden."}");

                // Rechnungsdatum (tu_rechnungsdatum)
                var invoiceDateString = entity.GetAttributeValue<string>("tu_rechnungsdatum");
                DateTime? invoiceDate = null;
                if (DateTime.TryParse(invoiceDateString, out DateTime parsedDate))
                {
                    invoiceDate = parsedDate;
                }
                Console.WriteLine($"Rechnungsdatum: {invoiceDate?.ToShortDateString() ?? invoiceDateString ?? "Kein Wert vorhanden."}");

                // Serviceperiode (tu_servicezeitraum)
                var servicePeriod = entity.GetAttributeValue<string>("tu_servicezeitraum");
                Console.WriteLine($"Serviceperiode: {servicePeriod ?? "Kein Wert vorhanden."}");

                // Stadt (tu_stadt)
                var city = entity.GetAttributeValue<string>("tu_stadt");
                Console.WriteLine($"Stadt: {city ?? "Kein Wert vorhanden."}");

                // Rechnungsnummer (tu_rechnungsnummer)
                var invoiceNumberString = entity.GetAttributeValue<string>("tu_rechnungsnummer");
                int? invoiceNumber = null;
                if (int.TryParse(invoiceNumberString, out int parsedInvoiceNumber))
                {
                    invoiceNumber = parsedInvoiceNumber;
                }
                Console.WriteLine($"Rechnungsnummer: {invoiceNumber?.ToString() ?? invoiceNumberString ?? "Kein Wert vorhanden."}");

                // Kostenstelle (tu_kostenstelle)
                var costCenterString = entity.GetAttributeValue<string>("tu_kostenstelle");
                int? costCenter = null;
                if (int.TryParse(costCenterString, out int parsedCostCenter))
                {
                    costCenter = parsedCostCenter;
                }
                Console.WriteLine($"Kostenstelle: {costCenter?.ToString() ?? costCenterString ?? "Kein Wert vorhanden."}");

                // Straße (tu_strasse)
                var street = entity.GetAttributeValue<string>("tu_strasse");
                Console.WriteLine($"Straße: {street ?? "Kein Wert vorhanden."}");

                // Postleitzahl (tu_postleitzahl)
                var postalCodeString = entity.GetAttributeValue<string>("tu_postleitzahl");
                int? postalCode = null;
                if (int.TryParse(postalCodeString, out int parsedPostalCode))
                {
                    postalCode = parsedPostalCode;
                }
                Console.WriteLine($"Postleitzahl: {postalCode?.ToString() ?? postalCodeString ?? "Kein Wert vorhanden."}");

                Console.WriteLine("-----------------------------");

            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Fehler beim Auslesen der Tabelle: {ex.Message}");
        }
    }

    private static async Task CreateRecordAsync(ServiceClient serviceClient)
    {
        string tableName = "tu_briefskopf";

        try
        {
            // Erstellen eines neuen Datensatzes
            var entity = new Entity(tableName);

            entity["tu_firmenname"] = "Beisdpiel GsmbH"; 
            entity["tu_rechnungsdatum"] = "133.09.2024"; 
            entity["tu_servicezeitraum"] = "913.05.1989 - 26.06.2022";
            entity["tu_stadt"] = "djeelhl";
            entity["tu_rechnungsnummer"] = "6367346";
            entity["tu_kostenstelle"] = "976489";
            entity["tu_strasse"] = "Biespeilgstralße 6523";
            entity["tu_postleitzahl"] = "3546568";

            // Datensatz in Dataverse einfügen
            Guid recordId = await serviceClient.CreateAsync(entity);

            Console.WriteLine($"Datensatz erfolgreich erstellt. ID: {recordId}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Fehler beim Erstellen des Datensatzes: {ex.Message}");
        }
    }

}

