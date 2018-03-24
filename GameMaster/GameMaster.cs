﻿using Messages;
using Player;
using System;
using System.Collections.Generic;
using System.Linq;
using GameArea;
using Configuration;

namespace GameArea
{
    public enum GameMasterState { AwaitingPlayers, GameInprogress, GameOver };
    public class GameMaster : IGameMaster
    {
        private GameMasterState state;
        private ulong nextPieceId = 0;
        private ulong goalsLeft;
        private Random random;
        private List<Player.Agent> agents;
        //private List<Piece> pieces;
        private Dictionary<string, uint> agentIdDictionary;
        private Board actualBoard;
        private GameMasterSettings gameSettings;

        public List<Player.Agent> GetAgents
        {
            get
            {
                return agents;
            }
        }

        public GameMasterState GetState
        {
            get
            {
                return state;
            }
        }

        public GameMasterSettingsGameDefinition GetGameDefinition
        {
            get
            {
                return gameSettings.GameDefinition;
            }
        }


        public List<Player.Agent> GetAgentsByTeam(Messages.TeamColour team)
        {
            return agents.Where(q => q.GetTeam == team).ToList();
        }

        public Player.Agent GetAgentByGuid(string guid)
        {
            return agents.Where(q => q.GUID == guid).FirstOrDefault();
        }

        public void RegisterAgent(Player.Agent agent)
        {
            var newId = agents.Count > 0 ? agents.Max(q => q.ID) + 1 : 1;
            agent.ID = newId;
            agent.SetBoard(new Board(GetGameDefinition.BoardWidth, GetGameDefinition.TaskAreaLength, GetGameDefinition.GoalAreaLength));
            agents.Add(new Player.Agent(agent));
        }

        public Board GetBoard
        {
            get
            {
                return actualBoard;
            }
        }

        public GameMaster(GameMasterSettings settings)
        {
            state = GameMasterState.AwaitingPlayers;
            random = new Random();
            agents = new List<Player.Agent>();
            goalsLeft = settings.GameDefinition.NumberOfGoalsPerGame;
            agentIdDictionary = new Dictionary<string, uint>();
            gameSettings = settings;
            InitBoard(gameSettings.GameDefinition);
        }

        private void InitBoard(GameMasterSettingsGameDefinition settings)
        {
            actualBoard = new Board((uint)settings.BoardWidth, settings.TaskAreaLength, settings.GoalAreaLength);
            PlaceInitialPieces(settings.InitialNumberOfPieces);
            PlaceInitialGoals(settings.Goals);
        }

        private void PlaceInitialGoals(Messages.GoalField[] goals)
        {
            foreach (var goal in goals)
            {
                actualBoard.SetGoalField(new GameArea.GoalField(goal.x, goal.y, goal.team, goal.type));
            }
        }


        /// <summary>
        /// Places a piece in TaskArea in a random way
        /// </summary>
        /// <param name="piecesCount"></param>
        private void PlaceInitialPieces(uint piecesCount)
        {
            for (uint i = 0; i < piecesCount; i++)
            {
                var piece = CreatePiece();
                var field = GetEmptyFieldForPiece();
                field.SetPiece(piece);
            }
        }

        private Piece CreatePiece()
        {
            var possibleSham = random.NextDouble();
            if (possibleSham <= gameSettings.GameDefinition.ShamProbability)
                return new Piece(PieceType.sham, nextPieceId++);
            else
                return new Piece(PieceType.normal, nextPieceId++);
        }

        private GameArea.TaskField GetEmptyFieldForPiece()
        {
            uint x, y;
            GameArea.TaskField field;
            if (actualBoard.TaskFields.Where(q => q.GetPiece != null).Count() == (int)(actualBoard.BoardWidth * actualBoard.TaskAreaHeight))
                return null;
            do
            {
                x = (uint)random.Next() % actualBoard.TaskAreaHeight + actualBoard.GoalAreaHeight;
                y = (uint)random.Next() % actualBoard.BoardWidth;
                field = actualBoard.GetTaskField(x, y);
            }
            while (field == null || field.GetPiece != null);
            return field;
        }

