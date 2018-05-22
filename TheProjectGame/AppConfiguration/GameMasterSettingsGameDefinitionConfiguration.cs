using System;
using System.Collections.Generic;
using System.Text;
using Configuration;
using System.Linq;

namespace GameArea.AppConfiguration
{
    public class GameMasterSettingsGameDefinitionConfiguration : IToBase<Configuration.GameMasterSettingsGameDefinition>
    {
        public GameObjects.GoalField[] Goals { get; set; }
        public double ShamProbability { get; set; }
        public int PlacingNewPiecesFrequency { get; set; }
        public int InitialNumberOfPieces{ get; set; }
        public int BoardWidth { get; set; }
        public int TaskAreaLength { get; set; }
        public int GoalAreaLength { get; set; }
        public int NumberOfPlayersPerTeam { get; set; }
        public string GameName { get; set; }
        public int NumberOfGoalsPerGame { get; set; }

        public GameMasterSettingsGameDefinitionConfiguration(Configuration.GameMasterSettingsGameDefinition set = null)
        {
            if (set == null)
                set = Configuration.GameMasterSettingsGameDefinition.GetDefaultGameDefinition();

            Goals = set.Goals?.Select(q => new GameObjects.GoalField(q)).ToArray();
            ShamProbability = set.ShamProbability;
            PlacingNewPiecesFrequency = (int)set.PlacingNewPiecesFrequency;
            InitialNumberOfPieces = (int)set.InitialNumberOfPieces;
            BoardWidth = (int)set.BoardWidth;
            TaskAreaLength = (int)set.TaskAreaLength;
            GoalAreaLength = (int)set.GoalAreaLength;
            NumberOfPlayersPerTeam = (int)set.NumberOfPlayersPerTeam;
            GameName = set.GameName;
            NumberOfGoalsPerGame = set.NumberOfGoalsPerGame;
        }


        public string Serialize()
        {
            return MessageParser.Serialize(ToBase());
        }

        public GameMasterSettingsGameDefinition ToBase()
        {
            return new GameMasterSettingsGameDefinition()
            {
                Goals = Goals?.Select(q => new Messages.GoalField((uint)q.X, (uint)q.Y, q.Team, q.Type)).ToArray(),
                ShamProbability = ShamProbability,
                PlacingNewPiecesFrequency = (uint)PlacingNewPiecesFrequency,
                InitialNumberOfPieces = (uint)InitialNumberOfPieces,
                BoardWidth = (uint)BoardWidth,
                TaskAreaLength = (uint)TaskAreaLength,
                GoalAreaLength = (uint)GoalAreaLength,
                NumberOfPlayersPerTeam = (uint)NumberOfPlayersPerTeam,
                GameName = GameName,
                NumberOfGoalsPerGame = NumberOfGoalsPerGame,
            };
        }
    }
}
