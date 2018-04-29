using Messages;
using System;
using GameArea;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using GameArea.AppMessages;

namespace Player
{

    public partial class Player:IPlayer
    {
        public PlayerRole Role { get; set; }
        public PlayerController Controller { get; set; }
        private List<GameArea.GameObjects.GameInfo> GamesList { get; set; }
        private bool gameFinished;
        private IGameMaster gameMaster;
        // pole uzywane przy czytaniu xml Data od servera - obiekt wie, jakiej akcji przed chwila zadal
        public ActionType? LastActionTaken { get; set; } //ustawiana przy wykonywaniu dowolnej akcji związanej z grą
        public MoveType? LastMoveTaken{ get; set; } //ustawiany przy każdym wykonaniu ruchu
        private ulong id;
        public AgentState State { get; set; }
        public ulong ID
        {
            get
            {
                return id;
            }
            set
            {
                this.id = value;
            }
        }

        private string guid;
        public string GUID
        {
            get
            {
                return guid;
            }
        }

        public void SetGuid(string newGuid) // setter?
        {
            guid = newGuid;
        }

        private ulong gameId;

        public List<GameArea.GameObjects.Player> myTeam;
        public List<GameArea.GameObjects.Player> otherTeam;

        public ulong GameId
        {
            get
            {
                return gameId;
            }
            set
            {
                gameId = value;
            }
        }

        private TeamColour team;
        public TeamColour GetTeam
        {
            get
            {
                return team;
            }
        }

        public void SetTeam(TeamColour newTeam) // setter?
        {
            team = newTeam;
        }

        public Player(TeamColour team, PlayerController gameController = null, string _guid = "TEST_GUID", IGameMaster gm = null)
        {
            this.gameMaster = gm;
            this.team = team;
            this.SetGuid(_guid);
            this.location = new GameArea.GameObjects.Location(0, 0);
            State = AgentState.SearchingForGame;
            Controller = gameController;
        }

        public Player(Player original)
        {
            this.team = original.GetTeam;
            this.guid = original.GUID;
            this.id = original.id;
            this.location = new GameArea.GameObjects.Location(original.location.X, original.location.Y);
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

        private GameArea.GameObjects.Location location;
        public GameArea.GameObjects.Location GetLocation
        {
            get
            {
                return location;
            }
        }

        public void SetLocation(GameArea.GameObjects.Location point)
        {
            location = new GameArea.GameObjects.Location(point.X, point.Y);
        }

        public void SetLocation(int x, int y)
        {
            location = new GameArea.GameObjects.Location(x, y);
        }

        public GameArea.GameObjects.Player ConvertToMessagePlayer()
        {
            return new GameArea.GameObjects.Player(ID, team, PlayerRole.member);
        }

        public void RegisteredGames(RegisteredGamesMessage messageObject)
        {
            GamesList = messageObject.Games.ToList();
            State = AgentState.Joining;
        }

        public void ConfirmJoiningGame(ConfirmJoiningGameMessage info)
        {
            State = AgentState.AwaitingForStart;
            gameId = info.GameId;
            ID = info.PlayerId; //u nas serwerowe ID i playerId na planszy to jedno i to samo
            guid = info.GUID;
            team = info.PlayerDefinition.Team;
        }

        public void GameStarted(GameArea.AppMessages.GameMessage messageObject)
        {
            State = AgentState.Playing;
            throw new NotImplementedException("Otrzymano wiadomość Game - zaktualizować struktury lokalne Agenta o dane gry");
        }

        public void GameMasterDisconnected(GameArea.AppMessages.GameMasterDisconnectedMessage messageObject)
        {
            State = AgentState.SearchingForGame;
            throw new NotImplementedException("Wypsiać stan agenta jaki jest aktualny, wyczyścić struktury lokalne Agenta związane z grą - ona już nie istnieje, została zakończona");
        }
    }
}
