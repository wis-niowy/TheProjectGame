using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using GameArea.AppConfiguration;
using Configuration;
using GameMaster.GMMessages;
using Messages;
using GameArea;
using GameArea.AppMessages;

namespace GameMaster.Tests
{
    [TestClass]
    public class GameMasterProcessTests
    {
        GameMasterSettingsConfiguration settings;
        GameArea.GameMaster gameMaster;

        public void InitGameMaster()
        {
            settings = new GameMasterSettingsConfiguration(GameMasterSettings.GetDefaultGameMasterSettings());
            gameMaster = new GameArea.GameMaster(settings);
        }
        public Player.Player GetPlayer(string guid, ulong id, TeamColour tc, ActionType action)
        {
            var player = new Player.Player(tc, _guid: guid);
            player.ID = id;
            player.LastActionTaken = action;

            return player;
        }
        public void EquipPlayerWithPiece(Piece piece, Player.Player player)
        {
            player.SetPiece(new GameArea.GameObjects.Piece(piece));

            gameMaster.RegisterPlayer(player, player.GUID, findFreeLocationAndPlacePlayer: false);

            var pieceUnknown = new Piece(piece.type, piece.id)
            {
                timestamp = piece.timestamp,

            };
            player.SetPiece(new GameArea.GameObjects.Piece(pieceUnknown));
        }

        [TestMethod]
        public void ProcessJoinGameRequest()
        {
            InitGameMaster();
            JoinGameGM data = new JoinGameGM("game", TeamColour.blue, PlayerRole.leader);
            data.Process(gameMaster);
            Assert.AreEqual(1, gameMaster.GetPlayers.Count);
            Assert.AreEqual(1, gameMaster.GetPlayersByTeam(TeamColour.blue).Count);
            Assert.AreEqual(0, gameMaster.GetPlayersByTeam(TeamColour.red).Count);
        }

        [TestMethod]
        public void ProcessPlayerDisconnectedRequest()
        {
            InitGameMaster();
            var player = GetPlayer("testGUID-0001", 10, TeamColour.red, ActionType.TestPiece);
            gameMaster.RegisterPlayer(player);
            GameArea.GameObjects.Location location = gameMaster.GetPlayers[0].Location;

            PlayerDisconnectedGM data = new PlayerDisconnectedGM(10);
            data.Process(gameMaster);

            Assert.AreEqual(0, gameMaster.GetPlayers.Count);
            Assert.AreEqual(false, gameMaster.GetBoard.GetField(location.X, location.Y).HasPlayer());
        }

        [TestMethod]
        public void ProcessHandleTestPieceRequestNormal()
        {
            InitGameMaster();
            var player = GetPlayer("testGUID-0001", 10, TeamColour.red, ActionType.TestPiece);
            //gameMaster.RegisterPlayer(player);
            GameArea.GameObjects.Piece p = gameMaster.GetPieceById(0);
            var messagePieceKnown = new Piece(p.Type, p.ID)
            {
                timestamp = DateTime.Now,
            };
            EquipPlayerWithPiece(messagePieceKnown, player);

            TestPieceGM data = new TestPieceGM("testGUID-0001", 0);
            string[] msg = data.Process(gameMaster);

            DataGM obj = (DataGM)GMReader.GetObjectFromXML(msg[0]);

            Assert.AreEqual(1, obj.Pieces.Length);
            Assert.AreEqual(p.Type, obj.Pieces[0].Type);
        }

        [TestMethod]
        public void ProcessHandleTestPieceRequestNoPiece()
        {
            InitGameMaster();
            var player = GetPlayer("testGUID-0001", 10, TeamColour.red, ActionType.TestPiece);
            gameMaster.RegisterPlayer(player, "testGUID-0001");
            GameArea.GameObjects.Piece p = gameMaster.GetPieceById(0);

            TestPieceGM data = new TestPieceGM("testGUID-0001", 0);
            string[] msg = data.Process(gameMaster);

            DataGM obj = (DataGM)GMReader.GetObjectFromXML(msg[0]);

            Assert.AreEqual(0,obj.Pieces.Length);
        }

        [TestMethod]
        public void ProcessHandleDestroyPieceRequest()
        {
            InitGameMaster();
            var player = GetPlayer("testGUID-0001", 10, TeamColour.red, ActionType.TestPiece);
            GameArea.GameObjects.Piece p = gameMaster.GetPieceById(0);
            ulong id = p.ID;
            var messagePieceKnown = new Piece(p.Type, p.ID)
            {
                timestamp = DateTime.Now,
            };
            EquipPlayerWithPiece(messagePieceKnown, player);

            DestroyPieceGM data = new DestroyPieceGM("testGUID-0001", 0);
            string[] msg = data.Process(gameMaster);

            Assert.IsNull(gameMaster.GetPieceById(id));
        }

