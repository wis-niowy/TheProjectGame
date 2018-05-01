using Microsoft.VisualStudio.TestTools.UnitTesting;
using GameArea;
using Messages;
using System;
using Configuration;

namespace Player.Tests
{
    [TestClass]
    public class PlayerTests
    {
        GameMasterSettings settings = GameMasterSettings.GetDefaultGameMasterSettings();
        GameArea.GameMaster gameMaster = new GameArea.GameMaster(GameMasterSettings.GetDefaultGameMasterSettings());

        [TestMethod]
        public void NewPlayer()
        {
            var Player = new Player(TeamColour.blue);
            Assert.IsNotNull(Player);
            Assert.AreEqual(0, Player.GetLocation.x);
            Assert.AreEqual(0, Player.GetLocation.y);
            Assert.AreEqual(TeamColour.blue, Player.Team);
            Assert.AreEqual("TEST_GUID", Player.GUID);
        }

        [TestMethod]
        public void GuidSet()
        {
            var Player = new Player(TeamColour.blue);
            Player.SetGuid("kakao");
            Assert.AreEqual("kakao", Player.GUID);
        }

        [TestMethod]
        public void BoardSet()
        {
            var board = new Board(2, 2, 2);
            var Player = new Player(TeamColour.blue);
            Assert.IsNull(Player.GetBoard);
            Player.SetBoard(board);
            Assert.IsNotNull(Player.GetBoard);
            Assert.AreSame(board, Player.GetBoard);
        }

        [TestMethod]
        public void SetLocation()
        {
            var Player = new Player(TeamColour.blue);
            var newLocation = new Location(2, 3);
            Player.SetLocation(newLocation);
            Assert.AreEqual(newLocation.x, Player.GetLocation.x);
            Assert.AreEqual(newLocation.y, Player.GetLocation.y);
        }

        // actions tests

        [TestMethod]
        public void PlayerWithoutPieceTestsPiece()
        {
            var settings = GameMasterSettings.GetDefaultGameMasterSettings();
            var gameMaster = new GameArea.GameMaster(settings);
            var Player = new Player(TeamColour.red, "testGUID-0000");

            gameMaster.RegisterPlayer(Player,Player.GUID, findFreeLocationAndPlacePlayer : false);

            var testResult = Player.TestPiece(gameMaster);

            Assert.AreEqual(false, testResult);
        }

        [TestMethod]
        public void PlayerWithShamPieceTestsPiece()
        {
            var settings = GameMasterSettings.GetDefaultGameMasterSettings();
            var gameMaster = new GameArea.GameMaster(settings);
            var Player = new Player(TeamColour.red, "testGUID-0001");
            // equip an Player with a sham piece
            Player.SetPiece(new Piece(PieceType.sham, 100)
            {
                timestamp = DateTime.Now,

            });
            gameMaster.RegisterPlayer(Player,Player.GUID, findFreeLocationAndPlacePlayer : false);
            Player.SetPiece(new Piece(PieceType.unknown, 100)
            {
                timestamp = DateTime.Now,

            });


            var testResult = Player.TestPiece(gameMaster);

            Assert.AreEqual(true, testResult);
            Assert.AreEqual(PieceType.sham, Player.GetPiece.type);
        }

        [TestMethod]
        public void PlayerWithNormalPieceTestsPiece()
        {
            var settings = GameMasterSettings.GetDefaultGameMasterSettings();
            var gameMaster = new GameArea.GameMaster(settings);
            var Player = new Player(TeamColour.blue, "testGUID-0002");
            // equip an Player with a normal piece
            Player.SetPiece(new Piece(PieceType.normal, 90)
            {
                timestamp = DateTime.Now,

            });
            gameMaster.RegisterPlayer(Player,Player.GUID, findFreeLocationAndPlacePlayer : false);
            Player.SetPiece(new Piece(PieceType.unknown, 90)
            {
                timestamp = DateTime.Now,

            });


            var testResult = Player.TestPiece(gameMaster);

            Assert.AreEqual(true, testResult);
            Assert.AreEqual(PieceType.normal, Player.GetPiece.type);
        }


        [TestMethod]
        public void PlayerPlacesShamPieceOnNotOccupiedTaskField()
        {
            var settings = GameMasterSettings.GetDefaultGameMasterSettings();
            var gameMaster = new GameArea.GameMaster(settings);
            var Player = new Player(TeamColour.blue, "testGUID-0003");
            // equip an Player with a sham piece
            Player.SetPiece(new Piece(PieceType.sham, 90)
            {
                timestamp = DateTime.Now,

            });
            gameMaster.RegisterPlayer(Player,Player.GUID, findFreeLocationAndPlacePlayer : false);
            Player.SetPiece(new Piece(PieceType.unknown, 90)
            {
                timestamp = DateTime.Now,
            });
            Player.SetLocation(1, 5); // we change a location of an original object

            // set an Player on a TaskField
            var setPositionResult = gameMaster.SetAbsolutePlayerLocation(1, 5, "testGUID-0003"); // we change a location of GM's copy

            // action: Player places a piece
            var actionResult = Player.PlacePiece(gameMaster);

            Assert.AreEqual(true, setPositionResult);
            Assert.AreEqual(true, actionResult);
            Assert.IsNull(Player.GetPiece);
            Assert.IsNotNull((gameMaster.GetBoard.GetField(1, 5) as GameArea.TaskField).GetPiece);
            Assert.AreEqual(90ul, (gameMaster.GetBoard.GetField(1, 5) as GameArea.TaskField).GetPiece.id);
        }

        [TestMethod]
        public void PlayerPlacesNormalPieceOnNotOccupiedTaskField()
        {
            var settings = GameMasterSettings.GetDefaultGameMasterSettings();
            var gameMaster = new GameArea.GameMaster(settings);
            var Player = new Player(TeamColour.blue, "testGUID-0004");
            // equip an Player with a sham piece
            Player.SetPiece(new Piece(PieceType.normal, 90)
            {
                timestamp = DateTime.Now,

            });
            gameMaster.RegisterPlayer(Player,Player.GUID, findFreeLocationAndPlacePlayer : false);
            Player.SetPiece(new Piece(PieceType.unknown, 90)
            {
                timestamp = DateTime.Now,
            });
            Player.SetLocation(1, 5); // we change a location of an original object

            // set an Player on a TaskField
            var setPositionResult = gameMaster.SetAbsolutePlayerLocation(1, 5, "testGUID-0004"); // we change a location of GM's copy
            // assure there is no piece on the field
            gameMaster.GetBoard.GetTaskField(1, 5).SetPiece(null);

            // action: Player places a piece
            var actionResult = Player.PlacePiece(gameMaster);

            Assert.AreEqual(true, setPositionResult);
            Assert.AreEqual(true, actionResult);
            Assert.IsNull(Player.GetPiece);
            Assert.IsNotNull((gameMaster.GetBoard.GetField(1, 5) as GameArea.TaskField).GetPiece);
            Assert.AreEqual(90ul, (gameMaster.GetBoard.GetField(1, 5) as GameArea.TaskField).GetPiece.id);
        }

