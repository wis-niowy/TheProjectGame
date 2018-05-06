using Messages;
using System;
using GameArea;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using GameArea.AppMessages;
using GameArea.AppConfiguration;

namespace Player
{

    public partial class Player:IPlayer
    {
        public PlayerRole Role { get; set; }
        public PlayerSettingsConfiguration Settings { get; set; }
        public PlayerController Controller { get; set; }
        private List<GameArea.GameObjects.GameInfo> GamesList { get; set; }
        private bool gameFinished;
        private IGameMaster gameMaster;
        // pole uzywane przy czytaniu xml Data od servera - obiekt wie, jakiej akcji przed chwila zadal
        public ActionType? LastActionTaken { get; set; } //ustawiana przy wykonywaniu dowolnej akcji związanej z grą
        public MoveType? LastMoveTaken{ get; set; } //ustawiany przy każdym wykonaniu ruchu
        public ActionType ActionToComplete { get; set; }
        public AgentState State { get; set; }
        public ulong ID { get; set; }
        public string GUID { get; set; }

        public List<GameArea.GameObjects.Player> myTeam;
        public List<GameArea.GameObjects.Player> otherTeam;

        public ulong GameId { get; set; }
        public TeamColour Team { get; set; }

        public Player(TeamColour team, PlayerSettingsConfiguration settings = null, PlayerController gameController = null, string _guid = "TEST_GUID", IGameMaster gm = null)
        {
            Settings = settings;
            gameMaster = gm;
            Team = team;
            GUID = _guid;
            Location = new GameArea.GameObjects.Location(0, 0);
            Controller = gameController;
            State = AgentState.SearchingForGame;
            LastMoveTaken = MoveType.up;
        }

        public Player(Player original)
        {
           Team = original.Team;
           GUID = original.GUID;
           ID = original.ID;
           Location = new GameArea.GameObjects.Location(original.Location.X, original.Location.Y);
            if (original.piece != null)
                this.piece = new GameArea.GameObjects.Piece(original.piece.ID, original.piece.TimeStamp, original.piece.Type, original.piece.PlayerId); // player can't see original piece (sham or goal info must be hidden)
        }

        private GameArea.GameObjects.GameBoard PlayerBoard;

        public GameArea.GameObjects.GameBoard GetBoard
        {
            get
            {
                return PlayerBoard;
            }
        }

        public void SetBoard(GameArea.GameObjects.GameBoard board) // setter?
        {
            PlayerBoard = board;
        }

        private GameArea.GameObjects.Piece piece;
        public GameArea.GameObjects.Piece GetPiece
        {
            get
            {
                return piece;
            }
        }

        public void SetPiece(GameArea.GameObjects.Piece piece)
        {
            this.piece = piece;
        }

        public bool HasPiece
        {
            get
            {
                return piece != null;
            }
        }

        public bool HasValidPiece
        {
            get
            {
                return piece != null && piece.Type == PieceType.normal;
            }
        }

        public bool HasUnknownPiece
        {
            get
            {
                return piece != null && piece.Type == PieceType.unknown;
            }
        }

        public bool HasShamPiece
        {
            get
            {
                return piece != null && piece.Type == PieceType.sham;
            }
        }

        public GameArea.GameObjects.Location Location { get;set; }

        public void SetLocation(GameArea.GameObjects.Location point)
        {
            Location = new GameArea.GameObjects.Location(point.X, point.Y);
        }

        public void SetLocation(int x, int y)
        {
            Location = new GameArea.GameObjects.Location(x, y);
        }

        public GameArea.GameObjects.Player ConvertToMessagePlayer()
        {
            return new GameArea.GameObjects.Player(ID, Team, PlayerRole.member);
        }

        public void RegisteredGames(RegisteredGamesMessage messageObject)
        {
            GamesList = messageObject.Games?.ToList();
            State = AgentState.Joining;
            ActionToComplete = ActionType.none;
        }

        public void ConfirmJoiningGame(ConfirmJoiningGameMessage info)
        {
            
            GameId = info.GameId;
            ID = info.PlayerId; //u nas serwerowe ID i playerId na planszy to jedno i to samo
            GUID = info.GUID;
            Team = info.PlayerDefinition.Team;
            State = AgentState.AwaitingForStart;
            ActionToComplete = ActionType.none;
        }

        public void GameStarted(GameArea.AppMessages.GameMessage messageObject)
        {
            myTeam = null;
            otherTeam = null;
            SetBoard(null);
            Location = null;
            myTeam = messageObject.Players.ToList().Where(p => p.Team == Team).ToList();
            otherTeam = messageObject.Players.ToList().Where(p => p.Team != Team).ToList();
            SetBoard(messageObject.Board);
            Location = messageObject.PlayerLocation;
            State = AgentState.Playing;
            ActionToComplete = ActionType.none;
        }

        public void GameMasterDisconnected(GameArea.AppMessages.GameMasterDisconnectedMessage messageObject)
        {
            State = AgentState.SearchingForGame;
            ConsoleWriter.Show("Player id: " + ID + " has state: " + State);
            ActionToComplete = ActionType.none;
        }

        public override string ToString()
        {
            return "Player id: " + ID + ", team: " + Team + 
                " role: " + Role +
                " in location (" + Location.X + ";" + Location.Y + ")";
        }

        public void ErrorMessage(GameArea.AppMessages.ErrorMessage error)
        {
            ConsoleWriter.Warning("Received an error from server:\n Type:" + error.Type + "\nCause: " + error.CauseParameterName + "\nMessage: " + error.Message);
        }
    }
}
