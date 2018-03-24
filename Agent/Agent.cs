using Messages;
using System;
using GameArea;
using System.Collections.Generic;

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

        public Agent(TeamColour team, string _guid = "TEST_GUID")
        {
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

        /// <summary>
        /// Method to send a request to place the piece
        /// </summary>
        /// <param name="gameMaster"></param>
        /// <returns></returns>
        public bool PlacePiece(IGameMaster gameMaster)
        {
            // should we check if received location is the same as the actual one?
            Data responseMessage = gameMaster.HandlePlacePieceRequest(this.GUID, this.GameId);
            var receivedLocation = responseMessage.PlayerLocation;

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

        public bool PickUpPiece(IGameMaster gameMaster)
        {
            Data responseMessage = gameMaster.HandlePickUpPieceRequest(this.GUID, this.GameId);
            //UpdateAgentState(responseMessage);
            // jeżeli Agent.Piece != null ---> return true;
            if (responseMessage.playerId == this.ID && !responseMessage.gameFinished)
            {
                // player is on a TaskField that contains a piece
                if (responseMessage.Pieces != null && responseMessage.Pieces.Length > 0 &&
                    responseMessage.Pieces[0] != null)
                {
                    var receivedPieceData = responseMessage.Pieces[0];
                    this.piece = receivedPieceData;
                    return true;
                }
            }
            // player is either on empty TaskField or on GoalField
            return false;
        }

        public bool Move(IGameMaster gameMaster, MoveType direction)
        {
            Data responseMessage = gameMaster.HandleMoveRequest(direction, this.GUID, this.GameId);
            var futureLocation = CalcualteFutureLoaction(this.location, direction);

            if (responseMessage.playerId == this.ID && !responseMessage.gameFinished)
            {
                // an attempt to exceed board's boundaries or to enter an opponent's GoalArea
                if (responseMessage.TaskFields != null && responseMessage.TaskFields.Length == 0)
                {
                    this.location = responseMessage.PlayerLocation;
                    return false;
                }
                // future position is a TaskField
                else if (responseMessage.TaskFields != null && responseMessage.TaskFields.Length > 0)
                {
                    // an agent attempted to enter an occupied TaskField
                    if (this.location.Equals(responseMessage.PlayerLocation))
                    {
                        // add encountered stranger agent to this agent's view
                        var stranger = new Messages.Agent()
                        {
                            id = (ulong)responseMessage.TaskFields[0].playerId,
                        };
                        agentBoard.GetField(futureLocation.x, futureLocation.y).Player = stranger;

                        return false;
                    }
                    // an action was valid
                    else
                    {
                        this.location = responseMessage.PlayerLocation;
                        return true;
                    }
                }
                // future position is a GoalField
                else if (responseMessage.GoalFields != null && responseMessage.GoalFields.Length > 0)
                {
                    // an agent attempted to enter an occupied GoalField
                    if (this.location.Equals(responseMessage.PlayerLocation))
                    {
                        // add encountered stranger agent to this agent's view
                        var stranger = new Messages.Agent()
                        {
                            id = (ulong)responseMessage.GoalFields[0].playerId,
                        };
                        agentBoard.GetField(futureLocation.x, futureLocation.y).Player = stranger;

                        return false;
                    }
                    // an action was valid
                    else
                    {
                        this.location = responseMessage.PlayerLocation;
                        return true;
                    }
                }
            }
            return false;
        }


        public void Discover(IGameMaster gameMaster)
        {
            Data responseMessage = gameMaster.HandleDiscoverRequest(this.GUID, this.GameId);
            if (responseMessage.playerId == this.ID && !responseMessage.gameFinished)
            {
                foreach (var respField in responseMessage.TaskFields)
                {
                    GameArea.TaskField updatedField = agentBoard.GetField(respField.x, respField.y) as GameArea.TaskField;
                    updatedField.UpdateTimeStamp(respField.timestamp);
                    updatedField.Distance = respField.distanceToPiece;

                    Type t = respField.GetType();
                    System.Reflection.PropertyInfo p = t.GetProperty("pieceId");
                    if (p != null)
                        updatedField.SetPiece(new Piece(PieceType.unknown, respField.pieceId));
                    p = t.GetProperty("playerId");
                    if (p != null)
                    {
                        updatedField.Player = new Messages.Agent();
                        updatedField.Player.id = (ulong)respField.playerId;
                    }
                }

                foreach (var respField in responseMessage.GoalFields)
                {
                    GameArea.GoalField updatedField = agentBoard.GetField(respField.x, respField.y) as GameArea.GoalField;
                    updatedField.UpdateTimeStamp(respField.timestamp);
                    Type t = respField.GetType();
                    System.Reflection.PropertyInfo p = t.GetProperty("playerId");
                    if (p != null)
                    {
                        updatedField.Player = new Messages.Agent();
                        updatedField.Player.id = (ulong)respField.playerId;
                    }
                }
            }
        }

        public void doStrategy()
        {

        }

        // additional methods
        private Location CalcualteFutureLoaction(Location oldLocation, MoveType direction)
        {
            Location newLocation = null;
            switch (direction)
            {
                case MoveType.up:
                    newLocation = new Location(oldLocation.x, oldLocation.y + 1);
                    break;
                case MoveType.down:
                    newLocation = new Location(oldLocation.x, oldLocation.y - 1);
                    break;
                case MoveType.left:
                    newLocation = new Location(oldLocation.x - 1, oldLocation.y);
                    break;
                case MoveType.right:
                    newLocation = new Location(oldLocation.x + 1, oldLocation.y);
                    break;
            }
            return newLocation;
        }
    }
}
