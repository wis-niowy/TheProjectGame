using Configuration;
using GameArea;
using GameArea.Texts;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using GameArea.AppConfiguration;
using System.Text.RegularExpressions;

namespace Player
{
    public class MainPlayer
    {
        private static IPAddress serverIP;
        private static int serverPort;
        private static PlayerSettingsConfiguration settings;
        private static Messages.TeamColour colour;
        private static Messages.PlayerRole role;
        private static string gameName;

        private static IPlayerController PController { get; set; }

        public static void Main(string[] args)
        {
            ParseArgs(args);

            ConsoleWriter.Show("Settings loaded. Establishing connection to server.");

            if (StartPlayerController(serverIP, serverPort, settings, colour, role))
            {
                PController.StartPerformance();
            }
        }

        public static bool StartPlayerController(IPAddress ip, Int32 port, PlayerSettingsConfiguration settings, Messages.TeamColour colour, Messages.PlayerRole role)
        {
            PController = new PlayerController(colour, role, settings, gameName);
            return PController.ConnectToServer(ip, port);
        }

        /// <summary>
        /// Method for integration tests
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        /// <param name="settings"></param>
        /// <param name="colour"></param>
        /// <param name="PController"></param>
        /// <param name="role"></param>
        /// <returns></returns>
        public static bool TestStartPlayer(IPAddress ip, Int32 port, PlayerSettingsConfiguration settings, Messages.TeamColour colour, out PlayerController PController, Messages.PlayerRole role = Messages.PlayerRole.member)
        {
            var player = new Player(colour, role, settings);
            PController = new PlayerController(player);
            player.Controller = PController;
            return PController.ConnectToServer(ip, port);
        }

        public static void ParseArgs(string[] args)
        {
            // initialize parameters with default values
            SetDefaultParameters();

            Regex optionPattern = new Regex("^--.+$");
            string currentOption = null;

            foreach (var arg in args)
            {
                if (optionPattern.Match(arg).Success)
                {
                    currentOption = arg;
                }
                else if (currentOption != null)
                {
                    AssignParameterValue(currentOption, arg);
                }
            }
        }

        public static void SetDefaultParameters()
        {
            serverIP = IPAddress.Parse("127.0.0.1");
            serverPort = Int32.Parse("5678");
            colour = Messages.TeamColour.blue;
            role = Messages.PlayerRole.member;
            settings = LoadSettingsFromFile("PlayerSettings.xml");
        }

        public static void AssignParameterValue(string option, string arg)
        {
            switch(option)
            {
                case "--address":
                    serverIP = IPAddress.Parse(arg);
                    break;
                case "--port":
                    serverPort = Int32.Parse(arg);
                    break;
                case "--conf":
                    settings = LoadSettingsFromFile(arg);
                    ConsoleWriter.Show("Loading settings from file: " + arg);
                    var settingsErrors = settings == null ? null : Validator.ValidateSettings(settings);
                    if (settings == null || (settingsErrors != null && settingsErrors != ""))
                    {
                        ConsoleWriter.Error("Failed to load settings from file. Verify file and try again.\n Closing Agent.");
                        settings = LoadSettingsFromFile("PlayerSettings.xml");
                    }
                    break;
                case "--game":
                    gameName = arg;
                    break;
                case "--team":
                    colour = StringToTeamColour(arg);
                    break;
                case "--role":
                    role = StringToRole(arg);
                    break;
            }
        }

        public static bool ValidateArgs(string[] args)
        {
            var valid = true;
            if (args.Length < 5)
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
            var colour = args[2];
            if (colour != "red" && colour != "blue")
            {
                ConsoleWriter.Error("Team colour not valid: " + args[2]);
                valid = false;
            }
            var role = args[3];
            if (role != "leader" && role != "member")
            {
                ConsoleWriter.Error("Team colour not valid: " + args[3]);
                valid = false;
            }

            return valid;
        }

        public static PlayerSettingsConfiguration LoadSettingsFromFile(string path)
        {
            PlayerSettings settings = null;
            try
            {
                using (StreamReader reader = new StreamReader(path))
                {
                    var xmlReader = XmlReader.Create(reader);
                    var serializer = new XmlSerializer(typeof(PlayerSettings));
                    if (serializer.CanDeserialize(xmlReader))
                    {
                        settings = (PlayerSettings)serializer.Deserialize(xmlReader);
                        var errors = Validator.ValidateSettings(new PlayerSettingsConfiguration(settings));
                        if (!string.IsNullOrEmpty(errors))
                        {
                            ConsoleWriter.Error(Constants.ERRORS_WHILE_PARSING_XML);
                            ConsoleWriter.Show(errors);
                            return null;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                ConsoleWriter.Error(Constants.UNEXPECTED_ERROR + e.Message);
                ConsoleWriter.Show(e.StackTrace);
            }
            return new PlayerSettingsConfiguration(settings);
        }

        private static Messages.TeamColour StringToTeamColour(string col)
        {
            switch(col)
            {
                case "red":
                    return Messages.TeamColour.red;
                case "blue":
                    return Messages.TeamColour.blue;
                default:
                    return Messages.TeamColour.blue;
            }
        }
        private static Messages.PlayerRole StringToRole(string role)
        {
            switch (role)
            {
                case "leader":
                    return Messages.PlayerRole.leader;
                default:
                    return Messages.PlayerRole.member;
            }
        }
    }
}
