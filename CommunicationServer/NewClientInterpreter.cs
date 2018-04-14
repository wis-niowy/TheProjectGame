using CommunicationServer.ServerObjects;
using System;
using System.Collections.Generic;
using System.Text;

namespace CommunicationServer
{
    //czyta wiadomości nowego klienta i wykonuje związane z nimi zadania
    public class NewClientInterpreter : IInterpreter
    {
        ObjectsManager objectsManager;

        public NewClientInterpreter(ObjectsManager manager)
        {
            objectsManager = manager;
        }
        public void ReadMessage(string message,int clientId)
        {
            string[] args = message.Split(',');
            if (args.Length == 0)
                return;
            switch(args[0])
            {
                case "register":
                    objectsManager.RegisterGame(args[1], clientId);
                    break;
                case "join":
                    objectsManager.RegisterAgentForGame(args[1], clientId);
                    break;
                default:
                    objectsManager.SendToClient(clientId, "received from you: " + message.Trim('\0'));
                    break;
            }
        }

        


    }
}
