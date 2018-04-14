using CommunicationServer.ServerObjects;
using System;
using System.Collections.Generic;
using System.Text;

namespace CommunicationServer
{
    public class GMInterpreter : IInterpreter
    {
        IGMController GameController { get; }
        public void ReadMessage(string message, int clientId)
        {
            var args = message.Split(',');
            switch(args[0])
            {
                case "accept":
                    GameController.RegisterClientAsAgent(args[1], uint.Parse(args[1]));
                    break;
                case "decline":
                    GameController.SendMessageToClient(args[0]);
                    break;
                default:
                    GameController.SendMessageToAgent(uint.Parse(args[0]), args[1]);
                    break;
            }
        }

        public GMInterpreter(IGMController controller)
        {
            GameController = controller;
        }
    }
}
