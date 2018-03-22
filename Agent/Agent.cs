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

        public Agent(TeamColour team, string _guid = "TEST_GUID")
        {
            this.team = team;
            this.SetGuid(_guid);
            this.location = new Location(0,0);
        }

        public Agent(Agent original)
        {
            this.team = original.GetTeam;
            this.guid = original.GUID;
            this.id = original.id;
            this.location = new Location(original.location.x, original.location.y);
            if (original.piece != null)
                this.piece = new Piece(original.piece); // agent can't see original piece (sham or goal info must be hidden)
            //this.agentBoard = original.agentBoard; // it seems it doesn't need to be copied, because GameMaster does not make changes to boards of agents on his list

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

        public Messages.Agent ConvertToMessageAgent()
        {
            return new Messages.Agent()
            {
                id = this.ID,
                team = this.team,
                type = PlayerType.member
            };
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
                responseMessage.Pieces != null && responseMessage.Pieces.Length > 0 && responseMessage.Pieces[0] != null &&
                responseMessage.Pieces[0].id == this.GetPiece.id)
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

        public bool PlacePiece(IGameMaster gameMaster)
        {
            // should we check if received location is the same as the actual one?
            Data responseMessage = gameMaster.HandlePlacePieceRequest(this.GUID, this.GameId);

            if (responseMessage.playerId == this.ID && !responseMessage.gameFinished)
            {
                // placing the piece on an empty task field
                if (responseMessage.TaskFields != null && responseMessage.TaskFields.Length > 0 && responseMessage.TaskFields[0].pieceId == this.piece.id)
                {
                    ((GameArea.TaskField)agentBoard.GetField(location.x, location.y)).SetPiece(this.GetPiece); // place the piece on the field
                    this.SetPiece(null); // drop the piece
                    return true;
                }
                // placing the sham piece on GoalField of any type - no data about GoalField received and placing a piece failed
                else if (responseMessage.GoalFields != null && responseMessage.GoalFields.Length > 0 &&
                         responseMessage.GoalFields[0].type == GoalFieldType.unknown)
                {
                    return false;
                }
                // placing the normal piece on a GoalField of type 'goal'
                else if (responseMessage.GoalFields != null && responseMessage.GoalFields.Length > 0 &&
                         responseMessage.GoalFields[0].type == GoalFieldType.goal)
                {
                    this.SetPiece(null); // drop the piece and score a point
                    return true;
                }
                // placing any piece either on occupied TaskField or a noraml piece on a non-goal GoalField
                else
                {
                    return false;
                }
            }
            else return false;
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
