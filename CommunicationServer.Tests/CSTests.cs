using Microsoft.VisualStudio.TestTools.UnitTesting;
using CommunicationServer;
using CommunicationServer.ServerMessages;
using CommunicationServer.ServerObjects;
using CommunicationServer.Interpreters;
using GameArea;
using GameArea.ControllerInterfaces;
using System;
using System.Collections.Generic;

namespace CommunicationServer.Tests
{
    [TestClass]
    public class CSTests
    {
        string GAME_NAME = "TEST_GAME_NAME";
        string PLAYER_GUID = "TEST_GUID";
        ulong PLAYER_ID = 2;
        ulong GAME_ID = 0;

        public GameController InitGameController()
        {
            MainController mainController = InitMainController();
            return mainController.GetGameControllerByName(GAME_NAME);
        }

        public MainController InitMainController()
        {
            MainController controller = new MainController();
            controller.AddClient(new ClientHandleMock(null, PLAYER_ID, null));
            controller.RegisterGame(GAME_NAME, 4, 4, PLAYER_ID);

            return controller;
        }

        public GameArea.GameObjects.Player GetPlayer()
        {
            return new GameArea.GameObjects.Player(new Messages.Player()
            {
                id = PLAYER_ID,
                role = Messages.PlayerRole.leader,
                team = Messages.TeamColour.red
            })
            {
                GUID = PLAYER_GUID
            };
        }

        [TestMethod]
        public void ConfirmJoiningGameServerProcess()
        {
            GameController gameController = InitGameController();
            GameArea.GameObjects.Player gameAreaPlayer = GetPlayer();
            IMessage<IGMController> message = new ConfirmJoiningGameServer(gameController.gameId,
                                                                           gameAreaPlayer,
                                                                           gameAreaPlayer.GUID,
                                                                           gameAreaPlayer.ID,
                                                                           gameAreaPlayer.ID);
            gameController.JoiningAgents.Add(new ClientHandleMock(null, PLAYER_ID, null));
            Assert.AreEqual(0, gameController.Agents.Count);

            // action
            message.Process(gameController);

            // assert
            Assert.AreEqual(0, gameController.JoiningAgents.Count);
            Assert.AreEqual(1, gameController.Agents.Count);
            Assert.AreEqual(PLAYER_ID, gameController.Agents[0].PlayerId);
        }

        [TestMethod]
        public void ErrorMessageServerProcess()
        {
            MainController mainController = InitMainController();
            GameController gameController = InitGameController();
            GameArea.GameObjects.Player gameAreaPlayer = GetPlayer();
            IMessage<IGMController> message1 = new ErrorMessageServer("", "", "", PLAYER_ID, null);
            IMessage<IAgentController> message2 = new ErrorMessageServer("", "", "", PLAYER_ID, null);
            IMessage<IMainController> message3 = new ErrorMessageServer("", "", "", PLAYER_ID, null);
            gameController.JoiningAgents.Add(new ClientHandleMock(null, PLAYER_ID, null));


            // assert
            Assert.ThrowsException<NotImplementedException>(() => message1.Process(gameController));
            Assert.ThrowsException<NotImplementedException>(() => message2.Process(gameController));
            Assert.ThrowsException<NotImplementedException>(() => message3.Process(mainController));
        }

        [TestMethod]
        public void GameMasterDisconnectedServerProcess()
        {
            MainController mainController = InitMainController();
            GameController gameController = mainController.GetGameControllerByName(GAME_NAME);
            GameArea.GameObjects.Player gameAreaPlayer = GetPlayer();
            IMessage<IGMController> message = new GameMasterDisconnectedServer(GAME_ID, PLAYER_ID);
            gameController.Agents.Add(new AGENT(new ClientHandleMock(null, PLAYER_ID, null)));
            gameController.Agents.Add(new AGENT(new ClientHandleMock(null, PLAYER_ID + 1, null)));
            gameController.Agents.Add(new AGENT(new ClientHandleMock(null, PLAYER_ID + 2, null)));
            gameController.JoiningAgents.Add(new ClientHandleMock(null, PLAYER_ID + 10, null));
            gameController.JoiningAgents.Add(new ClientHandleMock(null, PLAYER_ID + 11, null));
            gameController.JoiningAgents.Add(new ClientHandleMock(null, PLAYER_ID + 12, null));
            gameController.State = GameState.InProgress;
            Assert.AreEqual(3, gameController.JoiningAgents.Count);
            Assert.AreEqual(3, gameController.Agents.Count);
            Assert.AreEqual(0, mainController.GetClientsList().Count);

            // action
            message.Process(gameController);

            // assert
            Assert.AreEqual(GameState.Ended, gameController.State);
            Assert.ThrowsException<KeyNotFoundException>(() => mainController.GetGameControllerByName(GAME_NAME));
            Assert.AreEqual(7, mainController.GetClientsList().Count); // 3 joining players + 3 present players + game master
        }

        [TestMethod]
        public void GameStartedServerProcess()
        {
            MainController mainController = InitMainController();
            GameController gameController = mainController.GetGameControllerByName(GAME_NAME);
            GameArea.GameObjects.Player gameAreaPlayer = GetPlayer();
            IMessage<IGMController> message = new GameStartedServer(GAME_ID, PLAYER_ID);

            Assert.AreNotEqual(GameState.InProgress, gameController.State);

            // action
            message.Process(gameController);

            // assert
            Assert.AreEqual(GameState.InProgress, gameController.State);
        }

        [TestMethod]
        public void JoinGameServerProcess()
        {
            MainController mainController = InitMainController();
            GameController gameController = mainController.GetGameControllerByName(GAME_NAME);
            GameArea.GameObjects.Player gameAreaPlayer = GetPlayer();
            IMessage<IMainController> message = new JoinGameServer(GAME_NAME,
                                                                 gameAreaPlayer.Team,
                                                                 gameAreaPlayer.Role,
                                                                 gameAreaPlayer.ID,
                                                                 (long)gameAreaPlayer.ID);

            mainController.InsertClient(new ClientHandleMock(null, PLAYER_ID, null));

            // action
            message.Process(mainController);

            // assert
            Assert.AreEqual(0, mainController.GetClientsList().Count);
            Assert.AreEqual(1, gameController.JoiningAgents.Count);
            Assert.AreEqual(PLAYER_ID, gameController.JoiningAgents[0].ID);
        }

        [TestMethod]
        public void RegisterGameServerProcess()
        {
            MainController mainController = InitMainController();
            GameController gameController = mainController.GetGameControllerByName(GAME_NAME);
            GameArea.GameObjects.Player gameAreaPlayer = GetPlayer();
            IMessage<IMainController> message = new RegisterGameServer(GAME_NAME, 4, 4, PLAYER_ID);

            mainController.InsertClient(new ClientHandleMock(null, PLAYER_ID, null));

            // action
            message.Process(mainController);

            // assert
            Assert.IsNotNull(gameController.GameMaster);
            Assert.IsNotNull(gameController.GameInfo);
            Assert.AreEqual(GAME_NAME, gameController.GameInfo.GameName);
            Assert.AreEqual(4ul, gameController.GameInfo.BlueTeamPlayers);
            Assert.AreEqual(4ul, gameController.GameInfo.RedTeamPlayers);
        }



    }

}
