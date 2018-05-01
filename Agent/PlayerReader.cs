﻿using GameArea;
using GameArea.AppMessages;
using Messages;
using Player.PlayerMessages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Player
{
    public class PlayerReader
    {
        public static IAgentMessage GetObjectFromXML(string message)
        {
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
                case nameof(RegisteredGames):
                    var registeredGames = MessageParser.Deserialize<RegisteredGames>(message);
                    return new RegisteredGamesAgent(registeredGames.GameInfo.Select(q=>new GameArea.GameObjects.GameInfo(q.gameName,q.redTeamPlayers,q.blueTeamPlayers)).ToArray());
                case nameof(ConfirmJoiningGame):
                    var confirmJoin = MessageParser.Deserialize<ConfirmJoiningGame>(message);
                    return new ConfirmJoiningGameAgent(confirmJoin.gameId,
                                                        new GameArea.GameObjects.Player(confirmJoin.PlayerDefinition.id,
                                                                                        confirmJoin.PlayerDefinition.team,
                                                                                        confirmJoin.PlayerDefinition.role),
                                                        confirmJoin.privateGuid,
                                                        confirmJoin.playerId);
                case nameof(RejectJoiningGame):
                    var rejectJoin = MessageParser.Deserialize<RejectJoiningGame>(message);
                    return new RejectJoiningGameAgent(rejectJoin.gameName, rejectJoin.playerId);
                case nameof(Game):
                    var game = MessageParser.Deserialize<Game>(message);
                    return new GameAgent(game.playerId)
                    {
                        Board = new GameArea.GameObjects.GameBoard((int)game.Board.width,
                                                                    (int)game.Board.tasksHeight,
                                                                    (int)game.Board.goalsHeight),
                        PlayerLocation = new GameArea.GameObjects.Location((int)game.PlayerLocation.x,
                                                                            (int)game.PlayerLocation.y),
                        Players = game.Players.Select(q => new GameArea.GameObjects.Player(q.id, q.team, q.role)).ToArray()
                    };
                case nameof(Messages.GameMasterDisconnectedMessage):
                    var gmDisconnected = MessageParser.Deserialize<Messages.GameMasterDisconnectedMessage>(message);
                    return new GameMasterDisconnectedMessageAgent(gmDisconnected.gameId);
                case nameof(Data):
                    var data = MessageParser.Deserialize<Data>(message);
                    return new DataAgent(data.playerId)
                    {
                        GameFinished = data.gameFinished,
                        Goals = data.GoalFields.Select(q => new GameArea.GameObjects.GoalField((int)q.x,
                                                                                             (int)q.y,
                                                                                              q.team,
                                                                                              q.type)).ToArray(),
                        Pieces = data.Pieces.Select(q => new GameArea.GameObjects.Piece(q.id, q.timestamp, q.type, (q.playerIdSpecified ? (long)q.playerId : -1))).ToArray(),
                        PlayerGUID = data.playerGuid,
                        PlayerLocation = new GameArea.GameObjects.Location((int)data.PlayerLocation.x, (int)data.PlayerLocation.y),
                        Tasks = data.TaskFields.Select(q => new GameArea.GameObjects.TaskField((int)q.x,
                                                                                             (int)q.y,
                                                                                             q.distanceToPiece,
                                                                                             (q.pieceIdSpecified ? new GameArea.GameObjects.Piece(q.pieceId, DateTime.Now) : null),
                                                                                             (q.playerIdSpecified ? new GameArea.GameObjects.Player(q.playerId) : null))).ToArray()
                    };
                default:
                    return new ErrorMessageAgent("ReadingMessage", "Warning during reading message to server object, type not recognised\n Message read: " + message, "GetObjectFromXML", xmlDoc); //xmlDoc as default for other actions
            }
        }
    }
}