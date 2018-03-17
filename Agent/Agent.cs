using System;
using TheProjectGame.GameArea;

namespace Player
{
    public class Agent
    {
        private int guid;

        public int GUID
        {
            get
            {
                return guid;
            }
        }

        public void SetGuid (int newGuid)
        {
            guid = newGuid;
        }

        private Team team;
        public Team GetTeam
        {
            get
            {
                return team;
            }
        }

        public void SetTeam(Team newTeam)
        {
            team = newTeam;
        }

        public Agent(Team team,int guid = 0 )
        {
            this.team = team;
            this.guid = guid;
            this.location = new Point(0,0);
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

        private Point location;
        public Point GetLocation
        {
            get
            {
                return location;
            }
        }

        public void SetLocation(Point point)
        {
            location = point;
        }

        public void SetLocation(int x, int y)
        {
            location = new Point(x,y);
        }
    }
}
