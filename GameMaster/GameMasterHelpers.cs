using Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Timers;

namespace GameArea
{
    partial class GameMaster
    {
        // ------------ additional methods
        #region addition api methods
        
        /// <summary>
        /// Handles Player's request - place a Piece on a TaskField
        /// </summary>
        public Messages.TaskField[] TryPlacePieceOnTaskField(Location location, string playerGuid)
        {
            Messages.TaskField fieldMessage = null;
            var currentTaskField = actualBoard.GetField(location.x, location.y) as GameArea.TaskField;
            var Player = Players.Where(q => q.GUID == playerGuid).First();

            // if TaskField is not occupied
            if (currentTaskField.GetPiece == null)
            {
                fieldMessage = new Messages.TaskField(location.x, location.y)
                {
                    playerId = Player.ID,
                    playerIdSpecified = true,
                    pieceId = Player.GetPiece.id,
                    pieceIdSpecified = true,
                    timestamp = DateTime.Now
                };

                var piece = Player.GetPiece;
                currentTaskField.SetPiece(piece); // the piece is put on the field
                Player.SetPiece(null); // the piece is no longer possesed by an Player
                UpdateDistancesFromAllPieces();
            }
            return new Messages.TaskField[] { fieldMessage }; // fieldMessage is null if TaskField is occupied
        }

        /// <summary>
        /// Handles Player's request - place a sham Piece on a GoalField
        /// </summary>
        /// <param name="location"></param>
        /// <param name="playerGuid"></param>
        /// <returns></returns>
        public Messages.GoalField[] TryPlaceShamPieceOnGoalField(Location location, string playerGuid)
        {
            var teamColour = Players.Where(q => q.GUID == playerGuid).First().GetTeam;
            var Player = Players.Where(q => q.GUID == playerGuid).First();
            var fieldMessage = new Messages.GoalField()
            {
                x = location.x,
                y = location.y,
                playerId = Player.ID,
                playerIdSpecified = true,
                timestamp = DateTime.Now,
                team = teamColour
            };

            return new Messages.GoalField[] { fieldMessage };
        }

        /// <summary>
        /// Handles Player's request - place a normal Piece on a GoalField
        /// </summary>
        /// <param name="location"></param>
        /// <param name="playerGuid"></param>
        /// <returns></returns>
        public Messages.GoalField[] TryPlaceNormalPieceOnGoalField(Location location, string playerGuid)
        {
            var teamColour = Players.Where(q => q.GUID == playerGuid).First().GetTeam;
            var goalFieldType = actualBoard.GetGoalField(location.x, location.y).GoalType;
            var Player = Players.Where(q => q.GUID == playerGuid).First();
            var fieldMessage = new Messages.GoalField()
            {
                x = location.x,
                y = location.y,
                playerId = Player.ID,
                playerIdSpecified = true,
                timestamp = DateTime.Now,
                type = goalFieldType,
                team = teamColour
            };

            // if GoalField is of type 'goal' we update data and notify point score
            var currentGoalField = actualBoard.GetField(location.x, location.y) as GameArea.GoalField;
            if (currentGoalField.GoalType == GoalFieldType.goal)
            {
                var goal = actualBoard.GetField(location.x, location.y) as GameMasterGoalField;
                if (goal != null && !goal.IsFullfilled && state != GameMasterState.GameOver)
                {
                    switch (goal.GetOwner)
                    {
                        case TeamColour.red:
                            goalsRedLeft--;    // one goal less before the game is over
                            break;
                        case TeamColour.blue:
                            goalsBlueLeft--;
                            break;
                    }
                    if (goalsBlueLeft == 0 || goalsRedLeft == 0)
                    {
                        state = GameMasterState.GameOver;
                        PrintEndGameState();
                    }
                    Player.SetPiece(null); // the piece is no longer possesed by an Player
                }

            }

            return new Messages.GoalField[] { fieldMessage };
        }

        ///// <summary>
        ///// Handles Player's request - move towards TaskField - fills response message with data about futureField
        ///// </summary>
        ///// <param name="location"></param>
        ///// <param name="playerGuid"></param>
        ///// <returns>Info about future field</returns>
        //public void TryMovePlayerToTaskField(Data response, Location futureLocation, string playerGuid,
        //                                    out Messages.Piece piece, out Messages.Field field)
        //{
        //    GameArea.TaskField fieldFromBoard = actualBoard.GetField(futureLocation.x, futureLocation.y) as GameArea.TaskField;
        //    field = new Messages.TaskField(futureLocation.x, futureLocation.y)
        //    {
        //        distanceToPiece = fieldFromBoard.Distance,
        //        timestamp = DateTime.Now,
        //    };

        //    // check if there is a piece on the filed
        //    fieldFromBoard = actualBoard.GetField(futureLocation.x, futureLocation.y) as GameArea.TaskField;
        //    piece = (fieldFromBoard as GameArea.TaskField).GetPiece;
        //    if (piece != null)
        //    {
        //        response.Pieces = new Messages.Piece[] { piece };
        //        (field as Messages.TaskField).pieceId = piece.id;
        //        (field as Messages.TaskField).pieceIdSpecified = true;
        //    }
        //    response.TaskFields = new Messages.TaskField[] { (field as Messages.TaskField) };
        //}

