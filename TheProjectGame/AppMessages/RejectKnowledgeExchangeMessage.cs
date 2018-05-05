using Messages;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameArea.AppMessages
{
    public class RejectKnowledgeExchangeMessage : BetweenPlayersAbstractMessage, IToBase<RejectKnowledgeExchange>
    {
        public bool Permanent { get; set; }
        public string PlayerGUID { get; set; }
        public RejectKnowledgeExchangeMessage(ulong id, ulong senderId, bool permanent, string playerGuid = null) : base(id, senderId)
        {
            Permanent = permanent;
            PlayerGUID = playerGuid;
        }

        RejectKnowledgeExchange IToBase<RejectKnowledgeExchange>.ToBase()
        {
            return new RejectKnowledgeExchange()
            {
                permanent = Permanent,
                playerGuid = PlayerGUID,
                playerId = PlayerId,
                senderPlayerId = SenderPlayerId
            };
        }
    }
}