        [TestMethod]
        public void PlayerPlacesShamPieceOnGoalField()
        {
            var settings = GameMasterSettings.GetDefaultGameMasterSettings();
            var gameMaster = new GameArea.GameMaster(settings);
            var Player = new Player(TeamColour.blue, "testGUID-0005");
            // equip an Player with a sham piece
            Player.SetPiece(new Piece(PieceType.sham, 70)
            {
                timestamp = DateTime.Now,

            });
            gameMaster.RegisterPlayer(Player,Player.GUID, findFreeLocationAndPlacePlayer : false);
            Player.SetPiece(new Piece(PieceType.unknown, 70)
            {
                timestamp = DateTime.Now,
            });
            Player.SetLocation(1, 2); // we change a location of an original object

            // set an Player on a TaskField
            var setPositionResult = gameMaster.SetAbsolutePlayerLocation(1, 2, "testGUID-0005"); // we change a location of GM's copy

            // action: Player places a piece
            var actionResult = Player.PlacePiece(gameMaster);

            Assert.AreEqual(true, setPositionResult);
            Assert.AreEqual(false, actionResult);
            Assert.IsNotNull(Player.GetPiece);
        }

        [TestMethod]
        public void PlayerPlacesNormalPieceOnGoalFieldOfTypeGoal()
        {
            var settings = GameMasterSettings.GetDefaultGameMasterSettings();
            var gameMaster = new GameArea.GameMaster(settings);
            var Player = new Player(TeamColour.blue, "testGUID-0006");
            // equip an Player with a sham piece
            Player.SetPiece(new Piece(PieceType.normal, 23)
            {
                timestamp = DateTime.Now,

            });
            gameMaster.RegisterPlayer(Player,Player.GUID, findFreeLocationAndPlacePlayer : false);
            Player.SetPiece(new Piece(PieceType.unknown, 23)
            {
                timestamp = DateTime.Now,
            });
            Player.SetLocation(1, 1); // we change a location of an original object | (1,1) is GoalField of type 'goal' by default in GameMasterSettingsGameDefinition constructor

            // set an Player on a TaskField
            var setPositionResult = gameMaster.SetAbsolutePlayerLocation(1, 1, "testGUID-0006"); // we change a location of GM's copy

            // action: Player places a piece
            var actionResult = Player.PlacePiece(gameMaster);

            Assert.AreEqual(true, setPositionResult);
            Assert.AreEqual(true, actionResult);
            Assert.IsNull(Player.GetPiece);
        }

        [TestMethod]
        public void PlayerPlacesNormalPieceOnGoalFieldOfTypeNonGoal()
        {
            var settings = GameMasterSettings.GetDefaultGameMasterSettings();
            var gameMaster = new GameArea.GameMaster(settings);
            var Player = new Player(TeamColour.blue, "testGUID-0007");
            // equip an Player with a sham piece
            Player.SetPiece(new Piece(PieceType.normal, 23)
            {
                timestamp = DateTime.Now,

            });
            gameMaster.RegisterPlayer(Player,Player.GUID, findFreeLocationAndPlacePlayer : false);
            Player.SetPiece(new Piece(PieceType.unknown, 23)
            {
                timestamp = DateTime.Now,
            });
            Player.SetLocation(4, 1);

            // set an Player on a TaskField
            var setPositionResult = gameMaster.SetAbsolutePlayerLocation(4, 1, "testGUID-0007"); // we change a location of GM's copy

            // action: Player places a piece
            var actionResult = Player.PlacePiece(gameMaster);

            //Assert.AreEqual(true, setPositionResult);
            Assert.AreEqual(false, actionResult);
            Assert.IsNotNull(Player.GetPiece);
        }

        [TestMethod]
        public void PlayerPicksUpNormalPieceFromTaskField()
        {
            var settings = GameMasterSettings.GetDefaultGameMasterSettings();
            var gameMaster = new GameArea.GameMaster(settings);
            var Player = new Player(TeamColour.blue, "testGUID-0008");
            Player.SetLocation(2, 5);

            // place a piece on a TaskField
            gameMaster.SetPieceInLocation(2, 5, TeamColour.blue, PieceType.normal, 99);

            Assert.IsNotNull((gameMaster.GetBoard.GetField(2, 5) as GameArea.TaskField).GetPiece);

            gameMaster.RegisterPlayer(Player,Player.GUID, findFreeLocationAndPlacePlayer : false);

            // set an Player on a TaskField
            var setPositionResult = gameMaster.SetAbsolutePlayerLocation(2, 5, "testGUID-0008"); // we change a location of GM's copy

            // action: Player places a piece
            var actionResult = Player.PickUpPiece(gameMaster);

            Assert.AreEqual(true, setPositionResult);
            Assert.AreEqual(true, actionResult);
            Assert.IsNotNull(Player.GetPiece);
            Assert.AreEqual(99ul, Player.GetPiece.id);
            Assert.IsNull((gameMaster.GetBoard.GetField(2, 5) as GameArea.TaskField).GetPiece);
            Assert.AreEqual(PieceType.unknown, Player.GetPiece.type);
        }

        [TestMethod]
        public void PlayerPicksUpShamPieceFromTaskField()
        {
            var settings = GameMasterSettings.GetDefaultGameMasterSettings();
            var gameMaster = new GameArea.GameMaster(settings);
            var Player = new Player(TeamColour.blue, "testGUID-0009");
            Player.SetLocation(2, 5);

            // place a piece on a TaskField
            gameMaster.SetPieceInLocation(2, 5, TeamColour.blue, PieceType.sham, 98);

            Assert.IsNotNull((gameMaster.GetBoard.GetField(2, 5) as GameArea.TaskField).GetPiece);

            gameMaster.RegisterPlayer(Player,Player.GUID, findFreeLocationAndPlacePlayer : false);

            // set an Player on a TaskField
            var setPositionResult = gameMaster.SetAbsolutePlayerLocation(2, 5, "testGUID-0009"); // we change a location of GM's copy

            // action: Player places a piece
            var actionResult = Player.PickUpPiece(gameMaster);

            Assert.AreEqual(true, setPositionResult);
            Assert.AreEqual(true, actionResult);
            Assert.IsNotNull(Player.GetPiece);
            Assert.AreEqual(98ul, Player.GetPiece.id);
            Assert.IsNull((gameMaster.GetBoard.GetField(2, 5) as GameArea.TaskField).GetPiece);
            Assert.AreEqual(PieceType.unknown, Player.GetPiece.type);
        }

        [TestMethod]
        public void PlayerPicksUpFromEmptyTaskField()
        {
            var Player = new Player(TeamColour.blue, "testGUID-0010");
            Player.SetLocation(2, 5);

            gameMaster.RegisterPlayer(Player,Player.GUID, findFreeLocationAndPlacePlayer : false);

            // set an Player on a TaskField
            var setPositionResult = gameMaster.SetAbsolutePlayerLocation(2, 5, "testGUID-0010"); // we change a location of GM's copy

            // action: Player places a piece
            var actionResult = Player.PickUpPiece(gameMaster);

            Assert.AreEqual(true, setPositionResult);
            Assert.AreEqual(false, actionResult);
            Assert.IsNull(Player.GetPiece);
            Assert.IsNull((gameMaster.GetBoard.GetField(2, 5) as GameArea.TaskField).GetPiece);
        }


        // these two cases are impossible to occurr
        //[TestMethod]
        //public void BluePlayerPicksUpFromRedGoalFieldOfTypeGoal()
        //{
        //    Configuration.GameMasterSettingsGameDefinition conf = new Configuration.GameMasterSettingsGameDefinition();
        //    var gameMaster = new GameArea.GameMaster(conf);
        //    var Player = new Player.Player(TeamColour.blue, "testGUID-0011");

        //    gameMaster.RegisterPlayer(Player,Player.GUID, findFreeLocationAndPlacePlayer : false);

