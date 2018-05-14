using Microsoft.VisualStudio.TestTools.UnitTesting;
using GameArea;
using Messages;
using Configuration;
using GameArea.AppConfiguration;

namespace Player.Tests
{
    [TestClass]
    public partial class PlayerTests
    {
        GameMasterSettingsConfiguration settings;
        GameArea.GameMaster gameMaster;
        string guid;

        public void InitGameMaster()
        {
            settings = new GameMasterSettingsConfiguration(GameMasterSettings.GetDefaultGameMasterSettings());
            gameMaster = new GameArea.GameMaster(settings);
        }
        public Player GetPlayer(string guid, ulong id, TeamColour tc, ActionType action, IPlayerController controller = null)
        {
            var player = new Player(tc, _guid: guid);
            player.ID = id;
            player.LastActionTaken = action;
            player.Controller = controller;

            return player;
        }
        public void EquipPlayerWithPiece(Piece piece, Player player)
        {
            player.SetPiece(new GameArea.GameObjects.Piece(piece));

            RegisterPlayer(player, player.GUID);

            var pieceUnknown = new Piece(PieceType.unknown, piece.id)
            {
                timestamp = piece.timestamp,

            };
            player.SetPiece(new GameArea.GameObjects.Piece(pieceUnknown));
        }
        public void RegisterPlayer(Player Player, string guid = null)
        {
            InitGameMaster();
            Player.GUID = (guid != null ? guid : "Player" + Player.ID);
            Player.SetBoard(new GameArea.GameObjects.GameBoard(settings.GameDefinition.BoardWidth, settings.GameDefinition.TaskAreaLength, settings.GameDefinition.GoalAreaLength));
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

        [TestMethod]
        public void GoToNearestPieceInGoalAreaForBluePlayer()
        {
            int x = 0, y = 0;
            InitGameMaster();
            var Player1 = GetPlayer("testGUID-0027", 10, TeamColour.blue, ActionType.Move, new PlayerControllerMock());
            Player1.Controller.Player = Player1;
            Player1.SetLocation(x, y);

            RegisterPlayer(Player1, Player1.GUID);

            Player1.GoToNearestPiece();

            Location expectedLocationPlayer = new Location((uint)x, (uint)(y + 1));

            Assert.AreEqual(expectedLocationPlayer.x, (uint)Player1.Location.X);
            Assert.AreEqual(expectedLocationPlayer.y, (uint)Player1.Location.Y);
        }

        [TestMethod]
        public void GoToNearestPieceInGoalAreaForRedPlayer()
        {
            int x = 0, y = 12;
            InitGameMaster();
            var Player1 = GetPlayer("testGUID-0027", 10, TeamColour.blue, ActionType.Move, new PlayerControllerMock());
            Player1.Controller.Player = Player1;
            Player1.SetLocation(x, y);

            RegisterPlayer(Player1, Player1.GUID);

            Player1.GoToNearestPiece();

            Location expectedLocationPlayer = new Location((uint)x, (uint)(y + 1));

            Assert.AreEqual(expectedLocationPlayer.x, (uint)Player1.Location.X);
            Assert.AreEqual(expectedLocationPlayer.y, (uint)Player1.Location.Y);
        }

        //[TestMethod]
        //public void GoToNearestPieceInTaskAreaForBluePlayer()
        //{
        //    //sometimes not passed because of methods: PlaceInitialPieces, InitPieceAdder
        //    //(it's not known where will be another new piece)
        //    //to ensure that it will be passed we have to change constructor for game master
        //    //(give choice for enabling pieceAdder
        //    //
        //    GameArea.GameObjects.Location pieceLocation = new GameArea.GameObjects.Location(1, 5);
        //    GameArea.GameObjects.Location beforeMove = new GameArea.GameObjects.Location(3, 6);
        //    ulong pieceId = 51;

        //    InitGameMaster();
        //    var Player1 = GetPlayer("testGUID-0027", 10, TeamColour.blue, ActionType.Move, new PlayerControllerMock());
        //    Player1.Controller.Player = Player1;
        //    Player1.SetLocation(beforeMove);

        //    gameMaster.RegisterPlayer(Player1, Player1.GUID, findFreeLocationAndPlacePlayer: false);
        //    Player1.GetBoard.GetTaskField(pieceLocation).Piece = new GameArea.GameObjects.Piece(pieceId, DateTime.Now);

        //    Player1.GoToNearestPiece();

        //    GameArea.GameObjects.Location afterMove = Player1.Location;
        //    Assert.IsTrue(beforeMove.X - 1 == afterMove.X || beforeMove.Y - 1 == afterMove.Y);
        //}

        [TestMethod]
        public void GoToGoalAreaForRedTeamTest()
        {
            GameArea.GameObjects.Location PlayerLocation = new GameArea.GameObjects.Location(1, 5);

            InitGameMaster();
            var Player1 = GetPlayer("testGUID-0027", 10, TeamColour.red, ActionType.Move, new PlayerControllerMock());
            Player1.Controller.Player = Player1;
            Player1.SetLocation(PlayerLocation);

            RegisterPlayer(Player1, Player1.GUID);

            //powinno byc Player1.GoToGoalArea()
            Player1.GoToGoalArea(Player1.Team);
            GameArea.GameObjects.Location expectedLocationPlayer = new GameArea.GameObjects.Location(PlayerLocation.X, PlayerLocation.Y + 1);

            Assert.AreEqual(expectedLocationPlayer, Player1.Location);
        }

        ////slabo testowalny jest napisany default board z goalfieldami( za du¿o uknown field)
        [TestMethod]
        public void GetClosestUnknownGoalDirectionForRedTeamTestIfIsOnGoal()
        {
            GameArea.GameObjects.Location PlayerLocation = new GameArea.GameObjects.Location(12, 12);

            InitGameMaster();
            var Player1 = GetPlayer("testGUID-0027", 10, TeamColour.red, ActionType.Move, new PlayerControllerMock());
            Player1.Controller.Player = Player1;
            Player1.SetLocation(PlayerLocation);

            RegisterPlayer(Player1, Player1.GUID);

            MoveType PlayerDirection = Player1.GetClosestUnknownGoalDirection();
            //MoveType expectedPlayerDirection = MoveType.down;

            Assert.AreEqual(PlayerDirection, MoveType.left);
        }

        ////slabo testowalny jest napisany default board z goalfieldami( za du¿o uknown field)
        [TestMethod]
        public void GetClosestUnknownGoalForBlueTeamTestIfIsOnGoal()
        {
            GameArea.GameObjects.Location PlayerLocation = new GameArea.GameObjects.Location(1, 1);

            InitGameMaster();
            var Player1 = GetPlayer("testGUID-0027", 10, TeamColour.blue, ActionType.Move, new PlayerControllerMock());
            Player1.Controller.Player = Player1;
            Player1.SetLocation(PlayerLocation);

            RegisterPlayer(Player1, Player1.GUID);

            MoveType PlayerDirection = Player1.GetClosestUnknownGoalDirection();

            Assert.AreEqual(PlayerDirection, MoveType.left);
        }
    }
}
