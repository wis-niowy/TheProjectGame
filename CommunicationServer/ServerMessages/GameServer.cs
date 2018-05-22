using GameArea;
using GameArea.AppMessages;
using GameArea.ControllerInterfaces;
using Messages;
using System;
using System.Collections.Generic;
using System.Text;

namespace CommunicationServer.ServerMessages
{
    public class GameServer : GameArea.AppMessages.GameMessage, IMessage<IGMController>
    {
        [Obsolete("Zmienić na konstruktor z parametrami")]
        public GameServer(Game game,ulong clientId) : base(game)
        {
            ClientId = clientId;
        }

        public ulong ClientId { get; }
        public void Process(IGMController controller)
        {
            controller.SendMessageToAgent(PlayerId, Serialize());
        }
    }
}
