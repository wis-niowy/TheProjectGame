using Messages;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameArea.AppMessages
{
    public class AcceptExchangeRequestMessage : BetweenPlayersAbstractMessage, IToBase<AcceptExchangeRequest>
    {
        public AcceptExchangeRequestMessage(ulong id, ulong senderId) : base(id, senderId)
        {
        }

        AcceptExchangeRequest IToBase<AcceptExchangeRequest>.ToBase()
        {
            return new AcceptExchangeRequest()
            {
                playerId = PlayerId,
                senderPlayerId = SenderPlayerId
            };
        }
    }
}
