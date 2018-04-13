using Configuration;
using GameArea;
using GameArea.Texts;
using Player;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace MainApp
{
    public class MainApp
    {
        public static void Main(string[] args)
        {
            GameMasterSettings settings;
            ConsoleWriter.Show(Constants.MAIN_APP_START);
            if (args.Length == 0)
            {
                ConsoleWriter.Warning(Constants.NO_FILE_SPECIFIED);
                ConsoleWriter.Show("Loading default settings.");
                settings = LoadSettingsFromFile("Championship.xml");
            }
            else
            {
                ConsoleWriter.Show("Loading specified settings in file: " + args[0]);
                settings = LoadSettingsFromFile(args[0]);
            }
            if (settings != null)
            {
                ConsoleWriter.Show(Constants.SETTINGS_LOADED_SUCCES);
                DoGame(settings);
            }
            else
            {
                ConsoleWriter.Show(Constants.SETTINGS_LOADED_FAIL);
            }

            ConsoleWriter.Show(Constants.MAIN_APP_CLOSE);
        }


        public static GameMasterSettings LoadSettingsFromFile(string path)
        {
            GameMasterSettings settings = null;
            //try
            {
                using (StreamReader reader = new StreamReader(path))
                {
                    var xmlReader = XmlReader.Create(reader);
                    var serializer = new XmlSerializer(typeof(GameMasterSettings));
                    if (serializer.CanDeserialize(xmlReader))
                    {
                        settings = (GameMasterSettings)serializer.Deserialize(xmlReader);
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
            //catch(Exception e)
            //{
            //    ConsoleWriter.Error(Constants.UNEXPECTED_ERROR + e.Message);
            //    ConsoleWriter.Show(e.StackTrace);
            //}
            return settings;
        }

        public static void DoGame(GameMasterSettings settings,bool  testing = false)
        {
            var GameController = new GameController(settings, testing);
            GameController.RegisterAgents();
            GameController.HandleGame();
        }


    }
}
