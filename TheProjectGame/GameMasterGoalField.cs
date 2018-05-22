using System;
using System.Collections.Generic;
using System.Text;
using Messages;

namespace GameArea
{
    public class GameMasterGoalField : GameObjects.GoalField
    {
        public bool IsFullfilled { get; set; }
        public GameMasterGoalField(int x, int y,DateTime timeStamp, TeamColour owner, GoalFieldType goalType = GoalFieldType.unknown) : base(x, y, timeStamp, owner, goalType)
        {
        }
        public override string ToString()
        {
            StringBuilder value = new StringBuilder();
            if (Player != null)
            {
                value.Append(Player.Team == TeamColour.red ? "[R" : "[B");
            }
            else
                value.Append("[ ");
            switch (Type)
            {
                case GoalFieldType.goal:
                    if(IsFullfilled)
                        value.Append("FG");
                    else
                        value.Append("G ");
                    break;
                case GoalFieldType.nongoal:
                    value.Append("NG");
                    break;
                case GoalFieldType.unknown:
                    value.Append("UK");
                    break;
            }
            value.Append("]");
            return value.ToString();
        }
    }
}
