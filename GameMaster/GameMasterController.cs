using GameArea;
using GameArea.Parsers;
using GameMaster;
using Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GameMasterMain
{
    public class GameMasterController
    {
        private TcpClient clientSocket;
        private System.Timers.Timer keppAliveTimer;
        public IGameMaster GameMaster { get; set; }
        private Logger.Logger logger;
        private MessageManager messageManager;

        public GameMasterController(IGameMaster gm)
        {
            var GM = gm as GameArea.GameMaster;
            logger = new Logger.Logger(GM);
            clientSocket = new TcpClient();
            GameMaster = gm;
            messageManager = new MessageManager(this);
        }

        public bool ConnectToServer(IPAddress ip, Int32 port)
        {
            ConsoleWriter.Show("GameMaster connecting to server ...");
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
            ConsoleWriter.Show("GameMaster is now connected to server!");
            return true;
        }

        public void StartPerformance()
        {
            ConsoleWriter.Show("GameMaster is ready ...");
            BeginRead();
            BeginSend(GameMaster.RegisterGame().Serialize());
            SpinWait.SpinUntil(() => GameMaster.State == GameMasterState.GameOver);
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
                            logger.Log(message);
                            ConsoleWriter.Show("GameMaster read: \n" + message + "\n");
                            var msgObject = GMReader.GetObjectFromXML(message);
                            if (msgObject != null)
                                messageManager.ProcessMessage(msgObject);
                            else
                                ConsoleWriter.Warning("Could not obtain object from message: \n" + message);
                        }
                    }
                    BeginRead();
                }
                catch (Exception e)
                {
                    ConsoleWriter.Error("Error while handling message from communication server." + "\n Error message: \n" + e.ToString() + "\n");
                    GameMaster.State = GameMasterState.GameOver;
                }
            }
            else
            {
                ConsoleWriter.Warning("Communication server connection lost\n");
                GameMaster.State = GameMasterState.GameOver;
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
                    ConsoleWriter.Error("Error during BeginSend operation.\nErrorMessage:" + e.Message + "\nStackTrace: " + e.StackTrace);
                    GameMaster.State = GameMasterState.GameOver;
                }
            }
            else
            {
                ConsoleWriter.Warning("GameMaster socket lost connection.\n");
                GameMaster.State = GameMasterState.GameOver;
            }
        }

        public void EndSend(IAsyncResult result)
        {
            var bytes = (byte[])result.AsyncState;
            ConsoleWriter.Show("GameMaster sent  " + bytes.Length + " bytes to server");
            ConsoleWriter.Show("GameMaster sent: \n" + Encoding.ASCII.GetString(bytes).Trim('\0') + "\n");
        }

        private void InitKeepAliveTimer()
        {
            keppAliveTimer = new System.Timers.Timer((GameMaster.Settings.KeepAliveInterval * 2) / 3);//timer częściej aby serwer przedwcześnie go nie ubił
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