        //    // set an Player on a TaskField
        //    var setPositionResult = gameMaster.SetAbsolutePlayerLocation(2, 11, "testGUID-0011"); // we change a location of GM's copy

        //    // action: Player places a piece
        //    var actionResult = Player.PickUpPiece(gameMaster);

        //    Assert.AreEqual(false, setPositionResult);
        //    Assert.AreEqual(false, actionResult);
        //    Assert.IsNull(Player.GetPiece);
        //}

        //[TestMethod]
        //public void BluePlayerPicksUpFromRedGoalFieldOfTypeNonGoal()
        //{
        //    Configuration.GameMasterSettingsGameDefinition conf = new Configuration.GameMasterSettingsGameDefinition();
        //    var gameMaster = new GameArea.GameMaster(conf);
        //    var Player = new Player.Player(TeamColour.blue, "testGUID-0012");

        //    gameMaster.RegisterPlayer(Player,Player.GUID, findFreeLocationAndPlacePlayer : false);

        //    // set an Player on a TaskField
        //    var setPositionResult = gameMaster.SetAbsolutePlayerLocation(2, 12, "testGUID-0012"); // we change a location of GM's copy

        //    // action: Player places a piece
        //    var actionResult = Player.PickUpPiece(gameMaster);

        //    Assert.AreEqual(false, setPositionResult);
        //    Assert.AreEqual(false, actionResult);
        //    Assert.IsNull(Player.GetPiece);
        //}

        [TestMethod]
        public void BluePlayerPicksUpFromBlueGoalFieldOfTypeGoal()
        {
            var Player = new Player(TeamColour.blue, "testGUID-0011");
            Player.SetLocation(2, 2);

            gameMaster.RegisterPlayer(Player,Player.GUID, findFreeLocationAndPlacePlayer : false);

            // set an Player on a TaskField
            var setPositionResult = gameMaster.SetAbsolutePlayerLocation(2, 2, "testGUID-0011"); // we change a location of GM's copy

            // action: Player places a piece
            var actionResult = Player.PickUpPiece(gameMaster);

            Assert.AreEqual(true, setPositionResult);
            Assert.AreEqual(false, actionResult);
            Assert.IsNull(Player.GetPiece);
        }

        [TestMethod]
        public void BluePlayerPicksUpFromBlueGoalFieldOfTypeNonGoal()
        {
            var Player = new Player(TeamColour.blue, "testGUID-0012");
            Player.SetLocation(2, 1);

            gameMaster.RegisterPlayer(Player,Player.GUID, findFreeLocationAndPlacePlayer : false);

            // set an Player on a TaskField
            var setPositionResult = gameMaster.SetAbsolutePlayerLocation(2, 1, "testGUID-0012"); // we change a location of GM's copy

            // action: Player places a piece
            var actionResult = Player.PickUpPiece(gameMaster);

            Assert.AreEqual(true, setPositionResult);
            Assert.AreEqual(false, actionResult);
            Assert.IsNull(Player.GetPiece);
        }

        /// MOVE tests

        [TestMethod]
        public void PlayerMovesFromTaskFieldToEmptyTaskField()
        {
            var Player = new Player(TeamColour.blue, "testGUID-0013");
            Player.SetLocation(2, 4);

            gameMaster.RegisterPlayer(Player,Player.GUID, findFreeLocationAndPlacePlayer : false);

            // set an Player on a TaskField
            var setPositionResult = gameMaster.SetAbsolutePlayerLocation(2, 4, "testGUID-0013"); // we change a location of GM's copy

            Assert.IsNotNull(gameMaster.GetBoard.GetField(2, 4).Player);

            // action: Player moves up to (2,5)
            var actionResult = Player.Move(gameMaster, MoveType.up);

            Assert.AreEqual(true, setPositionResult);
            Assert.AreEqual(FieldType.Task, gameMaster.GetBoard.GetField(2, 4).GetFieldType);
            Assert.AreEqual(FieldType.Task, gameMaster.GetBoard.GetField(2, 5).GetFieldType);
            Assert.AreEqual(new Location(2, 5), Player.GetLocation);
            Assert.IsNull(gameMaster.GetBoard.GetField(2, 4).Player);
            Assert.IsNotNull(gameMaster.GetBoard.GetField(2, 5).Player);
            Assert.AreEqual(Player.ID, gameMaster.GetBoard.GetField(2, 5).Player.id);
        }

        [TestMethod]
        public void PlayerMovesFromTaskFieldToOccupiedTaskField()
        {
            var Player = new Player(TeamColour.blue, "testGUID-0014");
            var strangerPlayer = new Player(TeamColour.blue, "testGUID-0015");
            Player.SetLocation(2, 4);
            strangerPlayer.SetLocation(2, 5);

            gameMaster.RegisterPlayer(Player,Player.GUID, findFreeLocationAndPlacePlayer : false); // id = 1
            gameMaster.RegisterPlayer(strangerPlayer,strangerPlayer.GUID, findFreeLocationAndPlacePlayer : false); // id = 2

            // set Players on a TaskField
            var setPositionResult = gameMaster.SetAbsolutePlayerLocation(2, 4, "testGUID-0014");
            var setPositionResult2 = gameMaster.SetAbsolutePlayerLocation(2, 5, "testGUID-0015"); // we change a location of GM's copy

            Assert.IsNotNull(gameMaster.GetBoard.GetField(2, 4).Player);
            Assert.IsNotNull(gameMaster.GetBoard.GetField(2, 5).Player);

            // action: Player moves up to (2,5)
            var actionResult = Player.Move(gameMaster, MoveType.up);

            Assert.AreEqual(true, setPositionResult);
            Assert.AreEqual(true, setPositionResult2);
            Assert.AreEqual(false, actionResult);
            Assert.AreEqual(FieldType.Task, gameMaster.GetBoard.GetField(2, 4).GetFieldType);
            Assert.AreEqual(FieldType.Task, gameMaster.GetBoard.GetField(2, 5).GetFieldType);
            Assert.AreEqual(new Location(2, 4), Player.GetLocation);
            Assert.AreEqual(new Location(2, 5), strangerPlayer.GetLocation);
            Assert.IsNotNull(gameMaster.GetBoard.GetField(2, 4).Player);
            Assert.IsNotNull(gameMaster.GetBoard.GetField(2, 5).Player);
            Assert.AreEqual(Player.ID, gameMaster.GetBoard.GetField(2, 4).Player.id);
            Assert.AreEqual(strangerPlayer.ID, gameMaster.GetBoard.GetField(2, 5).Player.id);
            Assert.AreEqual(strangerPlayer.ID, Player.GetBoard.GetField(2, 5).Player.id); // check if Player has stored an encountered stranger in his board view

        }

        [TestMethod]
        public void PlayerMovesFromTaskFieldToRightEmptyGoalField()
        {
            var Player = new Player(TeamColour.blue, "testGUID-0016");
            Player.SetLocation(2, 3);

            gameMaster.RegisterPlayer(Player,Player.GUID, findFreeLocationAndPlacePlayer : false);

            // set an Player on a TaskField
            var setPositionResult = gameMaster.SetAbsolutePlayerLocation(2, 3, "testGUID-0016"); // we change a location of GM's copy

            Assert.IsNotNull(gameMaster.GetBoard.GetField(2, 3).Player);

            // action: Player moves up to (2,2)
            var actionResult = Player.Move(gameMaster, MoveType.down);

            Assert.AreEqual(true, setPositionResult);
            Assert.AreEqual(true, actionResult);
            Assert.AreEqual(FieldType.Goal, gameMaster.GetBoard.GetField(2, 2).GetFieldType);
            Assert.AreEqual(FieldType.Task, gameMaster.GetBoard.GetField(2, 3).GetFieldType);
            Assert.AreEqual(new Location(2, 2), Player.GetLocation);
            Assert.IsNull(gameMaster.GetBoard.GetField(2, 3).Player);
            Assert.IsNotNull(gameMaster.GetBoard.GetField(2, 2).Player);
            Assert.AreEqual(Player.ID, gameMaster.GetBoard.GetField(2, 2).Player.id);
        }

