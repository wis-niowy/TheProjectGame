using GameArea;
using GameArea.AppMessages;
using GameArea.ControllerInterfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace CommunicationServer.ServerMessages
{
    public class ErrorMessageServer : ErrorMessage,IMessage<IGMController>, IMessage<IAgentController>, IMessage<IMainController>
    {
        private XmlDocument xmlDoc;
        public ErrorMessageServer(string type, string message, string causeName, ulong clientId,XmlDocument document = null) : base(type, message, causeName)
        {
            ClientId = clientId;
            xmlDoc = document;
        }

        public ulong ClientId { get;}

        public void Process(IAgentController controller)
        {
            throw new NotImplementedException("Error message dla IAgentController");
        }

        public void Process(IGMController controller)
        {
            throw new NotImplementedException("Error message dla IGMController");
        }

        public void Process(IMainController controller)
        {
            throw new NotImplementedException("Error message dla IMainController");
        }
    }
}
