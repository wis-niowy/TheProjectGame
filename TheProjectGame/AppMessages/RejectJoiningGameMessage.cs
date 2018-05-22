using Messages;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameArea.AppMessages
{
    public class RejectJoiningGameMessage : PlayerMessage, IToBase<RejectJoiningGame>
    {
        public string GameName { get; set; }

        public RejectJoiningGameMessage(string name, ulong playerId):base(playerId)
        {
            GameName = name;
        }
        public RejectJoiningGameMessage(RejectJoiningGame reject):this(reject.gameName,reject.playerId) { }
        public override string Serialize()
        {
            return MessageParser.Serialize(ToBase());
        }

        public RejectJoiningGame ToBase()
        {
            return new RejectJoiningGame()
            {
                gameName = GameName
            };
        }
    }
}