        [TestMethod]
        public void PlayerMovesFromTaskFieldToToRightOccupiedGoalField()
        {
            var Player = new Player(TeamColour.blue, "testGUID-0017");
            var strangerPlayer = new Player(TeamColour.blue, "testGUID-0018");
            Player.SetLocation(2, 3);
            strangerPlayer.SetLocation(2, 2);

            gameMaster.RegisterPlayer(Player,Player.GUID, findFreeLocationAndPlacePlayer : false); // id = 1
            gameMaster.RegisterPlayer(strangerPlayer,strangerPlayer.GUID, findFreeLocationAndPlacePlayer : false); // id = 2

            // set Players on a TaskField
            var setPositionResult = gameMaster.SetAbsolutePlayerLocation(2, 3, "testGUID-0017");
            var setPositionResult2 = gameMaster.SetAbsolutePlayerLocation(2, 2, "testGUID-0018"); // we change a location of GM's copy

            Assert.IsNotNull(gameMaster.GetBoard.GetField(2, 3).Player);
            Assert.IsNotNull(gameMaster.GetBoard.GetField(2, 2).Player);

            // action: Player down up to (2,2)
            var actionResult = Player.Move(gameMaster, MoveType.down);

            Assert.AreEqual(true, setPositionResult);
            Assert.AreEqual(true, setPositionResult2);
            Assert.AreEqual(false, actionResult);
            Assert.AreEqual(FieldType.Goal, gameMaster.GetBoard.GetField(2, 2).GetFieldType);
            Assert.AreEqual(FieldType.Task, gameMaster.GetBoard.GetField(2, 3).GetFieldType);
            Assert.AreEqual(new Location(2, 3), Player.GetLocation);
            Assert.AreEqual(new Location(2, 2), strangerPlayer.GetLocation);
            Assert.IsNotNull(gameMaster.GetBoard.GetField(2, 3).Player);
            Assert.IsNotNull(gameMaster.GetBoard.GetField(2, 2).Player);
            Assert.AreEqual(Player.ID, gameMaster.GetBoard.GetField(2, 3).Player.id);
            Assert.AreEqual(strangerPlayer.ID, gameMaster.GetBoard.GetField(2, 2).Player.id);
            Assert.AreEqual(strangerPlayer.ID, Player.GetBoard.GetField(2, 2).Player.id); // check if Player has stored an encountered stranger in his board view
        }

        [TestMethod]
        public void PlayerMovesFromTaskFieldToWrongGoalField()
        {
            var Player = new Player(TeamColour.blue, "testGUID-0019");
            Player.SetLocation(1, 9);

            gameMaster.RegisterPlayer(Player,Player.GUID, findFreeLocationAndPlacePlayer : false);

            // set an Player on a TaskField
            var setPositionResult = gameMaster.SetAbsolutePlayerLocation(1, 9, "testGUID-0019"); // we change a location of GM's copy

            Assert.IsNotNull(gameMaster.GetBoard.GetField(1, 9).Player);

            // action: Player moves up to (1,10)
            var actionResult = Player.Move(gameMaster, MoveType.up);

            Assert.AreEqual(true, setPositionResult);
            Assert.AreEqual(false, actionResult);
            Assert.AreEqual(FieldType.Task, gameMaster.GetBoard.GetField(1, 9).GetFieldType);
            Assert.AreEqual(FieldType.Goal, gameMaster.GetBoard.GetField(1, 10).GetFieldType);
            Assert.AreEqual(new Location(1, 9), Player.GetLocation);
            Assert.IsNull(gameMaster.GetBoard.GetField(1, 10).Player);
            Assert.IsNotNull(gameMaster.GetBoard.GetField(1, 9).Player);
            Assert.AreEqual(Player.ID, gameMaster.GetBoard.GetField(1, 9).Player.id);
        }

        [TestMethod]
        public void PlayerMovesFromTaskFieldOutOfBoard()
        {
            var Player = new Player(TeamColour.red, "testGUID-0017");
            var Player2 = new Player(TeamColour.blue, "testGUID-0018");
            Player.SetLocation(4, 7);
            Player2.SetLocation(0, 5);

            gameMaster.RegisterPlayer(Player,Player.GUID, findFreeLocationAndPlacePlayer : false); // id = 1
            gameMaster.RegisterPlayer(Player2,Player2.GUID, findFreeLocationAndPlacePlayer : false); // id = 2

            // set Players on a TaskField
            var setPositionResult = gameMaster.SetAbsolutePlayerLocation(4, 7, "testGUID-0017");
            var setPositionResult2 = gameMaster.SetAbsolutePlayerLocation(0, 5, "testGUID-0018"); // we change a location of GM's copy

            Assert.IsNotNull(gameMaster.GetBoard.GetField(4, 7).Player);
            Assert.IsNotNull(gameMaster.GetBoard.GetField(0, 5).Player);

            var actionResult = Player.Move(gameMaster, MoveType.right);
            var actionResult2 = Player2.Move(gameMaster, MoveType.left);

            Assert.AreEqual(true, setPositionResult);
            Assert.AreEqual(true, setPositionResult2);
            Assert.AreEqual(false, actionResult);
            Assert.AreEqual(false, actionResult2);
            Assert.AreEqual(FieldType.Task, gameMaster.GetBoard.GetField(4, 7).GetFieldType);
            Assert.AreEqual(FieldType.Task, gameMaster.GetBoard.GetField(0, 5).GetFieldType);
            Assert.AreEqual(new Location(4, 7), Player.GetLocation);
            Assert.AreEqual(new Location(0, 5), Player2.GetLocation);
            Assert.IsNotNull(gameMaster.GetBoard.GetField(4, 7).Player);
            Assert.IsNotNull(gameMaster.GetBoard.GetField(0, 5).Player);
            Assert.AreEqual(Player.ID, gameMaster.GetBoard.GetField(4, 7).Player.id);
            Assert.AreEqual(Player2.ID, gameMaster.GetBoard.GetField(0, 5).Player.id);
        }

        [TestMethod]
        public void PlayerMovesFromGoalFieldToEmptyTaskField()
        {

            var Player = new Player(TeamColour.red, "testGUID-0020");
            Player.SetLocation(1, 10);

            gameMaster.RegisterPlayer(Player,Player.GUID, findFreeLocationAndPlacePlayer : false);

            // set an Player on a TaskField
            var setPositionResult = gameMaster.SetAbsolutePlayerLocation(1, 10, "testGUID-0020"); // we change a location of GM's copy

            Assert.IsNotNull(gameMaster.GetBoard.GetField(1, 10).Player);

            // action: Player moves up to (1,9)
            var actionResult = Player.Move(gameMaster, MoveType.down);

            Assert.AreEqual(true, setPositionResult);
            Assert.AreEqual(true, actionResult);
            Assert.AreEqual(FieldType.Task, gameMaster.GetBoard.GetField(1, 9).GetFieldType);
            Assert.AreEqual(FieldType.Goal, gameMaster.GetBoard.GetField(1, 10).GetFieldType);
            Assert.AreEqual(new Location(1, 9), Player.GetLocation);
            Assert.IsNull(gameMaster.GetBoard.GetField(1, 10).Player);
            Assert.IsNotNull(gameMaster.GetBoard.GetField(1, 9).Player);
            Assert.AreEqual(Player.ID, gameMaster.GetBoard.GetField(1, 9).Player.id);
        }

