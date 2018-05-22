using GameArea;
using GameArea.AppMessages;
using System;
using System.Collections.Generic;
using System.Text;

namespace Player.PlayerMessages
{
    public class RejectKnowledgeExchangeAgent : RejectKnowledgeExchangeMessage, IAgentMessage
    {
        public RejectKnowledgeExchangeAgent(ulong id, ulong senderId, bool permanent, string playerGuid = null) : base(id, senderId, permanent, playerGuid)
        {
        }

        public string[] Process(IPlayerController controller)
        {
            controller.Player.HandleRejectKnowledgeExchange(this);
            return null;
        }
    }
}
