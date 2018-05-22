using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using GameArea;
using Messages;
using Player.PlayerMessages;

namespace Player.Tests
{
    public partial class PlayerTests
    {
        // actions tests

        [TestMethod]
        public void PlayerWithoutPieceTestsPiece()
        {
            InitGameMaster();
            var Player = GetPlayer("testGUID-0001", 10, TeamColour.red, ActionType.TestPiece);

            RegisterPlayer(Player, Player.GUID);

            DataAgent data = new DataAgent(Player.ID)
            {
                PlayerGUID = guid,
                GameFinished = false,
                PlayerLocation = Player.Location
            };

            // action
            data.Process(Player.Controller);

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
            data.Process(Player.Controller);

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
            data.Process(Player.Controller);

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
            data.Process(Player.Controller);

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
            data.Process(Player.Controller);

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
            data.Process(Player.Controller);

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
            data.Process(Player.Controller);

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
            data.Process(Player.Controller);

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
            data.Process(Player.Controller);

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

            RegisterPlayer(Player, Player.GUID);

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
            data.Process(Player.Controller);

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

            RegisterPlayer(Player, Player.GUID);

            DataAgent data = new DataAgent(Player.ID)
            {
                PlayerGUID = Player.GUID,
                GameFinished = false,
                PlayerLocation = Player.Location,
                Pieces = new GameArea.GameObjects.Piece[] { null }
            };

            Player.GetBoard.GetTaskField(Player.Location).Piece = null;

            // action: Player places a piece
            data.Process(Player.Controller);

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

            RegisterPlayer(Player, Player.GUID);

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
                PlayerLocation = new GameArea.GameObjects.Location(2, 5),
                Tasks = new GameArea.GameObjects.TaskField[] { new GameArea.GameObjects.TaskField(taskField) }
            };

            // action: Player moves up to (2,5)
            data.Process(Player.Controller);

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

            RegisterPlayer(Player, Player.GUID);

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
            data.Process(Player.Controller);

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

            RegisterPlayer(Player, Player.GUID);

            var goalField = new GoalField(2, 2, TeamColour.blue, GoalFieldType.unknown)
            {
                timestamp = DateTime.Now,
            };

            DataAgent data = new DataAgent(Player.ID)
            {
                PlayerGUID = guid,
                GameFinished = false,
                PlayerLocation = new GameArea.GameObjects.Location(2, 2),
                Goals = new GameArea.GameObjects.GoalField[] { new GameArea.GameObjects.GoalField(goalField) }
            };

            // action: Player moves down to (2,2)
            data.Process(Player.Controller);

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

            RegisterPlayer(Player, Player.GUID);

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
            data.Process(Player.Controller);

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

            RegisterPlayer(Player, Player.GUID);

            DataAgent data = new DataAgent(Player.ID)
            {
                PlayerGUID = guid,
                GameFinished = false,
                PlayerLocation = Player.Location,
                Goals = new GameArea.GameObjects.GoalField[] { }
            };

            // action: Player moves down to (1, 10)
            data.Process(Player.Controller);

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

            RegisterPlayer(Player, Player.GUID);

            DataAgent data = new DataAgent(Player.ID)
            {
                PlayerGUID = guid,
                GameFinished = false,
                PlayerLocation = Player.Location,
                Goals = new GameArea.GameObjects.GoalField[] { }
            };

            // action: Player moves down to (5, 7)
            data.Process(Player.Controller);

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

            RegisterPlayer(Player, Player.GUID);

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
            data.Process(Player.Controller);

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

            RegisterPlayer(Player, Player.GUID);

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
            data.Process(Player.Controller);

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

            RegisterPlayer(Player, Player.GUID);

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
            data.Process(Player.Controller);

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

            RegisterPlayer(Player, Player.GUID);

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
            data.Process(Player.Controller);

            Assert.AreEqual(new GameArea.GameObjects.Location(1, 3), Player.Location);
            Assert.IsNotNull(Player.GetBoard.GetGoalField(1, 2).Player);
            Assert.AreEqual(4ul, Player.GetBoard.GetGoalField(1, 2).Player.ID);
        }
    }
}