        [TestMethod]
        public void PlayerMovesFromGoalFieldToOccupiedTaskField()
        {
            var Player = new Player(TeamColour.red, "testGUID-0014");
            var strangerPlayer = new Player(TeamColour.blue, "testGUID-0015");
            Player.SetLocation(3, 10);
            strangerPlayer.SetLocation(3, 9);

            gameMaster.RegisterPlayer(Player,Player.GUID, findFreeLocationAndPlacePlayer : false); // id = 1
            gameMaster.RegisterPlayer(strangerPlayer,strangerPlayer.GUID, findFreeLocationAndPlacePlayer : false); // id = 2

            // set Players on a TaskField
            var setPositionResult = gameMaster.SetAbsolutePlayerLocation(3, 10, "testGUID-0014");
            var setPositionResult2 = gameMaster.SetAbsolutePlayerLocation(3, 9, "testGUID-0015"); // we change a location of GM's copy

            Assert.IsNotNull(gameMaster.GetBoard.GetField(3, 10).Player);
            Assert.IsNotNull(gameMaster.GetBoard.GetField(3, 9).Player);

            // action: Player moves up to (3,9)
            var actionResult = Player.Move(gameMaster, MoveType.down);

            Assert.AreEqual(true, setPositionResult);
            Assert.AreEqual(true, setPositionResult2);
            Assert.AreEqual(false, actionResult);
            Assert.AreEqual(FieldType.Task, gameMaster.GetBoard.GetField(3, 9).GetFieldType);
            Assert.AreEqual(FieldType.Goal, gameMaster.GetBoard.GetField(3, 10).GetFieldType);
            Assert.AreEqual(new Location(3, 10), Player.GetLocation);
            Assert.AreEqual(new Location(3, 9), strangerPlayer.GetLocation);
            Assert.IsNotNull(gameMaster.GetBoard.GetField(3, 10).Player);
            Assert.IsNotNull(gameMaster.GetBoard.GetField(3, 9).Player);
            Assert.AreEqual(Player.ID, gameMaster.GetBoard.GetField(3, 10).Player.id);
            Assert.AreEqual(strangerPlayer.ID, gameMaster.GetBoard.GetField(3, 9).Player.id);
            Assert.AreEqual(strangerPlayer.ID, Player.GetBoard.GetField(3, 9).Player.id); // check if Player has stored an encountered stranger in his board view

        }

        [TestMethod]
        public void PlayerMovesFromGoalFieldToEmptyGoalField()
        {
            var Player = new Player(TeamColour.red, "testGUID-0016");
            Player.SetLocation(1, 12);

            gameMaster.RegisterPlayer(Player,Player.GUID, findFreeLocationAndPlacePlayer : false);

            // set an Player on a TaskField
            var setPositionResult = gameMaster.SetAbsolutePlayerLocation(1, 12, "testGUID-0016"); // we change a location of GM's copy

            Assert.IsNotNull(gameMaster.GetBoard.GetField(1, 12).Player);

            // action: Player moves up to (2,12)
            var actionResult = Player.Move(gameMaster, MoveType.right);

            Assert.AreEqual(true, setPositionResult);
            Assert.AreEqual(true, actionResult);
            Assert.AreEqual(FieldType.Goal, gameMaster.GetBoard.GetField(1, 12).GetFieldType);
            Assert.AreEqual(FieldType.Goal, gameMaster.GetBoard.GetField(2, 12).GetFieldType);
            Assert.AreEqual(new Location(2, 12), Player.GetLocation);
            Assert.IsNull(gameMaster.GetBoard.GetField(1, 12).Player);
            Assert.IsNotNull(gameMaster.GetBoard.GetField(2, 12).Player);
            Assert.AreEqual(Player.ID, gameMaster.GetBoard.GetField(2, 12).Player.id);
        }

        [TestMethod]
        public void PlayerMovesFromGoalFieldToToOccupiedGoalField()
        {
            var Player = new Player(TeamColour.red, "testGUID-0017");
            var strangerPlayer = new Player(TeamColour.red, "testGUID-0018");
            Player.SetLocation(3, 11);
            strangerPlayer.SetLocation(2, 11);

            gameMaster.RegisterPlayer(Player,Player.GUID, findFreeLocationAndPlacePlayer : false); // id = 1
            gameMaster.RegisterPlayer(strangerPlayer,strangerPlayer.GUID, findFreeLocationAndPlacePlayer : false); // id = 2

            // set Players on a TaskField
            var setPositionResult = gameMaster.SetAbsolutePlayerLocation(3, 11, "testGUID-0017");
            var setPositionResult2 = gameMaster.SetAbsolutePlayerLocation(2, 11, "testGUID-0018"); // we change a location of GM's copy

            Assert.IsNotNull(gameMaster.GetBoard.GetField(3, 11).Player);
            Assert.IsNotNull(gameMaster.GetBoard.GetField(2, 11).Player);

            // action: Player down up to (2,11)
            var actionResult = Player.Move(gameMaster, MoveType.left);

            Assert.AreEqual(true, setPositionResult);
            Assert.AreEqual(true, setPositionResult2);
            Assert.AreEqual(false, actionResult);
            Assert.AreEqual(FieldType.Goal, gameMaster.GetBoard.GetField(2, 11).GetFieldType);
            Assert.AreEqual(FieldType.Goal, gameMaster.GetBoard.GetField(3, 11).GetFieldType);
            Assert.AreEqual(new Location(3, 11), Player.GetLocation);
            Assert.AreEqual(new Location(2, 11), strangerPlayer.GetLocation);
            Assert.IsNotNull(gameMaster.GetBoard.GetField(3, 11).Player);
            Assert.IsNotNull(gameMaster.GetBoard.GetField(2, 11).Player);
            Assert.AreEqual(Player.ID, gameMaster.GetBoard.GetField(3, 11).Player.id);
            Assert.AreEqual(strangerPlayer.ID, gameMaster.GetBoard.GetField(2, 11).Player.id);
            Assert.AreEqual(strangerPlayer.ID, Player.GetBoard.GetField(2, 11).Player.id); // check if Player has stored an encountered stranger in his board view
        }

