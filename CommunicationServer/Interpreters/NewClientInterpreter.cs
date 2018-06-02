using CommunicationServer.ServerObjects;
using GameArea;
using GameArea.ControllerInterfaces;
using Messages;
using System;
using System.Collections.Generic;
using System.Text;

namespace CommunicationServer.Interpreters
{
    //czyta wiadomości nowego klienta i wykonuje związane z nimi zadania
    public class NewClientInterpreter : IInterpreter
    {
        MainController mainManager;

        public NewClientInterpreter(MainController manager)
        {
            mainManager = manager;
        }
        public void ReadMessage(string message,ulong clientId)
        {
            IMessage<IMainController> messageObject = null;
            if (message == "client disconnected")
                mainManager.RemoveClient(clientId);
            else
            {
                messageObject = ServerReader.GetObjectFromXML<IMainController>(message,clientId);//message must be without any \0 characters
                messageObject?.Process(mainManager);
            }
            ServerWriter.Show("Unknown Client: " + clientId + " sent message of type: " + messageObject?.GetType().Name);
        }
    }
}
