﻿using Configuration;
using GameArea;
using GameArea.Texts;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Player
{
    public class MainPlayer
    {
        private static PlayerController PController { get; set; }

        public static void Main(string[] args)
        {
            IPAddress serverIP;
            int serverPort;
            PlayerSettings settings;
            var valid = ValidateArgs(args);
            Messages.TeamColour colour;

            if (valid)
            {
                serverIP = IPAddress.Parse(args[0]);
                serverPort = Int32.Parse(args[1]);
                colour = StringToTeamColour(args[2]);
                settings = LoadSettingsFromFile(args[3]);

                ConsoleWriter.Show("Loading settings from file: " + args[3]);
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
                serverIP = IPAddress.Parse("123.0.0.1");
                serverPort = Int32.Parse("5678");
                colour = Messages.TeamColour.blue;
                settings = new PlayerSettings();
            }

            ConsoleWriter.Show("Settings loaded. Establishing connection to server.");

            if (StartPlayer(serverIP, serverPort, settings, colour))
            {
                PController.StartPerformance();
            }
        }

        public static bool StartPlayer(IPAddress ip, Int32 port, PlayerSettings settings, Messages.TeamColour colour)
        {
            PController = new PlayerController(new Player(colour));
            return PController.ConnectToServer(ip, port);
        }

        private static bool ValidateArgs(string[] args)
        {
            var valid = true;
            if (args.Length < 4)
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

            return valid;
        }

        public static PlayerSettings LoadSettingsFromFile(string path)
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
                        var errors = Validator.ValidateSettings(settings);
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
            return settings;
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
    }
}
