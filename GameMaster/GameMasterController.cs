using GameArea;
using GameArea.Parsers;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace GameMasterMain
{
    public class GameMasterController
    {
        private TcpClient clientSocket;
        private IGameMaster gameMaster;

        public IGameMaster GameMaster
        {
            get
            {
                return gameMaster;
            }
            set
            {
                gameMaster = value;
            }
        }

        public GameMasterController(IGameMaster gm)
        {
            clientSocket = new TcpClient();
            GameMaster = gm;
        }

        public bool ConnectToServer(IPAddress ip, Int32 port)
        {
            ConsoleWriter.Show("GameMaster connecting to server ...");
            try
            {
                clientSocket.Connect(ip, port);
            }
            catch (Exception e)
            {
                ConsoleWriter.Error("Connection to communication server not established with error: \n" + e.ToString());
                return false;
            }
            ConsoleWriter.Show("GameMaster is now connected to server!");
            return true;
        }

        public void StartPerformance()
        {
            ConsoleWriter.Show("GameMaster is ready ...");
            BeginRead();
            while (gameMaster.State != GameMasterState.GameOver);
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
                    Task.Run(() =>
                    {
                        var responseMsgs = gameMaster.HandleActionRequest(message);
                        foreach(var msg in responseMsgs)
                            BeginSend(msg);
                    });
                    BeginRead();
                }
                catch (Exception e)
                {
                    ConsoleWriter.Error("Error while handling message from communication server." + "\n Error message: \n" + e.ToString());
                }
            }
            else
            {
                ConsoleWriter.Warning("Communication server connection lost");
            }
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
                ConsoleWriter.Warning("GameMaster tries to send null message.");
        }

        public void EndSend(IAsyncResult result)
        {
            var bytes = (byte[])result.AsyncState;
            ConsoleWriter.Show("Sent  " + bytes.Length + " bytes to server:");
            ConsoleWriter.Show("Sent: " + Encoding.ASCII.GetString(bytes).Trim('\0'));
        }
    }

}
