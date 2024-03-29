﻿using System;
using System.Collections.Generic;
using System.Text;
using GameArea.Parsers;
using Messages;

namespace GameArea.GameObjects
{
    public class GoalField : Field,IToBase<Messages.GoalField>
    {
        public GoalFieldType Type { get; set; }
        public TeamColour Team { get; set; }
        public GoalField(Messages.GoalField field) : base(field)
        {
            Type = field.type;
            Team = field.team;
            Player = field.playerIdSpecified ? new Player(field.playerId) : null;
        }

        public GoalField(GoalField field) : base(field.ToBase())
        {
            Type = field.Type;
            Team = field.Team;
            if (field.Player != null)
                Player = new Player(field.Player);
        }

        public GoalField(int x, int y,DateTime timeStamp, TeamColour team, GoalFieldType type, Player player = null):base(x,y)
        {
            Type = type;
            Team = team;
            TimeStamp = timeStamp;
            Player = player;
        }

        public override FieldType GetFieldType => FieldType.Goal;

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

        public Messages.GoalField ToBase()
        {
            return new Messages.GoalField()
            {
                playerId = Player != null ? Player.ID : 0,
                playerIdSpecified = Player != null,
                team = Team,
                timestamp = TimeStamp,
                type = Type,
                x = (uint)X,
                y = (uint)Y
            };
        }

        public override string Serialize()
        {
            return MessageParser.Serialize(ToBase());
        }
    }
}
