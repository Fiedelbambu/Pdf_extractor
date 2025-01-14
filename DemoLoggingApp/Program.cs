using System;
using System.IO;
using log4net;
using log4net.Config;
//https://www.codeproject.com/Articles/140911/log-net-Tutorial
// Hier wird die log4net-Konfiguration aus der App.config-Datei gelesen
[assembly: log4net.Config.XmlConfigurator(Watch = true)]

namespace DemoLoggingApp
{
    class Program
    {
        // Log4net-Logger deklarieren
        private static readonly ILog log = LogManager.GetLogger(typeof(Program));

        [STAThread] // Single Thread starten ist für Ui Anwendungen
        static void Main(string[] args)
        {
            // log4net initialisieren
            XmlConfigurator.Configure();

            // Beispiel für das Loggen
            try
            {
                log.Info("Die Anwendung hat gestartet.");
                Console.WriteLine("Die Anwendung hat gestartet und ein Log-Eintrag wurde erstellt.");

                Console.WriteLine("Gib eine Zahl ein, die durch 0 geteilt werden soll:");
                var input = Console.ReadLine();
                var number = int.Parse(input);
                var result = 10 / number;
                Console.WriteLine($"Das Ergebnis ist: {result}");
            }
            catch (Exception ex)
            {
                log.Error("Ein Fehler ist aufgetreten", ex);
                Console.WriteLine("Ein Fehler wurde erkannt und ins Log geschrieben.");
            }
            finally
            {
                log.Info("Die Anwendung wird beendet.");
                Console.WriteLine("Die Anwendung wird beendet und ein Log-Eintrag wurde erstellt.");
            }
        }
    }
}


/*
 * Der nächste Eintrag wird einmal pro Klasse vorgenommen. Er erstellt eine Variable (in diesem Fall " log" genannt), die zum Aufrufen der log4net-Methoden verwendet wird. Dieser Code ist auch Code, den Sie kopieren und einfügen können (es sei denn, Sie verwenden das Compact Framework). Er führt einen System.ReflectionAufruf aus, um die aktuellen Klasseninformationen abzurufen. Dies ist nützlich, da wir diesen Code überall verwenden können, aber die spezifischen Informationen in jeder Klasse übergeben bekommen. Hier ist der Code:

C#
private static readonly log4net.ILog log = log4net.LogManager.GetLogger
    (System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
*/