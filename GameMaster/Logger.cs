using GameArea;
using GameMaster.GMMessages;
using Messages;
using Player.PlayerMessages;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Logger
{
    public class Logger
    {
        private StreamWriter sw;
        private string nameGameMasterLogger;
        GameArea.GameMaster gameMaster;

        public Logger(GameArea.GameMaster gameMaster)
        {
            this.gameMaster = gameMaster;
            nameGameMasterLogger = "GMlogs.csv";
        }

        public void Log(GameMasterState state)
        {
            bool isBlueTheWinner = gameMaster.GoalsBlueLeft == 0 ? true : false;
            StreamWriter sw = new StreamWriter(nameGameMasterLogger, true);
            var dt = DateTime.Now;
            String timestamp = String.Format("{0:yyyy-MM-dd}" + "T" + "{1:HH:mm:ss.fff}", dt, dt);
            var gameId = gameMaster.GameId;

            switch (state)
            {
                case GameMasterState.GameOver:
                    break;
                case GameMasterState.GameResolved:
                    foreach (var player in gameMaster.GetPlayers)
                    {
                        bool isPlayerTeamBlue = player.Team == TeamColour.blue ? true : false;
                        string nameMessage = "";
                        if (isBlueTheWinner == isPlayerTeamBlue)
                        {
                            nameMessage = "Victory";
                        }
                        else
                        {
                            nameMessage = "Defeat";
                        }

                        string stringPlayerId = "";
                        string stringPlayerGuid = "";
                        string stringColourPlayer = "";
                        string stringRole = "";

                        if (player != null)
                        {
                            stringPlayerId = player.ID.ToString();
                            stringPlayerGuid = player.GUID.ToString();
                            stringColourPlayer = player.Team.ToString();
                            stringRole = player.Role.ToString();
                        }
                        sw.WriteLine($"{nameMessage},{timestamp},{gameId},{stringPlayerId},{stringPlayerGuid},{stringColourPlayer},{stringRole}");
                    }
                    break;
                default: break;
            }

            sw.Close();
        }

        public void Log(string message)
        {

            if (string.IsNullOrWhiteSpace(message))
            {
                ConsoleWriter.Show("Read empty or whitespace message.");
                return;
            }
            var xmlDoc = new XmlDocument();
            try
            {
                xmlDoc.LoadXml(message);
            }
            catch (Exception e)
            {
                ConsoleWriter.Error("Could not load message to XML. \nMessage content: \n" + message);
                return;
            }
            if (xmlDoc == null)
            {
                ConsoleWriter.Error("Could not load message to XML. \nMessage content: \n" + message);
            }


            var gameId = gameMaster.GameId;

            string playerGuid;
            ulong playerId;
            Player.Player player = null;

            string nameMessage = xmlDoc.DocumentElement.Name;
            switch (nameMessage)
            {
                case nameof(TestPiece):
                    var test = MessageParser.Deserialize<TestPiece>(message);
                    playerGuid = test.playerGuid;
                    player = gameMaster.GetPlayerByGuid(playerGuid);
                    break;
                case nameof(DestroyPiece):
                    var destroy = MessageParser.Deserialize<DestroyPiece>(message);
                    playerGuid = destroy.playerGuid;
                    player = gameMaster.GetPlayerByGuid(playerGuid);
                    break;
                case nameof(PlacePiece):
                    var place = MessageParser.Deserialize<PlacePiece>(message);
                    playerGuid = place.playerGuid;
                    player = gameMaster.GetPlayerByGuid(playerGuid);
                    break;
                case nameof(PickUpPiece):
                    var pick = MessageParser.Deserialize<PickUpPiece>(message);
                    playerGuid = pick.playerGuid;
                    player = gameMaster.GetPlayerByGuid(playerGuid);
                    break;
                case nameof(Move):
                    var move = MessageParser.Deserialize<Move>(message);
                    playerGuid = move.playerGuid;
                    player = gameMaster.GetPlayerByGuid(playerGuid);
                    break;
                case nameof(Discover):
                    var discover = MessageParser.Deserialize<Discover>(message);
                    playerGuid = discover.playerGuid;
                    player = gameMaster.GetPlayerByGuid(playerGuid);
                    break;
                case nameof(RejectGameRegistration):
                    var reject = MessageParser.Deserialize<RejectGameRegistration>(message);
                    break;
                case nameof(ConfirmGameRegistration):
                    var confirm = MessageParser.Deserialize<ConfirmGameRegistration>(message);
                    break;
                case nameof(JoinGame):
                    var join = MessageParser.Deserialize<JoinGame>(message);
                    playerId = join.playerId;
                    player = gameMaster.GetPlayerById(playerId);
                    break;
                case nameof(PlayerDisconnected):
                    var playerDisconnected = MessageParser.Deserialize<PlayerDisconnected>(message);
                    playerId = playerDisconnected.playerId;
                    player = gameMaster.GetPlayerById(playerId);
                    break;

                case nameof(Configuration.GameMasterSettings):
                    var settings = MessageParser.Deserialize<Configuration.GameMasterSettings>(message);
                    break;

                case nameof(AuthorizeKnowledgeExchange):
                    var authMsg = MessageParser.Deserialize<AuthorizeKnowledgeExchange>(message);
                    playerGuid = authMsg.playerGuid;
                    break;
                case nameof(RejectKnowledgeExchange):
                    var rejectExchange = MessageParser.Deserialize<RejectKnowledgeExchange>(message);
                    playerGuid = rejectExchange.playerGuid;
                    player = gameMaster.GetPlayerByGuid(playerGuid);
                    break;
                case nameof(AcceptExchangeRequest):
                    var acceptMsg = MessageParser.Deserialize<AcceptExchangeRequest>(message);
                    playerId = acceptMsg.playerId;
                    player = gameMaster.GetPlayerById(playerId);
                    break;
                // zalatwic roznice typow TaskField i GoalField nizej - najlepiej konstruktory biorace obiekt z namespace Messages
                //case nameof(SuggestAction):
                //    var suggestMsg = MessageParser.Deserialize<SuggestAction>(message);
                //    return new SuggestActionGM(suggestMsg.playerId, suggestMsg.senderPlayerId, suggestMsg.playerGuid, suggestMsg.gameId, suggestMsg.TaskFields, suggestMsg.GoalFields);
                //case nameof(SuggestActionResponse):
                //    var answer = MessageParser.Deserialize<SuggestActionResponse>(message);
                //    return new SuggestActionResponseGM(answer.playerId, answer.senderPlayerId, answer.playerGuid, answer.TaskFields, answer.GoalFields);
                case nameof(Data):
                    var data = MessageParser.Deserialize<Data>(message);
                    playerGuid = data.playerGuid;
                    player = gameMaster.GetPlayerByGuid(playerGuid);
                    break;
                default:
                    ConsoleWriter.Error("Could not load message to XML. \nMessage content: \n" + message);
                    break;
            }

            //mainLogger.Log(decision, description);

            StreamWriter sw = new StreamWriter(nameGameMasterLogger, true);
            var dt = DateTime.Now;
            String timestamp = String.Format("{0:yyyy-MM-dd}" + "T" + "{1:HH:mm:ss.fff}", dt, dt);

            string stringPlayerId = "";
            string stringPlayerGuid = "";
            string stringColourPlayer = "";
            string stringRole = "";

            if (player != null)
            {
                stringPlayerId = player.ID.ToString();
                stringPlayerGuid = player.GUID.ToString();
                stringColourPlayer = player.Team.ToString();
                stringRole = player.Role.ToString();
            }

            sw.WriteLine($"{nameMessage},{timestamp},{gameId},{stringPlayerId},{stringPlayerGuid},{stringColourPlayer},{stringRole}");
            sw.Close();
        }

    }

}
