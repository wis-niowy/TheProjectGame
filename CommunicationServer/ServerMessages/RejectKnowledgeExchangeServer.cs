using GameArea;
using GameArea.AppMessages;
using GameArea.ControllerInterfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace CommunicationServer.ServerMessages
{
    public class RejectKnowledgeExchangeServer : RejectKnowledgeExchangeMessage, IMessage<IAgentController>, IMessage<IGMController>
    {
        public RejectKnowledgeExchangeServer(ulong id, ulong senderId, bool permanent, ulong clientId, string playerGuid = null) : base(id, senderId, permanent, playerGuid)
        {
            ClientId = clientId;
        }

        public ulong ClientId { get; }

        public void Process(IAgentController controller)
        {
            controller.SendMessageToGameMaster(Serialize());
        }

        public void Process(IGMController controller)
        {
            controller.SendMessageToAgent(PlayerId, Serialize());
        }
    }
}
