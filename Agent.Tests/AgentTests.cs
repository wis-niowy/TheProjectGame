using Microsoft.VisualStudio.TestTools.UnitTesting;
using GameArea;
using Messages;
using System;
using Configuration;
using Player.PlayerMessages;
using System.Collections.Generic;

namespace Player.Tests
{
    [TestClass]
    public class PlayerTests
    {
        GameMasterSettings settings;
        GameArea.GameMaster gameMaster;
        string guid;

        public void InitGameMaster()
        {
            settings = GameMasterSettings.GetDefaultGameMasterSettings();
            gameMaster = new GameArea.GameMaster(settings);
        }
        public Player GetPlayer(string guid, ulong id, TeamColour tc, ActionType action)
        {
            var player = new Player(tc, _guid: guid);
            player.ID = id;
            player.LastActionTaken = action;

            return player;
        }
        public void EquipPlayerWithPiece(Piece piece, Player player)
        {
            player.SetPiece(new GameArea.GameObjects.Piece(piece));

            gameMaster.RegisterPlayer(player, player.GUID, findFreeLocationAndPlacePlayer: false);

            var pieceUnknown = new Piece(PieceType.unknown, piece.id)
            {
                timestamp = piece.timestamp,

            };
            player.SetPiece(new GameArea.GameObjects.Piece(pieceUnknown));
        }


        ///// TEST BELOW //////

        [TestMethod]
        public void NewPlayer()
        {
            var Player = new Player(TeamColour.blue);
            Assert.IsNotNull(Player);
            Assert.AreEqual(0, Player.Location.X);
            Assert.AreEqual(0, Player.Location.Y);
            Assert.AreEqual(TeamColour.blue, Player.Team);
            Assert.AreEqual("TEST_GUID", Player.GUID);
        }

        [TestMethod]
        public void GuidSet()
        {
            var Player = new Player(TeamColour.blue);
            Player.GUID = "kakao";
            Assert.AreEqual("kakao", Player.GUID);
        }

        [TestMethod]
        public void BoardSet()
        {
            var board = new GameArea.GameObjects.GameBoard(2, 2, 2);
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
            var newLocation = new GameArea.GameObjects.Location(2, 3);
            Player.SetLocation(newLocation);
            Assert.AreEqual(newLocation.X, Player.Location.X);
            Assert.AreEqual(newLocation.Y, Player.Location.Y);
        }

        // actions tests

        [TestMethod]
        public void PlayerWithoutPieceTestsPiece()
        {
            InitGameMaster();
            var Player = GetPlayer("testGUID-0001", 10, TeamColour.red, ActionType.TestPiece);

            gameMaster.RegisterPlayer(Player, Player.GUID, findFreeLocationAndPlacePlayer: false);

            DataAgent data = new DataAgent(Player.ID)
            {
                PlayerGUID = guid,
                GameFinished = false,
                PlayerLocation = Player.Location
            };

            // action
            data.Process(Player);

            // assert
            Assert.IsNull(Player.GetPiece);
        }

        [TestMethod]
        public void PlayerWithShamPieceTestsPiece()
        {
            InitGameMaster();
            var Player = GetPlayer("testGUID-0001", 10, TeamColour.red, ActionType.TestPiece);

            // equip an Player with a sham piece
            var messagePieceKnown = new Piece(PieceType.sham, 100)
            {
                timestamp = DateTime.Now,
            };
            EquipPlayerWithPiece(messagePieceKnown, Player);

            DataAgent data = new DataAgent(Player.ID)
            {
                PlayerGUID = guid,
                GameFinished = false,
                PlayerLocation = Player.Location,
                Pieces = new GameArea.GameObjects.Piece[] { new GameArea.GameObjects.Piece(messagePieceKnown) }
            };

            // action
            data.Process(Player);

            // assert
            Assert.AreEqual(PieceType.sham, Player.GetPiece.Type);
        }

        [TestMethod]
        public void PlayerWithNormalPieceTestsPiece()
        {
            InitGameMaster();
            var Player = GetPlayer("testGUID-0002", 10, TeamColour.blue, ActionType.TestPiece);

            // equip an Player with a normal piece
            var messagePieceKnown = new Piece(PieceType.normal, 100)
            {
                timestamp = DateTime.Now,
            };
            EquipPlayerWithPiece(messagePieceKnown, Player);

            DataAgent data = new DataAgent(Player.ID)
            {
                PlayerGUID = guid,
                GameFinished = false,
                PlayerLocation = Player.Location,
                Pieces = new GameArea.GameObjects.Piece[] { new GameArea.GameObjects.Piece(messagePieceKnown) }
            };

            // action
            data.Process(Player);

            // assert
            Assert.AreEqual(PieceType.normal, Player.GetPiece.Type);
        }


        [TestMethod]
        public void PlayerPlacesShamPieceOnNotOccupiedTaskField()
        {
            InitGameMaster();
            var Player = GetPlayer("testGUID-0003", 10, TeamColour.blue, ActionType.PlacePiece);

            // equip an Player with a sham piece
            var messagePieceKnown = new Piece(PieceType.sham, 100)
            {
                timestamp = DateTime.Now,
            };
            EquipPlayerWithPiece(messagePieceKnown, Player);
            
            Player.SetLocation(1, 5); // we change a location of an original object

            var taskField = new TaskField(1, 5)
            {
                pieceIdSpecified = true,
                pieceId = messagePieceKnown.id,
                playerIdSpecified = true,
                playerId = Player.ID,
                timestamp = DateTime.Now,
            };

            DataAgent data = new DataAgent(Player.ID)
            {
                PlayerGUID = guid,
                GameFinished = false,
                PlayerLocation = Player.Location,
                Tasks = new GameArea.GameObjects.TaskField[] { new GameArea.GameObjects.TaskField(taskField) }
            };

            // action: Player places a piece
            data.Process(Player);

            // assert
            Assert.IsNull(Player.GetPiece);
            Assert.IsNotNull(Player.GetBoard.GetTaskField(Player.Location));
            Assert.AreEqual(messagePieceKnown.id, Player.GetBoard.GetTaskField(Player.Location).Piece.ID);
            //Assert.AreEqual(PieceType.sham, Player.GetBoard.GetTaskField(Player.Location).Piece.Type);
        }

