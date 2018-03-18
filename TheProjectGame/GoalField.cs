using Messages;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameArea
{
    public class GoalField : Field
    {
        private GoalFieldType type;
        public GoalField(uint x, uint y, TeamColour owner, GoalFieldType goalType = GoalFieldType.unknown) :base(x,y)
        {
            type = goalType;
            this.owner = owner;
        }

        public override FieldType GetFieldType()
        {
            return FieldType.Goal;
        }

        public GoalFieldType GoalType
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

        private TeamColour owner;

        public TeamColour Owner
        {
            get
            {
                return owner;
            }
        }
    }
}
