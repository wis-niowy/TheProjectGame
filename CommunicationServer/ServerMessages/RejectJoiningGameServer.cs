using GameArea;
using GameArea.AppMessages;
using GameArea.ControllerInterfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace CommunicationServer.ServerMessages
{
    public class RejectJoiningGameServer :RejectJoiningGameMessage, IMessage<IGMController>
    {
        public RejectJoiningGameServer(string name, ulong clientId) : base(name, clientId)
        {
            ClientId = clientId;
        }

        public ulong ClientId { get; }

        public void Process(IGMController controller)
        {
            controller.SendMessageToClient(PlayerId, Serialize());
            controller.PrintServerState("GM rejects joining game:" + GameName + " for client with ID: " + ClientId);
        }
    }
}
