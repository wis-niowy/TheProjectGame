using Messages;
using System;
using GameArea;

namespace Player
{
    public class Agent
    {
        private ulong guid;

        public ulong GUID
        {
            get
            {
                return guid;
            }
        }

        public void SetGuid (ulong newGuid)
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

        public void SetTeam(TeamColour newTeam)
        {
            team = newTeam;
        }

        public Agent(TeamColour team,ulong guid = 0 )
        {
            this.team = team;
            this.guid = guid;
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

        public void SetBoard(Board board)
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
    }
}
