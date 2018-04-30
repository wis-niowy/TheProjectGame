using GameArea;
using GameArea.AppMessages;
using GameArea.ControllerInterfaces;
using Messages;
using System;
using System.Collections.Generic;
using System.Text;

namespace CommunicationServer.ServerMessages
{
    public class JoinGameServer : JoinGameMessage, IMessage<IMainController>
    {
        public ulong ClientId { get;}

        public JoinGameServer(string name, TeamColour team, PlayerRole role, ulong clientId, long playerId = -1) : base(name, team, role, playerId)
        {
            ClientId = clientId;
        }

        public void Process(IMainController controller)
        {
            controller.JoinGame(GameName,PrefferedTeam,PrefferedRole,ClientId);
        }
    }
}
