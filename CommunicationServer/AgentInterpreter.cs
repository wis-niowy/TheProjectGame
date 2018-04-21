using CommunicationServer.ServerObjects;
using GameArea;
using Messages;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace CommunicationServer
{
    //interpreter wiadomości agenta, zakąłdamy że ten Agent jest już w grze (jedynie przeysła waidomości do GM)
    class AgentInterpreter : IInterpreter
    {
        IAgentController GameController { get; }
        public void ReadMessage(string message, ulong clientId)
        {
            object messageObject = new object();
            if (message == "client disconnected")
                GameController.RemoveClientOrAgent(clientId);
            else
            {
                messageObject = MessageReader.GetObjectFromXML(message);//message must be without any \0 characters

                switch (messageObject.GetType().Name)
                {
                    case nameof(JoinGame):
                        GameController.SendMessageToGameMaster(message);
                        break;
                    case nameof(XmlDocument):
                        GameController.SendMessageToGameMaster(message);
                        break;
                    default:
                        ConsoleWriter.Warning("Unknown message received from Agent/Joiner client: " + clientId + "\nReceived message:\n" + message);
                        break;
                }
            }
            ConsoleWriter.Show("Agent/Joiner Client: " + clientId + " sent message of type: " + messageObject.GetType().Name);
        }

        public AgentInterpreter(IAgentController controller)
        {
            GameController = controller;
        }
    }
}
