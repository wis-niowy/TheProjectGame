using GameArea;
using GameArea.AppMessages;
using GameArea.Parsers;
using Messages;
using Player.PlayerMessages;
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
            if (gameFinished != true)
            {
                if (GetPiece != null)
                    ConsoleWriter.Show(GUID + " tries to test piece: " + GetPiece.ID + " on location: " + Location);
                TestPieceMessage msg = new TestPieceMessage(GUID, GameId);
                LastActionTaken = ActionToComplete = ActionType.TestPiece;
                Controller.BeginSend(msg.Serialize()); //każda akcja od razu się wysyła, ustawia również LastActionTaken i dla move LastMoveTaken !!!!!
                return true;
            }
            return false;
        }

        /// <summary>
        /// Method to send a request to place the piece
        /// </summary>
        /// <param name="gameMaster"></param>
        /// <returns></returns>
        public bool PlacePiece()
        {
            if (gameFinished != true)
            {
                ConsoleWriter.Show(GUID + " places piece: " + piece.ID + " of type: " + piece.Type + " on location: " + Location);
                // should we check if received location is the same as the actual one?
                PlacePieceMessage msg = new PlacePieceMessage(GUID, GameId);
                LastActionTaken = ActionToComplete = ActionType.PlacePiece;
                Controller.BeginSend(msg.Serialize()); //każda akcja od razu się wysyła, ustawia również LastActionTaken i dla move LastMoveTaken !!!!!
                WaitForActionComplete(-1); //czekaj aż się wykona akcja
                return !HasPiece;
            }
            return false;
        }

        public bool PickUpPiece()
        {
            if (gameFinished != true)
            {
                ConsoleWriter.Show(GUID + " picks up piece on location: " + Location);
                PickUpPieceMessage msg = new PickUpPieceMessage(GUID, GameId);
                LastActionTaken = ActionToComplete = ActionType.PickUpPiece;
                Controller.BeginSend(msg.Serialize()); //każda akcja od razu się wysyła, ustawia również LastActionTaken i dla move LastMoveTaken !!!!!
                WaitForActionComplete();
                return HasPiece;
            }
            return false;
        }

        public bool Move(MoveType direction)
        {
            if (gameFinished != true)
            {
                ConsoleWriter.Show(GUID + " wants to move from: " + Location + " in direction: " + direction);
                MoveMessage msg = new MoveMessage(GUID, GameId, direction);
                LastActionTaken = ActionToComplete = ActionType.Move;
                LastMoveTaken = direction;
                var futureLocation = CalculateFutureLocation(Location, direction);
                Controller.BeginSend(msg.Serialize()); //każda akcja od razu się wysyła, ustawia również LastActionTaken i dla move LastMoveTaken !!!!!
                WaitForActionComplete();

                return Location != null && Location.Equals(futureLocation);
            }
            return false;
        }


        public bool Discover()
        {
            if (gameFinished != true)
            {
                ConsoleWriter.Show(GUID + " discovers on location: " + Location);
                DiscoverMessage msg = new DiscoverMessage(GUID, GameId);
                LastActionTaken = ActionToComplete = ActionType.Discover;

                Controller.BeginSend(msg.Serialize()); //każda akcja od razu się wysyła, ustawia również LastActionTaken i dla move LastMoveTaken !!!!!
                WaitForActionComplete();
                return true;
            }
            return false;
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

        public bool UpdateLocalBoard(DataMessage responseMessage)
        {
            bool updated = false;
            gameFinished = gameFinished || responseMessage.GameFinished;
                
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
                        else if (GetPiece.ID == piece.ID && piece.Type != PieceType.destroyed)
                        // Player tests piece
                        {
                            this.SetPiece(piece);
                        }
                        else if (GetPiece.ID == piece.ID && piece.Type == PieceType.destroyed)
                        // Player tests piece
                        {
                            this.SetPiece(null);
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
                    if (field.X == Location.X && field.Y == Location.Y)
                    {
                        if(field.Type == GoalFieldType.goal) //tylko wtedy ustawia się null
                            SetPiece(null);
                        //zawsze aktualizacja timestamp, bo jak już raz był na danym polu typu goal, to ma już do niego nie wracać 
                        var goalPlayer = PlayerBoard.GetGoalField(Location.X, Location.Y);
                        goalPlayer.TimeStamp = DateTime.Now.AddYears(100);
                        goalPlayer.Type = field.Type == GoalFieldType.goal ? GoalFieldType.goal : GoalFieldType.nongoal; 
                    }
                    
                }

                if (responseMessage.PlayerLocation != null)
                    SetLocation(responseMessage.PlayerLocation.X, responseMessage.PlayerLocation.Y);

                updated = true;
            }
            else if (gameFinished)
            {
                //wypisywnaie planszy po otryzmaniu Data, wymóg specyfikacji
                Console.WriteLine("!!!ACHTUNG!!!\nReceived DATA MESSAGE from GameMaster with GameFinished == true. PlayerId/ClientId:" + ID + "\nGUID: " + GUID);
                State = AgentState.SearchingForGame;

            }
            ActionToComplete = ActionType.none;
            return updated;
        }

        public virtual DataMessage HandleKnowledgeExchangeRequest(KnowledgeExchangeRequestMessage messageObject)
        {
            DataMessage responseData = null;

            var leaderId = myTeam.Where(p => p.Role == PlayerRole.leader).Select(p => p.ID).FirstOrDefault();

            if (leaderId == messageObject.SenderPlayerId)
                // wiadomosc od swojego leadera - natychmiastowa odpowiedz
            {
                responseData = PrepareKnowledgeExchangeMessage(messageObject);
            }
            else if (myTeam.Select(p => p.ID).Contains(messageObject.SenderPlayerId))
                // wiadomosc od swojego playera 
            {
                AddMyPlayerExhangeKnowledgeRequest(messageObject as KnowledgeExchangeRequestAgent);
            }
            else if (otherTeam.Select(p => p.ID).Contains(messageObject.SenderPlayerId))
            // wiadomosc od obcego playera
            {
                AddOtherPlayerExhangeKnowledgeRequest(messageObject as KnowledgeExchangeRequestAgent);
            }

            return responseData;
        }

        public virtual void HandleRejectKnowledgeExchange(RejectKnowledgeExchangeMessage messageObject)
        {
            // poki co - Player olewa
        }

        // helpers ---------------------

        protected DataMessage PrepareKnowledgeExchangeMessage(KnowledgeExchangeRequestMessage messageObject)
        {
            var responseData = new DataMessage(messageObject.SenderPlayerId)
            {
                Goals = GetBoard.GetRedGoalAreaFields.Union(GetBoard.GetBlueGoalAreaFields).Select(f => new GameArea.GameObjects.GoalField(f)).ToArray(),
                Tasks = GetBoard.TaskFields.Select(q => new GameArea.GameObjects.TaskField(q)).ToArray()
            };
            var xCoord = Location.X;
            var yCoord = Location.Y;

            // do Data musi też dodać, na Field na ktorym stoi, swoj stan !!!
            if (GetBoard.GetField(xCoord, yCoord) is GameArea.GameObjects.GoalField)
            {
                var field = responseData.Goals.Where(f => f.X == xCoord && f.Y == yCoord).FirstOrDefault();
                field.Player = new GameArea.GameObjects.Player(this.ID, this.Team, this.Role);
                field.TimeStamp = DateTime.Now;
                field.PlayerId = (long)this.ID;
            }
            else // is TaskField
            {
                var field = responseData.Tasks.Where(f => f.X == xCoord && f.Y == yCoord).FirstOrDefault();
                field.Player = new GameArea.GameObjects.Player(this.ID, this.Team, this.Role);
                if (this.HasPiece)
                    field.Piece = new GameArea.GameObjects.Piece(this.GetPiece.ID, this.GetPiece.TimeStamp, this.GetPiece.Type, this.GetPiece.PlayerId);
                field.TimeStamp = DateTime.Now;
                field.PlayerId = (long)this.ID;
            }

            return responseData;
        }

        public GameArea.GameObjects.Location CalculateFutureLocation(GameArea.GameObjects.Location oldLocation, MoveType direction)
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
