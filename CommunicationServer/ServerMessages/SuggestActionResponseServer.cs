using GameArea;
using GameArea.AppMessages;
using GameArea.ControllerInterfaces;
using GameArea.GameObjects;
using System;
using System.Collections.Generic;
using System.Text;

namespace CommunicationServer.ServerMessages
{
    public class SuggestActionResponseServer : SuggestActionResponseMessage, IMessage<IAgentController>, IMessage<IGMController>
    {
        public SuggestActionResponseServer(ulong id, ulong senderId, string guid, ulong gameId, ulong clientId, TaskField[] tasks = null, GoalField[] goals = null) : base(id, senderId, guid, gameId, tasks, goals)
        {
            ClientId = clientId;
        }

        public ulong ClientId { get; }

        public void Process(IAgentController controller)
        {
            controller.SendMessageToGameMaster(Serialize());
        }

        public void Process(IGMController controller)
        {
            controller.SendMessageToAgent(PlayerId, Serialize());
        }
    }
}
