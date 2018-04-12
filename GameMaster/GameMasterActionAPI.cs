using Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace GameArea
{
    public partial class GameMaster : IGameMaster
    {
        // --------------------------------------    API
        #region API
        /// <summary>
        /// Method to request a Test Piece action
        /// </summary>
        /// <param name="playerGuid">guid of player requesting an action</param>
        /// <param name="gameId">id of current game</param>
        /// <returns></returns>
        public Data HandleTestPieceRequest(string playerGuid, ulong gameId)
        {
            Monitor.Enter(lockObject);
            Piece pieceDataToSend = null;
            var agent = agents.Where(q => q.GUID == playerGuid).First();
            try
            {
                if (agent.GetPiece != null)
                {
                    pieceDataToSend = new Piece()
                    {
                        type = agent.GetPiece.type,
                        id = agent.GetPiece.id,
                        playerId = agent.ID,
                        timestamp = DateTime.Now
                    };
                }
            }
            finally
            {
                Monitor.Exit(lockObject);
            }
            Thread.Sleep((int)GetCosts.TestDelay);
            return new Data()
            {
                gameFinished = IsGameFinished,
                playerId = agent.ID,
                Pieces = new Piece[] { pieceDataToSend }
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="playerGuid"></param>
        /// <param name="gameId"></param>
        /// <returns></returns>
        public Data HandleDestroyPieceRequest(string playerGuid, ulong gameId)
        {
            Monitor.Enter(lockObject);
            Piece pieceDataToSend = null;
            var agent = agents.Where(q => q.GUID == playerGuid).First();
            try
            { 
                if (agent.GetPiece != null)
                {
                    pieceDataToSend = null;
                    agent.SetPiece(null);
                }
            }
            finally
            {
                Monitor.Exit(lockObject);
            }
            Thread.Sleep((int)GetCosts.TestDelay);
            return new Data()
            {
                gameFinished = IsGameFinished,
                playerId = agent.ID,
                Pieces = new Piece[] { pieceDataToSend }
            };
        }

        /// <summary>
        /// Method to request a Place Piece action
        /// Player cannot place the piece if the field is already claimed
        /// </summary>
        /// <param name="playerGuid">guid of player requesting an action</param>
        /// <param name="gameId">id of current game</param>
        /// <returns></returns>
        public Data HandlePlacePieceRequest(string playerGuid, ulong gameId)
        {
            var location = agents.Where(q => q.GUID == playerGuid).First().GetLocation;
            var agent = agents.Where(q => q.GUID == playerGuid).First();
            // basic information
            var response = new Data()
            {
                playerId = agent.ID,
            };

            Monitor.Enter(lockObject);
            try
            {
                // player posseses a piece
                if (agent.GetPiece != null)
                {
                    // player is on the TaskField
                    if (actualBoard.GetField(location.x, location.y) is GameArea.TaskField)
                    {
                        response.TaskFields = TryPlacePieceOnTaskField(location, playerGuid);
                    }
                    // player carries a sham piece and is on a GoalField - he receives no data about a current GoalField and cannot place a piece
                    else if (actualBoard.GetField(location.x, location.y) is GameArea.GoalField && agent.GetPiece.type == PieceType.sham)
                    {
                        response.GoalFields = TryPlaceShamPieceOnGoalField(location, playerGuid);
                    }
                    // the goal field is a goal or nongoal and player carries a normal piece
                    else if (actualBoard.GetField(location.x, location.y) is GameArea.GoalField)
                    {
                        response.GoalFields = TryPlaceNormalPieceOnGoalField(location, playerGuid);
                    }
                }
                // the field is task field and is claimed - action rejected
                // or the player doesn't posses a piece
                response.gameFinished = IsGameFinished;
            }
            finally
            {
                Monitor.Exit(lockObject);
            }
            Thread.Sleep((int)GetCosts.PlacingDelay);
            return response;

        }

        /// <summary>
        /// Method to request a Pick Up Piece action
        /// </summary>
        /// <param name="playerGuid">guid of player requesting an action</param>
        /// <param name="gameId">id of current game</param>
        /// <returns></returns>
        public Data HandlePickUpPieceRequest(string playerGuid, ulong gameId)
        {
            var location = agents.Where(a => a.GUID == playerGuid).First().GetLocation;
            var agent = agents.Where(q => q.GUID == playerGuid).First();
            Piece[] pieces = new Piece[] { null };

            var response = new Data()
            {
                playerId = agent.ID,
                Pieces = pieces
            };

            Monitor.Enter(lockObject);
            try
            {
                var currentTaskField = actualBoard.GetField(location.x, location.y) as GameArea.TaskField;
                // the TaskField contains a piece
                if (currentTaskField != null && currentTaskField.GetPiece != null)
                {
                    Piece pieceDataToSend = new Piece()
                    {
                        type = PieceType.unknown,
                        id = currentTaskField.GetPiece.id,
                        playerId = agent.ID,
                        timestamp = DateTime.Now
                    };

                    ConsoleWriter.Warning("Send piece from location: " + location + " to Agent with GUID: " + playerGuid);

                    response.Pieces[0] = pieceDataToSend;

                    var piece = currentTaskField.GetPiece;
                    agent.SetPiece(piece); // agent picks up a piece
                    currentTaskField.SetPiece(null); // the piece is no longer on the field  
                    UpdateDistancesFromAllPieces();
                }

                // player is either on an empty TaskField or on a GoalField
                response.gameFinished = IsGameFinished;
            }
            finally
            {
                Monitor.Exit(lockObject);
            }
            Thread.Sleep((int)GetCosts.PickUpDelay);
            return response;
        }

        /// <summary>
        /// Method to request a Move action
        /// </summary>
        /// <param name="direction">direction requested by a Player</param>
        /// <param name="playerGuid">guid of player requesting an action</param>
        /// <param name="gameId">id of current game</param>
        /// <returns></returns>
        public Data HandleMoveRequest(MoveType direction, string playerGuid, ulong gameId)
        {
            var currentLocation = agents.Where(a => a.GUID == playerGuid).First().GetLocation;
            var team = agents.Where(a => a.GUID == playerGuid).First().GetTeam;
            var agent = agents.Where(q => q.GUID == playerGuid).First();

            // perform location delta and get future field
            var futureLocation = PerformLocationDelta(direction, currentLocation, team);
            var futureBoardField = actualBoard.GetField(futureLocation.x, futureLocation.y);

            //basic info for response
            Data response = new Data()
            {
                playerId = agent.ID,
                TaskFields = new Messages.TaskField[] { },
                GoalFields = null,
                PlayerLocation = currentLocation
            };

            //player tried to step out of the board or enetr wrong GoalArea
            if (!ValidateFieldPosition((int)futureLocation.x, (int)futureLocation.y, team))
                return response;

            Messages.Piece piece;
            Messages.Field field;

            Monitor.Enter(lockObject);
            try
            {
                // what type of field are we trying to enter - Task or Goal?
                // we set info about a FutureField
                if (futureBoardField is GameArea.TaskField)
                {
                    TryMoveAgentToTaskField(response, futureLocation, playerGuid, out piece, out field);
                }
                else //if (futureBoardField is GameArea.GoalField)
                {
                    TryMoveAgentToGoalField(response, futureLocation, playerGuid, out field);
                }

                // check if there is another agent on the field we're trying to enter
                // if so, we don't actually move, just get an update on the field
                if (futureBoardField.HasAgent())
                {
                    field.playerId = futureBoardField.Player.id;
                    field.playerIdSpecified = true;
                }
                else    //there is no player, we can move
                {
                    // perform move action
                    PerformMoveAction(currentLocation, futureLocation, playerGuid, response);
                }
                response.gameFinished = IsGameFinished;
            }
            finally
            {
                Monitor.Exit(lockObject);
            }
            Thread.Sleep((int)GetCosts.MoveDelay);
            return response;

        }

        /// <summary>
        /// Method to request a Discover action
        /// </summary>
        /// <param name="playerGuid">guid of player requesting an action</param>
        /// <param name="gameId">id of current game</param>
        /// <returns></returns>
        public Data HandleDiscoverRequest(string playerGuid, ulong gameId)
        {
            ConsoleWriter.Show("Handling Discover Request for agent: " + playerGuid);
            var location = agents.Where(a => a.GUID == playerGuid).First().GetLocation;
            var team = agents.Where(q => q.GUID == playerGuid).First().GetTeam;
            List<Messages.TaskField> TaskFieldList = new List<Messages.TaskField>();
            List<Messages.GoalField> GoalFieldList = new List<Messages.GoalField>();

            Monitor.Enter(lockObject);
            try
            {
                for (int dx = -1; dx <= 1; ++dx)
                {
                    for (int dy = -1; dy <= 1; ++dy)
                    {
                        if (dx == 0 && dy == 0) continue;
                        if (ValidateFieldPosition((int)(location.x + dx), (int)(location.y + dy), team))
                        {
                            Field field = actualBoard.GetField((uint)(location.x + dx), (uint)(location.y + dy));
                            // discovered field is a TaskField - can contain players and pieces
                            if (field is TaskField)
                            {
                                SetInfoAboutDiscoveredTaskField(location, dx, dy, field, TaskFieldList);
                            }

                            // discovered field is a GoalField - can contain players
                            else if (field is GoalField)
                            {
                                SetInfoAboutDiscoveredGoalField(location, dx, dy, field, GoalFieldList);
                            }
                        }
                    }
                }
            }
            finally
            {
                Monitor.Exit(lockObject);
            }
            Thread.Sleep((int)GetCosts.DiscoverDelay);
            return new Data()
            {
                gameFinished = IsGameFinished,
                playerId = agents.Where(q => q.GUID == playerGuid).First().ID,
                TaskFields = TaskFieldList.ToArray(),
                GoalFields = GoalFieldList.ToArray()
            };
        }

        #endregion

    }
}
