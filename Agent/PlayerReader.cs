using GameArea;
using GameArea.AppMessages;
using Messages;
using Player.PlayerConfiguration;
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
                return new ErrorMessageAgent("ReadingMessage", "Error during proccessing XML, incorrect syntax\n Message read: " + message, "GetObjectFromXML", xmlDoc); //xmlDoc as default for other actions
            }
            if (xmlDoc == null)
            {
                return new ErrorMessageAgent("ReadingMessage", "Error during proccessing XML, incorrect syntax\n Message read: " + message, "GetObjectFromXML", xmlDoc); //xmlDoc as default for other actions
            }

            switch (xmlDoc.DocumentElement.Name)
            {
                case nameof(RegisteredGames):
                    var registeredGames = MessageParser.Deserialize<RegisteredGames>(message);
                    return new RegisteredGamesAgent(registeredGames.GameInfo?.Select(q => new GameArea.GameObjects.GameInfo(q.gameName, q.redTeamPlayers, q.blueTeamPlayers)).ToArray());
                case nameof(ConfirmJoiningGame):
                    var confirmJoin = MessageParser.Deserialize<ConfirmJoiningGame>(message);
                    return new ConfirmJoiningGameAgent(confirmJoin.gameId, new GameArea.GameObjects.Player(confirmJoin.PlayerDefinition.id,
                                                                                        confirmJoin.PlayerDefinition.team,
                                                                                        confirmJoin.PlayerDefinition.role),
                                                                                        confirmJoin.privateGuid,
                                                                                        confirmJoin.playerId);
                case nameof(RejectJoiningGame):
                    var rejectJoin = MessageParser.Deserialize<RejectJoiningGame>(message);
                    return new RejectJoiningGameAgent(rejectJoin.gameName, rejectJoin.playerId);
                case nameof(Game):
                    var game = MessageParser.Deserialize<Game>(message);

                    GameArea.GameObjects.Player[] players;
                    if (game.Players != null)
                        players = game.Players.Select(q => new GameArea.GameObjects.Player(q.id, q.team, q.role)).ToArray();
                    else
                        players = null;

                    return new GameAgent(game.playerId)
                    {
                        Board = new GameArea.GameObjects.GameBoard((int)game.Board.width,
                                                                   (int)game.Board.tasksHeight,
                                                                   (int)game.Board.goalsHeight),
                        PlayerLocation = new GameArea.GameObjects.Location((int)game.PlayerLocation.x,
                                                                           (int)game.PlayerLocation.y),
                        Players = players
                    };
                case nameof(Messages.GameMasterDisconnectedMessage):
                    var gmDisconnected = MessageParser.Deserialize<Messages.GameMasterDisconnectedMessage>(message);
                    return new GameMasterDisconnectedMessageAgent(gmDisconnected.gameId);
                case nameof(Data):
                    var data = MessageParser.Deserialize<Data>(message);

                    GameArea.GameObjects.GoalField[] goals;
                    if (data.GoalFields != null)
                        goals = data.GoalFields.Select(q => new GameArea.GameObjects.GoalField((int)q.x, (int)q.y, DateTime.Now, q.team, q.type)).ToArray();
                    else
                        goals = null;

                    GameArea.GameObjects.Piece[] pieces;
                    if (data.Pieces != null)
                        pieces = data.Pieces.Select(q => new GameArea.GameObjects.Piece(q.id, q.timestamp, q.type, (q.playerIdSpecified ? (long)q.playerId : -1))).ToArray();
                    else
                        pieces = null;

                    GameArea.GameObjects.TaskField[] tasks;
                    if (data.TaskFields != null)
                    {
                        tasks = data.TaskFields.Select(q => new GameArea.GameObjects.TaskField((int)q.x,
                                                                                                (int)q.y,
                                                                                                DateTime.Now,
                                                                                                q.distanceToPiece,
                                                                                                (q.pieceIdSpecified ? new GameArea.GameObjects.Piece(q.pieceId, DateTime.Now) : null),
                                                                                                (q.playerIdSpecified ? new GameArea.GameObjects.Player(q.playerId) : null))).ToArray();
                    }
                    else
                        tasks = null;

                    return new DataAgent(data.playerId)
                    {
                        GameFinished = data.gameFinished,
                        Goals = goals,
                        Pieces = pieces,
                        PlayerGUID = data.playerGuid,
                        PlayerLocation = data.PlayerLocation != null ? new GameArea.GameObjects.Location((int)data.PlayerLocation.x, (int)data.PlayerLocation.y) : null,
                        Tasks = tasks
                    };

                case nameof(Configuration.PlayerSettings):
                    var settings = MessageParser.Deserialize<Configuration.PlayerSettings>(message);
                    return new PlayerSettingsAgent(settings);

                default:
                    return new ErrorMessageAgent("ReadingMessage", "Warning during reading message to server object, type not recognised\n Message read: " + message, "GetObjectFromXML", xmlDoc); //xmlDoc as default for other actions
            }
        }
    }
}
