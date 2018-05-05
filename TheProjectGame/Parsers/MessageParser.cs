using Messages;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace GameArea
{
    public static class MessageParser
    {
        public static object Deserialize(string message)
        {
            //napisac tu analogicznie aprsowanie każdego typu wiadomości na podstawie MessageReader z communication server
            var xmlDoc = new XmlDocument();
            try
            {
                xmlDoc.LoadXml(message);
            }
            catch (Exception e)
            {
                ConsoleWriter.Error("Could not load message to XML. \nMessage content: \n" + message);
                xmlDoc = null;
            }
            switch (xmlDoc.DocumentElement.Name)
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
                case "DestroyPiece":
                    return Deserialize<DestroyPiece>(message);
                case "PlacePiece":
                    return Deserialize<PlacePiece>(message);
                case "PickUpPiece":
                    return Deserialize<PickUpPiece>(message);
                case "Move":
                    return Deserialize<Move>(message);
                case "Discover":
                    return Deserialize<Discover>(message);
                case "AuthorizeKnowledgeExchange":
                    return Deserialize<AuthorizeKnowledgeExchange>(message);
                case "KnowledgeExchangeRequest":
                    return Deserialize<KnowledgeExchangeRequest>(message);
                case "AcceptExchangeRequest":
                    return Deserialize<AcceptExchangeRequest>(message);
                case "RejectKnowledgeExchange":
                    return Deserialize<RejectKnowledgeExchange>(message);
               //case "SuggestAction":
               //    return Deserialize<SuggestAction>(message);  //add after shcema update
               //case "SuggestActionResponse":
               //    return Deserialize<SuggestActionResponse>(message);
                case "ConfirmGameRegistration":
                    return Deserialize<ConfirmGameRegistration>(message);
                case "RejectGameRegistration":
                    return Deserialize<RejectGameRegistration>(message);
                case "RegisteredGames":
                    return Deserialize<RegisteredGames>(message);
                case "GameMasterDisconnected":
                    return Deserialize<GameMasterDisconnectedMessage>(message);
                case "PlayerDisconnected":
                    return Deserialize<PlayerDisconnected>(message);

                case "GameMasterSettings":
                    return Deserialize<Configuration.GameMasterSettings>(message);
                case "PlayerSettings":
                    return Deserialize<Configuration.PlayerSettings>(message);

                default:
                    return xmlDoc; //xmlDoc as default for other actions
            }
        }

        public static string Serialize<T>(T msg)
            where T: class
        {
            if (msg == null)
            {
                return string.Empty;
            }
            try
            {
                var xmlserializer = new XmlSerializer(typeof(T));
                var stringWriter = new Utf8Writer();
                using (var writer = XmlWriter.Create(stringWriter,new XmlWriterSettings() { Encoding = Encoding.UTF8 }))
                {
                    xmlserializer.Serialize(writer, msg);
                    return stringWriter.ToString();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred", ex);
            }
        }

        public static T Deserialize<T>(string xml)
            where T: class
        {
            XmlSerializer ser = new XmlSerializer(typeof(T));

            using (StringReader sr = new StringReader(xml))
            {
                return (T)ser.Deserialize(sr);
            }
        }

        public static string[] SerializeObjects<T>(IEnumerable<T> msgs)
            where T:class
        {
            List<string> xmls = new List<string>();
            foreach(var msg in msgs)
            {
                xmls.Add(Serialize(msg));
            }
            return xmls.ToArray();
        }
    }
}
