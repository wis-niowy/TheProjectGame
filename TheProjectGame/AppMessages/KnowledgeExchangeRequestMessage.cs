using Messages;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameArea.AppMessages
{
    public class KnowledgeExchangeRequestMessage : BetweenPlayersAbstractMessage, IToBase<KnowledgeExchangeRequest>
    {
        public KnowledgeExchangeRequestMessage(ulong id, ulong senderId) : base(id, senderId)
        {
        }

        KnowledgeExchangeRequest IToBase<KnowledgeExchangeRequest>.ToBase()
        {
            return new KnowledgeExchangeRequest()
            {
                playerId = PlayerId,
                senderPlayerId = SenderPlayerId
            };
        }
    }
}