        [TestMethod]
        public void PlayerPlacesShamPieceOnOccupiedTaskField()
        {
            InitGameMaster();
            var Player = GetPlayer("testGUID-0003", 10, TeamColour.blue, ActionType.PlacePiece);

            // equip an Player with a sham piece
            var messagePieceKnown = new Piece(PieceType.sham, 100)
            {
                timestamp = DateTime.Now,
            };
            EquipPlayerWithPiece(messagePieceKnown, Player);

            Player.SetLocation(1, 5); // we change a location of an original object

            DataAgent data = new DataAgent(Player.ID)
            {
                PlayerGUID = guid,
                GameFinished = false,
                PlayerLocation = Player.Location,
                Tasks = new GameArea.GameObjects.TaskField[] { null }
            };

            Player.GetBoard.GetTaskField(Player.Location).Piece = new GameArea.GameObjects.Piece(new Piece(PieceType.unknown, 90));

            // action: Player places a piece
            data.Process(Player);

            // assert
            Assert.IsNotNull(Player.GetPiece);
            Assert.IsNotNull(Player.GetBoard.GetTaskField(Player.Location).Piece);
            Assert.AreNotEqual(Player.GetBoard.GetTaskField(Player.Location).Piece.ID, messagePieceKnown.id);
            Assert.AreEqual(messagePieceKnown.id, Player.GetPiece.ID);
        }

        [TestMethod]
        public void PlayerPlacesNormalPieceOnNotOccupiedTaskField()
        {
            InitGameMaster();
            var Player = GetPlayer("testGUID-0004", 10, TeamColour.blue, ActionType.PlacePiece);

            // equip an Player with a sham piece
            var messagePieceKnown = new Piece(PieceType.normal, 100)
            {
                timestamp = DateTime.Now,
            };
            EquipPlayerWithPiece(messagePieceKnown, Player);
           
            Player.SetLocation(1, 5); // we change a location of an original object

            var taskField = new TaskField(1, 5)
            {
                pieceIdSpecified = true,
                pieceId = messagePieceKnown.id,
                playerIdSpecified = true,
                playerId = Player.ID,
                timestamp = DateTime.Now,
            };

            DataAgent data = new DataAgent(Player.ID)
            {
                PlayerGUID = guid,
                GameFinished = false,
                PlayerLocation = Player.Location,
                Tasks = new GameArea.GameObjects.TaskField[] { new GameArea.GameObjects.TaskField(taskField) }
            };

            // action: Player places a piece
            data.Process(Player);

            // assert
            Assert.IsNull(Player.GetPiece);
            Assert.IsNotNull(Player.GetBoard.GetTaskField(Player.Location).Piece);
            Assert.AreEqual(messagePieceKnown.id, Player.GetBoard.GetTaskField(Player.Location).Piece.ID);
            //Assert.AreEqual(PieceType.normal, Player.GetBoard.GetTaskField(Player.Location).Piece.Type);
        }

        [TestMethod]
        public void PlayerPlacesShamPieceOnGoalField()
        {
            InitGameMaster();
            var Player = GetPlayer("testGUID-0004", 10, TeamColour.blue, ActionType.PlacePiece);

            // equip an Player with a sham piece
            var messagePieceKnown = new Piece(PieceType.sham, 100)
            {
                timestamp = DateTime.Now,
            };
            EquipPlayerWithPiece(messagePieceKnown, Player);

            Player.SetLocation(1, 2); // we change a location of an original object

            var goalField = new GoalField(1, 2, TeamColour.blue)
            {
                playerIdSpecified = true,
                playerId = Player.ID,
                timestamp = DateTime.Now,
                type = GoalFieldType.unknown,
            };

            DataAgent data = new DataAgent(Player.ID)
            {
                PlayerGUID = guid,
                GameFinished = false,
                PlayerLocation = Player.Location,
                Goals = new GameArea.GameObjects.GoalField[] { new GameArea.GameObjects.GoalField(goalField) }
            };

            // action: Player places a piece
            data.Process(Player);

            // assert
            Assert.IsNotNull(Player.GetPiece);
            //Assert.AreEqual(PieceType.normal, Player.GetBoard.GetTaskField(Player.Location).Piece.Type);
        }

        [TestMethod]
        public void PlayerPlacesNormalPieceOnGoalFieldOfTypeGoal()
        {
            InitGameMaster();
            var Player = GetPlayer("testGUID-0004", 10, TeamColour.blue, ActionType.PlacePiece);

            // equip an Player with a sham piece
            var messagePieceKnown = new Piece(PieceType.normal, 100)
            {
                timestamp = DateTime.Now,
            };
            EquipPlayerWithPiece(messagePieceKnown, Player);

            Player.SetLocation(1, 1); // we change a location of an original object

            var goalField = new GoalField(1, 1, TeamColour.blue)
            {
                playerIdSpecified = true,
                playerId = Player.ID,
                timestamp = DateTime.Now,
                type = GoalFieldType.goal,
            };

            DataAgent data = new DataAgent(Player.ID)
            {
                PlayerGUID = guid,
                GameFinished = false,
                PlayerLocation = Player.Location,
                Goals = new GameArea.GameObjects.GoalField[] { new GameArea.GameObjects.GoalField(goalField) }
            };

            // action: Player places a piece
            data.Process(Player);

            // assert
            Assert.IsNull(Player.GetPiece);
            Assert.AreEqual(GoalFieldType.goal, Player.GetBoard.GetGoalField(Player.Location).Type);
        }

