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
                ConsoleWriter.Show(GUID + " tries to test piece: " + GetPiece.ID + " on location: " + Location);
            TestPieceMessage msg = new TestPieceMessage(GUID, GameId);
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
            ConsoleWriter.Show(GUID + " places piece: " + piece.ID + " of type: " + piece.Type + " on location: " + Location);
            // should we check if received location is the same as the actual one?
            PlacePieceMessage msg = new PlacePieceMessage(GUID, GameId);
            LastActionTaken = ActionToComplete = ActionType.PlacePiece;
            Controller.BeginSend(msg.Serialize()); //każda akcja od razu się wysyła, ustawia również LastActionTaken i dla move LastMoveTaken !!!!!
            WaitForActionComplete();
            return !HasPiece;
        }

        public bool PickUpPiece()
        {
            ConsoleWriter.Show(GUID + " picks up piece on location: " + Location);
            PickUpPieceMessage msg = new PickUpPieceMessage(GUID, GameId);
            LastActionTaken = ActionToComplete = ActionType.PickUpPiece;
            Controller.BeginSend(msg.Serialize()); //każda akcja od razu się wysyła, ustawia również LastActionTaken i dla move LastMoveTaken !!!!!
            WaitForActionComplete();
            return HasPiece;
        }

        public bool Move(MoveType direction)
        {
            ConsoleWriter.Show(GUID + " wants to move from: " + Location + " in direction: " + direction);
            MoveMessage msg = new MoveMessage(GUID, GameId, direction);
            LastActionTaken = ActionToComplete = ActionType.Move;
            LastMoveTaken = direction;
            var futureLocation = CalculateFutureLocation(Location, direction);
            Controller.BeginSend(msg.Serialize()); //każda akcja od razu się wysyła, ustawia również LastActionTaken i dla move LastMoveTaken !!!!!
            WaitForActionComplete();

            return Location.Equals(futureLocation);
        }


        public bool Discover()
        {
            ConsoleWriter.Show(GUID + " discovers on location: " + Location);
            DiscoverMessage msg = new DiscoverMessage(GUID, GameId);
            LastActionTaken = ActionToComplete = ActionType.Discover;

            Controller.BeginSend(msg.Serialize()); //każda akcja od razu się wysyła, ustawia również LastActionTaken i dla move LastMoveTaken !!!!!
            WaitForActionComplete();
            return true;
        }

        public bool Destroy()
        {
            ConsoleWriter.Show(GUID + " tries to destroy piece: " + piece.ID + " which is: " + piece.Type + "on location: " + Location);
            DestroyPieceMessage msg = new DestroyPieceMessage(GUID, GameId);
            LastActionTaken = ActionToComplete = ActionType.Destroy;

            Controller.BeginSend(msg.Serialize()); //każda akcja od razu się wysyła, ustawia również LastActionTaken i dla move LastMoveTaken !!!!!
            WaitForActionComplete();
            return !HasPiece;
        }

        // additional methods

        public bool UpdateLocalBoard(DataMessage responseMessage, ActionType action)
        {
            bool updated = false;
            var gameFinished = responseMessage.GameFinished;
            var playerId = responseMessage.PlayerId;
            
            if (playerId == this.ID && !gameFinished)
            {
                GetBoard.UpdateGoalFields(responseMessage.Goals);
                GetBoard.UpdateTaskFields(responseMessage.Tasks);
                GetBoard.UpdatePieces(responseMessage.Pieces);


                if (responseMessage.Pieces != null && responseMessage.Pieces.Length == 1)
                {
                    var piece = responseMessage.Pieces[0];
                    if (piece != null)
                    {
                        if (GetPiece == null)
                            // Player picks up piece
                        {
                            this.SetPiece(piece);
                            GetBoard.GetTaskField(Location).Piece = null;
                        }
                        else if (GetPiece.ID == piece.ID)
                            // Player tests piece
                        {
                            this.SetPiece(piece);
                        }
                    }
                }

                if (responseMessage.Tasks != null && responseMessage.Tasks.Length == 1)
                {
                    var field = responseMessage.Tasks[0];
                    if (field != null && GetPiece != null)
                    {
                        if (field.Piece != null && field.X == Location.X && field.Y == Location.Y && field.Piece.ID == GetPiece.ID)
                        // player put down a piece
                        {
                            SetPiece(null);
                        }
                    }
                        
                }

                if (responseMessage.Goals != null && responseMessage.Goals.Length == 1)
                {
                    var field = responseMessage.Goals[0];
                    if (field.Type == GoalFieldType.goal && field.X == Location.X && field.Y == Location.Y)
                    {
                        SetPiece(null);
                        PlayerBoard.GetGoalField(Location.X, Location.Y).TimeStamp = DateTime.Now.AddYears(100);
                    }
                }

                if (responseMessage.PlayerLocation != null)
                    SetLocation(responseMessage.PlayerLocation.X, responseMessage.PlayerLocation.Y);

                updated = true;
            }
            ActionToComplete = ActionType.none;
            return updated;
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
