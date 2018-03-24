using GameArea.Texts;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameArea
{
    public static class ConsoleWriter
    {
        public static void Warning(string message)
        {
            Console.WriteLine(DateTime.Now.ToString() + " " + Constants.WARNING + message);
        }
        public static void Error(string error)
        {
            Console.WriteLine(DateTime.Now.ToString() + " " + Constants.ERROR + error);
        }

        public static void Show(string message)
        {
            Console.WriteLine(DateTime.Now.ToString() + " " + message);
        }
    }
}