        [TestMethod]
        public void PlayerPlacesNormalPieceOnGoalFieldOfTypeNonGoal()
        {
            InitGameMaster();
            var Player = GetPlayer("testGUID-0004", 10, TeamColour.blue, ActionType.PlacePiece);

            // equip an Player with a sham piece
            var messagePieceKnown = new Piece(PieceType.normal, 100)
            {
                timestamp = DateTime.Now,
            };
            EquipPlayerWithPiece(messagePieceKnown, Player);

            Player.SetLocation(4, 1); // we change a location of an original object

            var goalField = new GoalField(4, 1, TeamColour.blue)
            {
                playerIdSpecified = true,
                playerId = Player.ID,
                timestamp = DateTime.Now,
                type = GoalFieldType.nongoal,
            };

            DataAgent data = new DataAgent(Player.ID)
            {
                PlayerGUID = guid,
                GameFinished = false,
                PlayerLocation = Player.Location,
                Goals = new GameArea.GameObjects.GoalField[] { new GameArea.GameObjects.GoalField(goalField) }
            };

            // action: Player places a piece
            data.Process(Player);

            // assert
            Assert.IsNotNull(Player.GetPiece);
            Assert.AreEqual(GoalFieldType.nongoal, Player.GetBoard.GetGoalField(Player.Location).Type);
        }

        [TestMethod]
        public void PlayerPicksUpPieceFromTaskField()
        {
            InitGameMaster();
            var Player = GetPlayer("testGUID-0004", 10, TeamColour.blue, ActionType.PickUpPiece);
            Player.SetLocation(2, 5);

            gameMaster.RegisterPlayer(Player, Player.GUID, findFreeLocationAndPlacePlayer: false);

            var piece = new Piece(PieceType.unknown, 15)
            {
                playerIdSpecified = true,
                timestamp = DateTime.Now,
                playerId = Player.ID,
            };

            DataAgent data = new DataAgent(Player.ID)
            {
                PlayerGUID = Player.GUID,
                GameFinished = false,
                PlayerLocation = Player.Location,
                Pieces = new GameArea.GameObjects.Piece[] { new GameArea.GameObjects.Piece(piece) }
            };

            Player.GetBoard.GetTaskField(Player.Location).Piece = new GameArea.GameObjects.Piece(piece);

            // action: Player places a piece
            data.Process(Player);

            Assert.IsNotNull(Player.GetPiece);
            Assert.IsNull(Player.GetBoard.GetTaskField(Player.Location).Piece);
            Assert.AreEqual(PieceType.unknown, Player.GetPiece.Type);
        }

        [TestMethod]
        public void PlayerPicksUpFromEmptyTaskField()
        {
            InitGameMaster();
            var Player = GetPlayer("testGUID-0004", 10, TeamColour.blue, ActionType.PickUpPiece);
            Player.SetLocation(2, 5);

            gameMaster.RegisterPlayer(Player, Player.GUID, findFreeLocationAndPlacePlayer: false);

            DataAgent data = new DataAgent(Player.ID)
            {
                PlayerGUID = Player.GUID,
                GameFinished = false,
                PlayerLocation = Player.Location,
                Pieces = new GameArea.GameObjects.Piece[] { null }
            };

            Player.GetBoard.GetTaskField(Player.Location).Piece = null;

            // action: Player places a piece
            data.Process(Player);

            Assert.IsNull(Player.GetPiece);
            Assert.IsNull(Player.GetBoard.GetTaskField(Player.Location).Piece);
            
        }

        ///// MOVE tests

        [TestMethod]
        public void PlayerMovesFromTaskFieldToEmptyTaskField()
        {
            // from (2, 4) to (2, 5)
            InitGameMaster();
            var Player = GetPlayer("testGUID-0004", 10, TeamColour.blue, ActionType.Move);
            Player.SetLocation(2, 4);

            gameMaster.RegisterPlayer(Player, Player.GUID, findFreeLocationAndPlacePlayer: false);

            var taskField = new TaskField(2, 5)
            {
                pieceIdSpecified = true,
                pieceId = 250,
                timestamp = DateTime.Now,
            };

            DataAgent data = new DataAgent(Player.ID)
            {
                PlayerGUID = guid,
                GameFinished = false,
                PlayerLocation = Player.Location,
                Tasks = new GameArea.GameObjects.TaskField[] { new GameArea.GameObjects.TaskField(taskField) }
            };

            // action: Player moves up to (2,5)
            data.Process(Player);

            // assert
            Assert.AreEqual(new GameArea.GameObjects.Location(2, 5), Player.Location);
            Assert.IsNull(Player.GetBoard.GetField(2, 4).Player);
            Assert.IsNull(Player.GetBoard.GetField(2, 5).Player);
            Assert.AreEqual(taskField.pieceId, Player.GetBoard.GetTaskField(2, 5).Piece.ID);
        }

