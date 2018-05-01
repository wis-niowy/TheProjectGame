using GameArea;
using GameArea.AppMessages;
using GameArea.ControllerInterfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace CommunicationServer.ServerMessages
{
    public class DestroyPieceServer : DestroyPieceMessage, IMessage<IAgentController>
    {
        public DestroyPieceServer(string guid, ulong gameId) : base(guid, gameId)
        {
        }

        public ulong ClientId { get;}

        public void Process(IAgentController controller)
        {
            controller.SendMessageToGameMaster(Serialize());
        }
    }
}
