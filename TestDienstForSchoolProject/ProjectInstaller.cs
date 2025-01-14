using System.ComponentModel;
using System.Configuration.Install;
using System.ServiceProcess;

namespace TestDienstForSchoolProject
{
    [RunInstaller(true)]
    public partial class ProjectInstaller : Installer
    {
        private ServiceProcessInstaller serviceProcessInstaller;
        private ServiceInstaller serviceInstaller;

        public ProjectInstaller()
        {
            InitializeComponent();

            // Initialisierung der ServiceProcessInstaller
            serviceProcessInstaller = new ServiceProcessInstaller();
            serviceProcessInstaller.Account = ServiceAccount.LocalSystem; // Dienstkonto festlegen

            // Initialisierung der ServiceInstaller
            serviceInstaller = new ServiceInstaller();
            serviceInstaller.ServiceName = "MySchoolProject"; // Name des Dienstes
            serviceInstaller.DisplayName = "MySchoolProject"; // Anzeigename des Dienstes
            serviceInstaller.StartType = ServiceStartMode.Automatic; // Starttyp festlegen

            // Installer zur Installers-Sammlung hinzufügen
            Installers.Add(serviceProcessInstaller);
            Installers.Add(serviceInstaller);
        }

        // Optional: Ereignisbehandlungsroutinen für das Nachinstallieren
        private void serviceInstaller1_AfterInstall(object sender, InstallEventArgs e)
        {
            // Hier kannst du nach der Installation zusätzliche Logik hinzufügen, falls erforderlich
        }

        private void serviceProcessInstaller1_AfterInstall(object sender, InstallEventArgs e)
        {
            // Hier kannst du nach der Installation zusätzliche Logik hinzufügen, falls erforderlich
        }

    }
}