        [TestMethod]
        public void PlayerMovesFromTaskFieldToOccupiedTaskField()
        {
            // from (2, 4) to (2, 5)
            InitGameMaster();
            var Player = GetPlayer("testGUID-0004", 10, TeamColour.blue, ActionType.Move);
            Player.SetLocation(2, 4);

            gameMaster.RegisterPlayer(Player, Player.GUID, findFreeLocationAndPlacePlayer: false);

            var taskField = new TaskField(2, 5)
            {
                playerIdSpecified = true,
                playerId = 5,
                pieceIdSpecified = true,
                pieceId = 250,
                timestamp = DateTime.Now,
            };

            DataAgent data = new DataAgent(Player.ID)
            {
                PlayerGUID = guid,
                GameFinished = false,
                PlayerLocation = Player.Location,
                Tasks = new GameArea.GameObjects.TaskField[] { new GameArea.GameObjects.TaskField(taskField) }
            };

            // action: Player moves up to (2,5)
            data.Process(Player);

            // assert
            Assert.AreEqual(new GameArea.GameObjects.Location(2, 4), Player.Location);
            Assert.IsNull(Player.GetBoard.GetField(2, 4).Player);
            Assert.IsNotNull(Player.GetBoard.GetField(2, 5).Player);
            Assert.AreEqual(taskField.playerId, Player.GetBoard.GetField(2, 5).Player.ID); // check if Player has stored an encountered stranger in his board view
            Assert.AreEqual(taskField.pieceId, Player.GetBoard.GetTaskField(2, 5).Piece.ID);

        }

        [TestMethod]
        public void PlayerMovesFromTaskFieldToRightEmptyGoalField()
        {
            // from (2, 3) to (2, 2)
            InitGameMaster();
            var Player = GetPlayer("testGUID-0004", 10, TeamColour.blue, ActionType.Move);
            Player.SetLocation(2, 3);

            gameMaster.RegisterPlayer(Player, Player.GUID, findFreeLocationAndPlacePlayer: false);

            var goalField = new GoalField(2, 2, TeamColour.blue, GoalFieldType.unknown)
            {
                timestamp = DateTime.Now,
            };

            DataAgent data = new DataAgent(Player.ID)
            {
                PlayerGUID = guid,
                GameFinished = false,
                PlayerLocation = Player.Location,
                Goals = new GameArea.GameObjects.GoalField[] { new GameArea.GameObjects.GoalField(goalField) }
            };

            // action: Player moves down to (2,2)
            data.Process(Player);

            Assert.AreEqual(new GameArea.GameObjects.Location(2, 2), Player.Location);
            Assert.IsNull(Player.GetBoard.GetField(2, 3).Player);
            Assert.IsNull(Player.GetBoard.GetField(2, 2).Player);
        }

        [TestMethod]
        public void PlayerMovesFromTaskFieldToToRightOccupiedGoalField()
        {
            // from (2, 3) to (2, 2)
            InitGameMaster();
            var Player = GetPlayer("testGUID-0004", 10, TeamColour.blue, ActionType.Move);
            Player.SetLocation(2, 3);

            gameMaster.RegisterPlayer(Player, Player.GUID, findFreeLocationAndPlacePlayer: false);

            var goalField = new GoalField(2, 2, TeamColour.blue, GoalFieldType.unknown)
            {
                playerIdSpecified = true,
                playerId = 7,
                timestamp = DateTime.Now,
            };

            DataAgent data = new DataAgent(Player.ID)
            {
                PlayerGUID = guid,
                GameFinished = false,
                PlayerLocation = Player.Location,
                Goals = new GameArea.GameObjects.GoalField[] { new GameArea.GameObjects.GoalField(goalField) }
            };

            // action: Player moves down to (2,2)
            data.Process(Player);

            Assert.AreEqual(new GameArea.GameObjects.Location(2, 3), Player.Location);
            Assert.IsNull(Player.GetBoard.GetField(2, 3).Player);
            Assert.IsNotNull(Player.GetBoard.GetField(2, 2).Player);
            Assert.AreEqual(goalField.playerId, Player.GetBoard.GetField(2, 2).Player.ID); // check if Player has stored an encountered stranger in his board view
        }

        [TestMethod]
        public void PlayerMovesFromTaskFieldToWrongNotOccupiedGoalField()
        {
            // from (1, 9) to (1, 10)
            InitGameMaster();
            var Player = GetPlayer("testGUID-0004", 10, TeamColour.blue, ActionType.Move);
            Player.SetLocation(1, 9);

            gameMaster.RegisterPlayer(Player, Player.GUID, findFreeLocationAndPlacePlayer: false);

            DataAgent data = new DataAgent(Player.ID)
            {
                PlayerGUID = guid,
                GameFinished = false,
                PlayerLocation = Player.Location,
                Goals = new GameArea.GameObjects.GoalField[] { }
            };

            // action: Player moves down to (1, 10)
            data.Process(Player);

            Assert.AreEqual(new GameArea.GameObjects.Location(1, 9), Player.Location);
            Assert.IsNull(gameMaster.GetBoard.GetField(1, 9).Player);
            Assert.IsNull(gameMaster.GetBoard.GetField(1, 10).Player);
        }

        [TestMethod]
        public void PlayerMovesFromTaskFieldOutOfBoard()
        {
            // from (4, 7) to (5, 7)
            InitGameMaster();
            var Player = GetPlayer("testGUID-0004", 10, TeamColour.blue, ActionType.Move);
            Player.SetLocation(4, 7);

            gameMaster.RegisterPlayer(Player, Player.GUID, findFreeLocationAndPlacePlayer: false);

            DataAgent data = new DataAgent(Player.ID)
            {
                PlayerGUID = guid,
                GameFinished = false,
                PlayerLocation = Player.Location,
                Goals = new GameArea.GameObjects.GoalField[] { }
            };

            // action: Player moves down to (5, 7)
            data.Process(Player);

            Assert.AreEqual(new GameArea.GameObjects.Location(4, 7), Player.Location);
            Assert.IsNull(gameMaster.GetBoard.GetField(4, 7).Player);
            Assert.IsNull(gameMaster.GetBoard.GetField(5, 7));
        }

