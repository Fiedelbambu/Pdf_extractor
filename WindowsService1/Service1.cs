using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using WindowsService1.Controllers;
using WindowsService1.Services;

namespace WindowsService1
{
    public partial class Service1 : ServiceBase
    {
        private Timer timer;
        private ProcessController processController;

        public Service1()
        {
            InitializeComponent();
            this.ServiceName = "MySampleWindowsService";
            timer = new Timer();
            timer.Interval = 3600000; // 1 Stunde in Millisekunden
            timer.Elapsed += new ElapsedEventHandler(OnTimer);
            processController = new ProcessController(); // Instanz des ProcessController
        }

        protected override void OnStart(string[] args)
        {
            timer.Start();
            WriteEventLog("Service started.");
        }

        protected override void OnStop()
        {
            timer.Stop();
            WriteEventLog("Service stopped.");
        }

        private void OnTimer(object sender, ElapsedEventArgs e)
        {
            WriteEventLog("Timer event executed.");
            _ = processController.StartProcessAsync(); // Hier wird der Prozesscontroller aufgerufen
        }

        private void WriteEventLog(string message)
        {
            Logger.WriteEntry("Application", message);
        }
    }
}
