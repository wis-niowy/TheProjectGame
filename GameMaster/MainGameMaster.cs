using GameArea;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace GameMaster
{
    public static class MainGameMaster
    {
        private static GameMasterController GMController { get; set; }

        public static void Main(string[] args)
        {
            var valid = ValidateArgs(args);
            if (!valid)
                ConsoleWriter.Warning("Invalid args, laoding default!");
            IPAddress serverIP;
            int serverPort;
            if (valid)
            {
                serverIP = IPAddress.Parse(args[0]);
                serverPort = Int32.Parse(args[1]);
            }
            else
            {
                serverIP = IPAddress.Parse("127.0.0.1");
                serverPort = Int32.Parse("5678");
            }
            StartGameMaster(serverIP, serverPort);
            GMController.StartPerformance();
        }

        public static void StartGameMaster(IPAddress ip, Int32 port)
        {
            Configuration.GameMasterSettings settings = new Configuration.GameMasterSettings();
            GMController = new GameMasterController(new GameArea.GameMaster(settings));
            GMController.ConnectToServer(ip, port);
        }

        private static bool ValidateArgs(string[] args)
        {
            var valid = true;
            if (args.Length < 2)
            {
                ConsoleWriter.Error("Args too short, must be provided: IP adress, socket number");
                return false;
            }
            var ip = IPAddress.Parse(args[0]);
            if (ip == null)
            {
                ConsoleWriter.Error("IP adress is not valid: " + args[0]);
                valid = false;
            }
            var socket = Int32.Parse(args[1]);
            if (socket <= 0)
            {
                ConsoleWriter.Error("Socket number not valid: " + args[1]);
                valid = false;
            }
            return valid;
        }
    }
}
