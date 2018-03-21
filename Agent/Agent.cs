using Messages;
using System;
using GameArea;

namespace Player
{
    public class Agent
    {
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
        

        public void SetGuid (string newGuid) // setter?
        {
            guid = newGuid;
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

        public Agent(TeamColour team, ulong id = 0 )
        {
            this.team = team;
            this.id = id;
            this.location = new Location(0,0);
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
            location = new Location(x,y);
        }

        // API
        public void TestPiece(IGameMaster gameMaster)
        {

        }

        public void PlacePiece(IGameMaster gameMaster)
        {

        }

        public void PickUpPiece(IGameMaster gameMaster)
        {

        }

        public void Move(IGameMaster gameMaster, MoveType direction)
        {
            
        }

        public void Discover(IGameMaster gameMaster)
        {

        }

        public void doStrategy()
        {

        }
    }
}
