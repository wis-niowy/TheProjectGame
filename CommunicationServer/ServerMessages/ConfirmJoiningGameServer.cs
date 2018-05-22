using GameArea;
using GameArea.AppMessages;
using GameArea.ControllerInterfaces;
using GameArea.GameObjects;
using System;
using System.Collections.Generic;
using System.Text;

namespace CommunicationServer.ServerMessages
{
    public class ConfirmJoiningGameServer : ConfirmJoiningGameMessage, IMessage<IGMController>
    {
        public ConfirmJoiningGameServer(ulong gameId, Player playerDef, string guid, ulong playerId,ulong clientId) : base(gameId, playerDef, guid, playerId)
        {
            ClientId = clientId;
        }

        public ulong ClientId { get; }

        public void Process(IGMController controller)
        {
            controller.RegisterClientAsAgent(PlayerId, Serialize());
        }
    }
}
