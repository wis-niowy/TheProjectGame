using GameArea.AppMessages;
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
        public GameObjects.TaskField[] TryPlacePieceOnTaskField(GameObjects.Location location, string playerGuid)
        {
            GameObjects.TaskField fieldMessage = null;
            var currentTaskField = actualBoard.GetField(location.X, location.Y) as GameObjects.TaskField;
            var Player = Players.Where(q => q.GUID == playerGuid).First();

            // if TaskField is not occupied
            if (currentTaskField.Piece == null)
            {
                fieldMessage = new GameObjects.TaskField(location.X, location.Y, DateTime.Now)
                {
                    PlayerId = (long)Player.ID,
                    Piece = Player.GetPiece,
                    TimeStamp = DateTime.Now
                };

                var piece = Player.GetPiece;
                piece.PlayerId = 0;
                currentTaskField.Piece = piece; // the piece is put on the field
                Player.SetPiece(null); // the piece is no longer possesed by an Player
                UpdateDistancesFromAllPieces();
            }
            return new GameObjects.TaskField[] { fieldMessage }; // fieldMessage is null if TaskField is occupied
        }

        /// <summary>
        /// Handles Player's request - place a sham Piece on a GoalField
        /// </summary>
        /// <param name="location"></param>
        /// <param name="playerGuid"></param>
        /// <returns></returns>
        public GameObjects.GoalField[] TryPlaceShamPieceOnGoalField(GameObjects.Location location, string playerGuid)
        {
            var teamColour = Players.Where(q => q.GUID == playerGuid).First().Team;
            var player = Players.Where(q => q.GUID == playerGuid).First();
            var fieldMessage = new GameObjects.GoalField(location.X, location.Y, DateTime.Now, teamColour, GoalFieldType.unknown)
            {
                PlayerId = (long)player.ID,
            };

            return new GameObjects.GoalField[] { fieldMessage };
        }

        /// <summary>
        /// Handles Player's request - place a normal Piece on a GoalField
        /// </summary>
        /// <param name="location"></param>
        /// <param name="playerGuid"></param>
        /// <returns></returns>
        public GameObjects.GoalField[] TryPlaceNormalPieceOnGoalField(GameObjects.Location location, string playerGuid)
        {
            var teamColour = Players.Where(q => q.GUID == playerGuid).First().Team;
            var goalFieldType = actualBoard.GetGoalField(location.X, location.Y).Type;
            var Player = Players.Where(q => q.GUID == playerGuid).First();
            var fieldMessage = new GameObjects.GoalField(location.X, location.Y, DateTime.Now, teamColour, goalFieldType)
            {
                PlayerId = (long)Player.ID
            };

            // if GoalField is of type 'goal' we update data and notify point score
            var currentGoalField = actualBoard.GetField(location.X, location.Y) as GameObjects.GoalField;
            if (currentGoalField.Type == GoalFieldType.goal)
            {
                var goal = actualBoard.GetField(location.X, location.Y) as GameMasterGoalField;
                if (goal != null && !goal.IsFullfilled && State != GameMasterState.GameOver)
                {
                    switch (goal.Team)
                    {
                        case TeamColour.red:
                            GoalsRedLeft--;    // one goal less before the game is over
                            break;
                        case TeamColour.blue:
                            GoalsBlueLeft--;
                            break;
                    }
                    if (GoalsBlueLeft == 0 || GoalsRedLeft == 0)
                    {
                        if (IsGameFinished)
                            Console.WriteLine("!!!ACHTUNG!!!\nGame has ended.");
                        State = GameMasterState.GameResolved;
                        GameEndDate = DateTime.Now;
                        PrintEndGameState();
                    }
                    Player.SetPiece(null); // the piece is no longer possesed by an Player
                }

            }

            return new GameObjects.GoalField[] { fieldMessage };
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
        public void PerformMoveAction(GameObjects.Location currentLocation, GameObjects.Location futureLocation,
                                      string playerGuid, DataMessage response)
        {
            var Player = actualBoard.GetField(currentLocation.X, currentLocation.Y).Player;
            actualBoard.GetField(currentLocation.X, currentLocation.Y).Player = null;
            actualBoard.GetField(futureLocation.X, futureLocation.Y).Player = Player;
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
        public void SetInfoAboutDiscoveredTaskField(GameObjects.Location location, int dx, int dy,
                                                    GameObjects.Field field, List<GameObjects.TaskField> TaskFieldList)
        {
            //basic information
            GameObjects.TaskField responseField = new GameObjects.TaskField(location.X + dx, location.Y + dy, DateTime.Now)
            {
                TimeStamp = DateTime.Now,
                DistanceToPiece = (field as GameObjects.TaskField).DistanceToPiece
            };

            //anoter Player on the field
            if (field.HasPlayer())
            {
                responseField.PlayerId = (long)field.Player.ID;
                responseField.Player = field.Player;
            }
            else
            {
                responseField.PlayerId = -1;
                responseField.Player = null;
            }

            //piece on the field
            GameObjects.Piece piece = (field as GameObjects.TaskField).Piece;
            if (piece != null)
            {
                responseField.Piece = piece;
            }
            else
                responseField.Piece = null;

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
        public void SetInfoAboutDiscoveredGoalField(GameObjects.Location location, int dx, int dy,
                                                    GameObjects.Field field, List<GameObjects.GoalField> GoalFieldList)
        {
            GameObjects.GoalField responseField = new GameObjects.GoalField(location.X + dx, location.Y + dy, DateTime.Now, (field as GameObjects.GoalField).Team, GoalFieldType.unknown)
            {
                TimeStamp = DateTime.Now
            };

            if (field.HasPlayer())
            {
                responseField.PlayerId = (long)field.Player.ID;
                responseField.Player = field.Player;
            }
            else
            {
                responseField.PlayerId = -1;
                responseField.Player = null;
            }

            GoalFieldList.Add(responseField);
        }

        private void PrintEndGameState()
        {
            var winner = GoalsBlueLeft == 0 ? TeamColour.blue : TeamColour.red;
            var opponent = GoalsBlueLeft == 0 ? TeamColour.red : TeamColour.blue;
            var opponentScore = winner == TeamColour.blue ? GoalsRedLeft : GoalsBlueLeft;
            ConsoleWriter.Show("\n\n\n************************\n THE WINNERS IS: " + winner + "\n THE NOOBS ARE: " + opponent + "\n WITH GOALS LEFT: " + opponentScore + "\n \n \n END OF GAME: " + GetGameDefinition.GameName + " \n \n*****************");
            Logger.Logger logger = new Logger.Logger(this);
            logger.Log(GameMasterState.GameResolved);
        }

        public void PrintBoardState()
        {
            StringBuilder boardPrint = new StringBuilder("\n BOARD STATE: \n");
            for (int y = (int)GetBoard.Height - 1; y >= 0; y--)
            {
                boardPrint.Append("[" + y);
                if (y < 10)
                    boardPrint.Append(" ");
                boardPrint.Append("] ");
                for (int x = 0; x < GetBoard.Width; x++)
                {
                    var field = GetBoard.GetField(x, y);
                    boardPrint.Append(field.ToString());
                }
                boardPrint.AppendLine();
            }
            for (int x = 0; x < GetBoard.Width; x++)
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
        private GameObjects.Location PerformLocationDelta(MoveType moveType, GameObjects.Location currentLocation, TeamColour team)
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

            if (!ValidateFieldPosition(currentLocation.X, currentLocation.Y, team))
                return currentLocation;
            else
                return new GameObjects.Location(currentLocation.X + dx, currentLocation.Y + dy);
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
            if (x < 0 || x >= actualBoard.Width ||
                y < 0 || y >= actualBoard.Height)
                return true;
            else return false;
        }

        private bool CheckIfNotEnteringWrongGoalArea(int x, int y, TeamColour team)
        {
            if (team == TeamColour.red && y < actualBoard.GoalAreaHeight ||
                team == TeamColour.blue && y >= actualBoard.Height - actualBoard.GoalAreaHeight)
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

            var team = Players.Where(q => q.GUID == guid).First().Team;
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
            var piece = new GameObjects.Piece(id, DateTime.Now, type);
            if (ValidateFieldPosition(x, y, team) && actualBoard.GetField(x, y) is GameObjects.TaskField)
            {
                (actualBoard.GetField(x, y) as GameObjects.TaskField).Piece = piece;
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
                if (State == GameMasterState.GameInprogress)
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
