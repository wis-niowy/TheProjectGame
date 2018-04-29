using GameArea.AppMessages;
using GameArea.ControllerInterfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace CommunicationServer.ServerMessages
{
    public class GameMasterDisconnectedServer : GameMasterDisconnectedMessage, IServerMessage<IGMController>
    {
        public GameMasterDisconnectedServer(ulong id,ulong clientId) : base(id)
        {
            ClientId = clientId;
        }

        public ulong ClientId { get; }
        public void Process(IGMController controller)
        {
            controller.CloseGame();
        }
    }
}