        /// <summary>
        /// method provides agent with lists myTeam and otherTeam
        /// </summary>
        /// <param name="agent"></param>
        private void SetGameInfo(Player.Agent agent)
        {
            foreach (var otherAgent in agents)
            {
                agent.myTeam = new List<Messages.Agent>();
                agent.otherTeam = new List<Messages.Agent>();
                //player from another team
                if (otherAgent.GetTeam != agent.GetTeam)
                    agent.otherTeam.Add(otherAgent.ConvertToMessageAgent());
                //player from the same team
                else
                    agent.myTeam.Add(otherAgent.ConvertToMessageAgent());
            }
        }



        // --------------------------------------    API

        /// <summary>
        /// Method to request a Test Piece action
        /// </summary>
        /// <param name="playerGuid">guid of player requesting an action</param>
        /// <param name="gameId">id of current game</param>
        /// <returns></returns>
        public Data HandleTestPieceRequest(string playerGuid, ulong gameId)
        {
            Piece pieceDataToSend = null;

            if (agents.Where(q => q.GUID == playerGuid).First().GetPiece != null)
            {
                pieceDataToSend = new Piece()
                {
                    type = agents.Where(q => q.GUID == playerGuid).First().GetPiece.type,
                    id = agents.Where(q => q.GUID == playerGuid).First().GetPiece.id,
                    playerId = agents.Where(q => q.GUID == playerGuid).First().ID,
                    timestamp = DateTime.Now
                };
            }

            return new Data()
            {
                gameFinished = false,
                //playerId = agentIdDictionary[playerGuid],
                playerId = agents.Where(q => q.GUID == playerGuid).First().ID,
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

            // player posseses a piece
            if (agents.Where(q => q.GUID == playerGuid).First().GetPiece != null)
            {
                // the TaskField is not claimed
                if (actualBoard.GetField(location.x, location.y) is GameArea.TaskField && (actualBoard.GetField(location.x, location.y) as GameArea.TaskField).GetPiece == null)
                {
                    var fieldMessage = new Messages.TaskField(location.x, location.y)
                    {
                        playerId = agents.Where(q => q.GUID == playerGuid).First().ID,
                        playerIdSpecified = true,
                        pieceId = agents.Where(q => q.GUID == playerGuid).First().GetPiece.id,
                        pieceIdSpecified = true,
                        timestamp = DateTime.Now
                    };

                    (actualBoard.GetField(location.x, location.y) as GameArea.TaskField).SetPiece(agents.Where(q => q.GUID == playerGuid).First().GetPiece); // the piece is put on the field
                    agents.Where(q => q.GUID == playerGuid).First().SetPiece(null); // the piece is no longer possesed by an agent

                    return new Data()
                    {
                        gameFinished = false,
                        playerId = agents.Where(q => q.GUID == playerGuid).First().ID,
                        TaskFields = new Messages.TaskField[] { fieldMessage }
                    };
                }
                // player carries a sham piece and is on a GoalField - he receives no data about a current GoalField and cannot place a piece
                else if (actualBoard.GetField(location.x, location.y) is GameArea.GoalField && agents.Where(q => q.GUID == playerGuid).First().GetPiece.type == PieceType.sham)
                {
                    var teamColour = agents.Where(q => q.GUID == playerGuid).First().GetTeam;
                    var fieldMessage = new Messages.GoalField()
                    {
                        x = location.x,
                        y = location.y,
                        playerId = agents.Where(q => q.GUID == playerGuid).First().ID,
                        playerIdSpecified = true,
                        timestamp = DateTime.Now,
                        team = teamColour
                    };
                    return new Data()
                    {
                        gameFinished = false,
                        playerId = agents.Where(q => q.GUID == playerGuid).First().ID,
                        GoalFields = new Messages.GoalField[] { fieldMessage }
                    };
                }
                // the goal field is a goal and player carries a normal piece
                else if (actualBoard.GetField(location.x, location.y) is GameArea.GoalField && (actualBoard.GetField(location.x, location.y) as GameArea.GoalField).GoalType == GoalFieldType.goal)
                {
                    var teamColour = agents.Where(q => q.GUID == playerGuid).First().GetTeam;
                    var goalFieldType = actualBoard.GetGoalField(location.x, location.y).GoalType;
                    var fieldMessage = new Messages.GoalField()
                    {
                        x = location.x,
                        y = location.y,
                        playerId = agents.Where(q => q.GUID == playerGuid).First().ID,
                        playerIdSpecified = true,
                        timestamp = DateTime.Now,
                        type = goalFieldType,
                        team = teamColour
                    };
                    goalsLeft--;    // one goal less before the game is over
                    agents.Where(q => q.GUID == playerGuid).First().SetPiece(null); // the piece is no longer possesed by an agent

                    return new Data()
                    {
                        gameFinished = false,
                        playerId = agents.Where(q => q.GUID == playerGuid).First().ID,
                        GoalFields = new Messages.GoalField[] { fieldMessage }
                    };
                }



            }
            // the field is task field and is claimed OR the field is goal field and is non-goal - action rejected
            // or the player doesn't posses a piece
            return new Data()
            {
                gameFinished = false,
                playerId = agents.Where(q => q.GUID == playerGuid).First().ID,
                PlayerLocation = agents.Where(q => q.GUID == playerGuid).First().GetLocation
                //GoalFields = new Messages.GoalField[] { null }
            };

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
            Piece pieceDataToSend = null;

            // the TaskField contains a piece
            if (actualBoard.GetField(location.x, location.y) is GameArea.TaskField && (actualBoard.GetField(location.x, location.y) as GameArea.TaskField).GetPiece != null)
            {
                pieceDataToSend = new Piece()
                {
                    type = PieceType.unknown,
                    id = (actualBoard.GetField(location.x, location.y) as GameArea.TaskField).GetPiece.id,
                    playerId = agents.Where(q => q.GUID == playerGuid).First().ID,
                    timestamp = DateTime.Now
                };

                var piece = (actualBoard.GetField(location.x, location.y) as GameArea.TaskField).GetPiece;
                agents.Where(q => q.GUID == playerGuid).First().SetPiece(piece); // agent picks up a piece
                (actualBoard.GetField(location.x, location.y) as GameArea.TaskField).SetPiece(null); // the piece is no longer on the field  
            }
            // player is either on an empty TaskField or on a GoalField
            return new Data()
            {
                gameFinished = false,
                playerId = agents.Where(q => q.GUID == playerGuid).First().ID,
                Pieces = new Piece[] { pieceDataToSend }
            };
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

            // perform location data and get future field
            var futureLocation = PerformLocationDelta(direction, currentLocation, team);
            var futureBoardField = actualBoard.GetField(futureLocation.x, futureLocation.y);

            //basic info for response
            Data response = new Data()
            {
                gameFinished = false,
                playerId = agents.Where(q => q.GUID == playerGuid).First().ID,
                TaskFields = new Messages.TaskField[] { },
                GoalFields = null,
                PlayerLocation = currentLocation
            };

            //player tried to step out of the board or enetr wrong GoalArea
            if (!ValidateFieldPosition((int)futureLocation.x, (int)futureLocation.y, team))
                return response;
            

            Messages.Piece piece;
            Messages.Field field;
            
            // what type of field are we trying to enter - Task or Goal?
            if (futureBoardField is GameArea.TaskField)
            {
                GameArea.TaskField fieldFromBoard = actualBoard.GetField(futureLocation.x, futureLocation.y) as GameArea.TaskField;
                field = new Messages.TaskField(futureLocation.x, futureLocation.y)
                {
                    distanceToPiece = 0,
                    timestamp = DateTime.Now,
                };

                // check if there is a piece on the filed
                fieldFromBoard = actualBoard.GetField(futureLocation.x, futureLocation.y) as GameArea.TaskField;
                piece = (fieldFromBoard as GameArea.TaskField).GetPiece;
                if(piece != null)
                {
                    response.Pieces = new Messages.Piece[] { piece };
                    (field as Messages.TaskField).pieceId = piece.id;
                    (field as Messages.TaskField).pieceIdSpecified = true;
                }
                response.TaskFields = new Messages.TaskField[] { (field as Messages.TaskField) };

            }
            else //if (futureBoardField is GameArea.GoalField)
            {
                GameArea.GoalField fieldFromBoard = actualBoard.GetField(futureLocation.x, futureLocation.y) as GameArea.GoalField;
                field = new Messages.GoalField(futureLocation.x, futureLocation.y, fieldFromBoard.GetOwner)
                {
                    timestamp = DateTime.Now,
                };
                response.GoalFields = new Messages.GoalField[] { (field as Messages.GoalField) };
                response.TaskFields = null;
            }

            // check if there is another agent on the field we're trying to enter - if so, wedon't actually move, just get an update on the field
            if (futureBoardField.HasAgent())
            {
                field.playerId = futureBoardField.Player.id;
                field.playerIdSpecified = true;
            }
            else    //there is no player, we can move
            {
                // perform move action
                var agent = actualBoard.GetField(currentLocation.x, currentLocation.y).Player;
                actualBoard.GetField(currentLocation.x, currentLocation.y).Player = null;
                actualBoard.GetField(futureLocation.x, futureLocation.y).Player = agent;
                response.PlayerLocation = futureLocation;
            }

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
            var location = agents.Where(a => a.GUID == playerGuid).First().GetLocation;
            var team = agents.Where(q => q.GUID == playerGuid).First().GetTeam;
            List<Messages.TaskField> TaskFieldList = new List<Messages.TaskField>();
            List<Messages.GoalField> GoalFieldList = new List<Messages.GoalField>();

            for (int dx = -1; dx <= 1; ++dx)
                for (int dy = -1; dy <= 1; ++dy)
                {
                    if (dx == 0 && dy == 0) continue;
                    if (ValidateFieldPosition((int)(location.x + dx), (int)(location.y + dy), team))
                    {
                        Field field = actualBoard.GetField((uint)(location.x + dx), (uint)(location.y + dy));
                        // discovered field is a TaskField - can contain players and pieces
                        if (field is TaskField)
                        {
                            //basic information
                            Messages.TaskField responseField = new Messages.TaskField(location.x, location.y)
                            {
                                x = (uint)(location.x + dx),
                                y = (uint)(location.y + dy),
                                timestamp = DateTime.Now,
                                distanceToPiece = 1
                            };

                            //anoter agent on the field
                            if (field.HasAgent())
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

                        // discovered field is a GoalField - can contain players
                        else if (field is GoalField)
                        {
                            Messages.GoalField responseField = new Messages.GoalField(location.x, location.y, (field as GoalField).GetOwner)
                            {
                                x = (uint)(location.x + dx),
                                y = (uint)(location.y + dy),
                                timestamp = DateTime.Now
                            };

                            if (field.HasAgent())
                            {
                                responseField.playerId = field.Player.id;
                                responseField.playerIdSpecified = true;
                            }
                            else
                                responseField.playerIdSpecified = false;

                            GoalFieldList.Add(responseField);
                        }
                    }
                }
            return new Data()
            {
                gameFinished = false,
                playerId = agents.Where(q => q.GUID == playerGuid).First().ID,
                TaskFields = TaskFieldList.ToArray(),
                GoalFields = GoalFieldList.ToArray()
            };
        }

        // additional methods

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

            if (!ValidateFieldPosition((int)currentLocation.x, (int)currentLocation.y, team))
                return currentLocation;
            else
                return new Messages.Location((uint)(currentLocation.x + dx), (uint)(currentLocation.y + dy));
        }

        /// <summary>
        /// Validates if an agent can move on a given field or disvover it
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
        public bool SetAbsoluteAgentLocation(uint x, uint y, string guid)
        {
            var team = agents.Where(q => q.GUID == guid).First().GetTeam;
            if (ValidateFieldPosition((int)x, (int)y, team))
            {
                agents.Where(q => q.GUID == guid).First().SetLocation(x, y);
                actualBoard.GetField(x, y).Player = agents.Where(q => q.GUID == guid).First().ConvertToMessageAgent();
                // rzutowanie wymuszone lekkim balaganem: w fieldzie jest typ Message.Agent, na liscie Player.Agent - laczymy ich po ID w razie potrzeby
                return true;
            }
            return false;
        }

        /// <summary>
        /// FOR UNIT TESTING - set a piece of a given type in a given location
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="type"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool SetPieceInLocation(uint x, uint y, TeamColour team, PieceType type, ulong id)
        {
            var piece = new Piece(type, id);
            if (ValidateFieldPosition((int)x, (int)y, team) && actualBoard.GetField(x, y) is TaskField)
            {
                (actualBoard.GetField(x, y) as TaskField).SetPiece(piece);
                return true;
            }
            else return false;
        }
    }
}
