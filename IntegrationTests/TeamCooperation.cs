﻿using GameArea.GameObjects;
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
using System.Diagnostics;
using System.IO;

namespace IntegrationTests
{
    [TestClass]
    public class TeamCooperation
    {
        /// <summary>
        /// Test nawiązania połączenia
        /// CS - nasz
        /// GM - Rysiek
        /// </summary>
        [TestMethod]
        public void GMfromTeamA()
        {
            var ip = IPAddress.Parse("127.0.0.1");
            Int32 port = 5678;

            CS.TcpHelper.StartServer(ip, port);
            Thread t = new Thread(new ThreadStart(CS.TcpHelper.Listen));
            t.Start();
            Thread.Sleep(400);
            var InitialControlersCount = CS.TcpHelper.manager.defaultController.gameDefinitions.Count;
            Assert.AreEqual(0, InitialControlersCount);

            Thread t2 = new Thread(new ParameterizedThreadStart(ExecuteCommand));
            t2.Start("Teams/A/runGM.bat");
            Thread.Sleep(4000);

            var controlersCount = CS.TcpHelper.manager.defaultController.gameDefinitions.Count;
            Assert.AreEqual(1, controlersCount);
        }

        /// <summary>
        /// Test nawiązania połączenia 
        /// CS - Rysiek
        /// GM - nasz
        /// </summary>
        [TestMethod]
        public void CSfromTeamA()
        {
            var ip = IPAddress.Parse("127.0.0.1");
            Int32 port = 5678;
            var defaultSettings = new GameMasterSettingsConfiguration(GameMasterSettings.GetDefaultGameMasterSettings());
            var gameMaster = new GameArea.GameMaster(defaultSettings);

            Thread t2 = new Thread(new ParameterizedThreadStart(ExecuteCommand));
            t2.Start("Teams/A/runServer.ps1");
            Thread.Sleep(2000);

            bool isConnected = MainGameMaster.StartGameMaster(gameMaster, ip, port);
            Thread t = new Thread(new ThreadStart(MainGameMaster.GMController.StartPerformance));
            t.Start();
            Thread.Sleep(2000);

            Assert.AreEqual(GameMasterState.AwaitingPlayers, gameMaster.State);
        }
        /// <summary>
        /// Test nawiązania połączenia
        /// CS - Swierzaki
        /// GM - Nasz
        /// </summary>
        [TestMethod]
        public void CSfromTeamB()
        {
            var ip = IPAddress.Parse("127.0.0.1");
            Int32 port = 5678;
            var defaultSettings = new GameMasterSettingsConfiguration(GameMasterSettings.GetDefaultGameMasterSettings());
            var gameMaster = new GameArea.GameMaster(defaultSettings);

            Thread t2 = new Thread(new ParameterizedThreadStart(ExecuteCommand));
            t2.Start("Teams/B/runCS.bat");
            Thread.Sleep(2000);

            bool isConnected = MainGameMaster.StartGameMaster(gameMaster, ip, port);
            Thread t = new Thread(new ThreadStart(MainGameMaster.GMController.StartPerformance));
            t.Start();
            Thread.Sleep(2000);

            Assert.AreEqual(GameMasterState.AwaitingPlayers, gameMaster.State);
        }

        /// <summary>
        /// Test nawiązania połączenia
        /// CS - Nasz
        /// GM - Swierzaki
        /// </summary>
        [TestMethod]
        public void GMfromTeamB()
        {
            var ip = IPAddress.Parse("127.0.0.1");
            Int32 port = 5678;

            CS.TcpHelper.StartServer(ip, port);
            Thread t = new Thread(new ThreadStart(CS.TcpHelper.Listen));
            t.Start();
            Thread.Sleep(400);
            var InitialControlersCount = CS.TcpHelper.manager.defaultController.gameDefinitions.Count;
            Assert.AreEqual(0, InitialControlersCount);
            Thread.Sleep(4000);

            Thread t2 = new Thread(new ParameterizedThreadStart(ExecuteCommand));
            t2.Start("Teams/B/runGM.bat");
            Thread.Sleep(2000);

            


            var controlersCount = CS.TcpHelper.manager.defaultController.gameDefinitions.Count;
            Assert.AreEqual(1, controlersCount);
        }

        /// <summary>
        /// Test nawiązania połączenia
        /// CS - Nasz
        /// GM - Swierzaki
        /// Player - Swierzaki
        /// </summary>
        [TestMethod]
        public void GMandPlayerfromTeamB()
        {
            var ip = IPAddress.Parse("127.0.0.1");
            Int32 port = 5678;


            CS.TcpHelper.StartServer(ip, port);
            Thread t = new Thread(new ThreadStart(CS.TcpHelper.Listen));
            t.Start();
            Thread.Sleep(400);

            Thread t2 = new Thread(new ParameterizedThreadStart(ExecuteCommand));
            t2.Start("Teams/B/runGM.bat");
            Thread.Sleep(2000);

            

            Thread t3 = new Thread(new ParameterizedThreadStart(ExecuteCommand));
            t3.Start("Teams/B/bluePlayerLeader.bat");
            

            Thread.Sleep(2000);


            var controler = CS.TcpHelper.manager.defaultController.GetGameControllerByName("Endgame");
            int countClients = controler.Agents.Count + 1; 
            Assert.AreEqual(2, countClients);
        }




        /// <summary>
        /// Funkcja pomocnicza, startuje proces .bat lub .ps1
        /// </summary>
        /// <param name="_name">Nazwa skryptu. Za cwd przyjmujemy ścieżkę do solucji</param>
        static void ExecuteCommand(object _name)
        {
            var name = _name as string;
            var str = "../../../../" + name; // current cwd = <solution>/<project_name>/bin/debug
            var exactPath = Path.GetFullPath(str);

            if (File.Exists(exactPath))
            {
                ProcessStartInfo processInfo;
                Process proc;
                processInfo = new ProcessStartInfo(exactPath);
                processInfo.WorkingDirectory = Path.GetDirectoryName(str);
                processInfo.CreateNoWindow = false;
                processInfo.UseShellExecute = true;
                proc = Process.Start(processInfo);
               
                Thread.Sleep(10000);
                proc.Kill();
            }
            else throw new FileNotFoundException();
                
        }

    }
}