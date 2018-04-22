using GameArea;
using GameArea.Parsers;
using Messages;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Player
{
    public class PlayerController
    {
        private TcpClient clientSocket;
        private IPlayer player;

        public IPlayer Player
        {
            get
            {
                return player;
            }
            set
            {
                player = value;
            }
        }

        public PlayerController(IPlayer player)
        {
            clientSocket = new TcpClient();
            Player = player;
        }

        public bool ConnectToServer(IPAddress ip, Int32 port)
        {
            ConsoleWriter.Show("Player connecting to server ...");
            try
            {
                clientSocket.Connect(ip, port);
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
            Player.DoStrategy();
            while(Player.State != AgentState.Dead)
            {
                
            }
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
                    var message = Encoding.ASCII.GetString(buffer);
                    message = message.Trim('\0');
                    ConsoleWriter.Show("Agent read: \n" + message + "\n");
                    DoAgentLogic(message);
                    BeginRead();
                }
                catch (Exception e)
                {
                    ConsoleWriter.Error("Error while handling message from communication server." + "\n Error message: \n" + e.ToString() + "\n");
                }
            }
            else
            {
                ConsoleWriter.Warning("Communication server connection lost\n");
            }
        }

        private void DoAgentLogic(string message)
        {
            object messageObject = new object();

            messageObject = MessageParser.Deserialize(message);//message must be without any \0 characters

            switch (messageObject.GetType().Name)
            {
                case nameof(RegisteredGames):
                    Player.RegisteredGames( (RegisteredGames)messageObject);
                    Player.DoStrategy();
                    break;
                case nameof(ConfirmJoiningGame):
                    Player.ConfirmJoiningGame((ConfirmJoiningGame)messageObject);
                    break;
                case nameof(RejectJoiningGame): //gracz nie połączył się z daną grą, próbuj do kolejnej z listy
                    Player.DoStrategy();
                    break;
                case nameof(Game):
                    Player.GameStarted((Game)messageObject);
                    Player.DoStrategy();
                    break;
                case nameof(GameMasterDisconnected):
                    Player.GameMasterDisconnected((GameMasterDisconnected)messageObject);
                    Player.DoStrategy();
                    break;
                case nameof(Data):
                    Player.UpdateLocalBoard((Data)messageObject, (ActionType)Player.LastActionTaken, (MoveType)Player.LastMoveTaken); //update związany z ostatnią wykonaną akcją
                    Player.DoStrategy();
                    break;

                default:
                    ConsoleWriter.Warning("Unknown message received by Player \nReceived message:\n" + message + "\n");
                    break;
            }
            ConsoleWriter.Show("Player received message of type: " + messageObject.GetType().Name + "\n");
        }

        public void BeginSend(string message)
        {
            if (message != null)
            {
                var bytes = Encoding.ASCII.GetBytes(message);
                var ns = clientSocket.GetStream();
                ns.BeginWrite(bytes, 0, bytes.Length, EndSend, bytes);
            }
            else
                ConsoleWriter.Warning("Agent tries to send null message. \n");
        }

        public void EndSend(IAsyncResult result)
        {
            var bytes = (byte[])result.AsyncState;
            ConsoleWriter.Show("Agent sent  " + bytes.Length + " bytes to server\n");
            ConsoleWriter.Show("Agent message: \n" + Encoding.ASCII.GetString(bytes).Trim('\0') + "\n");
        }
    }
}