        ///// <summary>
        ///// Handles Player's request - move towards GoalField - fills response message with data about futureField
        ///// </summary>
        ///// <param name="location"></param>
        ///// <param name="playerGuid"></param>
        ///// <returns>Info about future field</returns>
        //public void TryMovePlayerToGoalField(Data response, Location futureLocation, string playerGuid,
        //                                    out Messages.Field field)
        //{
        //    GameArea.GoalField fieldFromBoard = actualBoard.GetField(futureLocation.x, futureLocation.y) as GameArea.GoalField;
        //    field = new Messages.GoalField(futureLocation.x, futureLocation.y, fieldFromBoard.GetOwner)
        //    {
        //        timestamp = DateTime.Now,
        //    };
        //    response.GoalFields = new Messages.GoalField[] { (field as Messages.GoalField) };
        //    response.TaskFields = null;
        //} // najprawdopodobniej do wyrzucenia

        /// <summary>
        /// Performs move action for an Player - called when action is valid
        /// </summary>
        /// <param name="currentLocation"></param>
        /// <param name="futureLocation"></param>
        /// <param name="playerGuid"></param>
        /// <param name="response"></param>
        public void PerformMoveAction(Location currentLocation, Location futureLocation,
                                      string playerGuid, Data response)
        {
            var Player = actualBoard.GetField(currentLocation.x, currentLocation.y).Player;
            actualBoard.GetField(currentLocation.x, currentLocation.y).Player = null;
            actualBoard.GetField(futureLocation.x, futureLocation.y).Player = Player;
            response.PlayerLocation = futureLocation;
            Players.Where(q => q.GUID == playerGuid).First().SetLocation(futureLocation);
        }

        /// <summary>
        /// Sets info about discovered TaskField
        /// </summary>
        /// <param name="location"></param>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        /// <param name="field"></param>
        /// <param name="TaskFieldList"></param>
        public void SetInfoAboutDiscoveredTaskField(Location location, int dx, int dy,
                                                    Field field, List<Messages.TaskField> TaskFieldList)
        {
            //basic information
            Messages.TaskField responseField = new Messages.TaskField(location.x, location.y)
            {
                x = location.x + dx,
                y = location.y + dy,
                timestamp = DateTime.Now,
                distanceToPiece = (field as TaskField).Distance
            };

            //anoter Player on the field
            if (field.HasPlayer())
            {
                responseField.playerId = field.Player.id;
                responseField.playerIdSpecified = true;
            }
            else
                responseField.playerIdSpecified = false;

            //piece on the field
            Messages.Piece piece = (field as TaskField).GetPiece;
            if (piece != null)
            {
                responseField.pieceId = piece.id;
                responseField.pieceIdSpecified = true;
            }
            else
                responseField.pieceIdSpecified = false;

            TaskFieldList.Add(responseField);
        }

        /// <summary>
        /// Sets info about discovered GoalField
        /// </summary>
        /// <param name="location"></param>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        /// <param name="field"></param>
        /// <param name="GoalFieldList"></param>
        public void SetInfoAboutDiscoveredGoalField(Location location, int dx, int dy,
                                                    Field field, List<Messages.GoalField> GoalFieldList)
        {
            Messages.GoalField responseField = new Messages.GoalField(location.x, location.y, (field as GoalField).GetOwner)
            {
                x = location.x + dx,
                y = location.y + dy,
                timestamp = DateTime.Now
            };

            if (field.HasPlayer())
            {
                responseField.playerId = field.Player.id;
                responseField.playerIdSpecified = true;
            }
            else
                responseField.playerIdSpecified = false;

            GoalFieldList.Add(responseField);
        }

        private void PrintEndGameState()
        {
            var winner = GoalsBlueLeft == 0 ? TeamColour.blue : TeamColour.red;
            var opponent = GoalsBlueLeft == 0 ? TeamColour.red : TeamColour.blue;
            var opponentScore = winner == TeamColour.blue ? goalsRedLeft : goalsBlueLeft;
            ConsoleWriter.Show("\n\n\n************************\n THE WINNERS IS: " + winner + "\n THE NOOBS ARE: " + opponent + "\n WITH GOALS LEFT: " + opponentScore + "\n \n \n END OF GAME: " + GetGameDefinition.GameName + " \n \n*****************");
        }

        public void PrintBoardState()
        {
            StringBuilder boardPrint = new StringBuilder("\n BOARD STATE: \n");
            for (int y = (int)GetBoard.BoardHeight - 1; y >= 0; y--)
            {
                boardPrint.Append("[" + y);
                if (y < 10)
                    boardPrint.Append(" ");
                boardPrint.Append("] ");
                for (int x = 0; x < (int)GetBoard.BoardWidth; x++)
                { 
                    var field = GetBoard.GetField(x, y);
                    boardPrint.Append(field.ToString());
                }
                boardPrint.AppendLine();
            }
            for (int x = 0; x < (int)GetBoard.BoardWidth; x++)
            {
                if (x == 0)
                    boardPrint.Append("     ");
                boardPrint.Append("[ " + x + " ]");
            }
            ConsoleWriter.Show(boardPrint.ToString());
        }

