using Messages;
using Player;
using System;
using System.Collections.Generic;
using System.Linq;
using GameArea;
using Messages;
using Configuration;

namespace GameArea
{
    public enum GameMasterState { AwaitingPlayers, GameInprogress, GameOver};
    public class GameMaster: IGameMaster
    {
        private GameMasterState state;
        private ulong nextPieceId = 0;
        private Random random;
        private List<Player.Agent> agents;
        //private List<Piece> pieces;
        private Dictionary<ulong, uint> agentIdDictionary;
        private Board actualBoard;
        private GameMasterSettingsGameDefinition gameSettings;

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

        public GameMasterSettingsGameDefinition GetSettings
        {
            get
            {
                return gameSettings;
            }
        }


        public List<Player.Agent> GetAgentsByTeam(Messages.TeamColour team)
        {
            return agents.Where(q => q.GetTeam == team).ToList();
        }

        public Player.Agent GetAgentByGuid(ulong guid)
        {
            return agents.Where(q => q.GUID == guid).FirstOrDefault();
        }

        public void RegisterAgent(Player.Agent agent)
        {
            var newGuid = agents.Max(q => q.GUID) + 1;
            agent.SetGuid(newGuid);
            agents.Add(agent);
        }

        public Board GetBoard
        {
            get
            {
                return actualBoard;
            }
        }

        public GameMaster(GameMasterSettingsGameDefinition settings)
        {
            state = GameMasterState.AwaitingPlayers;
            random = new Random();
            agents = new List<Player.Agent>();
            //pieces = new List<Piece>();
            agentIdDictionary = new Dictionary<ulong, uint>();
            gameSettings = settings;
            InitBoard(gameSettings);
        }

        private void InitBoard(GameMasterSettingsGameDefinition settings)
        {
            actualBoard = new Board(uint.Parse(settings.BoardWidth), uint.Parse(settings.TaskAreaLength), uint.Parse(settings.GoalAreaLength));
            PlaceInitialPieces(settings.InitialNumberOfPieces);
            PlaceInitialGoals(settings.Goals);
        }

        private void PlaceInitialGoals(Messages.GoalField[] goals)
        {
            foreach(var goal in goals)
            {
                actualBoard.SetGoalField(new GameArea.GoalField(goal.x,goal.y,goal.team,goal.type));
            }
        }

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
            if (possibleSham <= gameSettings.ShamProbability)
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
            while (field.GetPiece != null);
            return field;
        }

        // API

        /// <summary>
        /// Method to request a Test Piece action
        /// </summary>
        /// <param name="playerGuid">guid of player requesting an action</param>
        /// <param name="gameId">id of current game</param>
        /// <returns></returns>
        public Data HandleTestPieceRequest(ulong playerGuid, ulong gameId)
        {
            Piece pieceDataToSend = new Piece()
            {
                type = agents.Where(q => q.GUID == playerGuid).First().GetPiece.type,
                id = agents.Where(q => q.GUID == playerGuid).First().GetPiece.id,
                playerId = agentIdDictionary[playerGuid],
                timestamp = DateTime.Now
            };
            
            return new Data()
            {
                gameFinished = false,
                playerId = agentIdDictionary[playerGuid],
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
        public Data HandlePlacePieceRequest(ulong playerGuid, ulong gameId)
        {
            // DODAC PRZYPADEK, GDY POLE JEST ZAJETE

            uint x = agents.Where(q => q.GUID == playerGuid).First().GetLocation.x;
            uint y = agents.Where(q => q.GUID == playerGuid).First().GetLocation.y;
            TeamColour teamColour = agents.Where(q => q.GUID == playerGuid).First().GetTeam;
            GoalFieldType goalFieldType = actualBoard.GetGoalField(x ,y).GoalType;
            Messages.GoalField goalField = new Messages.GoalField()
            {
                x = x,
                y = y,
                playerId = agentIdDictionary[playerGuid],
                timestamp = DateTime.Now,
                type = goalFieldType,
                team = teamColour
                
            };

            return new Data()
            {
                gameFinished = false,
                playerId = agentIdDictionary[playerGuid],
                GoalFields = new Messages.GoalField[] {}
            };
        }

        /// <summary>
        /// Method to request a Pick Up Piece action
        /// </summary>
        /// <param name="playerGuid">guid of player requesting an action</param>
        /// <param name="gameId">id of current game</param>
        /// <returns></returns>
        public Data HandlePickUpPieceRequest(ulong playerGuid, ulong gameId)
        {
            Piece pieceDataToSend = new Piece()
            {
                type = PieceType.unknown,
                id = agents.Where(q => q.GUID == playerGuid).First().GetPiece.id,
                playerId = agentIdDictionary[playerGuid],
                timestamp = DateTime.Now
            };

            return new Data()
            {
                gameFinished = false,
                playerId = agentIdDictionary[playerGuid],
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
        public Data HandleMoveRequest(MoveType direction, ulong playerGuid, ulong gameId)
        {
            var currentLocation = agents.Where(a => a.GUID == playerGuid).First().GetLocation;
            var futureLocation = PerformLocationDelta(direction, currentLocation);

            return new Data()
            {
                gameFinished = false,
                playerId = agentIdDictionary[playerGuid],
                TaskFields = new Messages.TaskField[] {},
                PlayerLocation = 
            };
        }

        /// <summary>
        /// Method to request a Discover action
        /// </summary>
        /// <param name="playerGuid">guid of player requesting an action</param>
        /// <param name="gameId">id of current game</param>
        /// <returns></returns>
        public Data HandleDiscoverRequest(ulong playerGuid, ulong gameId)
        {
            throw new NotImplementedException();
        }

        // additional methods

        /// <summary>
        /// Converts MoveType enum object to Location object
        /// </summary>
        /// <returns></returns>
        private Messages.Location PerformLocationDelta(MoveType moveType, Messages.Location currentLocation)
        {
            // is MoveUp the same for red and blue team? or if for red Up is +1 for blue should be -1 on OY???
            int dx = 0, dy = 0;
            
            switch(moveType)
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
            
        }
    }
}
