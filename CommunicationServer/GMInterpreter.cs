using CommunicationServer.ServerObjects;
using GameArea;
using Messages;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace CommunicationServer
{
    public class GMInterpreter : IInterpreter
    {
        IGMController GameController { get; }
        public void ReadMessage(string message, ulong clientId)
        {
            object messageObject = new object();
            if (message == "client disconnected")
                GameController.CloseGame();
            else
            {
                messageObject = MessageReader.GetObjectFromXML(message);//message must be without any \0 characters

                switch (messageObject.GetType().Name)
                {
                    case nameof(ConfirmJoiningGame):
                        GameController.RegisterClientAsAgent((ConfirmJoiningGame)messageObject);
                        break;
                    case nameof(RejectJoiningGame):
                        GameController.RejectJoin((RejectJoiningGame)messageObject);
                        break;
                    case nameof(Data):
                        GameController.DataSend((Data)messageObject);
                        break;
                    case nameof(GameStarted):
                        GameController.BeginGame();
                        break;
                    default:
                        ConsoleWriter.Warning("Unknown message received from GM with clientId: " + clientId + "\nReceived message:\n" + message);
                        break;
                }
            }
            ConsoleWriter.Show("GM Client: " + clientId + " sent message of type: " + messageObject.GetType().Name);
        }

        public GMInterpreter(IGMController controller)
        {
            GameController = controller;
        }
    }
}
