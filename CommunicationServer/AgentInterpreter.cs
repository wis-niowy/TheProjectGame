using CommunicationServer.ServerObjects;
using System;
using System.Collections.Generic;
using System.Text;

namespace CommunicationServer
{
    //interpreter wiadomości agenta, zakąłdamy że ten Agent jest już w grze (jedynie przeysła waidomości do GM)
    class AgentInterpreter : IInterpreter
    {
        IAgentController GameController { get; }
        public void ReadMessage(string message, int clientId)
        {
            GameController.SendMessageToGameMaster(message);
        }

        public AgentInterpreter(IAgentController controller)
        {
            GameController = controller;
        }
    }
}
