using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CommunicationServer.ServerObjects
{
    public class MessageManager
    {
        int idMax = 0;
        NewClientInterpreter defaultInterpreter;
        ObjectsManager defaultManager;


        public MessageManager()
        {
            defaultManager = new ObjectsManager();
            defaultInterpreter = new NewClientInterpreter(defaultManager);
        }

        public bool AddClient(TcpClient client)
        {
            var newClient = new ClientHandle(client, idMax, defaultInterpreter);
            defaultManager.AddClient(newClient);
            idMax += 1;
            newClient.BeginRead();
            return true;
        }
    }

    public class ClientHandle
    {
        public int ID { get; set; } 
        TcpClient Client { get; }
        public IInterpreter MessageInterpreter { get; set; } //odpowiada za poprawny odczyt i wykonanie akcji na daną wiadomość

        public ClientHandle(TcpClient me, int id, IInterpreter interpreter)
        {
            Client = me;
            ID = id;
            MessageInterpreter = interpreter;
        }

        public void BeginRead()
        {
            var buffer = new byte[4096];
            var ns = Client.GetStream();
            ns.BeginRead(buffer, 0, buffer.Length, EndRead, buffer);
        }

        public void EndRead(IAsyncResult result)
        {
            var buffer = (byte[])result.AsyncState;
            var ns = Client.GetStream();
            var bytesAvailable = ns.EndRead(result);
            var message = Encoding.ASCII.GetString(buffer);
            message = message.Trim('\0');
            MessageInterpreter.ReadMessage(message, ID);

            Console.WriteLine("Read by Client " + ID + ": " +message);
            BeginRead();
        }

        public void BeginSend(string message)
        {
            message = message.Trim('\0');
            var bytes = Encoding.ASCII.GetBytes(message);
            var ns = Client.GetStream();
            ns.BeginWrite(bytes, 0, bytes.Length, EndSend, bytes);
        }

        public void EndSend(IAsyncResult result)
        {
            var bytes = (byte[])result.AsyncState;
            Console.WriteLine("Sent  {0} bytes to server by: " + ID, bytes.Length);
            Console.WriteLine("Sent: {0}", Encoding.ASCII.GetString(bytes).Trim('\0'));
        }
    }
}
