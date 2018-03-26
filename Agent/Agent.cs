using Messages;
using System;
using GameArea;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Player
{
    enum ActionType
    {
        none,
        TestPiece,
        PlacePiece,
        PickUpPiece,
        Move,
        Discover
    }

    public partial class Agent
    {
        private bool gameFinished;
        private IGameMaster gameMaster;
        private ulong id;
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

        public List<Messages.Agent> myTeam;
        public List<Messages.Agent> otherTeam;

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

        public Agent(TeamColour team, string _guid = "TEST_GUID", IGameMaster gm = null)
        {
            this.gameMaster = gm;
            this.team = team;
            this.SetGuid(_guid);
            this.location = new Location(0, 0);
        }

        public Agent(Agent original)
        {
            this.team = original.GetTeam;
            this.guid = original.GUID;
            this.id = original.id;
            this.location = new Location(original.location.x, original.location.y);
            if (original.piece != null)
                this.piece = new Piece(original.piece); // agent can't see original piece (sham or goal info must be hidden)
        }

        private Board agentBoard;

        public Board GetBoard
        {
            get
            {
                return agentBoard;
            }
        }

        public void SetBoard(Board board) // setter?
        {
            agentBoard = board;
        }

        private Piece piece;
        public Piece GetPiece
        {
            get
            {
                return piece;
            }
        }

        public void SetPiece(Piece piece)
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

        private Location location;
        public Location GetLocation
        {
            get
            {
                return location;
            }
        }

        public void SetLocation(Location point)
        {
            location = point;
        }

        public void SetLocation(uint x, uint y)
        {
            location = new Location(x, y);
        }

        public Messages.Agent ConvertToMessageAgent()
        {
            return new Messages.Agent()
            {
                id = this.ID,
                team = this.team,
                type = PlayerType.member
            };
        }


    }
}
