using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace CommunicationServer.ServerObjects
{
    public class GM
    {
        public DateTime TimeStamp { get; set; }
        public ClientHandle Client { get; }
        public GM(ClientHandle client)
        {
            Client = client;
        }

        internal void SendMessage(string message)
        {
            Client.BeginSend(message);
        }
    }
}
