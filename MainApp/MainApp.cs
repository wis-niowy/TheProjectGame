using Configuration;
using GameMaster;
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
            Console.WriteLine("Hello World!");
            if (args.Length == 0)
            {
                Console.WriteLine("No file specified for loading game settings");
                return;
            }
            var settings = LoadSettingsFromFile("Championship.xml");
            if (settings != null)
            {
                DoGame(settings);
            }

            Console.WriteLine("Closing main app");
            //TODO: solve exceptions issues while reading XML
        }


        public static GameMasterSettings LoadSettingsFromFile(string path)
        {
            GameMasterSettings settings = null;
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
                        Console.WriteLine(errors);
                        return null;
                    }
                }
            }
            return settings;
        }

        public static void DoGame(GameMasterSettings settings)
        {

        }
    }
}
