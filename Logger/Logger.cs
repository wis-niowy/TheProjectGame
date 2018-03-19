using System;
using System.IO;
using System.Text;
using System.Xml;
namespace Logger
{
    public class Logger
    {
        // note: StreamWriter is not thread safe
        protected static StreamWriter sw; // main log
        private static Logger instance;
        protected Logger() { }
        protected static int counter = 0;

        public static Logger Instance
        {
            get
            {
                if (instance == null)
                {
                    Console.WriteLine("should Initialize first");
                }
                return instance;
            }
        }

        public static void Initialize(string filename = "logs.txt")
        {
            if (instance == null)
            {
                instance = new Logger();
                sw = new StreamWriter(filename, true);
            }
        }

        public void Log(string xml)
        {
            var dt = DateTime.Now;
            string str = String.Format("{0:yyyy-mm-ddTHH:mm:ss.fff}", dt);
            sw.WriteLine(str + " " + xml);
        }
    }

    public class GameMasterLogger : Logger
    {
        private Logger mainLogger;
        private new StreamWriter sw;

        public GameMasterLogger()
        {
            mainLogger = Logger.Instance;
            StreamWriter sw = new StreamWriter("GMlogs.txt", true);
        }

        public void Log(string xml)
        {
            // Deserialize xml
            mainLogger.Log(xml);
        }
    }

    public class PlayerLogger : Logger
    {
        private Logger mainLogger;
        private new StreamWriter sw;
        private int myId;

        public PlayerLogger()
        {
            myId = counter++;
            mainLogger = Logger.Instance;
            StreamWriter sw = new StreamWriter("log" + myId + ".txt", true);
        }

        public new void Log(string xml)
        {
            mainLogger.Log(xml);
            var dt = DateTime.Now;
            string str = String.Format("{0:yyyy-mm-ddTHH:mm:ss.fff}", dt);
            sw.WriteLine(str + " " + xml);
        }
    }
}
