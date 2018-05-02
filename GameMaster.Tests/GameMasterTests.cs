using Configuration;
using Messages;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GameArea.Tests
{
    [TestClass]
    public class GameMasterTests
    {
        //GameMasterSettingsGameDefinition defaultSettings = GameMasterSettingsGameDefinition.GetDefaultGameDefinition();
        //GameMaster defaultGameMaster = new GameMaster(GameMasterSettings.GetDefaultGameMasterSettings());

        GameMasterSettings defaultSettings;
        GameMaster defaultGameMaster;

        public void InitGameMaster()
        {
            defaultSettings = GameMasterSettings.GetDefaultGameMasterSettings();
            defaultGameMaster = new GameArea.GameMaster(defaultSettings);
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

            defaultGameMaster.RegisterPlayer(player, player.GUID, findFreeLocationAndPlacePlayer: false);

            var pieceUnknown = new Piece(PieceType.unknown, piece.id)
            {
                timestamp = piece.timestamp,

            };
            player.SetPiece(new GameArea.GameObjects.Piece(pieceUnknown));
        }




        [TestMethod]
        public void InitBoard()
        {
            InitGameMaster();
            var taskFields = defaultGameMaster.GetBoard.TaskFields;
            Assert.AreEqual((int)defaultSettings.GameDefinition.InitialNumberOfPieces, taskFields.Where(q => q.Piece != null).Count());
            var ids = new List<ulong>();
            foreach (var piece in taskFields.Where(q=>q.Piece!= null).Select(q=> q.Piece))
            {
                Assert.IsFalse(ids.Contains(piece.ID));
                ids.Add(piece.ID);
            }
        }

        [TestMethod]
        public void InitPlayersList()
        {
            InitGameMaster();
            Assert.IsNotNull(defaultGameMaster.GetPlayers);
            Assert.AreEqual(0, defaultGameMaster.GetPlayers.Count);
        }

        [TestMethod]
        public void GameMasterAfterInitState()
        {
            InitGameMaster();
            Assert.AreEqual(GameMasterState.RegisteringGame, defaultGameMaster.GetState);
        }

        [TestMethod]
        public void GameMasterInitialGoals()
        {
            InitGameMaster();
            var blueGoalFields = defaultGameMaster.GetBoard.GetBlueGoalAreaFields.Where(q => q.Type == GoalFieldType.goal).ToList();
            var blueGoals = defaultGameMaster.GetGameDefinition.Goals.Where(q => q.team == TeamColour.blue).ToList();
            Assert.AreEqual(blueGoals.Count, blueGoalFields.Count);
            foreach(var blueGoal in blueGoalFields)
            {
                Assert.IsTrue(blueGoals.Where(q => q.x == blueGoal.X && q.y == blueGoal.Y && q.type == GoalFieldType.goal && q.team ==TeamColour.blue).Any());
            }


            var redGoalFields = defaultGameMaster.GetBoard.GetRedGoalAreaFields.Where(q => q.Type == GoalFieldType.goal).ToList();
            var redGoals = defaultGameMaster.GetGameDefinition.Goals.Where(q => q.team == TeamColour.red).ToList();
            Assert.AreEqual(redGoals.Count, redGoals.Count);
            foreach (var redGoal in redGoalFields)
            {
                Assert.IsTrue(redGoals.Where(q => q.x == redGoal.X && q.y == redGoal.Y && q.type == GoalFieldType.goal && q.team == TeamColour.red).Any());
            }
        }

        [TestMethod]
        public void ManhattanDistance()
        {
            GameMasterSettingsGameDefinition settings = GameMasterSettingsGameDefinition.GetDefaultGameDefinition();
            settings.InitialNumberOfPieces = 0;
            GameMaster defaultGameMaster = new GameMaster(GameMasterSettings.GetGameMasterSettings(settings));

            //first piece
            defaultGameMaster.SetPieceInLocation(2, 5, TeamColour.blue, PieceType.sham, 97);
            defaultGameMaster.UpdateDistancesFromAllPieces();
            Assert.IsNotNull((defaultGameMaster.GetBoard.GetField(2, 5) as GameArea.GameObjects.TaskField).Piece);
            Assert.AreEqual(0, (defaultGameMaster.GetBoard.GetField(2, 5) as GameArea.GameObjects.TaskField).DistanceToPiece);
            Assert.AreEqual(1, (defaultGameMaster.GetBoard.GetField(1, 5) as GameArea.GameObjects.TaskField).DistanceToPiece);
            Assert.AreEqual(1, (defaultGameMaster.GetBoard.GetField(3, 5) as GameArea.GameObjects.TaskField).DistanceToPiece);
            Assert.AreEqual(1, (defaultGameMaster.GetBoard.GetField(2, 4) as GameArea.GameObjects.TaskField).DistanceToPiece);
            Assert.AreEqual(1, (defaultGameMaster.GetBoard.GetField(2, 6) as GameArea.GameObjects.TaskField).DistanceToPiece);
            Assert.AreEqual(3, (defaultGameMaster.GetBoard.GetField(4, 6) as GameArea.GameObjects.TaskField).DistanceToPiece);
            Assert.AreEqual(4, (defaultGameMaster.GetBoard.GetField(4, 3) as GameArea.GameObjects.TaskField).DistanceToPiece);
            Assert.AreEqual(2, (defaultGameMaster.GetBoard.GetField(0, 5) as GameArea.GameObjects.TaskField).DistanceToPiece);

            //second piece
            defaultGameMaster.SetPieceInLocation(4, 5, TeamColour.blue, PieceType.sham, 98);
            defaultGameMaster.UpdateDistancesFromAllPieces();
            Assert.IsNotNull((defaultGameMaster.GetBoard.GetField(2, 5) as GameArea.GameObjects.TaskField).Piece);
            Assert.IsNotNull((defaultGameMaster.GetBoard.GetField(4, 5) as GameArea.GameObjects.TaskField).Piece);
            Assert.AreEqual(0, (defaultGameMaster.GetBoard.GetField(2, 5) as GameArea.GameObjects.TaskField).DistanceToPiece);
            Assert.AreEqual(0, (defaultGameMaster.GetBoard.GetField(4, 5) as GameArea.GameObjects.TaskField).DistanceToPiece);
            Assert.AreEqual(1, (defaultGameMaster.GetBoard.GetField(3, 5) as GameArea.GameObjects.TaskField).DistanceToPiece);
            Assert.AreEqual(1, (defaultGameMaster.GetBoard.GetField(4, 6) as GameArea.GameObjects.TaskField).DistanceToPiece);
            Assert.AreEqual(2, (defaultGameMaster.GetBoard.GetField(4, 3) as GameArea.GameObjects.TaskField).DistanceToPiece);
            Assert.AreEqual(2, (defaultGameMaster.GetBoard.GetField(0, 5) as GameArea.GameObjects.TaskField).DistanceToPiece);

            //third piece
            defaultGameMaster.SetPieceInLocation(1, 8, TeamColour.blue, PieceType.sham, 99);
            Assert.IsNotNull((defaultGameMaster.GetBoard.GetField(2, 5) as GameArea.GameObjects.TaskField).Piece);
            Assert.IsNotNull((defaultGameMaster.GetBoard.GetField(4, 5) as GameArea.GameObjects.TaskField).Piece);
            Assert.IsNotNull((defaultGameMaster.GetBoard.GetField(1, 8) as GameArea.GameObjects.TaskField).Piece);
            Assert.AreEqual(0, (defaultGameMaster.GetBoard.GetField(4, 5) as GameArea.GameObjects.TaskField).DistanceToPiece);
            Assert.AreEqual(1, (defaultGameMaster.GetBoard.GetField(3, 5) as GameArea.GameObjects.TaskField).DistanceToPiece);
            Assert.AreEqual(1, (defaultGameMaster.GetBoard.GetField(4, 6) as GameArea.GameObjects.TaskField).DistanceToPiece);
            Assert.AreEqual(2, (defaultGameMaster.GetBoard.GetField(4, 3) as GameArea.GameObjects.TaskField).DistanceToPiece);
            Assert.AreEqual(2, (defaultGameMaster.GetBoard.GetField(0, 5) as GameArea.GameObjects.TaskField).DistanceToPiece);
            Assert.AreEqual(1, (defaultGameMaster.GetBoard.GetField(1, 7) as GameArea.GameObjects.TaskField).DistanceToPiece);
            Assert.AreEqual(2, (defaultGameMaster.GetBoard.GetField(3, 8) as GameArea.GameObjects.TaskField).DistanceToPiece);
        }

        [TestMethod]
        public void GameMasterPlacesShamPieceOnGoalField()
        {
            InitGameMaster();
            var Player = GetPlayer("testGUID-0001", 10, TeamColour.blue, ActionType.PlacePiece);
            // equip an Player with a sham piece
            var messagePieceKnown = new Piece(PieceType.sham, 70)
            {
                timestamp = DateTime.Now,
            };
            EquipPlayerWithPiece(messagePieceKnown, Player);
            
            Player.SetLocation(1, 2); // we change a location of an original object

            // set an Player on a TaskField
            var setPositionResult = defaultGameMaster.SetAbsolutePlayerLocation(1, 2, "testGUID-0001"); // we change a location of GM's copy

            // action: Player places a piece
            var actionResult = defaultGameMaster.TryPlaceShamPieceOnGoalField(new GameArea.GameObjects.Location(1,2), "testGUID-0001");

            Assert.AreEqual(true, setPositionResult);
            Assert.AreEqual(1, actionResult[0].X);
            Assert.AreEqual(2, actionResult[0].Y);
            Assert.AreEqual((long)Player.ID, actionResult[0].PlayerId);
            Assert.IsTrue(defaultGameMaster.GetBoard.GetGoalField(1,2).HasPlayer());
            Assert.AreEqual(GoalFieldType.unknown, actionResult[0].Type);
        }

        [TestMethod]
        public void GameMasterPlacesNormalPieceOnGoalFieldOfTypeGoal()
        {
            InitGameMaster();
            var Player = GetPlayer("testGUID-0001", 10, TeamColour.blue, ActionType.PlacePiece);
            // equip an Player with a sham piece
            var messagePieceKnown = new Piece(PieceType.normal, 70)
            {
                timestamp = DateTime.Now,
            };
            EquipPlayerWithPiece(messagePieceKnown, Player);

            Player.SetLocation(1, 1); // we change a location of an original object

            // set an Player on a TaskField
            var setPositionResult = defaultGameMaster.SetAbsolutePlayerLocation(1, 1, "testGUID-0001"); // we change a location of GM's copy
            var expectedScore = defaultGameMaster.GoalsBlueLeft - 1;

            // action: Player places a piece
            var actionResult = defaultGameMaster.TryPlaceNormalPieceOnGoalField(new GameArea.GameObjects.Location(1, 1), "testGUID-0001");

            Assert.AreEqual(true, setPositionResult);
            Assert.AreEqual(1, actionResult[0].X);
            Assert.AreEqual(1, actionResult[0].Y);
            Assert.AreEqual((long)Player.ID, actionResult[0].PlayerId);
            Assert.IsTrue(defaultGameMaster.GetBoard.GetGoalField(1, 1).HasPlayer());
            Assert.AreEqual(GoalFieldType.goal, actionResult[0].Type);
            Assert.AreEqual(expectedScore, defaultGameMaster.GoalsBlueLeft);
        }

        [TestMethod]
        public void GameMasterPlacesNormalPieceOnGoalFieldOfTypeNonGoal()
        {
            InitGameMaster();
            var Player = GetPlayer("testGUID-0001", 10, TeamColour.blue, ActionType.PlacePiece);
            // equip an Player with a sham piece
            var messagePieceKnown = new Piece(PieceType.normal, 70)
            {
                timestamp = DateTime.Now,
            };
            EquipPlayerWithPiece(messagePieceKnown, Player);

            Player.SetLocation(2, 1); // we change a location of an original object

            // set an Player on a TaskField
            var setPositionResult = defaultGameMaster.SetAbsolutePlayerLocation(2, 1, "testGUID-0001"); // we change a location of GM's copy
            var expectedScore = defaultGameMaster.GoalsBlueLeft;

            // action: Player places a piece
            var actionResult = defaultGameMaster.TryPlaceNormalPieceOnGoalField(new GameArea.GameObjects.Location(2, 1), "testGUID-0001");

            Assert.AreEqual(true, setPositionResult);
            Assert.AreEqual(2, actionResult[0].X);
            Assert.AreEqual(1, actionResult[0].Y);
            Assert.AreEqual((long)Player.ID, actionResult[0].PlayerId);
            Assert.IsTrue(defaultGameMaster.GetBoard.GetGoalField(2, 1).HasPlayer());
            Assert.AreEqual(GoalFieldType.nongoal, actionResult[0].Type);
            Assert.AreEqual(expectedScore, defaultGameMaster.GoalsBlueLeft);
        }

        [TestMethod]
        public void GameMasterPlacesShamPieceOnNotOccupiedTaskField()
        {
            InitGameMaster();
            var Player = GetPlayer("testGUID-0003", 10, TeamColour.blue, ActionType.PlacePiece);
            // equip an Player with a sham piece
            var messagePieceKnown = new Piece(PieceType.sham, 90)
            {
                timestamp = DateTime.Now,
            };
            EquipPlayerWithPiece(messagePieceKnown, Player);

            Player.SetLocation(1, 5); // we change a location of an original object

            // set an Player on a TaskField
            var setPositionResult = defaultGameMaster.SetAbsolutePlayerLocation(1, 5, "testGUID-0003"); // we change a location of GM's copy
            // assure there is no piece on the field
            defaultGameMaster.GetBoard.GetTaskField(1, 5).Piece = null;

            // action: Player places a piece
            var actionResult = defaultGameMaster.TryPlacePieceOnTaskField(new GameArea.GameObjects.Location(1, 5), "testGUID-0003");

            var PlayerGameMasterCopy = defaultGameMaster.GetPlayers.Where(q => q.GUID == "testGUID-0003").First();

            Assert.AreEqual(true, setPositionResult);
            Assert.AreEqual(1, actionResult[0].X);
            Assert.AreEqual(5, actionResult[0].Y);
            Assert.AreEqual((long)Player.ID, actionResult[0].PlayerId);
            Assert.IsTrue(defaultGameMaster.GetBoard.GetTaskField(1, 5).HasPlayer());
            Assert.IsNotNull(defaultGameMaster.GetBoard.GetTaskField(1, 5).Piece);
            Assert.AreEqual(90ul, defaultGameMaster.GetBoard.GetTaskField(1, 5).Piece.ID);
            Assert.IsNull(PlayerGameMasterCopy.GetPiece);
        }

        [TestMethod]
        public void GameMasterPlacesShamPieceOnOccupiedTaskField()
        {
            InitGameMaster();
            var Player = GetPlayer("testGUID-0003", 10, TeamColour.blue, ActionType.PlacePiece);
            // equip an Player with a sham piece
            var messagePieceKnown = new Piece(PieceType.sham, 90)
            {
                timestamp = DateTime.Now,
            };
            EquipPlayerWithPiece(messagePieceKnown, Player);

            Player.SetLocation(1, 5); // we change a location of an original object

            // set an Player on a TaskField
            var setPositionResult = defaultGameMaster.SetAbsolutePlayerLocation(1, 5, "testGUID-0003"); // we change a location of GM's copy
            // assure there is a piece on the field
            defaultGameMaster.GetBoard.GetTaskField(1, 5).Piece = new GameObjects.Piece(new Messages.Piece(PieceType.unknown, 100));

            // action: Player places a piece
            var actionResult = defaultGameMaster.TryPlacePieceOnTaskField(new GameArea.GameObjects.Location(1, 5), "testGUID-0003");

            var PlayerGameMasterCopy = defaultGameMaster.GetPlayers.Where(q => q.GUID == "testGUID-0003").First();

            Assert.AreEqual(true, setPositionResult);
            Assert.IsNull(actionResult[0]);
            Assert.IsTrue(defaultGameMaster.GetBoard.GetTaskField(1, 5).HasPlayer());
            Assert.IsNotNull(defaultGameMaster.GetBoard.GetTaskField(1, 5).Piece);
            Assert.AreEqual(100ul, defaultGameMaster.GetBoard.GetTaskField(1, 5).Piece.ID);
            Assert.IsNotNull(PlayerGameMasterCopy.GetPiece);
        } 

        [TestMethod]
        public void GameMasterPerformsMoveActionToTaskField()
        {
            InitGameMaster();
            var Player = GetPlayer("testGUID-0013", 10, TeamColour.blue, ActionType.Move);
            var currentLocation = new GameArea.GameObjects.Location(2, 4);
            var futureLocation = new GameArea.GameObjects.Location(2, 5);
            Player.SetLocation(2, 4);

            defaultGameMaster.RegisterPlayer(Player, Player.GUID, findFreeLocationAndPlacePlayer: false);

            // set an Player on a TaskField
            var setPositionResult = defaultGameMaster.SetAbsolutePlayerLocation(2, 4, "testGUID-0013"); // we change a location of GM's copy

            Assert.IsNotNull(defaultGameMaster.GetBoard.GetField(2, 4).Player);

            // action: Player moves up to (2,5)
            AppMessages.DataMessage response = new AppMessages.DataMessage(new Messages.Data()
            {
                playerId = Player.ID,
                TaskFields = new Messages.TaskField[] { },
                GoalFields = null,
                PlayerLocation = currentLocation.ToBase()
            });
            Messages.Piece piece;
            Messages.Field field;
            defaultGameMaster.PerformMoveAction(currentLocation, futureLocation, "testGUID-0013", response);

            var PlayerGameMasterCopy = defaultGameMaster.GetPlayerByGuid("testGUID-0013");

            Assert.AreEqual(true, setPositionResult);
            Assert.AreEqual(GameArea.GameObjects.FieldType.Task, defaultGameMaster.GetBoard.GetField(2, 4).GetFieldType);
            Assert.AreEqual(GameArea.GameObjects.FieldType.Task, defaultGameMaster.GetBoard.GetField(2, 5).GetFieldType);
            Assert.AreEqual(futureLocation, PlayerGameMasterCopy.Location);
            Assert.IsNull(defaultGameMaster.GetBoard.GetField(2, 4).Player);
            Assert.IsNotNull(defaultGameMaster.GetBoard.GetField(2, 5).Player);
            Assert.AreEqual(Player.ID, defaultGameMaster.GetBoard.GetField(2, 5).Player.ID);
            Assert.AreEqual(futureLocation, response.PlayerLocation);
        }

        [TestMethod]
        public void GameMasterSetsInfoAboutDiscoveredTaskField()
        {
            InitGameMaster();
            var Player = GetPlayer("testGUID-0013", 10, TeamColour.blue, ActionType.Discover);
            List<GameArea.GameObjects.TaskField> list = new List<GameArea.GameObjects.TaskField>();
            var field = defaultGameMaster.GetBoard.GetTaskField(2, 5);
            var piece = new Messages.Piece(PieceType.normal, 10);
            field.Piece = new GameObjects.Piece(piece);
            Player.SetLocation(2, 5);
            defaultGameMaster.RegisterPlayer(Player, Player.GUID, findFreeLocationAndPlacePlayer: false);
            // set an Player on a TaskField
            var setPositionResult = defaultGameMaster.SetAbsolutePlayerLocation(2, 5, "testGUID-0013"); // we change a location of GM's copy

            Assert.IsNotNull(defaultGameMaster.GetBoard.GetField(2, 5).Player);
            Assert.AreEqual(true, setPositionResult);

            defaultGameMaster.SetInfoAboutDiscoveredTaskField(new GameArea.GameObjects.Location(2, 5), 0, 0, field, list);
            var responseField = list[0];

            Assert.AreEqual(2, responseField.X);
            Assert.AreEqual(5, responseField.Y);
            Assert.AreEqual(Player.ID, responseField.Player.ID);
            Assert.AreEqual(piece.id, responseField.Piece.ID);
        }

        [TestMethod]
        public void GameMasterSetsInfoAboutDiscoveredGoalkField()
        {
            InitGameMaster();
            var Player = GetPlayer("testGUID-0013", 10, TeamColour.blue, ActionType.Discover);
            List<GameArea.GameObjects.GoalField> list = new List<GameArea.GameObjects.GoalField>();
            var field = defaultGameMaster.GetBoard.GetGoalField(2, 2);
            Player.SetLocation(2, 2);
            defaultGameMaster.RegisterPlayer(Player, Player.GUID, findFreeLocationAndPlacePlayer: false);
            // set an Player on a TaskField
            var setPositionResult = defaultGameMaster.SetAbsolutePlayerLocation(2, 2, "testGUID-0013"); // we change a location of GM's copy

            Assert.IsNotNull(defaultGameMaster.GetBoard.GetField(2, 2).Player);
            Assert.AreEqual(true, setPositionResult);

            defaultGameMaster.SetInfoAboutDiscoveredGoalField(new GameArea.GameObjects.Location(2, 2), 0, 0, field, list);
            var responseField = list[0];

            Assert.AreEqual(2, responseField.X);
            Assert.AreEqual(2, responseField.Y);
            Assert.AreEqual(Player.ID, responseField.Player.ID);
        }
    }
}
