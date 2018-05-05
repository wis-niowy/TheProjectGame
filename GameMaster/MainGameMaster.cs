using Configuration;
using GameArea;
using GameArea.Texts;
using GameMasterMain;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using GameArea.AppConfiguration;

namespace GameMaster
{
    public static class MainGameMaster
    {
        public static GameMasterController GMController { get; set; }

        public static void Main(string[] args)
        {
            IPAddress serverIP;
            int serverPort;
            GameMasterSettingsConfiguration settings;
            var valid = ValidateArgs(args);

            if (valid)
            {
                serverIP = IPAddress.Parse(args[0]);
                serverPort = Int32.Parse(args[1]);
                settings = LoadSettingsFromFile(args[2]);
                ConsoleWriter.Show("Loading settings from file: " + args[2]);
                var settingsErrors = settings == null ? null : Validator.ValidateSettings(settings);
                if (settings == null || (settingsErrors != null && settingsErrors != ""))
                {
                    ConsoleWriter.Error("Failed to load settings from file. Verify file and try again.\n Closing GameMaster.");
                    return;
                }
            }
            else
            {
                ConsoleWriter.Warning("Invalid args, loading default!");
                serverIP = IPAddress.Parse("127.0.0.1");
                serverPort = Int32.Parse("5678");
                settings = new GameMasterSettingsConfiguration(GameMasterSettings.GetDefaultGameMasterSettings());
            }

            ConsoleWriter.Show("Settings loaded. Establishing connection to server.");

            if (StartGameMaster(serverIP, serverPort, settings))
            {
                GMController.StartPerformance();
            }
        }

        public static bool StartGameMaster(IPAddress ip, Int32 port, GameMasterSettingsConfiguration settings)
        {
            GMController = new GameMasterController(new GameArea.GameMaster(settings));
            return GMController.ConnectToServer(ip, port);
        }

        private static bool ValidateArgs(string[] args)
        {
            var valid = true;
            if (args.Length < 3)
            {
                ConsoleWriter.Error("Args too short, must be provided: IP adress, socket number, path to file");
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

        public static GameMasterSettingsConfiguration LoadSettingsFromFile(string path)
        {
            GameMasterSettings settings = null;
            try
            {
                using (StreamReader reader = new StreamReader(path))
                {
                    var xmlReader = XmlReader.Create(reader);
                    var serializer = new XmlSerializer(typeof(GameMasterSettings));
                    if (serializer.CanDeserialize(xmlReader))
                    {
                        settings = (GameMasterSettings)serializer.Deserialize(xmlReader);
                        var errors = Validator.ValidateSettings(new GameMasterSettingsConfiguration(settings));
                        if (!string.IsNullOrEmpty(errors))
                        {
                            ConsoleWriter.Error(Constants.ERRORS_WHILE_PARSING_XML);
                            ConsoleWriter.Show(errors);
                            return null;
                        }
                    }
                }
            }
            catch(Exception e)
            {
                ConsoleWriter.Error(Constants.UNEXPECTED_ERROR + e.Message);
                ConsoleWriter.Show(e.StackTrace);
            }
            return new GameMasterSettingsConfiguration(settings);
        }
    }


}
