using GameArea;
using GameArea.AppConfiguration;
using GameArea.AppMessages;
using GameArea.Parsers;
using Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Player
{
    public class PlayerController: IPlayerController
    {
        private TcpClient clientSocket;
        private System.Timers.Timer keppAliveTimer;
        public IPlayer Player { get; set; }
        public AgentState State { get; set; }
        public ActionType ActionToComplete { get; set; }
        public PlayerSettingsConfiguration Settings { get; set; }
        private List<GameArea.GameObjects.GameInfo> GamesList { get; set; }
        TeamColour PrefferedColor { get; set; }
        PlayerRole PrefferedRole { get; set; }

        public PlayerController(IPlayer player)
        {
            clientSocket = new TcpClient();
            Player = player;
        }

        public PlayerController(TeamColour team, PlayerRole role, PlayerSettingsConfiguration settings)
        {
            clientSocket = new TcpClient();
            PrefferedColor = team;
            PrefferedRole = role;
            Settings = settings;
            
        }

        public bool ConnectToServer(IPAddress ip, Int32 port)
        {
            ConsoleWriter.Show("Player connecting to server ...");
            try
            {
                clientSocket.Connect(ip, port);
                InitKeepAliveTimer();
            }
            catch (Exception e)
            {
                ConsoleWriter.Error("Connection to communication server not established with error: \n" + e.ToString());
                return false;
            }
            ConsoleWriter.Show("Player is now connected to server!");
            return true;
        }

        public void StartPerformance()
        {
            ConsoleWriter.Show("Player is ready ...");
            BeginRead();
            //Player.DoStrategy();
            DoInitialStrategy();
        }

        public void DoInitialStrategy()
        {
            while (State != AgentState.Dead)
            {
                switch (State)
                {
                    case AgentState.SearchingForGame:
                        ActionToComplete = ActionType.SearchingForGame;
                        BeginSend(new GetGamesMessage().Serialize());
                        break;
                    case AgentState.Joining:
                        TryJoinGame();
                        break;
                    case AgentState.AwaitingForStart:
                        Player.DoStrategy();
                        break;
                }
                WaitForActionComplete();
            }
        }

        private void TryJoinGame()
        {
            if (GamesList == null || GamesList.Count == 0)
            {
                State = AgentState.SearchingForGame;
                Thread.Sleep(Settings.RetryJoinGameInterval);
                //nie ustawiamy akcji, strategia sama dojdzie do tego co ma zrobić
            }
            else
            {
                var game = GamesList[0];
                GamesList.RemoveAt(0);
                BeginSend(new JoinGameMessage(game.GameName, PrefferedColor, PrefferedRole).Serialize());
                ActionToComplete = ActionType.Joining;
            }
        }

        public void RegisteredGames(RegisteredGamesMessage messageObject)
        {
            GamesList = messageObject.Games?.ToList();
            State = AgentState.Joining;
            ActionToComplete = ActionType.none;
        }

        public void ConfirmJoiningGame(ConfirmJoiningGameMessage info)
        {

            var gameId = info.GameId;
            var id = info.PlayerId; //u nas serwerowe ID i playerId na planszy to jedno i to samo
            var guid = info.GUID;
            var team = info.PlayerDefinition.Team;
            Player = info.PlayerDefinition.Role == PlayerRole.leader ? new Leader(team, PlayerRole.leader, Settings, this, guid) : new Player(team, PlayerRole.leader, Settings, this, guid);
            State = AgentState.AwaitingForStart;
            ActionToComplete = ActionType.none;
        }

        /// <summary>
        /// Awaits for property change.
        /// Returns true if value has changed.
        /// Returns false after timeout.
        /// </summary>
        /// <param name="miliSec">Miliseconds of timeout</param>
        public bool WaitForActionComplete(int miliSec = 5000)
        {
            var result = SpinWait.SpinUntil(() => ActionToComplete == ActionType.none, miliSec);
            if (!result)
            {
                ConsoleWriter.Warning("Waiting for operation complete timeout: " + ActionToComplete);
            }
            return result;
        }


        public void BeginRead()
        {
            var buffer = new byte[25000];
            var ns = clientSocket.GetStream();
            ns.BeginRead(buffer, 0, buffer.Length, EndRead, buffer);
        }

        public void EndRead(IAsyncResult result)
        {
            var buffer = (byte[])result.AsyncState;
            if (clientSocket.Connected)
            {
                try
                {
                    var ns = clientSocket.GetStream();
                    var bytesAvailable = ns.EndRead(result);
                    var messages = Encoding.ASCII.GetString(buffer).Split((char)23);

                    if (messages != null)
                    {
                        foreach (var message in messages.Select(q => q.Trim('\0')))
                        {
                            ConsoleWriter.Show("Agent read: \n" + message + "\n");
                            var msgObject = PlayerReader.GetObjectFromXML(message);
                            if (msgObject != null)
                            {
                                var responseMsgs = msgObject.Process(this);
                                if (responseMsgs != null)
                                    foreach (var msg in responseMsgs)
                                        BeginSend(msg);
                            } 
                            else
                                ConsoleWriter.Warning("Not recognised message object\n Message object is null \n Received message: \n" + message);
                        }
                    }
                    BeginRead();
                }
                catch (Exception e)
                {
                    ConsoleWriter.Error("Error while handling message from communication server." + "\n Error message: \n" + e.ToString() + "\n");
                    Player.State = AgentState.Dead;
                }
            }
            else
            {
                ConsoleWriter.Warning("Communication server connection lost\n");
                Player.State = AgentState.Dead;
            }
        }

        public void BeginSend(string message)
        {
            if (clientSocket.Connected)
            {
                var bytes = Encoding.ASCII.GetBytes(message + (char)23);
                try
                {
                    var ns = clientSocket.GetStream();
                    ns.BeginWrite(bytes, 0, bytes.Length, EndSend, bytes);
                }
                catch (Exception e)
                {
                    ConsoleWriter.Error("Error occured when writing message to socket.\n Error text: \n" + e.ToString());
                    Player.State = AgentState.Dead;
                }
            }
            else
            {
                ConsoleWriter.Warning("Agent socket lost connection.\n");
                Player.State = AgentState.Dead;
            }
        }

        public void EndSend(IAsyncResult result)
        {
            var bytes = (byte[])result.AsyncState;
            ConsoleWriter.Show("Agent sent  " + bytes.Length + " bytes to server\n");
            ConsoleWriter.Show("Agent message: \n" + Encoding.ASCII.GetString(bytes).Trim('\0') + "\n");
        }

        private void InitKeepAliveTimer()
        {
            keppAliveTimer = new System.Timers.Timer((Settings.KeepAliveInterval * 2) / 3); //timer częściej aby serwer przedwcześnie go nie ubił
            keppAliveTimer.Elapsed += KeppAliveTimer_Elapsed; ;
            keppAliveTimer.AutoReset = true;
            keppAliveTimer.Start();
        }

        private void KeppAliveTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (clientSocket.Connected)
                BeginSend("");
            else
                keppAliveTimer.Stop();
        }
    }
}
