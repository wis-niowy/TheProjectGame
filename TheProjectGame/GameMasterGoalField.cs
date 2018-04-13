using System;
using System.Collections.Generic;
using System.Text;
using Messages;

namespace GameArea
{
    public class GameMasterGoalField : GoalField
    {
        public bool IsFullfilled { get; set; }
        public GameMasterGoalField(int x, int y, TeamColour owner, GoalFieldType goalType = GoalFieldType.unknown) : base(x, y, owner, goalType)
        {
        }
    }
}
