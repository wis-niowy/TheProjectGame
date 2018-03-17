using System;
using System.Collections.Generic;
using System.Text;

namespace TheProjectGame.GameArea
{
    public enum Team { Red, Blue}
    public enum GoalType { Empty, Fullfilled, NotFullfilled, Unknown}
    public class GoalField : Field
    {
        private GoalType type;
        public GoalField(int x, int y, Team owner,GoalType goalType = GoalType.Unknown):base(x,y)
        {
            type = goalType;
            this.owner = owner;
        }

        public override FieldType GetFieldType()
        {
            return FieldType.Goal;
        }

        public GoalType GoalType
        {
            get
            {
                return type;
            }
            set
            {
                type = value;
            }
        }

        private Team owner;

        public Team Owner
        {
            get
            {
                return owner;
            }
        }
    }
}
