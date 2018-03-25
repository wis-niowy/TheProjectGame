using System;
using System.Collections.Generic;
using System.Text;

namespace MainApp
{
    public static class ValidatorMessages
    {
        public const string INVALID_SHAM_PROBABILITY = "Sham probability not valid";
        public const string INVALID_PIECES_FREQUENCY = "Piece frequency not valid";
        public const string INVALID_INITIAL_NUMBER = "Initial number of pieces not valid";
        public const string INVALID_BOARD_WIDTH = "Board width not valid";
        public const string INVALID_TASK_AREA_LENGTH = "Task area length not valid";
        public const string INVALID_GOAL_AREA_LENGTH = "Goal area length not valid";
        public const string NUMBER_OF_PLAYERS_EQUALS_ZERO = "Numbers of players per team equals zero";
        public const string NUMBER_OF_PLAYERS_TOO_BIG = "Numbers of players per team exceeds task area";
        public const string INVALID_GAMENAME = "Game name empty or consist only white spaces";
        public const string GOALS_ARE_NOT_UNIQUE = "At least two goals at the same position";
        public const string BLUE_TEAM_HAS_NO_GOAL = "Blue team has no goal";
        public const string RED_TEAM_HAS_NO_GOAL = "Red team has no goal";
        public const string GOALS_OUTSIDE_GOAL_AREA = "Goals outside goal area";
        public const string RED_GOAL_IN_BLUE_GOAL_AREA = "Red goal in blue goal area";
        public const string BLUE_GOAL_IN_RED_GOAL_AREA = "Blue goal in red goal area";
        public const string NULL_GOALFIELD_ARRAY = "Array is null";
        public const string NULL_GOALFIELD = "Goalfields array has null element";
        public const string ACTION_COSTS_NULL = "Action costs are null";
        public const string NUMBER_OF_GOALS_ZERO_OR_LESS = "Number of goal is zero or less";
        public const string NUMBER_OF_RED_GOALS_NOT_EQUAL_GOALS_NUMBER_PER_GAME = "Number of red goals is not equal to number of goals per game";
        public const string NUMBER_OF_BLUE_GOALS_NOT_EQUAL_GOALS_NUMBER_PER_GAME = "Number of blue goals is not equal to number of goals per game";
    }
}
