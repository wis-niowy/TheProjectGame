using CommunicationServer.ServerObjects;
using GameArea;
using Messages;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using GameArea.ControllerInterfaces;

namespace CommunicationServer.Interpreters
{
    //interpreter wiadomości agenta, zakąłdamy że ten Agent jest już w grze (jedynie przeysła waidomości do GM)
    class AgentInterpreter : IInterpreter
    {
        IAgentController GameController { get; }
        public void ReadMessage(string message, ulong clientId)
        {
            IServerMessage<IAgentController> messageObject = null;
            if (message == "client disconnected")
                GameController.RemoveClientOrAgent(clientId);
            else
            {
                messageObject = ServerReader.GetObjectFromXML<IAgentController>(message, clientId);//message must be without any \0 characters
                messageObject.Process(GameController);
            }
            ConsoleWriter.Show("Agent/Joiner Client: " + clientId + " sent message of type: " + messageObject.GetType().Name);
        }

        public AgentInterpreter(IAgentController controller)
        {
            GameController = controller;
        }
    }
}