        [TestMethod]
        public void ProcessPlacePieceRequestOnTaskField()
        {
            InitGameMaster();
            var player = GetPlayer("testGUID-0001", 10, TeamColour.red, ActionType.TestPiece);
            
            GameArea.GameObjects.Piece p = gameMaster.GetPieceById(0);
            p.Type = PieceType.normal;
            var messagePieceKnown = new Piece(p.Type, p.ID)
            {
                timestamp = DateTime.Now,
            };
            EquipPlayerWithPiece(messagePieceKnown, player);
            player.SetLocation(1, 5);
            gameMaster.SetAbsolutePlayerLocation(1, 5, "testGUID-0001");

            PlacePieceGM data = new PlacePieceGM("testGUID-0001", 0);
            string[] msg = data.Process(gameMaster);

            Assert.AreEqual(p.ID, gameMaster.GetBoard.GetTaskField(player.Location.X, player.Location.Y).Piece.ID);
        }

        [TestMethod]
        public void ProcessPlacePieceRequestOnGoal()
        {
            InitGameMaster();
            var player = GetPlayer("testGUID-0001", 10, TeamColour.blue, ActionType.TestPiece);
            ulong goalsLeft = gameMaster.GoalsBlueLeft;
            GameArea.GameObjects.Piece p = gameMaster.GetPieceById(0);
            p.Type = PieceType.normal;
            var messagePieceKnown = new Piece(p.Type, p.ID)
            {
                timestamp = DateTime.Now,
            };
            EquipPlayerWithPiece(messagePieceKnown, player);
            player.SetLocation(1, 1);
            gameMaster.SetAbsolutePlayerLocation(1, 1, "testGUID-0001");
            gameMaster.GetBoard.GetGoalField(1, 1).Type = GoalFieldType.goal;

            PlacePieceGM data = new PlacePieceGM("testGUID-0001", 0);
            string[] msg = data.Process(gameMaster);

            Assert.AreEqual(goalsLeft-1, gameMaster.GoalsBlueLeft);
        }

        [TestMethod]
        public void ProcessPickUpPieceRequest()
        {
            InitGameMaster();
            var player = GetPlayer("testGUID-0001", 10, TeamColour.blue, ActionType.TestPiece);
            gameMaster.RegisterPlayer(player, "testGUID-0001");
            gameMaster.GetBoard.GetTaskField(1, 5).Piece = new GameArea.GameObjects.Piece(10, DateTime.Now);
            player.SetLocation(1, 5);
            gameMaster.SetAbsolutePlayerLocation(1, 5, "testGUID-0001");

            PickUpPieceGM data = new PickUpPieceGM("testGUID-0001", 0);
            string[] msg = data.Process(gameMaster);

            Assert.IsNull(gameMaster.GetBoard.GetTaskField(1, 5).Piece);
            Assert.AreEqual((uint)10, gameMaster.GetPlayerById(10).GetPiece.ID);
        }

        [TestMethod]
        public void ProcessMoveRequest()
        {
            InitGameMaster();
            var player = GetPlayer("testGUID-0001", 10, TeamColour.blue, ActionType.TestPiece);
            gameMaster.RegisterPlayer(player, "testGUID-0001");
            player.SetLocation(1, 5);
            gameMaster.SetAbsolutePlayerLocation(1, 5, "testGUID-0001");

            MoveGM data = new MoveGM("testGUID-0001", 0, MoveType.down);
            string[] msg = data.Process(gameMaster);
            
            Assert.AreEqual(1, gameMaster.GetPlayerById(10).Location.X);
            Assert.AreEqual(4, gameMaster.GetPlayerById(10).Location.Y);
        }

        [TestMethod]
        public void ProcessDiscoverRequest()
        {
            InitGameMaster();
            var player = GetPlayer("testGUID-0001", 10, TeamColour.blue, ActionType.TestPiece);
            gameMaster.RegisterPlayer(player, "testGUID-0001");
            player.SetLocation(1, 5);
            gameMaster.SetAbsolutePlayerLocation(1, 5, "testGUID-0001");

            DiscoverGM data = new DiscoverGM("testGUID-0001", 0);
            string[] msg = data.Process(gameMaster);
            DataGM obj = (DataGM)GMReader.GetObjectFromXML(msg[0]);

            Assert.AreEqual(8, obj.Tasks.Length);
            Assert.AreEqual(0, obj.Goals.Length);
        }
        
    }
}
