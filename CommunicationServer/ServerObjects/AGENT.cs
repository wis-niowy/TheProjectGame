using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace CommunicationServer.ServerObjects
{
    public class AGENT
    {
        public ulong PlayerId { get; }
        public DateTime TimeStamp { get; set; }

        public ClientHandle Client { get; }

        public AGENT(ClientHandle client, ulong playerId)
        {
            Client = client;
            PlayerId = playerId;
        }

        internal void SendMessage(string message)
        {
            Client.BeginSend(message);
        }
    }
}
