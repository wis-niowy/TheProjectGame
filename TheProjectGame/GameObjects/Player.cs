using System;
using System.Collections.Generic;
using System.Text;
using GameArea.Parsers;
using Messages;

namespace GameArea.GameObjects
{
    public class Player : IToBase<Messages.Player>
    {
        public TeamColour Team { get; set; }
        public PlayerRole Role { get; set; }
        public ulong ID { get; set; }
        public string GUID { get; set; }
        public Messages.Player ToBase()
        {
            return new Messages.Player()
            {
                id = ID,
                role = Role,
                team = Team
            };
        }

        public string Serialize()
        {
            return MessageParser.Serialize(ToBase());
        }

        public Player(ulong id, TeamColour team = TeamColour.blue, PlayerRole role = PlayerRole.member)
        {
            ID = id;
            Team = team;
            Role = role;
        }

        public Player(Messages.Player player)
        {
            ID = player.id;
            Role = player.role;
            Team = player.team;
        }
    }
}
