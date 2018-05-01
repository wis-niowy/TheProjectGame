using GameArea;
using GameArea.AppMessages;
using GameArea.ControllerInterfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace CommunicationServer.ServerMessages
{
    public class TestPieceServer : TestPieceMessage, IMessage<IAgentController>
    {
        public TestPieceServer(string guid, ulong gameId) : base(guid, gameId)
        {
        }

        public ulong ClientId => throw new NotImplementedException();

        public void Process(IAgentController controller)
        {
            controller.SendMessageToGameMaster(Serialize());
        }
    }
}
