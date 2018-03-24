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

        public override FieldType GetFieldType
        {
            get
            {
                return FieldType.Goal;
            }
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

        public TeamColour GetOwner
        {
            get
            {
                return owner;
            }
        }

        public Messages.GoalField ConvertToMessageGoalField()
        {
            ulong pId = 0;
            if (this.Player != null)
                pId = this.Player.id;

            return new Messages.GoalField()
            {
                x = this.x,
                y = this.y,
                playerIdSpecified = this.Player != null,
                playerId = pId,
                team = this.GetOwner,
                timestamp = this.TimeStamp,
                type = this.GoalType,
            };
        }
    }
}
