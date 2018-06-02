using CommunicationServer.ServerObjects;
using GameArea;
using GameArea.ControllerInterfaces;
using Messages;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace CommunicationServer.Interpreters
{
    public class GMInterpreter : IInterpreter
    {
        IGMController GameController { get; }
        public void ReadMessage(string message, ulong clientId)
        {
            IMessage<IGMController> messageObject = null;
            if (message == "client disconnected")
                GameController.CloseGame();
            else
            {
                messageObject = ServerReader.GetObjectFromXML<IGMController>(message,clientId);//message must be without any \0 characters
                messageObject?.Process(GameController);
            }
            ServerWriter.Show("GM Client: " + clientId + " sent message of type: " + messageObject?.GetType().Name);
        }

        public GMInterpreter(IGMController controller)
        {
            GameController = controller;
        }
    }
}
