using Messages;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameArea.AppMessages
{
    public class BetweenPlayersAbstractMessage : PlayerMessage, IToBase<BetweenPlayersMessage>
    {
        public ulong SenderPlayerId { get; set; }
        public BetweenPlayersAbstractMessage(ulong playerId, ulong senderId) : base(playerId)
        {
            SenderPlayerId = senderId;
        }

        public override string Serialize()
        {
            return MessageParser.Serialize(ToBase());
        }

        public BetweenPlayersMessage ToBase()
        {
            return null;
        }
    }
}
