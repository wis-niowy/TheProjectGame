using GameArea;
using GameArea.AppMessages;
using GameArea.ControllerInterfaces;
using Messages;
using System;
using System.Collections.Generic;
using System.Text;

namespace CommunicationServer.ServerMessages
{
    public class MoveServer : MoveMessage, IMessage<IAgentController>
    {
        public MoveServer(string guid, ulong gameId, MoveType? move = null) : base(guid, gameId, move)
        {
        }

        public ulong ClientId { get;}

        public void Process(IAgentController controller)
        {
            controller.SendMessageToGameMaster(Serialize());
        }
    }
}
