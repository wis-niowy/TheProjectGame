using GameArea;
using GameArea.AppMessages;
using GameArea.ControllerInterfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace CommunicationServer.ServerMessages
{
    public class AcceptExchangeRequestServer : AcceptExchangeRequestMessage, IMessage<IAgentController>
    {
        public AcceptExchangeRequestServer(ulong id, ulong senderId) : base(id, senderId)
        {
        }

        public ulong ClientId { get; }

        public void Process(IAgentController controller)
        {
            controller.SendMessageToGameMaster(Serialize());
        }
    }
}