        [TestMethod]
        public void PlayerMovesFromGoalFieldOutOfBoard()
        {
            var Player = new Player(TeamColour.red, "testGUID-0017");
            var Player2 = new Player(TeamColour.blue, "testGUID-0018");
            Player.SetLocation(4, 11);
            Player2.SetLocation(0, 1);

            gameMaster.RegisterPlayer(Player,Player.GUID, findFreeLocationAndPlacePlayer : false); // id = 1
            gameMaster.RegisterPlayer(Player2,Player2.GUID, findFreeLocationAndPlacePlayer : false); // id = 2

            // set Players on a TaskField
            var setPositionResult = gameMaster.SetAbsolutePlayerLocation(4, 11, "testGUID-0017");
            var setPositionResult2 = gameMaster.SetAbsolutePlayerLocation(0, 1, "testGUID-0018"); // we change a location of GM's copy

            Assert.IsNotNull(gameMaster.GetBoard.GetField(4, 11).Player);
            Assert.IsNotNull(gameMaster.GetBoard.GetField(0, 1).Player);

            var actionResult = Player.Move(gameMaster, MoveType.right);
            var actionResult2 = Player2.Move(gameMaster, MoveType.left);

            Assert.AreEqual(true, setPositionResult);
            Assert.AreEqual(true, setPositionResult2);
            Assert.AreEqual(false, actionResult);
            Assert.AreEqual(false, actionResult2);
            Assert.AreEqual(FieldType.Goal, gameMaster.GetBoard.GetField(4, 11).GetFieldType);
            Assert.AreEqual(FieldType.Goal, gameMaster.GetBoard.GetField(0, 1).GetFieldType);
            Assert.AreEqual(new Location(4, 11), Player.GetLocation);
            Assert.AreEqual(new Location(0, 1), Player2.GetLocation);
            Assert.IsNotNull(gameMaster.GetBoard.GetField(4, 11).Player);
            Assert.IsNotNull(gameMaster.GetBoard.GetField(0, 1).Player);
            Assert.AreEqual(Player.ID, gameMaster.GetBoard.GetField(4, 11).Player.id);
            Assert.AreEqual(Player2.ID, gameMaster.GetBoard.GetField(0, 1).Player.id);
        }

        [TestMethod]
        public void PlayerDiscoveryNothingInSight()
        {
            var Player = new Player(TeamColour.blue, "testGUID-0020");
            Player.SetLocation(1, 5);

            gameMaster.RegisterPlayer(Player,Player.GUID, findFreeLocationAndPlacePlayer : false);

            // set an Player on a TaskField
            var setPositionResult = gameMaster.SetAbsolutePlayerLocation(1, 5, "testGUID-0020"); // we change a location of GM's copy

            Assert.IsNotNull(gameMaster.GetBoard.GetField(1, 5).Player);


            // action: Player discovers area
            Player.Discover(gameMaster);

            Assert.AreEqual(true, setPositionResult);
            Assert.AreEqual(new Location(1, 5), Player.GetLocation);
            Assert.IsNull(gameMaster.GetBoard.GetField(2, 5).Player);
            Assert.IsNull(Player.GetBoard.GetField(2, 5).Player);
            Assert.IsNotNull(gameMaster.GetBoard.GetField(1, 5).Player);
            Assert.AreEqual(Player.ID, gameMaster.GetBoard.GetField(1, 5).Player.id);
            Assert.IsNull((gameMaster.GetBoard.GetField(2, 5) as GameArea.TaskField).GetPiece);
            Assert.IsNull((Player.GetBoard.GetField(2, 5) as GameArea.TaskField).GetPiece);
            //Assert.AreEqual(1, (Player.GetBoard.GetField(2, 5) as GameArea.TaskField).Distance);
        }

        [TestMethod]
        public void PlayerDiscoverySeesPiece()
        {
            var Player = new Player(TeamColour.blue, "testGUID-0021");
            Player.SetLocation(1, 6);

            gameMaster.RegisterPlayer(Player,Player.GUID, findFreeLocationAndPlacePlayer : false);

            // set an Player on a TaskField
            var setPositionResult = gameMaster.SetAbsolutePlayerLocation(1, 6, "testGUID-0021"); // we change a location of GM's copy

            Assert.IsNotNull(gameMaster.GetBoard.GetField(1, 6).Player);

            // place a piece on a TaskField
            gameMaster.SetPieceInLocation(2, 6, TeamColour.blue, PieceType.sham, 98);
            Assert.IsNotNull((gameMaster.GetBoard.GetField(2, 6) as GameArea.TaskField).GetPiece);


            // action: Player discovers area
            Player.Discover(gameMaster);

            Assert.AreEqual(true, setPositionResult);
            Assert.AreEqual(new Location(1, 6), Player.GetLocation);
            Assert.IsNull(gameMaster.GetBoard.GetField(2, 3).Player);
            Assert.IsNotNull(gameMaster.GetBoard.GetField(1, 6).Player);
            Assert.AreEqual(Player.ID, gameMaster.GetBoard.GetField(1, 6).Player.id);
            Assert.IsNotNull((gameMaster.GetBoard.GetField(2, 6) as GameArea.TaskField).GetPiece);
            Assert.IsNotNull((Player.GetBoard.GetField(2, 6) as GameArea.TaskField).GetPiece);
            Assert.AreEqual(98ul, (gameMaster.GetBoard.GetField(2, 6) as GameArea.TaskField).GetPiece.id);
            Assert.AreEqual(98ul, (Player.GetBoard.GetField(2, 6) as GameArea.TaskField).GetPiece.id);
            Assert.IsNull((gameMaster.GetBoard.GetField(1, 6) as GameArea.TaskField).GetPiece);
            //Assert.AreEqual(1, (Player.GetBoard.GetField(1, 5) as GameArea.TaskField).Distance);
        }

        [TestMethod]
        public void PlayerDiscoverySeesPlayerInTaskArea()
        {
            var Player1 = new Player(TeamColour.blue, "testGUID-0022");
            var Player2 = new Player(TeamColour.blue, "testGUID-0023");
            Player1.SetLocation(1, 6);
            Player2.SetLocation(1, 5);

            gameMaster.RegisterPlayer(Player1,Player1.GUID, findFreeLocationAndPlacePlayer : false);
            gameMaster.RegisterPlayer(Player2,Player2.GUID, findFreeLocationAndPlacePlayer : false);

            // set an Player on a TaskField
            var setPositionResult1 = gameMaster.SetAbsolutePlayerLocation(1, 6, "testGUID-0022"); // we change a location of GM's copy
            var setPositionResult2 = gameMaster.SetAbsolutePlayerLocation(1, 5, "testGUID-0023"); // we change a location of GM's copy

            Assert.IsNotNull(gameMaster.GetBoard.GetField(1, 6).Player);
            Assert.IsNotNull(gameMaster.GetBoard.GetField(1, 5).Player);


            // action: Player discovers area
            Player1.Discover(gameMaster);

            Assert.AreEqual(true, setPositionResult1);
            Assert.AreEqual(true, setPositionResult2);
            Assert.AreEqual(new Location(1, 6), Player1.GetLocation);
            Assert.AreEqual(new Location(1, 5), Player2.GetLocation);
            Assert.IsNull(gameMaster.GetBoard.GetField(2, 3).Player);
            Assert.IsNotNull(gameMaster.GetBoard.GetField(1, 6).Player);
            Assert.IsNotNull(gameMaster.GetBoard.GetField(1, 5).Player);
            Assert.IsNotNull(Player1.GetBoard.GetField(1, 5).Player);
            Assert.AreEqual(Player2.ID, Player1.GetBoard.GetField(1, 5).Player.id);
            Assert.AreEqual(gameMaster.GetBoard.GetField(1, 5).Player.id, Player1.GetBoard.GetField(1, 5).Player.id);
            Assert.AreEqual(Player1.ID, gameMaster.GetBoard.GetField(1, 6).Player.id);
            Assert.AreEqual(Player2.ID, gameMaster.GetBoard.GetField(1, 5).Player.id);
        }

