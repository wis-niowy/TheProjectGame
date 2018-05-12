using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace CommunicationServer.ServerObjects
{
    public class AGENT
    {
        public ulong PlayerId { get { return Client.ID; } }
        public DateTime TimeStamp { get; set; }

        public IClientHandle Client { get; }

        public AGENT(IClientHandle client)
        {
            Client = client;
        }

        internal void SendMessage(string message)
        {
            Client.BeginSend(message);
        }
    }
}