        /// <summary>
        /// Converts MoveType enum object to Location object
        /// </summary>
        /// <returns></returns>
        private Messages.Location PerformLocationDelta(MoveType moveType, Messages.Location currentLocation, TeamColour team)
        {
            // is MoveUp the same for red and blue team? or if for red Up is +1 for blue should be -1 on OY???
            int dx = 0, dy = 0;

            switch (moveType)
            {
                case MoveType.right:
                    dx = 1;
                    break;
                case MoveType.left:
                    dx = -1;
                    break;
                case MoveType.down:
                    dy = -1;
                    break;
                case MoveType.up:
                    dy = 1;
                    break;
            }

            if (!ValidateFieldPosition(currentLocation.x, currentLocation.y, team))
                return currentLocation;
            else
                return new Messages.Location(currentLocation.x + dx, currentLocation.y + dy);
        }

        /// <summary>
        /// Validates if an Player can move on a given field or disvover it
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="team"></param>
        /// <returns></returns>
        private bool ValidateFieldPosition(int x, int y, TeamColour team)
        {
            if (CheckIfNotOutOfBorad(x, y))
                return false;
            else if (CheckIfNotEnteringWrongGoalArea(x, y, team))
                return false;
            else return true;
        }

        private bool CheckIfNotOutOfBorad(int x, int y)
        {
            //stepping out of the board
            if (x < 0 || x >= actualBoard.BoardWidth ||
                y < 0 || y >= actualBoard.BoardHeight)
                return true;
            else return false;
        }

        private bool CheckIfNotEnteringWrongGoalArea(int x, int y, TeamColour team)
        {
            if (team == TeamColour.red && y < actualBoard.GoalAreaHeight ||
                team == TeamColour.blue && y >= actualBoard.BoardHeight - actualBoard.GoalAreaHeight)
                return true;
            else return false;
        }

        /// <summary>
        /// FOR UNIT TESTING - set player in a given board location
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="guid"></param>
        /// <returns></returns>
        public bool SetAbsolutePlayerLocation(int x, int y, string guid)
        {
            var player = Players.Where(q => q.GUID == guid).First();

            var team = Players.Where(q => q.GUID == guid).First().GetTeam;
            if (ValidateFieldPosition(x, y, team))
            {
                Players.Where(q => q.GUID == guid).First().SetLocation(x, y);
                actualBoard.GetField(x, y).Player = Players.Where(q => q.GUID == guid).First().ConvertToMessagePlayer();
                // rzutowanie wymuszone lekkim balaganem: w fieldzie jest typ Message.Player, na liscie Player.Player - laczymy ich po ID w razie potrzeby
                return true;
            }
            return false;
        }

        /// <summary>
        /// FOR UNIT TESTING - add player to the GM list and set player in a given board location
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="guid"></param>
        /// <returns></returns>
        public bool SetAbsolutePlayerLocation(int x, int y, Player.Player Player)
        {
            bool result = false;
            var player = Players.Where(q => q.GUID == Player.GUID).First(); // znajdujemy Playera na liscie Playerow Game Mastera -- czy jest zarejestrowany

            if (player != null)
            {
                result = SetAbsolutePlayerLocation(x, y, Player.GUID);
            }

            return result;

            //var team = Player.GetTeam;
            //if (ValidateFieldPosition((int)x, (int)y, team))
            //{
            //    Player.SetLocation(x, y);
            //    actualBoard.GetField(x, y).Player = Player.ConvertToMessagePlayer();

            //    // rzutowanie wymuszone lekkim balaganem: w fieldzie jest typ Message.Player, na liscie Player.Player - laczymy ich po ID w razie potrzeby
            //    return true;
            //}

            //return false;
        }

        /// <summary>
        /// FOR UNIT TESTING - set a piece of a given type in a given location
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="type"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool SetPieceInLocation(int x, int y, TeamColour team, PieceType type, ulong id)
        {
            var piece = new Piece(type, id);
            if (ValidateFieldPosition(x, y, team) && actualBoard.GetField(x, y) is TaskField)
            {
                (actualBoard.GetField(x, y) as TaskField).SetPiece(piece);
                UpdateDistancesFromAllPieces();
                return true;
            }
            else return false;
        }

        #endregion
        private void PieceAdder_Elapsed(object sender, ElapsedEventArgs e)
        {
            Monitor.Enter(lockObject);
            try
            {
                if (state == GameMasterState.GameInprogress)
                {
                    PlaceNewPiece();
                }
            }
            finally
            {
                Monitor.Exit(lockObject);
            }
        }
    }
}
