using GameArea;
using GameArea.AppMessages;
using GameArea.ControllerInterfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace CommunicationServer.ServerMessages
{
    public class RegisterGameServer : RegisterGameMessage, IMessage<IMainController>, IMessage<IGMController>
    {
        public RegisterGameServer(string name, ulong red, ulong blue, ulong clientId):base(name,red,blue)
        {
            ClientId = clientId;
        }

        public ulong ClientId { get; }

        public void Process(IMainController controller)
        {
            controller.RegisterGame(NewGameInfo.GameName, NewGameInfo.RedTeamPlayers, NewGameInfo.BlueTeamPlayers, ClientId);
        }

        public void Process(IGMController controller)
        {
            controller.GameFinished(NewGameInfo.GameName,NewGameInfo.RedTeamPlayers,NewGameInfo.BlueTeamPlayers,ClientId);
        }
    }
}
