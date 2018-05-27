using GameArea;
using GameArea.AppMessages;
using GameArea.ControllerInterfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace CommunicationServer.ServerMessages
{
    class EmptyMessage : IMessage<IMainController>, IMessage<IAgentController>, IMessage<IGMController>
    {
        public ulong ClientId { get; }

        public EmptyMessage(ulong clientId)
        {
            ClientId = clientId;
        }

        public void Process(IGMController controller)
        {
            controller.SendKeepAliveToGM();
        }

        public void Process(IAgentController controller)
        {
            controller.SendKeepAlive(ClientId);
        }

        public void Process(IMainController controller)
        {
            controller.SendKeepAlive(ClientId);
        }
    }
}
