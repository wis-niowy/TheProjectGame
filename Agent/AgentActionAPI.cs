using GameArea;
using Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Player
{
    public partial class Agent
    {
       

        /// <summary>
        /// Method to send request to test the piece
        /// </summary>
        /// <param name="gameMaster">Addressee of the request</param>
        /// <returns>True - request was valid; False - request was not valid</returns>
        public bool TestPiece(IGameMaster gameMaster)
        {
            ConsoleWriter.Show("Agent: " + GUID + "tries to test piece with id: " + (GetPiece != null ? GetPiece.id.ToString() : "none") + " on location " + location);
            Data responseMessage = gameMaster.HandleTestPieceRequest(this.GUID, this.GameId);
            if (responseMessage.gameFinished)
                gameFinished = true;
            ConsoleWriter.Show("Received response for TestPiece for agent: " + guid);

            return UpdateLocalBoard(responseMessage, ActionType.TestPiece);
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
            if (responseMessage.gameFinished)
                gameFinished = true;
            ConsoleWriter.Show("Received response for PlacePiece for agent: " + guid);

            var receivedLocation = responseMessage.PlayerLocation;

            return UpdateLocalBoard(responseMessage, ActionType.PlacePiece);
        }

        public bool PickUpPiece(IGameMaster gameMaster)
        {
            Data responseMessage = gameMaster.HandlePickUpPieceRequest(this.GUID, this.GameId);
            if (responseMessage.gameFinished)
                gameFinished = true;
            ConsoleWriter.Show("Received response for PickUpPiece for agent: " + guid);

            return UpdateLocalBoard(responseMessage, ActionType.PickUpPiece);
        }

        // TODO: refactor this method
        public bool Move(IGameMaster gameMaster, MoveType direction)
        {
            ConsoleWriter.Show("Agent: " + guid + " send request for move from location: " + location + " in direction: " + direction);

            Data responseMessage = gameMaster.HandleMoveRequest(direction, this.GUID, this.GameId);
            if (responseMessage.gameFinished)
                gameFinished = true;
            ConsoleWriter.Show("Received response for Move for agent: " + guid);

            //return MoveUpdate(responseMessage, direction);// ---- dla tego sypia sie 3 testy - do sprawdzenia pozniej!

            var futureLocation = CalculateFutureLocation(this.location, direction);

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
            if (responseMessage.gameFinished)
                gameFinished = true;
            UpdateLocalBoard(responseMessage, ActionType.Discover);

            ConsoleWriter.Show("End of discovery for agent: " + guid);
        }
        
        // additional methods

        private bool UpdateLocalBoard(Data responseMessage, ActionType action, MoveType direction = MoveType.up)
        {
            bool result = false;

            var gameFinished = responseMessage.gameFinished;
            var playerId = responseMessage.playerId;

            if (playerId == this.ID && !gameFinished)
            {
                switch (action)
                {
                    case ActionType.TestPiece:
                        result = TestPieceUpdate(responseMessage);
                        break;
                    case ActionType.PlacePiece:
                        result = PlacePieceUpdate(responseMessage);
                        break;
                    case ActionType.PickUpPiece:
                        result = PickUpPieceUpdate(responseMessage);
                        break;
                    case ActionType.Move:
                        MoveUpdate(responseMessage, direction);
                        break;
                    case ActionType.Discover:
                        DiscoverUpdate(responseMessage);
                        break;
                }
            }

            ConsoleWriter.Show("Updated state for agent: " + guid);

            return result;
        }

        private bool TestPieceUpdate(Data responseMessage)
        {
            bool resultValue = false;

            var taskFieldsArray = responseMessage.TaskFields;
            var goalFieldsArray = responseMessage.GoalFields;
            var piecesArray = responseMessage.Pieces;

            if (piecesArray != null && piecesArray.Length > 0 && piecesArray[0] != null) // otzymano informacje o kawalku
                                                                                         // piecesArray[0] != null oznacza, ze akcja byla poprawna
            {
                var receivedPiece = piecesArray[0];
                if (receivedPiece.type != PieceType.unknown && this.GetPiece.id == receivedPiece.id)  // testowano kawalek -- wynik normal lub sham
                {
                    this.piece = receivedPiece; // aktualizacja lokalnego kawalka
                    this.piece.timestamp = DateTime.Now;
                    resultValue = true;
                }
            }

            return resultValue;
        }
        private bool PlacePieceUpdate(Data responseMessage)
        {
            bool resultValue = false;

            var taskFieldsArray = responseMessage.TaskFields;
            var goalFieldsArray = responseMessage.GoalFields;
            var piecesArray = responseMessage.Pieces;

            if (taskFieldsArray != null && taskFieldsArray.Length > 0 && taskFieldsArray[0] != null) // agent kladzie kawalek na wolne pole
                                                                                                     // jezeli taskFieldsArray[0] == null to probowano polozyc na zajetym TaskField
            {
                ConsoleWriter.Error("Placed piece when location: " + location + " by Agent with GUID: " + guid);
                var receivedField = taskFieldsArray[0];
                var field = this.agentBoard.GetTaskField(location.x, location.y);
                field.SetPiece(this.GetPiece); // odkladamy kawalek
                field.UpdateTimeStamp(DateTime.Now);
                this.SetPiece(null);
                resultValue = true;
            }
            else if (goalFieldsArray != null && goalFieldsArray.Length > 0 && goalFieldsArray[0] != null) // agent kladzie kawalek na GoalField                                                                                              
            {
                var receivedField = goalFieldsArray[0];
                if (receivedField.type == GoalFieldType.goal) // agent trafil gola - puszcza kawalek
                {
                    this.SetPiece(null);
                    resultValue = true;
                }
                else if (receivedField.type == GoalFieldType.nongoal) // agent chybil probujac kawalkiem 'normal'
                {
                    var field = agentBoard.GetGoalField(location.x, location.y);
                    field.GoalType = GoalFieldType.nongoal;
                    field.UpdateTimeStamp(DateTime.Now);
                    resultValue = false;
                }
                else // (receivedField.type == GoalFieldType.unknown) -- polozono sham na GoalField - zadnych info o polu, wiec wiemy ze agent ma typ sham
                {
                    this.GetPiece.type = PieceType.sham;
                    this.GetPiece.timestamp = DateTime.Now;
                    resultValue = false;
                }
            }

            return resultValue;
        }
        private bool PickUpPieceUpdate(Data responseMessage)
        {
            bool resultValue = false;

            var taskFieldsArray = responseMessage.TaskFields;
            var goalFieldsArray = responseMessage.GoalFields;
            var piecesArray = responseMessage.Pieces;

            if (piecesArray != null && piecesArray.Length > 0 && piecesArray[0] != null)
            {
                ConsoleWriter.Warning("Received piece from location: " + location + " to Agent with GUID: " + guid);
                var receivedPiece = piecesArray[0];
                this.SetPiece(receivedPiece);
                this.GetPiece.timestamp = DateTime.Now;
                agentBoard.GetTaskField(location.x, location.y).SetPiece(null);
                resultValue = true;
            }

            return resultValue;
        }
        private bool MoveUpdate(Data responseMessage, MoveType direction)
        {
            bool resultValue = false;

            var futureLocation = CalculateFutureLocation(this.location, direction);
            var currentLocation = responseMessage.PlayerLocation;
            var taskFieldsArray = responseMessage.TaskFields;
            var goalFieldsArray = responseMessage.GoalFields;
            var piecesArray = responseMessage.Pieces;

            if (taskFieldsArray != null && taskFieldsArray.Length > 0)
            {
                // an attempt to exceed board's boundaries or to enter an opponent's GoalArea
                if (responseMessage.TaskFields != null && responseMessage.TaskFields.Length == 0)
                {
                    this.location = responseMessage.PlayerLocation;
                    resultValue = false;
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

                        resultValue = false;
                    }
                    // an action was valid
                    else
                    {
                        this.location = responseMessage.PlayerLocation;
                        resultValue = true;
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

                        resultValue = false;
                    }
                    // an action was valid
                    else
                    {
                        this.location = responseMessage.PlayerLocation;
                        resultValue = true;
                    }
                }
            }
            return resultValue;
        }
        private void DiscoverUpdate(Data responseMessage)
        {
            var taskFieldsArray = responseMessage.TaskFields;
            var goalFieldsArray = responseMessage.GoalFields;
            var piecesArray = responseMessage.Pieces;

            if (taskFieldsArray != null && taskFieldsArray.Length > 0)
            {
                foreach (var respField in taskFieldsArray)
                {
                    if (respField != null)
                    {
                        var coordX = respField.x;
                        var coordY = respField.y;

                        GameArea.TaskField updatedField = agentBoard.GetField(respField.x, respField.y) as GameArea.TaskField;
                        updatedField.UpdateTimeStamp(respField.timestamp);
                        updatedField.Distance = respField.distanceToPiece;


                        if (respField.playerIdSpecified)
                            updatedField.Player = new Messages.Agent()
                            {
                                id = respField.playerId
#warning Wymaga napisania metody do otrzymywania listy wszystkich graczy
                                //team = myTeam.Union(otherTeam).First(p => p.id == respField.playerId).team,
                                //type = myTeam.Union(otherTeam).First(p => p.id == respField.playerId).type,
                            };

                        if (respField.pieceIdSpecified)
                            updatedField.SetPiece(new Piece(PieceType.unknown, respField.pieceId));
                    }
                }
            }
            if (goalFieldsArray != null && goalFieldsArray.Length > 0)
            {
                foreach (var respField in goalFieldsArray)
                {
                    if (respField != null)
                    {
                        var coordX = respField.x;
                        var coordY = respField.y;

                        GameArea.GoalField updatedField = agentBoard.GetField(respField.x, respField.y) as GameArea.GoalField;
                        updatedField.UpdateTimeStamp(respField.timestamp);

                        if (respField.playerIdSpecified)
                            updatedField.Player = new Messages.Agent()
                            {
                                id = respField.playerId
#warning Wymaga napisania metody do otrzymywania listy wszystkich graczy
                                //team = myTeam.Union(otherTeam).First(p => p.id == respField.playerId).team,
                                //type = myTeam.Union(otherTeam).First(p => p.id == respField.playerId).type,
                            };
                    }
                }
            }

        }

        // helpers ---------------------

        private Location CalculateFutureLocation(Location oldLocation, MoveType direction)
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
