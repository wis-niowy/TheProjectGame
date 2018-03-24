using Messages;
using System;
using GameArea;
using System.Collections.Generic;
using System.Linq;

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

            return UpdateLocalBoard(responseMessage, ActionType.TestPiece);

            //// if this message was sent to this
            //if (responseMessage.playerId == this.ID && !responseMessage.gameFinished &&
            //    responseMessage.Pieces != null && responseMessage.Pieces.Length > 0 && responseMessage.Pieces[0] != null &&
            //    responseMessage.Pieces[0].id == this.GetPiece.id)
            //{
            //    var receivedPieceData = responseMessage.Pieces[0];
            //    if (receivedPieceData != null && this.GetPiece != null && receivedPieceData.id == this.GetPiece.id)
            //    {
            //        this.piece.type = receivedPieceData.type;
            //        return true;
            //    }
            //}
            //return false;
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

            return UpdateLocalBoard(responseMessage, ActionType.PlacePiece);

            //if (responseMessage.playerId == this.ID && !responseMessage.gameFinished)
            //{
            //    // placing the piece on an empty task field
            //    if (responseMessage.TaskFields != null && responseMessage.TaskFields.Length > 0 && responseMessage.TaskFields[0].pieceId == this.piece.id)
            //    {
            //        ((GameArea.TaskField)agentBoard.GetField(location.x, location.y)).SetPiece(this.GetPiece); // place the piece on the field
            //        this.SetPiece(null); // drop the piece
            //        return true;
            //    }
            //    // placing the sham piece on GoalField of any type - no data about GoalField received and placing a piece failed
            //    else if (responseMessage.GoalFields != null && responseMessage.GoalFields.Length > 0 &&
            //             responseMessage.GoalFields[0].type == GoalFieldType.unknown)
            //    {
            //        return false;
            //    }
            //    // placing the normal piece on a GoalField of type 'goal'
            //    else if (responseMessage.GoalFields != null && responseMessage.GoalFields.Length > 0 &&
            //             responseMessage.GoalFields[0].type == GoalFieldType.goal)
            //    {
            //        this.SetPiece(null); // drop the piece and score a point
            //        return true;
            //    }
            //    // placing any piece either on occupied TaskField or a noraml piece on a non-goal GoalField
            //    else
            //    {
            //        return false;
            //    }
            //}
            //else return false;
        }

        public bool PickUpPiece(IGameMaster gameMaster)
        {
            Data responseMessage = gameMaster.HandlePickUpPieceRequest(this.GUID, this.GameId);

            return UpdateLocalBoard(responseMessage, ActionType.PickUpPiece);

            ////UpdateAgentState(responseMessage);
            //// jeżeli Agent.Piece != null ---> return true;
            //if (responseMessage.playerId == this.ID && !responseMessage.gameFinished)
            //{
            //    // player is on a TaskField that contains a piece
            //    if (responseMessage.Pieces != null && responseMessage.Pieces.Length > 0 &&
            //        responseMessage.Pieces[0] != null)
            //    {
            //        var receivedPieceData = responseMessage.Pieces[0];
            //        this.piece = receivedPieceData;
            //        return true;
            //    }
            //}
            //// player is either on empty TaskField or on GoalField
            //return false;
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

            UpdateLocalBoard(responseMessage, ActionType.Discover);

            //if (responseMessage.playerId == this.ID && !responseMessage.gameFinished)
            //{
            //    foreach (var respField in responseMessage.TaskFields)
            //    {
            //        GameArea.TaskField updatedField = agentBoard.GetField(respField.x, respField.y) as GameArea.TaskField;
            //        updatedField.UpdateTimeStamp(respField.timestamp);
            //        updatedField.Distance = respField.distanceToPiece;

            //        Type t = respField.GetType();
            //        System.Reflection.PropertyInfo p = t.GetProperty("pieceId");
            //        if (p != null)
            //            updatedField.SetPiece(new Piece(PieceType.unknown, respField.pieceId.Value));
            //        p = t.GetProperty("playerId");
            //        if (p != null)
            //        {
            //            updatedField.Player = new Messages.Agent();
            //            updatedField.Player.id = (ulong)respField.playerId;
            //        }
            //    }

            //    foreach (var respField in responseMessage.GoalFields)
            //    {
            //        GameArea.GoalField updatedField = agentBoard.GetField(respField.x, respField.y) as GameArea.GoalField;
            //        updatedField.UpdateTimeStamp(respField.timestamp);
            //        Type t = respField.GetType();
            //        System.Reflection.PropertyInfo p = t.GetProperty("playerId");
            //        if (p != null)
            //        {
            //            updatedField.Player = new Messages.Agent();
            //            updatedField.Player.id = (ulong)respField.playerId;
            //        }
            //    }
            //}
        }

        public void doStrategy()
        {

        }

        // additional methods

        private bool UpdateLocalBoard(Data responseMessage, ActionType action)
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
                        MoveUpdate(responseMessage);
                        break;
                    case ActionType.Discover:
                        DiscoverUpdate(responseMessage);
                        break;
                }
            }

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
                var receivedField = taskFieldsArray[0];
                this.agentBoard.GetTaskField(location.x, location.y).SetPiece(this.GetPiece); // odkladamy kawalek
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
                    agentBoard.GetGoalField(location.x, location.y).GoalType = GoalFieldType.nongoal;
                    resultValue = false;
                }
                else // (receivedField.type == GoalFieldType.unknown) -- polozono sham na GoalField - zadnych info o polu, wiec wiemy ze agent ma typ sham
                {
                    this.GetPiece.type = PieceType.sham;
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
                var receivedPiece = piecesArray[0];
                this.SetPiece(receivedPiece);
                agentBoard.GetTaskField(location.x, location.y).SetPiece(null);
                resultValue = true;
            }

            return resultValue;
        }
        private void MoveUpdate(Data responseMessage)
        {
            bool resultValue = false;

            var currentLocation = responseMessage.PlayerLocation;
            var taskFieldsArray = responseMessage.TaskFields;
            var goalFieldsArray = responseMessage.GoalFields;
            var piecesArray = responseMessage.Pieces;


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
                                id = respField.playerId.Value,
                                team = myTeam.Union(otherTeam).First(p => p.id == respField.playerId.Value).team,
                                type = myTeam.Union(otherTeam).First(p => p.id == respField.playerId.Value).type,
                            };

                        if (respField.pieceIdSpecified)
                            updatedField.SetPiece(new Piece(PieceType.unknown, respField.pieceId.Value));
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
                                id = respField.playerId.Value,
                                team = myTeam.Union(otherTeam).First(p => p.id == respField.playerId.Value).team,
                                type = myTeam.Union(otherTeam).First(p => p.id == respField.playerId.Value).type,
                            };
                    }
                }
            }
            
        }

        // // //

        //private void UpdateLocalBoard2(Data responseMessage)
        //{
        //    var gameFinished = responseMessage.gameFinished;
        //    var currentLocation = responseMessage.PlayerLocation;
        //    var playerId = responseMessage.playerId;

        //    var taskFieldsArray = responseMessage.TaskFields;
        //    var goalFieldsArray = responseMessage.GoalFields;
        //    var piecesArray = responseMessage.Pieces;

        //    if (playerId == this.ID && !gameFinished)
        //    {
        //        if (piecesArray != null && piecesArray.Length > 0 && piecesArray[0] == null) // otzymano informacje o kawalku
        //            // element != null oznacza, ze akcja byla poprawna
        //        {
        //            var receivedPiece = piecesArray[0];
        //            if (receivedPiece.type == PieceType.unknown) // podniesiono kawalek (testowanie zawsze da normal/sham)
        //            {
        //                this.SetPiece(receivedPiece); // podnosimy kawalek
        //                this.agentBoard.GetTaskField(currentLocation.x, currentLocation.y).SetPiece(null); // zabieramy kawalek z planszy
        //            }
        //            else // testowano kawalek -- wynik normal lub sham
        //            {
        //                if (this.GetPiece.id == receivedPiece.id) // agent posiada ten sam kawalek, o ktorym dostal info
        //                {
        //                    this.piece = receivedPiece; // aktualizacja lokalnego kawalka
        //                }
        //            }
        //        }

        //        if (taskFieldsArray != null && taskFieldsArray.Length > 0 && taskFieldsArray[0] != null) // otzymano informacje o TaskField
        //            // jezeli taskFieldsArray.Length == 0 to probowal wyjsc poza plansze - zadnych zmian na planszy
        //        {
        //            var receivedField = taskFieldsArray[0];
        //            if (responseMessage.PlayerLocation == null) // wiadomosc zwrotna z akcji PlacePiece
        //            {
        //                this.agentBoard.GetTaskField(currentLocation.x, currentLocation.y).SetPiece(this.GetPiece); // odkladamy kawalek
        //                this.SetPiece(null);
        //            }
        //            else // PlayerLocation ustawione, a wiec wiadomosc zwrotna z akcji Move
        //            {
        //                var futureX = receivedField.x;
        //                var futureY = receivedField.y;
        //                if (receivedField.playerId != this.ID) // ruch sie nie udal bo napotkano innego gracza
        //                {
        //                    var stranger = myTeam.Union(otherTeam).First(p => p.id == receivedField.playerId);
        //                    agentBoard.GetField(futureX, futureY).Player = stranger;
        //                }
        //                else // wraz z danymi TaskField otrzymalismy nasze id, wiec ruch sie udal
        //                {
        //                    this.location = new Location(futureX, futureY);
        //                }
        //            }
        //        }

        //    }
        //}



        //private void GrabPieceFromLocalBoard(uint x, uint y)
        //{
            
        //}

        //private void PutPieceOnLocalBoard(uint x, uint y)
        //{
        //    this.agentBoard.GetTaskField(x, y).SetPiece(this.GetPiece); // odkladamy kawalek
        //    this.SetPiece(null);
        //}

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
