using GameArea;
using GameArea.Parsers;
using Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Player
{
    public class PlayerController
    {
        private TcpClient clientSocket;

        public IPlayer Player { get; set; }

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
                            if(msgObject != null)
                                msgObject.Process(Player);
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
    }
}