        [TestMethod]
        public void PlayerDiscoveryNothingInSight()
        {
            InitGameMaster();
            var Player = GetPlayer("testGUID-0004", 10, TeamColour.blue, ActionType.Discover);
            Player.SetLocation(1, 5);

            gameMaster.RegisterPlayer(Player, Player.GUID, findFreeLocationAndPlacePlayer: false);

            List<GameArea.GameObjects.TaskField> list = new List<GameArea.GameObjects.TaskField>();
            for (int i = 0; i < 9; ++i)
            {
                int xIdx = i % 3;
                int yIdx = i / 3 + 4;
                if (xIdx == 1 && yIdx == 5) continue;
                list.Add(new GameArea.GameObjects.TaskField(new TaskField((uint)xIdx, (uint)yIdx)
                {
                    timestamp = DateTime.Now,
                    distanceToPiece = i + 1, // przykladowe dystanse - tylko dla testu czy Player sobie zapisuje u siebie
                }));
            }

            DataAgent data = new DataAgent(Player.ID)
            {
                PlayerGUID = guid,
                GameFinished = false,
                PlayerLocation = Player.Location,
                Tasks = list.ToArray(),
            };

            // action: Player discovers area
            data.Process(Player);

            Assert.AreEqual(new GameArea.GameObjects.Location(1, 5), Player.Location);
            for (int i = 0; i < 9; ++i)
            {
                int xIdx = i % 3;
                int yIdx = i / 3 + 4;
                if (xIdx == 1 && yIdx == 5) continue;
                Assert.IsNull(Player.GetBoard.GetTaskField(xIdx, yIdx).Player);
                Assert.IsNull(Player.GetBoard.GetTaskField(xIdx, yIdx).Piece);
                Assert.AreEqual(i + 1, Player.GetBoard.GetTaskField(xIdx, yIdx).DistanceToPiece);
            }
        }

        [TestMethod]
        public void PlayerDiscoverySeesPiece()
        {
            InitGameMaster();
            var Player = GetPlayer("testGUID-0004", 10, TeamColour.blue, ActionType.Discover);
            Player.SetLocation(1, 5);

            gameMaster.RegisterPlayer(Player, Player.GUID, findFreeLocationAndPlacePlayer: false);

            List<GameArea.GameObjects.TaskField> list = new List<GameArea.GameObjects.TaskField>();
            for (int i = 0; i < 9; ++i)
            {
                int xIdx = i % 3;
                int yIdx = i / 3 + 4;
                if (xIdx == 1 && yIdx == 5) continue;
                list.Add(new GameArea.GameObjects.TaskField(new TaskField((uint)xIdx, (uint)yIdx)
                {
                    timestamp = DateTime.Now,
                    distanceToPiece = i + 1, // przykladowe dystanse - tylko dla testu czy Player sobie zapisuje u siebie
                }));
            }

            list[1] = new GameArea.GameObjects.TaskField(new TaskField((uint)0, (uint)5)
            {
                pieceIdSpecified = true,
                pieceId = 15,
                timestamp = DateTime.Now,
                distanceToPiece = 0,
            });

            DataAgent data = new DataAgent(Player.ID)
            {
                PlayerGUID = guid,
                GameFinished = false,
                PlayerLocation = Player.Location,
                Tasks = list.ToArray(),
            };

            // action: Player discovers area
            data.Process(Player);

            Assert.AreEqual(new GameArea.GameObjects.Location(1, 5), Player.Location);
            Assert.IsNull(Player.GetBoard.GetTaskField(0, 5).Player);
            Assert.IsNotNull(Player.GetBoard.GetTaskField(0, 5).Piece);
            Assert.AreEqual(0, Player.GetBoard.GetTaskField(0, 5).DistanceToPiece);
            Assert.AreEqual(15ul, Player.GetBoard.GetTaskField(0, 5).Piece.ID);
        }

        [TestMethod]
        public void PlayerDiscoverySeesPlayerInTaskArea()
        {
            InitGameMaster();
            var Player = GetPlayer("testGUID-0004", 10, TeamColour.blue, ActionType.Discover);
            Player.SetLocation(1, 5);

            gameMaster.RegisterPlayer(Player, Player.GUID, findFreeLocationAndPlacePlayer: false);

            List<GameArea.GameObjects.TaskField> list = new List<GameArea.GameObjects.TaskField>();
            for (int i = 0; i < 9; ++i)
            {
                int xIdx = i % 3;
                int yIdx = i / 3 + 4;
                if (xIdx == 1 && yIdx == 5) continue;
                list.Add(new GameArea.GameObjects.TaskField(new TaskField((uint)xIdx, (uint)yIdx)
                {
                    timestamp = DateTime.Now,
                    distanceToPiece = i + 1, // przykladowe dystanse - tylko dla testu czy Player sobie zapisuje u siebie
                }));
            }

            list[2] = new GameArea.GameObjects.TaskField(new TaskField((uint)0, (uint)6)
            {
                playerIdSpecified = true,
                playerId = 3,
                timestamp = DateTime.Now,
                distanceToPiece = 1,
            });


            DataAgent data = new DataAgent(Player.ID)
            {
                PlayerGUID = guid,
                GameFinished = false,
                PlayerLocation = Player.Location,
                Tasks = list.ToArray(),
            };

            // action: Player discovers area
            data.Process(Player);

            Assert.AreEqual(new GameArea.GameObjects.Location(1, 5), Player.Location);
            Assert.IsNotNull(Player.GetBoard.GetTaskField(0, 6).Player);
            Assert.IsNull(Player.GetBoard.GetTaskField(0, 6).Piece);
            Assert.AreEqual(1, Player.GetBoard.GetTaskField(0, 6).DistanceToPiece);
            Assert.AreEqual(3ul, Player.GetBoard.GetTaskField(0, 6).Player.ID);
        }

