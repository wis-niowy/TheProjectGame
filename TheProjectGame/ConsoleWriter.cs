using GameArea.Texts;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameArea
{
    public class ConsoleWriter
    {
        public static bool WriteEnabled = true;
        public static void Warning(string message)
        {
            if(WriteEnabled)
                Console.WriteLine(DateTime.Now.ToString() + " " + Constants.WARNING + message);
        }
        public static void Error(string error)
        {
            if (WriteEnabled)
                Console.WriteLine(DateTime.Now.ToString() + " " + Constants.ERROR + error);
        }

        public static void Show(string message)
        {
            if (WriteEnabled)
                Console.WriteLine(DateTime.Now.ToString() + " " + message);
        }
    }
}
