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
using System.Threading;

namespace IntegrationTests
{
    [TestClass]
    public class IntegrationTest
    {

        GameMasterSettingsConfiguration defaultSettings;
        volatile GameArea.GameMaster gameMaster;

        public void InitGameMaster()
        {
            defaultSettings = new GameMasterSettingsConfiguration(GameMasterSettings.GetDefaultGameMasterSettings());
            gameMaster = new GameArea.GameMaster(defaultSettings);
        }

        [TestMethod]
        public void ConnectingGMtoCS()
        {
            InitGameMaster();
            var ip = IPAddress.Parse("127.0.0.1");
            Int32 port = 2000;

            CS.TcpHelper.StartServer(ip, port);
            Thread t = new Thread(new ThreadStart(CS.TcpHelper.Listen));
            t.Start();

            bool isConnected = MainGameMaster.StartGameMaster(gameMaster, ip, port);
            Thread t2 = new Thread(new ThreadStart(MainGameMaster.GMController.StartPerformance));
            t2.Start();

            //CS.TcpHelper.StopListening(t);
            Thread.Sleep(200);

            int count = CS.TcpHelper.manager.defaultController.clients.Count;
            Assert.IsTrue(isConnected);
            Assert.AreEqual(GameMasterState.AwaitingPlayers, gameMaster.State);
        }

        [TestMethod]
        public void ConnectingAgentToCS()
        {
            var ip = IPAddress.Parse("127.0.0.2");
            Int32 port = 2000;

            CS.TcpHelper.StartServer(ip, port);
            Thread t = new Thread(new ThreadStart(CS.TcpHelper.Listen));
            t.Start();

            bool isConnectedAgent = MainPlayer.StartPlayerController(ip, port, new PlayerSettingsConfiguration(5000), TeamColour.blue, PlayerRole.member);

            Thread.Sleep(500);

            int count = CS.TcpHelper.manager.defaultController.clients.Count;
            Assert.AreEqual(1, count);
            Assert.AreEqual(true, isConnectedAgent);
        }


        [TestMethod]
        public void TerminatingAgent()
        {
            var ip = IPAddress.Parse("127.0.0.3");
            Int32 port = 2000;

            InitGameMaster();

            CS.TcpHelper.StartServer(ip, port);
            Thread t = new Thread(new ThreadStart(CS.TcpHelper.Listen));
            t.Start();
            Thread.Sleep(250);

            bool isConnected = MainGameMaster.StartGameMaster(gameMaster, ip, port);
            Thread t2 = new Thread(new ThreadStart(MainGameMaster.GMController.StartPerformance));
            t2.Start();
            Thread.Sleep(250);

            bool isConnectedAgent = MainPlayer.TestStartPlayer(ip, port, new PlayerSettingsConfiguration(5000), TeamColour.blue, out PlayerController PC);
            Thread t3 = new Thread(new ThreadStart(PC.StartPerformance));
            t3.Start();
            Thread.Sleep(500);

            Assert.IsTrue(isConnected);
            Assert.IsTrue(isConnectedAgent);
            t3.Interrupt();

            Thread.Sleep(500);
            Assert.AreEqual(0, CS.TcpHelper.manager.defaultController.clients.Count);
        }


        [TestMethod]
        public void ConnectingAgentAndGMtoCS()
        {
            var ip = IPAddress.Parse("127.0.0.4");
            Int32 port = 2000;

            InitGameMaster();

            CS.TcpHelper.StartServer(ip, port);
            Thread t = new Thread(new ThreadStart(CS.TcpHelper.Listen));
            t.Start();
            Thread.Sleep(250);

            bool isConnected = MainGameMaster.StartGameMaster(gameMaster, ip, port);
            Thread t2 = new Thread(new ThreadStart(MainGameMaster.GMController.StartPerformance));
            t2.Start();
            Thread.Sleep(250);

            var player = new Player.Player(TeamColour.blue);
            bool isConnectedAgent = MainPlayer.TestStartPlayer(ip, port, new PlayerSettingsConfiguration(5000), TeamColour.blue, out PlayerController PC);
            Thread t3 = new Thread(new ThreadStart(PC.StartPerformance));
            t3.Start();
            Thread.Sleep(500);

            Assert.IsTrue(isConnected);
            Assert.IsTrue(isConnectedAgent);
            Assert.AreEqual(AgentState.AwaitingForStart, PC.Player.State);
        }

        [TestMethod]
        public void GMterminates()
        {
            var ip = IPAddress.Parse("127.0.0.5");
            Int32 port = 2000;

            InitGameMaster();

            CS.TcpHelper.StartServer(ip, port);
            Thread t = new Thread(new ThreadStart(CS.TcpHelper.Listen));
            t.Start();
            Thread.Sleep(250);

            bool isConnected = MainGameMaster.StartGameMaster(gameMaster, ip, port);
            Thread t2 = new Thread(new ThreadStart(MainGameMaster.GMController.StartPerformance));
            t2.Start();
            Thread.Sleep(250);

            var player = new Player.Player(TeamColour.blue);
            bool isConnectedAgent = MainPlayer.TestStartPlayer(ip, port, new PlayerSettingsConfiguration(500), TeamColour.blue, out PlayerController PC);
            Thread t3 = new Thread(new ThreadStart(PC.StartPerformance));
            t3.Start();
            Thread.Sleep(250);

            Assert.IsTrue(isConnected);
            Assert.IsTrue(isConnectedAgent);

            // zabijamy GM
            gameMaster.State = GameMasterState.GameOver;
            Thread.Sleep(1000);
            Assert.AreEqual(0, CS.TcpHelper.manager.defaultController.clients.Count);
        }

    }
}
