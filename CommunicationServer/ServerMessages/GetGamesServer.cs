using GameArea;
using GameArea.AppMessages;
using GameArea.ControllerInterfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace CommunicationServer.ServerMessages
{
    public class GetGamesServer : GetGamesMessage, IMessage<IMainController>
    {
        public GetGamesServer(ulong clientID)
        {
            ClientId = clientID;
        }
        public ulong ClientId { get; }

        public void Process(IMainController controller)
        {
            controller.GetGames(ClientId);
        }
    }
}
