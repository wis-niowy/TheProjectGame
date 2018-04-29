using GameArea;
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
        public T PrepareMessageObject<T>(string guid, ulong gameId) where T: GameMessage, new()
        {
            T msg = new T
            {
                playerGuid = guid,
                gameId = gameId
            };
            return msg;
        }

        /// <summary>
        /// Method to send request to test the piece
        /// </summary>
        /// <param name="gameMaster">Addressee of the request</param>
        /// <returns>True - request was valid; False - request was not valid</returns>
        public bool TestPiece(IGameMaster gameMaster)
        {
            if (GetPiece != null)
                ConsoleWriter.Show(GUID + " tries to test piece: " + GetPiece.id + " on location: " + GetLocation);
            TestPiece msg = PrepareMessageObject<TestPiece>(this.GUID, this.gameId);
            LastActionTaken = ActionType.TestPiece;
            try
            {
                Controller.BeginSend(MessageParser.Serialize(msg)); //każda akcja od razu się wysyła, ustawia również LastActionTaken i dla move LastMoveTaken !!!!!
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
        public bool PlacePiece(IGameMaster gameMaster)
        {
            ConsoleWriter.Show(guid + " places piece: " + piece.id + " of type: " + piece.type + " on location: " + GetLocation);
            // should we check if received location is the same as the actual one?
            PlacePiece msg = PrepareMessageObject<PlacePiece>(this.GUID, this.GameId);
            LastActionTaken = ActionType.PlacePiece;
            try
            {
                Controller.BeginSend(MessageParser.Serialize(msg)); //każda akcja od razu się wysyła, ustawia również LastActionTaken i dla move LastMoveTaken !!!!!
            }
            catch (Exception e)
            {
                ConsoleWriter.Error("Error occured when writing message to socket.\n Error text: \n" + e.ToString());
                State = AgentState.Dead;
                return false;
            }
            return true;
        }

        public bool PickUpPiece(IGameMaster gameMaster)
        {
            ConsoleWriter.Show(guid + " picks up piece on location: " + GetLocation);
            PickUpPiece msg = PrepareMessageObject<PickUpPiece>(this.GUID, this.GameId);
            LastActionTaken = ActionType.PickUpPiece;
            try
            {
                Controller.BeginSend(MessageParser.Serialize(msg)); //każda akcja od razu się wysyła, ustawia również LastActionTaken i dla move LastMoveTaken !!!!!
            }
            catch (Exception e)
            {
                ConsoleWriter.Error("Error occured when writing message to socket.\n Error text: \n" + e.ToString());
                State = AgentState.Dead;
                return false;
            }
            return true;
        }

        public bool Move(IGameMaster gameMaster, MoveType direction)
        {
            ConsoleWriter.Show(guid + " wants to move from: " + GetLocation + " in direction: " + direction);
            Move msg = PrepareMessageObject<Move>(this.GUID, this.GameId);
            msg.direction = direction;
            msg.directionSpecified = true;
            LastActionTaken = ActionType.Move;
            LastMoveTaken = direction;
            try
            {
                Controller.BeginSend(MessageParser.Serialize(msg)); //każda akcja od razu się wysyła, ustawia również LastActionTaken i dla move LastMoveTaken !!!!!
            }
            catch (Exception e)
            {
                ConsoleWriter.Error("Error occured when writing message to socket.\n Error text: \n" + e.ToString());
                State = AgentState.Dead;
                return false;
            }
            return true;
        }


        public bool Discover(IGameMaster gameMaster)
        {
            ConsoleWriter.Show(guid + " discovers on location: " + GetLocation);
            Discover msg = PrepareMessageObject<Discover>(this.GUID, this.GameId);
            LastActionTaken = ActionType.Discover;
            try
            {
                Controller.BeginSend(MessageParser.Serialize(msg)); //każda akcja od razu się wysyła, ustawia również LastActionTaken i dla move LastMoveTaken !!!!!
            }
            catch (Exception e)
            {
                ConsoleWriter.Error("Error occured when writing message to socket.\n Error text: \n" + e.ToString());
                State = AgentState.Dead;
                return false;
            }
            return true;
        }

        public bool Destroy(IGameMaster gameMaster)
        {
            ConsoleWriter.Show(guid + " tries to destroy piece: " + piece.id + " which is: " + piece.type + "on location: " + GetLocation);
            DestroyPiece msg = PrepareMessageObject<DestroyPiece>(this.GUID, this.GameId);
            LastActionTaken = ActionType.Destroy;
            try
            {
                Controller.BeginSend(MessageParser.Serialize(msg)); //każda akcja od razu się wysyła, ustawia również LastActionTaken i dla move LastMoveTaken !!!!!
            }
            catch (Exception e)
            {
                ConsoleWriter.Error("Error occured when writing message to socket.\n Error text: \n" + e.ToString());
                State = AgentState.Dead;
                return false;
            }
            return true;
        }
        
        // additional methods

        public bool UpdateLocalBoard(Data responseMessage, ActionType action)
        {
            bool updated = false;
            var gameFinished = responseMessage.gameFinished;
            var playerId = responseMessage.playerId;

            if (playerId == this.ID && !gameFinished)
            {
                Piece[] pieces = responseMessage.Pieces;
                GameArea.TaskField[] taskFields = responseMessage.TaskFields.ToList().Select(p => ConvertToGameAreaTaskField(p)).ToArray();
                GameArea.GoalField[] goalFields = responseMessage.GoalFields.ToList().Select(p => ConvertToGameAreaGoalField(p)).ToArray();

                GetBoard.UpdateGoalFields(goalFields);
                GetBoard.UpdateTaskFields(taskFields);
                GetBoard.UpdatePieces(pieces);

                ConsoleWriter.Show("Updated state by: " + guid + " from action: " + action);
                updated = true;
            }

            return updated;

        }

        public GameArea.TaskField ConvertToGameAreaTaskField(Messages.TaskField field)
        {
            GameArea.TaskField newField = new GameArea.TaskField(field.x, field.y);
            Messages.Player player = new Messages.Player()
            {
                id = field.playerId,
                role = myTeam.Union(otherTeam).First(p => p.id == field.playerId).role,
                team = myTeam.Union(otherTeam).First(p => p.id == field.playerId).team,
            };
            if (field.playerIdSpecified)
                newField.Player = player;

            Piece piece = new Piece()
            {
                playerId = field.playerId,
                timestamp = field.timestamp,
                playerIdSpecified = field.playerIdSpecified,
                id = field.pieceId,
                type = PieceType.unknown
            };
            if (field.pieceIdSpecified)
                newField.SetPiece(piece);

            return newField;

        }

        public GameArea.GoalField ConvertToGameAreaGoalField(Messages.GoalField field)
        {
            GameArea.GoalField newField = new GameArea.GoalField(field.x, field.y, field.team);
            Messages.Player player = new Messages.Player()
            {
                id = field.playerId,
                //role = myTeam.Union(otherTeam).First(p => p.id == field.playerId).role,
                //team = myTeam.Union(otherTeam).First(p => p.id == field.playerId).team,
            };
            if (field.playerIdSpecified)
                newField.Player = player;

            return newField;
        }

        public GameArea.Board ConvertToGameAreaBoard(Messages.GameBoard board)
        {
            return new GameArea.Board(board.width, board.tasksHeight, board.goalsHeight);
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
