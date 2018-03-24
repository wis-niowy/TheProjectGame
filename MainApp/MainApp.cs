using Configuration;
using GameArea.Texts;
using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace MainApp
{
    public class MainApp
    {
        public static void Main(string[] args)
        {
            Show(Constants.MAIN_APP_START);
            if (args.Length == 0)
            {
                Warning(Constants.NO_FILE_SPECIFIED);
            }
            var settings = LoadSettingsFromFile("Championship.xml");
            if (settings != null)
            {
                Show(Constants.SETTINGS_LOADED_SUCCES);
                DoGame(settings);
            }
            else
            {
                Show(Constants.SETTINGS_LOADED_FAIL);
            }

            Show(Constants.MAIN_APP_CLOSE);
        }


        public static GameMasterSettings LoadSettingsFromFile(string path)
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
                        var errors = Validator.ValidateSettings(settings);
                        if (!string.IsNullOrEmpty(errors))
                        {
                            Error(Constants.ERRORS_WHILE_PARSING_XML);
                            Show(errors);
                            return null;
                        }
                    }
                }
            }
            catch(Exception e)
            {
                Error(Constants.UNEXPECTED_ERROR + e.Message);
                Show(e.StackTrace);
            }
            return settings;
        }

        public static void DoGame(GameMasterSettings settings)
        {

        }

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
