using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loggers
{
    class MainAppLogger:Logger
    {
        private string mainAppLog = "";
        private new StreamWriter sw = null;
        private int myId;


        public MainAppLogger()
        {

            objectName = "MainApp";
            myId = counter++;
            mainAppLog = $"MainAppLogger.txt";
            //
        }

        public new void Log(string action, string description)
        {
            sw = new StreamWriter(mainAppLog, true);
            var dt = DateTime.Now;
            string time = String.Format(Const.timeFormat, dt);
            sw.WriteLine($"{time} {objectName} {action} {description}");
            sw.Close();
        }

    }
}
