using GameArea;
using GameArea.AppMessages;
using GameArea.Parsers;
using Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Player
{
    public partial class Player
    {

        /// <summary>
        /// Method to send request to test the piece
        /// </summary>
        /// <param name="gameMaster">Addressee of the request</param>
        /// <returns>True - request was valid; False - request was not valid</returns>
        public bool TestPiece()
        {
            if (GetPiece != null)
                ConsoleWriter.Show(GUID + " tries to test piece: " + GetPiece.ID + " on location: " + GetLocation);
            TestPieceMessage msg = new TestPieceMessage(GUID, gameId);
            LastActionTaken = ActionToComplete = ActionType.TestPiece;
            try
            {
                Controller.BeginSend(msg.Serialize()); //każda akcja od razu się wysyła, ustawia również LastActionTaken i dla move LastMoveTaken !!!!!
            }
            catch (Exception e)
            {
                ConsoleWriter.Error("Error occured when writing message to socket.\n Error text: \n" + e.ToString());
                State = AgentState.Dead;
                return false;
            }
            return true;
        }

        /// <summary>
        /// Method to send a request to place the piece
        /// </summary>
        /// <param name="gameMaster"></param>
        /// <returns></returns>
        public bool PlacePiece()
        {
            ConsoleWriter.Show(guid + " places piece: " + piece.ID + " of type: " + piece.Type + " on location: " + GetLocation);
            // should we check if received location is the same as the actual one?
            PlacePieceMessage msg = new PlacePieceMessage(GUID, GameId);
            LastActionTaken = ActionToComplete = ActionType.PlacePiece;
            Controller.BeginSend(msg.Serialize()); //każda akcja od razu się wysyła, ustawia również LastActionTaken i dla move LastMoveTaken !!!!!
            WaitForActionComplete();
            return !HasPiece;
        }

        public bool PickUpPiece()
        {
            ConsoleWriter.Show(guid + " picks up piece on location: " + GetLocation);
            PickUpPieceMessage msg = new PickUpPieceMessage(GUID, GameId);
            LastActionTaken = ActionToComplete = ActionType.PickUpPiece;
            Controller.BeginSend(msg.Serialize()); //każda akcja od razu się wysyła, ustawia również LastActionTaken i dla move LastMoveTaken !!!!!
            WaitForActionComplete();
            return HasPiece;
        }

        public bool Move(MoveType direction)
        {
            ConsoleWriter.Show(guid + " wants to move from: " + GetLocation + " in direction: " + direction);
            MoveMessage msg = new MoveMessage(GUID, GameId, direction);
            LastActionTaken = ActionToComplete = ActionType.Move;
            LastMoveTaken = direction;
            var futureLocation = CalculateFutureLocation(GetLocation, direction);
            Controller.BeginSend(msg.Serialize()); //każda akcja od razu się wysyła, ustawia również LastActionTaken i dla move LastMoveTaken !!!!!
            WaitForActionComplete();

            return GetLocation.Equals(futureLocation);
        }


        public bool Discover()
        {
            ConsoleWriter.Show(guid + " discovers on location: " + GetLocation);
            DiscoverMessage msg = new DiscoverMessage(GUID, GameId);
            LastActionTaken = ActionToComplete = ActionType.Discover;

            Controller.BeginSend(msg.Serialize()); //każda akcja od razu się wysyła, ustawia również LastActionTaken i dla move LastMoveTaken !!!!!
            WaitForActionComplete();
            return true;
        }

        public bool Destroy()
        {
            ConsoleWriter.Show(guid + " tries to destroy piece: " + piece.ID + " which is: " + piece.Type + "on location: " + GetLocation);
            DestroyPieceMessage msg = new DestroyPieceMessage(GUID, GameId);
            LastActionTaken = ActionToComplete = ActionType.Destroy;

            Controller.BeginSend(msg.Serialize()); //każda akcja od razu się wysyła, ustawia również LastActionTaken i dla move LastMoveTaken !!!!!
            WaitForActionComplete();
            return !HasPiece;
        }
        
        // additional methods

        public bool UpdateLocalBoard(DataMessage responseMessage, ActionType action, MoveType direction = MoveType.up)
        {
            bool result = false;

            var gameFinished = responseMessage.GameFinished;
            var playerId = responseMessage.PlayerId;

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
                        result = MoveUpdate(responseMessage, direction);
                        break;
                    case ActionType.Discover:
                        DiscoverUpdate(responseMessage);
                        break;
                    case ActionType.Destroy:
                        result = DestroyUpdate(responseMessage);
                        break;
                }
            }

            ConsoleWriter.Show("Updated state by: " + guid + " from action: " + action);

            return result;
        }

        private bool DestroyUpdate(DataMessage responseMessage)
        {
            var resultValue = false;

            var piecesArray = responseMessage.Pieces;
            if (piecesArray != null && piecesArray.Length > 0 && piecesArray[0] == null && HasPiece)
                resultValue =  true; //poprawna akcja, miał piece, usunął
            piece = null;
            return resultValue; //akcja bez sensu - nie miał piece
        }

        private bool TestPieceUpdate(DataMessage responseMessage)
        {
            bool resultValue = false;

            var taskFieldsArray = responseMessage.Tasks;
            var goalFieldsArray = responseMessage.Goals;
            var piecesArray = responseMessage.Pieces;

            if (piecesArray != null && piecesArray.Length > 0 && piecesArray[0] != null) // otzymano informacje o kawalku
                                                                                         // piecesArray[0] != null oznacza, ze akcja byla poprawna
            {
                var receivedPiece = piecesArray[0];
                if (receivedPiece.Type != PieceType.unknown && this.GetPiece.ID == receivedPiece.ID)  // testowano kawalek -- wynik normal lub sham
                {
                    this.piece = receivedPiece; // aktualizacja lokalnego kawalka
                    this.piece.TimeStamp = DateTime.Now;
                    resultValue = true;
                }
            }

            return resultValue;
        }
        private bool PlacePieceUpdate(DataMessage responseMessage)
        {
            bool resultValue = false;

            var taskFieldsArray = responseMessage.Tasks;
            var goalFieldsArray = responseMessage.Goals;
            var piecesArray = responseMessage.Pieces;

            if (taskFieldsArray != null && taskFieldsArray.Length > 0 && taskFieldsArray[0] != null) // Player kladzie kawalek na wolne pole
                                                                                                     // jezeli taskFieldsArray[0] == null to probowano polozyc na zajetym TaskField
            {
                var receivedField = taskFieldsArray[0];
                var field = this.PlayerBoard.GetTaskField(location.X, location.Y);
                field.Piece = this.GetPiece; // odkladamy kawalek
                field.TimeStamp = DateTime.Now;
                this.SetPiece(null);
                resultValue = true;
            }
            else if (goalFieldsArray != null && goalFieldsArray.Length > 0 && goalFieldsArray[0] != null) // Player kladzie kawalek na GoalField                                                                                              
            {
                var receivedField = goalFieldsArray[0];
                if (receivedField.Type == GoalFieldType.goal) // Player trafil gola - puszcza kawalek
                {
                    this.SetPiece(null);
                    resultValue = true;
                }
                else if (receivedField.Type == GoalFieldType.nongoal) // Player chybil probujac kawalkiem 'normal'
                {
                    var field = PlayerBoard.GetGoalField(location.X, location.Y);
                    field.Type = GoalFieldType.nongoal;
                    field.TimeStamp  =DateTime.Now;
                    resultValue = false;
                }
                else // (receivedField.type == GoalFieldType.unknown) -- polozono sham na GoalField - zadnych info o polu, wiec wiemy ze Player ma typ sham
                {
                    this.GetPiece.Type = PieceType.sham;
                    this.GetPiece.TimeStamp = DateTime.Now;
                    resultValue = false;
                }
            }

            return resultValue;
        }
        private bool PickUpPieceUpdate(DataMessage responseMessage)
        {
            bool resultValue = false;

            var taskFieldsArray = responseMessage.Tasks;
            var goalFieldsArray = responseMessage.Goals;
            var piecesArray = responseMessage.Pieces;

            if (piecesArray != null && piecesArray.Length > 0 && piecesArray[0] != null)
            {
                var receivedPiece = piecesArray[0];
                this.SetPiece(receivedPiece);
                this.GetPiece.TimeStamp = DateTime.Now;
                PlayerBoard.GetTaskField(location.X, location.Y).Piece = null;
                resultValue = true;
            }
            else
            {
                ConsoleWriter.Warning(guid + " has not picked a piece on location:" + location);
            }

            return resultValue;
        }
        private bool MoveUpdate(DataMessage responseMessage, MoveType direction)
        {
            bool resultValue = false;
            if (direction != LastMoveTaken)
                ConsoleWriter.Warning("MoveUpdate updates for direction: " + direction + " while LastMoveTaken is: " + LastMoveTaken);
            // MoveUpdate oraz gameMaster.HandleMoveRequest updatuja lokacje Playera przez to potrafi ruszyc sie 2 razy
            var futureLocation = CalculateFutureLocation(location, direction);
            var currentLocation = responseMessage.PlayerLocation;
            var taskFieldsArray = responseMessage.Tasks;
            var goalFieldsArray = responseMessage.Goals;
            var piecesArray = responseMessage.Pieces;

            // an attempt to exceed board's boundaries or to enter an opponent's GoalArea
            if (responseMessage.Tasks != null && responseMessage.Tasks.Length == 0)
            {
                this.location = responseMessage.PlayerLocation;
                resultValue = false;
            }
            // future position is a TaskField
            else if (responseMessage.Tasks != null && responseMessage.Tasks.Length > 0)
            {
                // an Player attempted to enter an occupied TaskField
                if (this.location.Equals(responseMessage.PlayerLocation))
                {
                    // add encountered stranger Player to this Player's view
                    var stranger = new GameArea.GameObjects.Player((ulong)responseMessage.Tasks[0].PlayerId);
                    PlayerBoard.GetField(futureLocation.X, futureLocation.Y).Player = stranger;

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
            else if (responseMessage.Goals != null && responseMessage.Goals.Length > 0)
            {
                // an Player attempted to enter an occupied GoalField
                if (this.location.Equals(responseMessage.PlayerLocation))
                {
                    // add encountered stranger Player to this Player's view
                    var stranger = new GameArea.GameObjects.Player((ulong)responseMessage.Goals[0].PlayerId);
                    PlayerBoard.GetField(futureLocation.X, futureLocation.Y).Player = stranger;

                    resultValue = false;
                }
                // an action was valid
                else
                {
                    this.location = responseMessage.PlayerLocation;
                    resultValue = true;
                }
            }
            return resultValue;
        }
        private void DiscoverUpdate(DataMessage responseMessage)
        {
            var taskFieldsArray = responseMessage.Tasks;
            var goalFieldsArray = responseMessage.Goals;
            var piecesArray = responseMessage.Pieces;

            if (taskFieldsArray != null && taskFieldsArray.Length > 0)
            {
                foreach (var respField in taskFieldsArray)
                {
                    if (respField != null)
                    {
                        var coordX = respField.X;
                        var coordY = respField.Y;

                        GameArea.GameObjects.TaskField updatedField = PlayerBoard.GetField(respField.X, respField.Y) as GameArea.GameObjects.TaskField;
                        updatedField.TimeStamp  = respField.TimeStamp;
                        updatedField.DistanceToPiece = respField.DistanceToPiece;


                        if (respField.PlayerId > -1)
                            updatedField.Player = new GameArea.GameObjects.Player((ulong)respField.PlayerId);

                        if (respField.Piece != null)
                            updatedField.Piece = new GameArea.GameObjects.Piece(respField.Piece.ID,DateTime.Now, playerId:respField.Piece.PlayerId);
                    }
                }
            }
            if (goalFieldsArray != null && goalFieldsArray.Length > 0)
            {
                foreach (var respField in goalFieldsArray)
                {
                    if (respField != null)
                    {
                        var coordX = respField.X;
                        var coordY = respField.Y;

                        GameArea.GameObjects.GoalField updatedField = PlayerBoard.GetField(respField.X, respField.Y) as GameArea.GameObjects.GoalField;
                        updatedField.TimeStamp  = respField.TimeStamp;

                        if (respField.Player != null)
                            updatedField.Player = respField.Player;
                    }
                }
            }

        }

        // helpers ---------------------

        private GameArea.GameObjects.Location CalculateFutureLocation(GameArea.GameObjects.Location oldLocation, MoveType direction)
        {
            GameArea.GameObjects.Location newLocation = null;
            switch (direction)
            {
                case MoveType.up:
                    newLocation = new GameArea.GameObjects.Location(oldLocation.X, oldLocation.Y + 1);
                    break;
                case MoveType.down:
                    newLocation = new GameArea.GameObjects.Location(oldLocation.X, oldLocation.Y - 1);
                    break;
                case MoveType.left:
                    newLocation = new GameArea.GameObjects.Location(oldLocation.X - 1, oldLocation.Y);
                    break;
                case MoveType.right:
                    newLocation = new GameArea.GameObjects.Location(oldLocation.X + 1, oldLocation.Y);
                    break;
            }
            return newLocation;
        }
    }
}
