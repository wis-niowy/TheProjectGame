﻿using CommunicationServer.ServerMessages;
using GameArea;
using GameArea.Parsers;
using Messages;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace CommunicationServer
{
    public class ServerReader
    {
        public static IMessage<T> GetObjectFromXML<T>(string message, ulong clientId)
        {
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
                case "Data":
                    return new DataServer(Deserialize<Data>(message),clientId) as IMessage<T>;
                case "ConfirmJoiningGame":
                    var confirmJoin = Deserialize<ConfirmJoiningGame>(message);
                    return new ConfirmJoiningGameServer(confirmJoin.gameId
                                                         ,new GameArea.GameObjects.Player(confirmJoin.PlayerDefinition.id, confirmJoin.PlayerDefinition.team,confirmJoin.PlayerDefinition.role)
                                                         ,confirmJoin.privateGuid
                                                         ,confirmJoin.playerId,clientId) as IMessage<T>;
                case "Game":
                    var game = Deserialize<Game>(message);
                    return new GameServer(game,clientId) as IMessage<T>;
                case "GameStarted":
                    var gameStarted =  Deserialize<GameStarted>(message);
                    return new GameStartedServer(gameStarted.gameId,clientId) as IMessage<T>;
                case "RejectJoiningGame":
                    var rejectJoin =  Deserialize<RejectJoiningGame>(message);
                    return new RejectJoiningGameServer(rejectJoin.gameName, rejectJoin.playerId) as IMessage<T>;
                case "GameMasterDisconnected":
                    var gmDisconnected = Deserialize<GameMasterDisconnectedMessage>(message);
                    return new GameMasterDisconnectedServer(gmDisconnected.gameId,clientId) as IMessage<T>;
                case "RegisterGame":
                    var register = Deserialize<RegisterGame>(message);
                    return (new RegisterGameServer(register.NewGameInfo.gameName, register.NewGameInfo.redTeamPlayers, register.NewGameInfo.blueTeamPlayers, clientId)) as IMessage<T>;
                case "GetGames":
                    return (new GetGamesServer(clientId)) as IMessage<T>;
                case "JoinGame":
                    var join =  Deserialize<JoinGame>(message);
                    return new JoinGameServer(join.gameName, join.preferredTeam, join.preferredRole, clientId, (join.playerIdSpecified ? (long)join.playerId : -1)) as IMessage<T>;
                case "TestPiece":
                    var test = Deserialize<TestPiece>(message);
                    return new TestPieceServer(test.playerGuid, test.gameId) as IMessage<T>;
                case "DestroyPiece":
                    var destroy = Deserialize<DestroyPiece>(message);
                    return new DestroyPieceServer(destroy.playerGuid, destroy.gameId) as IMessage<T>;
                case "PlacePiece":
                    var place = Deserialize<PlacePiece>(message);
                    return new PlacePieceServer(place.playerGuid, place.gameId) as IMessage<T>;
                case "PickUpPiece":
                    var pick = Deserialize<PickUpPiece>(message);
                    return new PickUpPieceServer(pick.playerGuid, pick.gameId) as IMessage<T>;
                case "Move":
                    var move = Deserialize<Move>(message);
                    return new MoveServer(move.playerGuid, move.gameId, (MoveType?)move.direction) as IMessage<T>;
                case "Discover":
                    var discover = Deserialize<Discover>(message);
                    return new DiscoverServer(discover.playerGuid, discover.gameId) as IMessage<T>;
                default:
                    return new ErrorMessageServer("ReadingMessage","Warning during reading message to server object, type not recognised\n Message read: " + message, "GetObjectFromXML", clientId, xmlDoc) as IMessage<T>; //xmlDoc as default for other actions
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
            where T:class
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
