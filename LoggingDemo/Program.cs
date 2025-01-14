


namespace LoggingDemo
{
    class Program
    {
        static void Main()
        {
            Logger logger = new Logger();

            logger.Log("Anwendung konnte folgendene Datei nicht lesen......");
            logger.LogFeature("Folgende Pdf Dateien wurden gesendet........");
        }
    }
}