using GameArea;
using GameMaster.GMConfiguration;
using GameMaster.GMMessages;
using Messages;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace GameMaster
{
    class GMReader
    {
        public static IGMMessage GetObjectFromXML(string message)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                ConsoleWriter.Show("Read empty or whitespace message.");
                return null;
            }
            var xmlDoc = new XmlDocument();
            try
            {
                xmlDoc.LoadXml(message);
            }
            catch (Exception e)
            {
                ConsoleWriter.Error("Could not load message to XML. \nMessage content: \n" + message);
                return null;
            }
            switch (xmlDoc.DocumentElement.Name)
            {
                case nameof(TestPiece):
                    var test = MessageParser.Deserialize<TestPiece>(message);
                    return new TestPieceGM(test.playerGuid, test.gameId);
                case nameof(DestroyPiece):
                    var destroy  = MessageParser.Deserialize<DestroyPiece>(message);
                    return new DestroyPieceGM(destroy.playerGuid, destroy.gameId);
                case nameof(PlacePiece):
                    var place = MessageParser.Deserialize<PlacePiece>(message);
                    return new PlacePieceGM(place.playerGuid, place.gameId);
                case nameof(PickUpPiece):
                    var pick = MessageParser.Deserialize<PickUpPiece>(message);
                    return new PickUpPieceGM(pick.playerGuid, pick.gameId);
                case nameof(Move):
                    var move = MessageParser.Deserialize<Move>(message);
                    return new MoveGM(move.playerGuid, move.gameId, (MoveType?)move.direction);
                case nameof(Discover):
                    var discover = MessageParser.Deserialize<Discover>(message);
                    return new DiscoverGM(discover.playerGuid, discover.gameId);
                case nameof(RejectGameRegistration):
                    var reject = MessageParser.Deserialize<RejectGameRegistration>(message);
                    return new RejectGameRegistrationGM(reject.gameName);
                case nameof(ConfirmGameRegistration):
                    var confirm = MessageParser.Deserialize<ConfirmGameRegistration>(message);
                    return new ConfirmGameRegistrationGM(confirm.gameId);
                case nameof(JoinGame):
                    var join = MessageParser.Deserialize<JoinGame>(message);
                    return new JoinGameGM(join.gameName, join.preferredTeam, join.preferredRole, (long)join.playerId);
                case nameof(PlayerDisconnected):
                    var playerDisconnected = MessageParser.Deserialize<PlayerDisconnected>(message);
                    return new PlayerDisconnectedGM(playerDisconnected.playerId);

                case nameof(Configuration.GameMasterSettings):
                    var settings = MessageParser.Deserialize<Configuration.GameMasterSettings>(message);
                    return new GameMasterSettingsGM(settings);

                case nameof(AuthorizeKnowledgeExchange):
                    var authMsg = MessageParser.Deserialize<AuthorizeKnowledgeExchange>(message);
                    return new AuthorizeKnowledgeExchangeGM(authMsg.playerGuid, authMsg.gameId, authMsg.withPlayerId);
                case nameof(RejectKnowledgeExchange):
                    var rejectExchange = MessageParser.Deserialize<RejectKnowledgeExchange>(message);
                    return new RejectKnowledgeExchangeGM(rejectExchange.playerId, rejectExchange.senderPlayerId, rejectExchange.permanent, rejectExchange.playerGuid);
                case nameof(AcceptExchangeRequest):
                    var acceptMsg = MessageParser.Deserialize<AcceptExchangeRequest>(message);
                    return new AcceptExchangeRequestGM(acceptMsg.playerId, acceptMsg.senderPlayerId);
                // zalatwic roznice typow TaskField i GoalField nizej - najlepiej konstruktory biorace obiekt z namespace Messages
                //case nameof(SuggestAction):
                //    var suggestMsg = MessageParser.Deserialize<SuggestAction>(message);
                //    return new SuggestActionGM(suggestMsg.playerId, suggestMsg.senderPlayerId, suggestMsg.playerGuid, suggestMsg.gameId, suggestMsg.TaskFields, suggestMsg.GoalFields);
                //case nameof(SuggestActionResponse):
                //    var answer = MessageParser.Deserialize<SuggestActionResponse>(message);
                //    return new SuggestActionResponseGM(answer.playerId, answer.senderPlayerId, answer.playerGuid, answer.TaskFields, answer.GoalFields);
                case nameof(Data):
                    var data = MessageParser.Deserialize<Data>(message);
                    return new DataGM(data);


                default:
                    return new ErrorMessageGM("ReadingMessage", "Warning during reading message to server object, type not recognised\n Message read: " + message, "GetObjectFromXML", xmlDoc); //xmlDoc as default for other actions
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

        private static T Deserialize<T>(string message)
            where T : class
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            return serializer.Deserialize(GenerateStreamFromString(message)) as T;
        }

        public static string Serialize<T>(T obj)
        {
            var stringWriter = new Utf8Writer();
            var serializer = new XmlSerializer(typeof(T));

            using (var writer = XmlWriter.Create(stringWriter, new XmlWriterSettings() { Encoding = Encoding.UTF8 }))
            {
                serializer.Serialize(writer, obj);
                return stringWriter.ToString();
            }
        }
    }
}