        [TestMethod]
        public void PlayerDiscoverySeesPlayerInGoalArea()
        {
            var Player1 = new Player(TeamColour.blue, "testGUID-0024");
            var Player2 = new Player(TeamColour.blue, "testGUID-0025");
            Player1.SetLocation(1, 0);
            Player2.SetLocation(1, 1);

            gameMaster.RegisterPlayer(Player1,Player1.GUID, findFreeLocationAndPlacePlayer : false);
            gameMaster.RegisterPlayer(Player2,Player2.GUID, findFreeLocationAndPlacePlayer : false);

            // set an Player on a TaskField
            var setPositionResult1 = gameMaster.SetAbsolutePlayerLocation(1, 0, "testGUID-0024"); // we change a location of GM's copy
            var setPositionResult2 = gameMaster.SetAbsolutePlayerLocation(1, 1, "testGUID-0025"); // we change a location of GM's copy

            Assert.IsNotNull(gameMaster.GetBoard.GetField(1, 0).Player);
            Assert.IsNotNull(gameMaster.GetBoard.GetField(1, 1).Player);


            // action: Player discovers area
            Player1.Discover(gameMaster);

            Assert.AreEqual(true, setPositionResult1);
            Assert.AreEqual(true, setPositionResult2);
            Assert.AreEqual(new Location(1, 0), Player1.GetLocation);
            Assert.AreEqual(new Location(1, 1), Player2.GetLocation);
            Assert.IsNull(gameMaster.GetBoard.GetField(2, 3).Player);
            Assert.IsNotNull(gameMaster.GetBoard.GetField(1, 0).Player);
            Assert.IsNotNull(gameMaster.GetBoard.GetField(1, 1).Player);
            Assert.IsNotNull(Player1.GetBoard.GetField(1, 1).Player);
            Assert.AreEqual(Player2.ID, Player1.GetBoard.GetField(1, 1).Player.id);
            Assert.AreEqual(gameMaster.GetBoard.GetField(1, 1).Player.id, Player1.GetBoard.GetField(1, 1).Player.id);
            Assert.AreEqual(Player1.ID, gameMaster.GetBoard.GetField(1, 0).Player.id);
            Assert.AreEqual(Player2.ID, gameMaster.GetBoard.GetField(1, 1).Player.id);
        }

        [TestMethod]
        public void PlayerDiscoveryNearBoardEdge()
        {
            var Player = new Player(TeamColour.blue, "testGUID-0026");
            Player.SetLocation(0, 3);

            gameMaster.RegisterPlayer(Player,Player.GUID, findFreeLocationAndPlacePlayer : false);

            // set an Player on a TaskField
            var setPositionResult = gameMaster.SetAbsolutePlayerLocation(0, 3, "testGUID-0026"); // we change a location of GM's copy

            Assert.IsNotNull(gameMaster.GetBoard.GetField(0, 3).Player);


            // action: Player discovers area
            Player.Discover(gameMaster);

            Assert.AreEqual(true, setPositionResult);
            Assert.AreEqual(new Location(0, 3), Player.GetLocation);
            Assert.IsNull(gameMaster.GetBoard.GetField(2, 5).Player);
            Assert.IsNull(Player.GetBoard.GetField(2, 5).Player);
            Assert.IsNotNull(gameMaster.GetBoard.GetField(0, 3).Player);
            Assert.AreEqual(Player.ID, gameMaster.GetBoard.GetField(0, 3).Player.id);
        }

        [TestMethod]
        public void PlayerDiscoveryCorner()
        {
            var Player1 = new Player(TeamColour.blue, "testGUID-0027");
            var Player2 = new Player(TeamColour.red, "testGUID-0028");
            Player1.SetLocation(0, 0);
            Player2.SetLocation(4, 9);

            gameMaster.RegisterPlayer(Player1,Player1.GUID, findFreeLocationAndPlacePlayer : false);
            gameMaster.RegisterPlayer(Player2,Player2.GUID, findFreeLocationAndPlacePlayer : false);

            // set an Player on a TaskField
            var setPositionResult1 = gameMaster.SetAbsolutePlayerLocation(0, 0, "testGUID-0027"); // we change a location of GM's copy
            var setPositionResult2 = gameMaster.SetAbsolutePlayerLocation(4, 9, "testGUID-0028"); // we change a location of GM's copy
            Assert.IsNotNull(gameMaster.GetBoard.GetField(0, 0).Player);
            Assert.IsNotNull(gameMaster.GetBoard.GetField(4, 9).Player);


            // action: Player discovers area
            Player1.Discover(gameMaster);

            Assert.AreEqual(true, setPositionResult1);
            Assert.AreEqual(new Location(0, 0), Player1.GetLocation);
            Assert.IsNull(gameMaster.GetBoard.GetField(2, 5).Player);
            Assert.IsNull(Player1.GetBoard.GetField(2, 5).Player);
            Assert.IsNotNull(gameMaster.GetBoard.GetField(0, 0).Player);
            Assert.AreEqual(Player1.ID, gameMaster.GetBoard.GetField(0, 0).Player.id);

            Assert.AreEqual(true, setPositionResult2);
            Assert.AreEqual(new Location(4, 9), Player2.GetLocation);
            Assert.IsNotNull(gameMaster.GetBoard.GetField(4, 9).Player);
            Assert.AreEqual(Player2.ID, gameMaster.GetBoard.GetField(4, 9).Player.id);
        }
        [TestMethod]
        public void GoToNearestPieceInGoalAreaForBluePlayer()
        {
            int x = 0, y = 0;
            var Player1 = new Player(TeamColour.blue, "testGUID-0027",gameMaster);
            Player1.SetLocation(x, y);

            gameMaster.RegisterPlayer(Player1, Player1.GUID, findFreeLocationAndPlacePlayer: false);
            
           
            var setPositionResult1 = gameMaster.SetAbsolutePlayerLocation(x,y, "testGUID-0027"); // we change a location of GM's copy
            Player1.GoToNearestPiece();
            //rusza sie o 2 do gory cos jest nie tak z TryMove (pozwala na 2 ruchy do gory)

            Location expectedLocationPlayer = new Location(x, y+1);

            Assert.AreEqual(expectedLocationPlayer, Player1.GetLocation);

        }

        [TestMethod]
        public void GoToNearestPieceInGoalAreaForRedPlayer()
        {
            int x = 0, y = 12;
            var Player1 = new Player(TeamColour.red, "testGUID-0027", gameMaster);
            Player1.SetLocation(x, y);

            gameMaster.RegisterPlayer(Player1, Player1.GUID, findFreeLocationAndPlacePlayer: false);


            var setPositionResult1 = gameMaster.SetAbsolutePlayerLocation(x, y, "testGUID-0027"); // we change a location of GM's copy
            Player1.GoToNearestPiece();
            //rusza sie o 2 do gory cos jest nie tak z TryMove (pozwala na 2 ruchy do gory)

            Location expectedLocationPlayer = new Location(x, y-1);

            Assert.AreEqual(expectedLocationPlayer, Player1.GetLocation);

        }

