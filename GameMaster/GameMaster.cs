using Messages;
using Player;
using System;
using System.Collections.Generic;
using System.Linq;
using GameArea;
using Configuration;
using System.Threading;
using System.Timers;

namespace GameArea
{
    public enum GameMasterState { AwaitingPlayers, GameInprogress, GameOver };
    public partial class GameMaster : IGameMaster
    {
        static readonly object lockObject = new object();
        private GameMasterState state;
        private ulong nextPieceId = 0;
        private ulong goalsRedLeft;
        private ulong goalsBlueLeft;
        private Random random;
        private List<Player.Agent> agents;
        //private List<Piece> pieces;
        private Dictionary<string, ulong> agentIdDictionary;
        private Board actualBoard;
        private GameMasterSettings gameSettings;
        private System.Timers.Timer pieceAdder;

        public ulong GoalsRedLeft
        {
            get
            {
                return goalsRedLeft;
            }
        }
        public ulong GoalsBlueLeft
        {
            get
            {
                return goalsBlueLeft;
            }
        }

        public bool IsGameFinished
        {
            get
            {
                return state == GameMasterState.GameOver;
            }
        }
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

        public GameMasterSettingsActionCosts GetCosts
        {
            get
            {
                return gameSettings.ActionCosts;
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="agent"></param>
        /// <param name="guid"></param>
        /// <param name="findFreeLocationAndPlacePlayer">Ustaw na false aby Game Master nie przydzielal znalezionego przez siebie miejsca i nie ustawial gracza na pozycji</param>
        public void RegisterAgent(Player.Agent agent, string guid = null, bool findFreeLocationAndPlacePlayer = true)
        {
            var newId = agents.Count > 0 ? agents.Max(q => q.ID) + 1 : 1;
            agent.ID = newId;
            agent.SetGuid(guid != null ? guid : "Agent" + newId);
            agent.SetBoard(new Board(GetGameDefinition.BoardWidth, GetGameDefinition.TaskAreaLength, GetGameDefinition.GoalAreaLength));
            if (findFreeLocationAndPlacePlayer)
            {
                var playerField = GetEmptyFieldForPlayer(agent.GetTeam);
                agent.SetLocation(playerField);
                playerField.Player = new Messages.Agent()
                {
                    id = agent.ID,
                    team = agent.GetTeam
                };
            }
            agents.Add(new Player.Agent(agent));
            ConsoleWriter.Show("Registered agent with params: GUID: " + agent.GUID + ", ID: " + agent.ID + " , Location: " + agent.GetLocation + ", Team: " + agent.GetTeam);

            if (agents.Count == 2 * GetGameDefinition.NumberOfPlayersPerTeam)
            {
                state = GameMasterState.GameInprogress;
                PrintBoardState();
            }
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
            goalsRedLeft = (ulong)settings.GameDefinition.Goals.Where(q => q.team == TeamColour.red).Count();
            goalsBlueLeft = (ulong)settings.GameDefinition.Goals.Where(q => q.team == TeamColour.blue).Count();
            agentIdDictionary = new Dictionary<string, ulong>();
            gameSettings = settings;
            InitBoard(gameSettings.GameDefinition);
            InitPieceAdder();
        }

        private void InitPieceAdder()
        {
            pieceAdder = new System.Timers.Timer(GetGameDefinition.PlacingNewPiecesFrequency);
            pieceAdder.Elapsed += PieceAdder_Elapsed;
            pieceAdder.AutoReset = true;
            pieceAdder.Start();
        }

        private void InitBoard(GameMasterSettingsGameDefinition settings)
        {
            actualBoard = new Board(settings.BoardWidth, settings.TaskAreaLength, settings.GoalAreaLength,GoalFieldType.nongoal);
            PlaceInitialPieces(settings.InitialNumberOfPieces);
            PlaceInitialGoals(settings.Goals);
        }

        private void PlaceInitialGoals(Messages.GoalField[] goals)
        {
            foreach (var goal in goals)
            {
                actualBoard.SetGoalField(new GameArea.GameMasterGoalField(goal.x, goal.y, goal.team, goal.type));
            }
        }


        /// <summary>
        /// Places a piece in TaskArea in a random way
        /// </summary>
        /// <param name="piecesCount"></param>
        private void PlaceInitialPieces(int piecesCount)
        {
            for (int i = 0; i < piecesCount; i++)
            {
                var piece = CreatePiece();
                var field = GetEmptyFieldForPiece();
                field.SetPiece(piece);
            }
        }

        private bool PlaceNewPiece()
        {
            var piece = CreatePiece();
            var field = GetEmptyFieldForPiece();
            if (field != null)
            {
                field.SetPiece(piece);
                UpdateDistancesFromAllPieces();
                return true;
            }
            else
            {
                return false;
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
            int x, y;
            GameArea.TaskField field;
            if (actualBoard.TaskFields.Where(q => q.GetPiece != null).Count() == (int)(actualBoard.BoardWidth * actualBoard.TaskAreaHeight))
                return null;
            do
            {
                y = random.Next() % actualBoard.TaskAreaHeight + actualBoard.GoalAreaHeight;
                x = random.Next() % actualBoard.BoardWidth;
                field = actualBoard.GetTaskField(x, y);
            }
            while (field == null || field.GetPiece != null);
            return field;
        }

        private GameArea.TaskField GetEmptyFieldForPlayer(TeamColour team)
        {
            int x = 0, y = team == TeamColour.blue ? GetGameDefinition.GoalAreaLength : GetGameDefinition.GoalAreaLength + GetGameDefinition.TaskAreaLength - 1;
            GameArea.TaskField field = actualBoard.GetTaskField(x, y);
            while (field == null || field.Player != null)
            {
                x++;
                if (x / GetGameDefinition.BoardWidth == 1)
                {
                    x = 0;
                    y = team == TeamColour.blue ? y + 1 : y - 1;
                }
                field = actualBoard.GetTaskField(x, y);
            }
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

        public void UpdateDistancesFromAllPieces()
        {
            List<GameArea.TaskField> taskFields = actualBoard.TaskFields;
            foreach (var field in taskFields)
            {
                //if there is a piece on this field - distance = 0
                if (field.GetPiece != null)
                {
                    field.Distance = 0;
                    continue;
                }
                else    //no piece -> check if there is a piece 1, 2, 3... fields away
                {
                    int currentDist = 1;
                    int x, y;
                    while (currentDist < field.Distance && currentDist < taskFields.Count)
                    {
                        for (x = 0; x <= currentDist; x++)
                        {
                            y = currentDist - x;


                            if (CheckIfNotOutOfTaskArea(field.x + x, field.y + y) && ((actualBoard.GetField(field.x + x, field.y + y)) as TaskField).GetPiece != null)
                            {
                                field.Distance = currentDist;
                                break;
                            }//first quarter
                            if (CheckIfNotOutOfTaskArea(field.x - x, field.y + y) && ((actualBoard.GetField(field.x - x, field.y + y)) as TaskField).GetPiece != null)
                            {
                                field.Distance = currentDist;
                                break;
                            }//second quarter
                            if (CheckIfNotOutOfTaskArea(field.x + x, field.y - y) && ((actualBoard.GetField(field.x + x, field.y - y)) as TaskField).GetPiece != null)
                            {
                                field.Distance = currentDist;
                                break;
                            }
                            if (CheckIfNotOutOfTaskArea(field.x - x, field.y - y) && ((actualBoard.GetField(field.x - x, field.y - y)) as TaskField).GetPiece != null)
                            {
                                field.Distance = currentDist;
                                break;
                            }//fourth quarter

                        }
                        currentDist++;
                    }

                }
            }
        }

        private bool CheckIfNotOutOfTaskArea(int x, int y)
        {
            if (x < 0 || x >= actualBoard.BoardWidth ||
                y < actualBoard.GoalAreaHeight || y >= actualBoard.GoalAreaHeight + actualBoard.TaskAreaHeight)
                return false;
            return true;
        }

        
    }


}
