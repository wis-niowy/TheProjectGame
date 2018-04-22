using GameArea;
using GameArea.Parsers;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace GameMaster
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

        public void ConnectToServer(IPAddress ip, Int32 port)
        {
            Console.WriteLine("GameMaster connecting to server ...");
            clientSocket.Connect(ip, port);
            Console.WriteLine("GameMaster is now connected to server!");
        }

        public void StartPerformance()
        {
            Console.WriteLine("GameMaster is ready ...");
            BeginRead();
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
                    //message = message.Trim('\0');
                    var responseMsg = gameMaster.HandleActionRequest(message);
                    //MessageInterpreter.ReadMessage(message, ID);
                    BeginSend(responseMsg);
                    BeginRead();
                }
                catch (Exception e)
                {
                    ConsoleWriter.Error("Error while handling message from socket for client: " + "ID" + "\n Error message: \n" + e.ToString());
                    //MessageInterpreter.ReadMessage("client disconnected", ID);

                }
            }
            else
            {
                ConsoleWriter.Warning("Client disconnected: " + "ID");
                //MessageInterpreter.ReadMessage("client disconnected", ID);
            }
        }

        public void BeginSend(string message)
        {
            //message = message.Trim('\0');
            var bytes = Encoding.ASCII.GetBytes(message);
            var ns = clientSocket.GetStream();
            ns.BeginWrite(bytes, 0, bytes.Length, EndSend, bytes);
        }

        public void EndSend(IAsyncResult result)
        {
            var bytes = (byte[])result.AsyncState;
            Console.WriteLine("Sent  {0} bytes to server by: " + "ID", bytes.Length);
            Console.WriteLine("Sent: {0}", Encoding.ASCII.GetString(bytes)/*.Trim('\0')*/);
        }
    }

}
