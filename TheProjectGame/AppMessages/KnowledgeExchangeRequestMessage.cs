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
        public override string Serialize()
        {
            KnowledgeExchangeRequest exch = new KnowledgeExchangeRequest()
            {
                playerId = PlayerId,
                senderPlayerId = SenderPlayerId
            };
            return MessageParser.Serialize(exch);
        }

        //nie nadpisuje nulla z BetweenPlayersAbstractMesssage!!!
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
