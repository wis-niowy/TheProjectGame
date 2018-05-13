using GameArea.GameObjects;
using GameArea.AppConfiguration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Configuration;
using System.Threading;
using GameMasterMain;
using GameMaster;
using System.Net;
using System;
using CommunicationServer;
using Player;
using Messages;
using System.Threading.Tasks;
using GameArea;

namespace IntegrationTests
{
    [TestClass]
    public class IntegrationTest
    {

        GameMasterSettingsConfiguration defaultSettings;
        GameArea.GameMaster defaultGameMaster;

        public void InitGameMaster()
        {
            defaultSettings = new GameMasterSettingsConfiguration(GameMasterSettings.GetDefaultGameMasterSettings());
            defaultGameMaster = new GameArea.GameMaster(defaultSettings);
        }

        [TestMethod]
        public void ConnectingGMtoCS()
        {

            InitGameMaster();

            var ip = IPAddress.Parse("127.0.0.1");
            Int32 port = 2000;

            CS.TcpHelper.StartServer(ip, port);

            //Thread.Sleep(500);

            bool isConnected = MainGameMaster.StartGameMaster(ip, port, defaultSettings);


            Assert.AreEqual(true, isConnected);
        }
        [TestMethod]
        public void ConnectingAgentstoCS()
        {
            var ip = IPAddress.Parse("127.0.0.2");
            Int32 port = 2000;

            CS.TcpHelper.StartServer(ip, port);


            bool isConnectedGM = MainGameMaster.StartGameMaster(ip, port, defaultSettings);

            var player = new Player.Player(TeamColour.blue);

            bool isConnectedAgent = MainPlayer.StartPlayer(ip, port, new PlayerSettingsConfiguration(5000), TeamColour.blue, PlayerRole.member);

            Assert.AreEqual(true, isConnectedGM);
            Assert.AreEqual(true, isConnectedAgent);
        }


        [TestMethod]
        public void ConnectingAgentsAndGMtoCS()
        {

            InitGameMaster();

            var ip = IPAddress.Parse("127.0.0.3");
            Int32 port = 2000;

            CS.TcpHelper.StartServer(ip, port);

            //Thread.Sleep(500);

            var player = new Player.Player(TeamColour.blue);

            bool isConnected = MainPlayer.StartPlayer(ip, port, new PlayerSettingsConfiguration(5000), TeamColour.blue, PlayerRole.member);

            Assert.AreEqual(true, isConnected);
        }


        [TestMethod]
        public void ConfirmMessageFromGMToCM()
        {
            InitGameMaster();

            var ip = IPAddress.Parse("127.0.0.3");
            Int32 port = 2002;


            CS.TcpHelper.StartServer(ip, port);
            Task.Run(() => CS.TcpHelper.Listen());



            //Thread.Sleep(500);

            bool isConnectedGM = MainGameMaster.StartGameMaster(ip, port, defaultSettings);

            Task taskGameMaster = Task.Run(() => MainGameMaster.GMController.StartPerformance());


            Thread.Sleep(500);


            Assert.AreEqual(GameMasterState.AwaitingPlayers, MainGameMaster.GMController.GameMaster.State);

        }


        [TestMethod]
        public void SendingMoveMessageFromPlayer()
        {

            InitGameMaster();

            var ip = IPAddress.Parse("127.0.0.3");
            Int32 port = 2001;


            CS.TcpHelper.StartServer(ip, port);
            Task.Run(() => CS.TcpHelper.Listen());
            
            

            //Thread.Sleep(500);

            bool isConnectedGM = MainGameMaster.StartGameMaster(ip, port, defaultSettings);

            Task.Run(() => MainGameMaster.GMController.StartPerformance());

            PlayerController playerController;


            bool isConnectedAgent = MainPlayer.TestStartPlayer(ip, port, new PlayerSettingsConfiguration(5000), TeamColour.blue,out playerController);

            var player = playerController.Player as Player.Player;

           
            bool isActionCompleted = player.Move(MoveType.up);

            Thread.Sleep(30 * 1000);


            Assert.AreEqual(true, isActionCompleted);
        }








    }
}
