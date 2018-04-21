using CommunicationServer.ServerObjects;
using GameArea;
using Messages;
using System;
using System.Collections.Generic;
using System.Text;

namespace CommunicationServer
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
            object messageObject = new object();
            if (message == "client disconnected")
                mainManager.RemoveClient(clientId);
            else
            {
                messageObject = MessageReader.GetObjectFromXML(message);//message must be without any \0 characters

                switch (messageObject.GetType().Name)
                {
                    case nameof(RegisterGame):
                        mainManager.RegisterGame((RegisterGame)messageObject, clientId);
                        break;
                    case nameof(GetGames):
                        mainManager.GetGames(clientId);
                        break;
                    case nameof(JoinGame):
                        mainManager.JoinGame((JoinGame)messageObject, clientId);
                        break;
                    default:
                        ConsoleWriter.Warning("Unknown message received from Unknown with clientId: " + clientId + "\nReceived message:\n" + message);
                        break;
                }
            }
            ConsoleWriter.Show("Unknown Client: " + clientId + " sent message of type: " + messageObject.GetType().Name);
        }
    }
}
