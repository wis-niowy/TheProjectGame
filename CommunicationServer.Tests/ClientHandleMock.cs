using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using CommunicationServer.ServerObjects;

namespace CommunicationServer.Tests
{
    public class ClientHandleMock: IClientHandle
    {
        public DateTime LastKeepAlive { get; set; }
        public ulong ID { get; set; }
        TcpClient Client { get; }
        public IInterpreter MessageInterpreter { get; set; } //odpowiada za poprawny odczyt i wykonanie akcji na daną wiadomość
        public bool IsAlive { get { return Client != null ? Client.Connected : true; } }

        public ClientHandleMock(TcpClient me, ulong id, IInterpreter interpreter)
        {
            Client = me;
            ID = id;
            MessageInterpreter = interpreter;
            LastKeepAlive = DateTime.Now;
        }

        public void BeginRead()
        {
            
        }

        public void EndRead(IAsyncResult result)
        {
            
        }

        public void BeginSend(string message)
        {
            
        }

        public void EndSend(IAsyncResult result)
        {
            
        }
    }
}