        [TestMethod]
        public void PlayerDiscoverySeesPlayerInGoalArea()
        {
            InitGameMaster();
            var Player = GetPlayer("testGUID-0004", 10, TeamColour.blue, ActionType.Discover);
            Player.SetLocation(1, 3);

            gameMaster.RegisterPlayer(Player, Player.GUID, findFreeLocationAndPlacePlayer: false);

            List<GameArea.GameObjects.TaskField> listTask = new List<GameArea.GameObjects.TaskField>();
            List<GameArea.GameObjects.GoalField> listGoal = new List<GameArea.GameObjects.GoalField>();

            for (int i = 0; i < 6; ++i)
            {
                int xIdx = i % 3;
                int yIdx = i / 3 + 3;
                if (xIdx == 1 && yIdx == 4) continue;
                listTask.Add(new GameArea.GameObjects.TaskField(new TaskField((uint)xIdx, (uint)yIdx)
                {
                    timestamp = DateTime.Now,
                    distanceToPiece = i + 1, // przykladowe dystanse - tylko dla testu czy Player sobie zapisuje u siebie
                }));
            }
            for (int i = 0; i < 3; ++i)
            {
                int xIdx = i;
                int yIdx = 2;
                listGoal.Add(new GameArea.GameObjects.GoalField(new GoalField((uint)xIdx, (uint)yIdx, TeamColour.blue)
                {
                    playerIdSpecified = false,
                    timestamp = DateTime.Now,
                }));
            }
            listGoal[1] = new GameArea.GameObjects.GoalField(new GoalField((uint)1, (uint)2, TeamColour.blue)
            {
                playerIdSpecified = true,
                playerId = 4,
                timestamp = DateTime.Now,
            });

            DataAgent data = new DataAgent(Player.ID)
            {
                PlayerGUID = guid,
                GameFinished = false,
                PlayerLocation = Player.Location,
                Tasks = listTask.ToArray(),
                Goals = listGoal.ToArray(),
            };

            // action: Player discovers area
            data.Process(Player);

            Assert.AreEqual(new GameArea.GameObjects.Location(1, 3), Player.Location);
            Assert.IsNotNull(Player.GetBoard.GetGoalField(1, 2).Player);
            Assert.AreEqual(4ul, Player.GetBoard.GetGoalField(1, 2).Player.ID);
        }

        //[TestMethod]
        //public void PlayerDiscoveryNearBoardEdge()
        //{
        //    var Player = new Player(TeamColour.blue, "testGUID-0026");
        //    Player.SetLocation(0, 3);

        //    gameMaster.RegisterPlayer(Player,Player.GUID, findFreeLocationAndPlacePlayer : false);

        //    // set an Player on a TaskField
        //    var setPositionResult = gameMaster.SetAbsolutePlayerLocation(0, 3, "testGUID-0026"); // we change a location of GM's copy

        //    Assert.IsNotNull(gameMaster.GetBoard.GetField(0, 3).Player);


        //    // action: Player discovers area
        //    Player.Discover(gameMaster);

        //    Assert.AreEqual(true, setPositionResult);
        //    Assert.AreEqual(new Location(0, 3), Player.Location);
        //    Assert.IsNull(gameMaster.GetBoard.GetField(2, 5).Player);
        //    Assert.IsNull(Player.GetBoard.GetField(2, 5).Player);
        //    Assert.IsNotNull(gameMaster.GetBoard.GetField(0, 3).Player);
        //    Assert.AreEqual(Player.ID, gameMaster.GetBoard.GetField(0, 3).Player.id);
        //}

        //[TestMethod]
        //public void PlayerDiscoveryCorner()
        //{
        //    var Player1 = new Player(TeamColour.blue, "testGUID-0027");
        //    var Player2 = new Player(TeamColour.red, "testGUID-0028");
        //    Player1.SetLocation(0, 0);
        //    Player2.SetLocation(4, 9);

        //    gameMaster.RegisterPlayer(Player1,Player1.GUID, findFreeLocationAndPlacePlayer : false);
        //    gameMaster.RegisterPlayer(Player2,Player2.GUID, findFreeLocationAndPlacePlayer : false);

        //    // set an Player on a TaskField
        //    var setPositionResult1 = gameMaster.SetAbsolutePlayerLocation(0, 0, "testGUID-0027"); // we change a location of GM's copy
        //    var setPositionResult2 = gameMaster.SetAbsolutePlayerLocation(4, 9, "testGUID-0028"); // we change a location of GM's copy
        //    Assert.IsNotNull(gameMaster.GetBoard.GetField(0, 0).Player);
        //    Assert.IsNotNull(gameMaster.GetBoard.GetField(4, 9).Player);


        //    // action: Player discovers area
        //    Player1.Discover(gameMaster);

