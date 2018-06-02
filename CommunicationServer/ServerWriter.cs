using GameArea;
using System;
using System.Collections.Generic;
using System.Text;

namespace CommunicationServer
{
    class ServerWriter:ConsoleWriter
    {
        public static bool StateOnly { get; set; }
        public static void Warning(string message)
        {
            if (!StateOnly)
                ConsoleWriter.Warning(message);
        }
        public static void Error(string error)
        {
            if (!StateOnly)
                ConsoleWriter.Error(error);
        }

        public static void Show(string message)
        {
            if (!StateOnly)
                ConsoleWriter.Show(message);
        }

        public static void ForcedShow(string message)
        {
            if (StateOnly)
                Console.Clear();
            ConsoleWriter.Show(message);
        }
    }
}
