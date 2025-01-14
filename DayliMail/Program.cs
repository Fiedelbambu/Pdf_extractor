using System;
using System.Timers;

namespace DayliMail {
    
        
    class Programm
    {
        private  System.Timers.Timer _hourlyTimer;
        private  System.Timers.Timer _dailyTimer;

        public static void Main(string[] args)
        {
            var program = new Programm();
            Console.ReadKey();

        }

        public Programm()
        {
            TimerCount();

        }

        public void TimerCount()
        {
            // Timer für stündliche Aufgabe konfigurieren
            _hourlyTimer = new System.Timers.Timer(3600000); // 1 Stunde in Millisekunden
            _hourlyTimer.Elapsed += (sender, e) => SenDataPdf();
            _hourlyTimer.AutoReset = true;
            _hourlyTimer.Enabled = true;

            // Timer für tägliche Aufgabe konfigurieren
            var now = DateTime.Now;
            var nextDailyTrigger = DateTime.Today.AddHours(21).AddMinutes(15);
            if (nextDailyTrigger < now)
            {
                nextDailyTrigger = nextDailyTrigger.AddDays(1);
            }

            var initialDelay = nextDailyTrigger - now;
            if (initialDelay.TotalMilliseconds < 0)
            {
                initialDelay = TimeSpan.Zero;
            }

            _dailyTimer = new System.Timers.Timer(initialDelay.TotalMilliseconds);
            _dailyTimer.Elapsed += (sender, e) => onDailyElapsed();
            _dailyTimer.AutoReset = true;
            _dailyTimer.Interval = TimeSpan.FromDays(1).TotalMilliseconds; // 24 Stunden
            _dailyTimer.Enabled = true;
        }


        private void onDailyElapsed()
        {
            EmailSend();
        }

        private void SenDataPdf()
        {
            Console.WriteLine("SendDataPdf wird ausgeführt: " + DateTime.Now);
        }


        private void EmailSend()
        {
            Console.WriteLine("EmailSend wird ausgeführt: " + DateTime.Now);         
        }
    }
}