        //    Assert.AreEqual(true, setPositionResult1);
        //    Assert.AreEqual(new Location(0, 0), Player1.GetLocation);
        //    Assert.IsNull(gameMaster.GetBoard.GetField(2, 5).Player);
        //    Assert.IsNull(Player1.GetBoard.GetField(2, 5).Player);
        //    Assert.IsNotNull(gameMaster.GetBoard.GetField(0, 0).Player);
        //    Assert.AreEqual(Player1.ID, gameMaster.GetBoard.GetField(0, 0).Player.id);

        //    Assert.AreEqual(true, setPositionResult2);
        //    Assert.AreEqual(new Location(4, 9), Player2.GetLocation);
        //    Assert.IsNotNull(gameMaster.GetBoard.GetField(4, 9).Player);
        //    Assert.AreEqual(Player2.ID, gameMaster.GetBoard.GetField(4, 9).Player.id);
        //}
        //[TestMethod]
        //public void GoToNearestPieceInGoalAreaForBluePlayer()
        //{
        //    int x = 0, y = 0;
        //    var Player1 = new Player(TeamColour.blue, "testGUID-0027",gameMaster);
        //    Player1.SetLocation(x, y);

        //    gameMaster.RegisterPlayer(Player1, Player1.GUID, findFreeLocationAndPlacePlayer: false);


        //    var setPositionResult1 = gameMaster.SetAbsolutePlayerLocation(x,y, "testGUID-0027"); // we change a location of GM's copy
        //    Player1.GoToNearestPiece();
        //    //rusza sie o 2 do gory cos jest nie tak z TryMove (pozwala na 2 ruchy do gory)

        //    Location expectedLocationPlayer = new Location(x, y+1);

        //    Assert.AreEqual(expectedLocationPlayer, Player1.GetLocation);

        //}

        //[TestMethod]
        //public void GoToNearestPieceInGoalAreaForRedPlayer()
        //{
        //    int x = 0, y = 12;
        //    var Player1 = new Player(TeamColour.red, "testGUID-0027", gameMaster);
        //    Player1.SetLocation(x, y);

        //    gameMaster.RegisterPlayer(Player1, Player1.GUID, findFreeLocationAndPlacePlayer: false);


        //    var setPositionResult1 = gameMaster.SetAbsolutePlayerLocation(x, y, "testGUID-0027"); // we change a location of GM's copy
        //    Player1.GoToNearestPiece();
        //    //rusza sie o 2 do gory cos jest nie tak z TryMove (pozwala na 2 ruchy do gory)

        //    Location expectedLocationPlayer = new Location(x, y-1);

        //    Assert.AreEqual(expectedLocationPlayer, Player1.GetLocation);

        //}

        //[TestMethod]
        //public void GoToNearestPieceInTaskAreaForBluePlayer()
        //{
        //    //sometimes not passed because of methods: PlaceInitialPieces, InitPieceAdder
        //    //(it's not known where will be another new piece)
        //    //to ensure that it will be passed we have to change constructor for game master
        //    //(give choice for enabling pieceAdder
        //    //
        //    Location pieceLocation = new Location(1, 5);
        //    Location beforeMove = new Location(3, 6);
        //    ulong pieceId = 51;

        //    var Player1 = new Player(TeamColour.blue, "testGUID-0027", gameMaster);
        //    Player1.SetLocation(beforeMove);

        //    gameMaster.RegisterPlayer(Player1, Player1.GUID, findFreeLocationAndPlacePlayer: false);
        //    gameMaster.SetPieceInLocation(pieceLocation.x,pieceLocation.y, TeamColour.blue,PieceType.unknown, pieceId);

        //    var setPositionResult1 = gameMaster.SetAbsolutePlayerLocation(beforeMove.x,beforeMove.y, "testGUID-0027"); // we change a location of GM's copy
        //    Player1.GoToNearestPiece();
        //    //rusza sie o 2 do gory cos jest nie tak z TryMove (pozwala na 2 ruchy do gory)

        //    //Location expectedLocationPlayer = new Location();
        //    Location afterMove = Player1.GetLocation;
        //    Assert.IsTrue(beforeMove.x - 1 == afterMove.x || beforeMove.y -1 == afterMove.y);
        //}
        //[TestMethod]
        //public void TryPickPieceTest()
        //{
        //    Location pieceLocation = new Location(1, 5);
        //    Location PlayerLocation = new Location(1, 5);
        //    ulong pieceId = 51;

        //    var Player1 = new Player(TeamColour.blue, "testGUID-0027", gameMaster);
        //    Player1.SetLocation(PlayerLocation);

        //    gameMaster.RegisterPlayer(Player1, Player1.GUID, findFreeLocationAndPlacePlayer: false);
        //    gameMaster.SetPieceInLocation(pieceLocation.x, pieceLocation.y, TeamColour.blue, PieceType.unknown, pieceId);

        //    var setPositionResult1 = gameMaster.SetAbsolutePlayerLocation(PlayerLocation.x, PlayerLocation.y, "testGUID-0027"); // we change a location of GM's copy

        //    Assert.IsTrue(Player1.GetPiece == null);
        //    Player1.TryPickPiece();
        //    Assert.IsTrue(Player1.GetPiece != null);
        //}
        //[TestMethod]
        //public void TryTestPieceShamTest()
        //{
        //    Location pieceLocation = new Location(1, 5);
        //    Location PlayerLocation = new Location(1, 5);
        //    ulong pieceId = 51;

        //    PieceType expectedPieceType = PieceType.sham;

        //    var Player1 = new Player(TeamColour.blue, "testGUID-0027", gameMaster);
        //    Player1.SetLocation(PlayerLocation);

