using Messages;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace CommunicationServer
{
    public class MessageReader
    {
        public static object GetObjectFromXML(string message)
        {
            var xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(message);
            switch(xmlDoc.DocumentElement.Name)
            {
                case "Data":
                    return Deserialize<Data>(message);
                case "RegisterGame":
                    return Deserialize<RegisterGame>(message);
                case "ConfirmJoiningGame":
                    return Deserialize<ConfirmJoiningGame>(message);
                case "RejectJoiningGame":
                    return Deserialize<RejectJoiningGame>(message);
                case "Game":
                    return Deserialize<Game>(message);
                case "GameStarted":
                    return Deserialize<GameStarted>(message);
                case "GetGames":
                    return Deserialize<GetGames>(message);
                case "JoinGame":
                    return Deserialize<JoinGame>(message);
                default:
                    return xmlDoc; //xmlDoc as default for other actions
            }
        }

        private static Stream GenerateStreamFromString(string str)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(str);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }

        private static object Deserialize<T>(string message)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            return serializer.Deserialize(GenerateStreamFromString(message));
        }

        public static string Serialize<T>(T obj)
        {
            var stringwriter = new System.IO.StringWriter();
            var serializer = new XmlSerializer(typeof(T));
            serializer.Serialize(stringwriter, obj);
            return stringwriter.ToString();
        }
    }
}
