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

        private ulong gameId;
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
        /// <summary>
        /// Method to send request to test the piece
        /// </summary>
        /// <param name="gameMaster">Addressee of the request</param>
        /// <returns>True - request was valid; False - request was not valid</returns>
        public bool TestPiece(IGameMaster gameMaster)
        {
            Data responseMessage = gameMaster.HandleTestPieceRequest(this.GUID, this.GameId);
            // if this message was sent to this
            if (responseMessage.playerId == this.ID && !responseMessage.gameFinished &&
                responseMessage.Pieces != null && responseMessage.Pieces.Length > 0 && responseMessage.Pieces[0].id == this.GetPiece.id)
            {
                var receivedPieceData = responseMessage.Pieces[0];
                if (receivedPieceData != null && this.GetPiece != null && receivedPieceData.id == this.GetPiece.id)
                {
                    this.piece.type = receivedPieceData.type;
                    return true;
                }
            }
            return false;
        }

        public void PlacePiece(IGameMaster gameMaster)
        {
            // should we check if received location is the same as the actual one?
            Data responseMessage = gameMaster.HandlePlacePieceRequest(this.GUID, this.GameId);

            if (responseMessage.playerId == this.ID && !responseMessage.gameFinished)
            {
                // placing the piece on a task field
                if (responseMessage.TaskFields != null && responseMessage.TaskFields.Length > 0 && responseMessage.TaskFields[0].pieceId == this.piece.id)
                {
                    ((GameArea.TaskField)agentBoard.GetField(location.x, location.y)).SetPiece(this.GetPiece); // place the piece on the field
                    this.SetPiece(null); // drop the piece
                }
                // placing the piece on a goal field
                else if (responseMessage.GoalFields != null && responseMessage.GoalFields.Length > 0)
                {
                    switch (responseMessage.GoalFields[0].type)
                    {
                        case GoalFieldType.goal:
                            this.SetPiece(null); // drop the piece
                            break;
                        case GoalFieldType.nongoal:

                            break;
                    }
                }
                else
                {
                    // do nothing ?
                }
            }
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