        //    gameMaster.RegisterPlayer(Player1, Player1.GUID, findFreeLocationAndPlacePlayer: false);
        //    gameMaster.SetPieceInLocation(pieceLocation.x, pieceLocation.y, TeamColour.blue, expectedPieceType, pieceId);

        //    var setPositionResult1 = gameMaster.SetAbsolutePlayerLocation(PlayerLocation.x, PlayerLocation.y, "testGUID-0027"); // we change a location of GM's copy

        //    Player1.PickUpPiece(gameMaster);


        //    Assert.IsTrue(Player1.TryTestPiece());
        //    Assert.AreEqual(expectedPieceType, Player1.GetPiece.type);
        //}

        //[TestMethod]
        //public void TryTestPieceValidTest()
        //{
        //    Location pieceLocation = new Location(1, 5);
        //    Location PlayerLocation = new Location(1, 5);
        //    ulong pieceId = 51;

        //    PieceType expectedPieceType = PieceType.normal;

        //    var Player1 = new Player(TeamColour.blue, "testGUID-0027", gameMaster);
        //    Player1.SetLocation(PlayerLocation);

        //    gameMaster.RegisterPlayer(Player1, Player1.GUID, findFreeLocationAndPlacePlayer: false);
        //    gameMaster.SetPieceInLocation(pieceLocation.x, pieceLocation.y, TeamColour.blue, expectedPieceType, pieceId);

        //    var setPositionResult1 = gameMaster.SetAbsolutePlayerLocation(PlayerLocation.x, PlayerLocation.y, "testGUID-0027"); // we change a location of GM's copy

        //    Player1.PickUpPiece(gameMaster);


        //    Assert.IsTrue(Player1.TryTestPiece());
        //    Assert.AreEqual(expectedPieceType, Player1.GetPiece.type);
        //}
        //[TestMethod]
        //public void GoToGoalAreaForBlueTeamTest()
        //{
        //    Location PlayerLocation = new Location(1, 5);

        //    var Player1 = new Player(TeamColour.blue, "testGUID-0027", gameMaster);
        //    Player1.SetLocation(PlayerLocation);

        //    gameMaster.RegisterPlayer(Player1, Player1.GUID, findFreeLocationAndPlacePlayer: false);

        //    var setPositionResult1 = gameMaster.SetAbsolutePlayerLocation(PlayerLocation.x, PlayerLocation.y, "testGUID-0027"); // we change a location of GM's copy


        //    //powinno byc Player1.GoToGoalArea()
        //    Player1.GoToGoalArea(Player1.Team);
        //    Location expectedLocationPlayer = new Location(PlayerLocation.x,PlayerLocation.y-1);

        //    Assert.AreEqual(expectedLocationPlayer, Player1.GetLocation);
        //}
        //[TestMethod]
        //public void GoToGoalAreaForRedTeamTest()
        //{
        //    Location PlayerLocation = new Location(1, 5);

        //    var Player1 = new Player(TeamColour.red, "testGUID-0027", gameMaster);
        //    Player1.SetLocation(PlayerLocation);

        //    gameMaster.RegisterPlayer(Player1, Player1.GUID, findFreeLocationAndPlacePlayer: false);

        //    var setPositionResult1 = gameMaster.SetAbsolutePlayerLocation(PlayerLocation.x, PlayerLocation.y, "testGUID-0027"); // we change a location of GM's copy


        //    //powinno byc Player1.GoToGoalArea()
        //    Player1.GoToGoalArea(Player1.Team);
        //    Location expectedLocationPlayer = new Location(PlayerLocation.x, PlayerLocation.y + 1);

        //    Assert.AreEqual(expectedLocationPlayer, Player1.GetLocation);
        //}
        ////slabo testowalny jest napisany default board z goalfieldami( za du¿o uknown field)
        //[TestMethod]
        //public void GetClosestUnknownGoalDirectionForRedTeamTestIfIsOnGoal()
        //{
        //    Location PlayerLocation = new Location(12,12);

        //    var Player1 = new Player(TeamColour.red, "testGUID-0027", gameMaster);
        //    Player1.SetLocation(PlayerLocation);

        //    gameMaster.RegisterPlayer(Player1, Player1.GUID, findFreeLocationAndPlacePlayer: false);

        //    var setPositionResult1 = gameMaster.SetAbsolutePlayerLocation(PlayerLocation.x, PlayerLocation.y, "testGUID-0027"); // we change a location of GM's copy


        //    MoveType PlayerDirection = Player1.GetClosestUnknownGoalDirection();
        //    //MoveType expectedPlayerDirection = MoveType.down;


        //    Assert.AreEqual(PlayerDirection , MoveType.left );
        //}

        ////slabo testowalny jest napisany default board z goalfieldami( za du¿o uknown field)
        //[TestMethod]
        //public void GetClosestUnknownGoalForBlueTeamTestIfIsOnGoal()
        //{
        //    Location PlayerLocation = new Location(1, 1);

        //    var Player1 = new Player(TeamColour.blue, "testGUID-0027", gameMaster);
        //    Player1.SetLocation(PlayerLocation);

        //    gameMaster.RegisterPlayer(Player1, Player1.GUID, findFreeLocationAndPlacePlayer: false);

        //    var setPositionResult1 = gameMaster.SetAbsolutePlayerLocation(PlayerLocation.x, PlayerLocation.y, "testGUID-0027"); // we change a location of GM's copy


        //    MoveType PlayerDirection = Player1.GetClosestUnknownGoalDirection();

        //    Assert.AreEqual(PlayerDirection, MoveType.left);
        //}
    }
}
