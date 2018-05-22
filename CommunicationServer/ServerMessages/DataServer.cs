using GameArea;
using GameArea.AppMessages;
using GameArea.ControllerInterfaces;
using Messages;
using System;
using System.Collections.Generic;
using System.Text;

namespace CommunicationServer.ServerMessages
{
    public class DataServer : DataMessage, IMessage<IGMController>,IMessage<IAgentController>, IMessage<IMainController>
    {
        [Obsolete("Zamienić na konstruktor z parametrami.")]
        public DataServer(Data data, ulong clientId) : base(data)
        {
            ClientId = clientId;
        }

        public ulong ClientId { get; }

        public void Process(IGMController controller)
        {
            controller.SendMessageToAgent(PlayerId, Serialize());
        }

        public void Process(IAgentController controller)
        {
            controller.SendMessageToGameMaster(Serialize());
        }

        public void Process(IMainController controller)
        {
            controller.SendToClient(ClientId, Serialize());
        }
    }
}
