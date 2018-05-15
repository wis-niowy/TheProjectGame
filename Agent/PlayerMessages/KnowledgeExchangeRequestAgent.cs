using GameArea;
using GameArea.AppMessages;
using System;
using System.Collections.Generic;
using System.Text;

namespace Player.PlayerMessages
{
    public class KnowledgeExchangeRequestAgent : KnowledgeExchangeRequestMessage, IAgentMessage
    {
        public KnowledgeExchangeRequestAgent(ulong id, ulong senderId) : base(id, senderId)
        {
        }

        public string[] Process(IPlayer player)
        {
            return new string[] { player.HandleKnowledgeExchangeRequest(this)?.Serialize() };
        }
    }
}