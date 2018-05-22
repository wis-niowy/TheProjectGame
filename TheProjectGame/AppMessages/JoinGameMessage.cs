using Messages;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameArea.AppMessages
{
    public class JoinGameMessage : IToBase<JoinGame>
    {
        public string GameName { get; set; }
        public TeamColour PrefferedTeam { get; set; }
        public PlayerRole PrefferedRole { get; set; }
        public long PlayerId { get; set; }
        public string Serialize()
        {
            return MessageParser.Serialize(ToBase());
        }

        public JoinGameMessage(JoinGame join) : this(join.gameName, join.preferredTeam, join.preferredRole, (long)join.playerId) { }

        public JoinGameMessage(string name, TeamColour team, PlayerRole role, long playerId = -1)
        {
            GameName = name;
            PrefferedTeam = team;
            PrefferedRole = role;
            PlayerId = playerId;
        }

        public JoinGame ToBase()
        {
            return new JoinGame()
            {
                gameName = GameName,
                playerId = (uint)PlayerId,
                playerIdSpecified = PlayerId > -1,
                preferredRole = PrefferedRole,
                preferredTeam = PrefferedTeam
            };
        }
    }
}