        [TestMethod]
        public void GoToNearestPieceInTaskAreaForBluePlayer()
        {
            //sometimes not passed because of methods: PlaceInitialPieces, InitPieceAdder
            //(it's not known where will be another new piece)
            //to ensure that it will be passed we have to change constructor for game master
            //(give choice for enabling pieceAdder
            //
            Location pieceLocation = new Location(1, 5);
            Location beforeMove = new Location(3, 6);
            ulong pieceId = 51;

            var Player1 = new Player(TeamColour.blue, "testGUID-0027", gameMaster);
            Player1.SetLocation(beforeMove);

            gameMaster.RegisterPlayer(Player1, Player1.GUID, findFreeLocationAndPlacePlayer: false);
            gameMaster.SetPieceInLocation(pieceLocation.x,pieceLocation.y, TeamColour.blue,PieceType.unknown, pieceId);

            var setPositionResult1 = gameMaster.SetAbsolutePlayerLocation(beforeMove.x,beforeMove.y, "testGUID-0027"); // we change a location of GM's copy
            Player1.GoToNearestPiece();
            //rusza sie o 2 do gory cos jest nie tak z TryMove (pozwala na 2 ruchy do gory)

            //Location expectedLocationPlayer = new Location();
            Location afterMove = Player1.GetLocation;
            Assert.IsTrue(beforeMove.x - 1 == afterMove.x || beforeMove.y -1 == afterMove.y);
        }
        [TestMethod]
        public void TryPickPieceTest()
        {
            Location pieceLocation = new Location(1, 5);
            Location PlayerLocation = new Location(1, 5);
            ulong pieceId = 51;

            var Player1 = new Player(TeamColour.blue, "testGUID-0027", gameMaster);
            Player1.SetLocation(PlayerLocation);

            gameMaster.RegisterPlayer(Player1, Player1.GUID, findFreeLocationAndPlacePlayer: false);
            gameMaster.SetPieceInLocation(pieceLocation.x, pieceLocation.y, TeamColour.blue, PieceType.unknown, pieceId);

            var setPositionResult1 = gameMaster.SetAbsolutePlayerLocation(PlayerLocation.x, PlayerLocation.y, "testGUID-0027"); // we change a location of GM's copy

            Assert.IsTrue(Player1.GetPiece == null);
            Player1.TryPickPiece();
            Assert.IsTrue(Player1.GetPiece != null);
        }
        [TestMethod]
        public void TryTestPieceShamTest()
        {
            Location pieceLocation = new Location(1, 5);
            Location PlayerLocation = new Location(1, 5);
            ulong pieceId = 51;

            PieceType expectedPieceType = PieceType.sham;

            var Player1 = new Player(TeamColour.blue, "testGUID-0027", gameMaster);
            Player1.SetLocation(PlayerLocation);

            gameMaster.RegisterPlayer(Player1, Player1.GUID, findFreeLocationAndPlacePlayer: false);
            gameMaster.SetPieceInLocation(pieceLocation.x, pieceLocation.y, TeamColour.blue, expectedPieceType, pieceId);

            var setPositionResult1 = gameMaster.SetAbsolutePlayerLocation(PlayerLocation.x, PlayerLocation.y, "testGUID-0027"); // we change a location of GM's copy

            Player1.PickUpPiece(gameMaster);

            
            Assert.IsTrue(Player1.TryTestPiece());
            Assert.AreEqual(expectedPieceType, Player1.GetPiece.type);
        }

        [TestMethod]
        public void TryTestPieceValidTest()
        {
            Location pieceLocation = new Location(1, 5);
            Location PlayerLocation = new Location(1, 5);
            ulong pieceId = 51;

            PieceType expectedPieceType = PieceType.normal;

            var Player1 = new Player(TeamColour.blue, "testGUID-0027", gameMaster);
            Player1.SetLocation(PlayerLocation);

            gameMaster.RegisterPlayer(Player1, Player1.GUID, findFreeLocationAndPlacePlayer: false);
            gameMaster.SetPieceInLocation(pieceLocation.x, pieceLocation.y, TeamColour.blue, expectedPieceType, pieceId);

            var setPositionResult1 = gameMaster.SetAbsolutePlayerLocation(PlayerLocation.x, PlayerLocation.y, "testGUID-0027"); // we change a location of GM's copy

            Player1.PickUpPiece(gameMaster);


            Assert.IsTrue(Player1.TryTestPiece());
            Assert.AreEqual(expectedPieceType, Player1.GetPiece.type);
        }
        [TestMethod]
        public void GoToGoalAreaForBlueTeamTest()
        {
            Location PlayerLocation = new Location(1, 5);

            var Player1 = new Player(TeamColour.blue, "testGUID-0027", gameMaster);
            Player1.SetLocation(PlayerLocation);

            gameMaster.RegisterPlayer(Player1, Player1.GUID, findFreeLocationAndPlacePlayer: false);

            var setPositionResult1 = gameMaster.SetAbsolutePlayerLocation(PlayerLocation.x, PlayerLocation.y, "testGUID-0027"); // we change a location of GM's copy


            //powinno byc Player1.GoToGoalArea()
            Player1.GoToGoalArea(Player1.Team);
            Location expectedLocationPlayer = new Location(PlayerLocation.x,PlayerLocation.y-1);

            Assert.AreEqual(expectedLocationPlayer, Player1.GetLocation);
        }
        [TestMethod]
        public void GoToGoalAreaForRedTeamTest()
        {
            Location PlayerLocation = new Location(1, 5);

            var Player1 = new Player(TeamColour.red, "testGUID-0027", gameMaster);
            Player1.SetLocation(PlayerLocation);

            gameMaster.RegisterPlayer(Player1, Player1.GUID, findFreeLocationAndPlacePlayer: false);

            var setPositionResult1 = gameMaster.SetAbsolutePlayerLocation(PlayerLocation.x, PlayerLocation.y, "testGUID-0027"); // we change a location of GM's copy


            //powinno byc Player1.GoToGoalArea()
            Player1.GoToGoalArea(Player1.Team);
            Location expectedLocationPlayer = new Location(PlayerLocation.x, PlayerLocation.y + 1);

            Assert.AreEqual(expectedLocationPlayer, Player1.GetLocation);
        }
        //slabo testowalny jest napisany default board z goalfieldami( za du�o uknown field)
        [TestMethod]
        public void GetClosestUnknownGoalDirectionForRedTeamTestIfIsOnGoal()
        {
            Location PlayerLocation = new Location(12,12);

            var Player1 = new Player(TeamColour.red, "testGUID-0027", gameMaster);
            Player1.SetLocation(PlayerLocation);

            gameMaster.RegisterPlayer(Player1, Player1.GUID, findFreeLocationAndPlacePlayer: false);

            var setPositionResult1 = gameMaster.SetAbsolutePlayerLocation(PlayerLocation.x, PlayerLocation.y, "testGUID-0027"); // we change a location of GM's copy


            MoveType PlayerDirection = Player1.GetClosestUnknownGoalDirection();
            //MoveType expectedPlayerDirection = MoveType.down;


            Assert.AreEqual(PlayerDirection , MoveType.left );
        }

        //slabo testowalny jest napisany default board z goalfieldami( za du�o uknown field)
        [TestMethod]
        public void GetClosestUnknownGoalForBlueTeamTestIfIsOnGoal()
        {
            Location PlayerLocation = new Location(1, 1);

            var Player1 = new Player(TeamColour.blue, "testGUID-0027", gameMaster);
            Player1.SetLocation(PlayerLocation);

            gameMaster.RegisterPlayer(Player1, Player1.GUID, findFreeLocationAndPlacePlayer: false);

            var setPositionResult1 = gameMaster.SetAbsolutePlayerLocation(PlayerLocation.x, PlayerLocation.y, "testGUID-0027"); // we change a location of GM's copy


            MoveType PlayerDirection = Player1.GetClosestUnknownGoalDirection();

            Assert.AreEqual(PlayerDirection, MoveType.left);
        }

    }
}
