using GameArea;
using GameArea.AppMessages;
using GameArea.ControllerInterfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace CommunicationServer.ServerMessages
{
    public class AuthorizeKnowledgeExchangeServer : AuthorizeKnowledgeExchangeMessage, IMessage<IAgentController>
    {
        public AuthorizeKnowledgeExchangeServer(string guid, ulong gameId, ulong withPlayerId) : base(guid, gameId, withPlayerId)
        {
        }

        public ulong ClientId { get; }

        public void Process(IAgentController controller)
        {
            controller.SendMessageToGameMaster(Serialize());
        }
    }
}
