using GameArea.AppMessages;
using GameArea.ControllerInterfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace CommunicationServer.ServerMessages
{
    public class RejectJoiningGameServer :RejectJoiningGameMessage, IServerMessage<IGMController>
    {
        public RejectJoiningGameServer(string name, ulong clientId) : base(name, clientId)
        {
            ClientId = clientId;
        }

        public ulong ClientId { get; }

        public void Process(IGMController controller)
        {
            controller.SendMessageToClient(PlayerId, Serialize());
        }
    }
}
