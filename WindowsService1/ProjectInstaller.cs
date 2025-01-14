using System;
using System.Collections;
using System.ComponentModel;
using System.Configuration.Install;
using System.ServiceProcess;

namespace WindowsService1
{
    [RunInstaller(true)]
    public partial class ProjectInstaller : Installer
    {
        private ServiceProcessInstaller serviceProcessInstaller;
        private ServiceInstaller serviceInstaller;

        public ProjectInstaller()
        {
            // Initialisieren der Installer-Komponenten
            InitializeComponent();

            // Erstellen eines neuen ServiceProcessInstaller
            serviceProcessInstaller = new ServiceProcessInstaller();

            // Dienst soll unter dem Benutzerkonto "Lokales System" ausgeführt werden
            serviceProcessInstaller.Account = ServiceAccount.LocalSystem;

            // Erstellen des ServiceInstaller
            serviceInstaller = new ServiceInstaller();

            // Festlegen des Namens, wie er im Windows-Dienstmanager erscheinen soll
            serviceInstaller.ServiceName = "WindowsService1";
            serviceInstaller.DisplayName = "Windows Service 1";
            serviceInstaller.Description = "Dies ist ein Test-Windows-Dienst.";

            // Startmodus des Dienstes festlegen (Automatisch starten)
            serviceInstaller.StartType = ServiceStartMode.Automatic;

            // Installer-Klassen hinzufügen
            Installers.Add(serviceProcessInstaller);
            Installers.Add(serviceInstaller);
        }
    }
}
