using Messages;
using Player;
using System;
using System.Collections.Generic;
using System.Linq;
using GameArea;
using Messages;
using Configuration;

namespace GameMaster
{
    public enum GameMasterState { AwaitingPlayers, GameInprogress, GameOver};
    public class GameMaster
    {
        private GameMasterState state;
        private ulong nextPieceId = 0;
        private Random random;
        private List<Player.Agent> agents;
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

        public GameMaster(GameMasterSettings settings)
        {
            state = GameMasterState.AwaitingPlayers;
            random = new Random();
            agents = new List<Player.Agent>();
            gameSettings = settings;
            InitBoard(gameSettings.GameDefinition);
        }

        private void InitBoard(GameMasterSettingsGameDefinition settings)
        {
            actualBoard = new Board(settings.BoardWidth, settings.TaskAreaLength, settings.GoalAreaLength);
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
            while (field.GetPiece != null);
            return field;
        }
    }
}
