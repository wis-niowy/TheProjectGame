using GameArea;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace CommunicationServer.ServerObjects
{
    public class ClientHandle: IClientHandle
    {
        public DateTime LastKeepAlive {get;set;}
        public ulong ID { get; set; }
        TcpClient Client { get; }
        public IInterpreter MessageInterpreter { get; set; } //odpowiada za poprawny odczyt i wykonanie akcji na daną wiadomość
        public bool IsAlive { get { return Client != null ? Client.Connected : false; } }
        public ClientHandle(TcpClient me, ulong id, IInterpreter interpreter)
        {
            Client = me;
            ID = id;
            MessageInterpreter = interpreter;
            LastKeepAlive = DateTime.Now;
        }

        public void BeginRead()
        {
            var buffer = new byte[25000];
            var ns = Client.GetStream();
            ns.BeginRead(buffer, 0, buffer.Length, EndRead, buffer);
        }

        public void EndRead(IAsyncResult result)
        {
            var buffer = (byte[])result.AsyncState;
            if (Client.Connected)
            {
                try
                {
                    var ns = Client.GetStream();
                    var bytesAvailable = ns.EndRead(result);
                    var messages = Encoding.ASCII.GetString(buffer).Split((char)23);
                    LastKeepAlive = DateTime.Now;
                    if (messages != null)
                    {
                        Task.Run(() =>
                        {
                            foreach (var message in messages.Select(q => q.Trim('\0')))
                            {
                                ConsoleWriter.Show("Server read from client:" + ID + ", \n" + message + "\n");
                                MessageInterpreter.ReadMessage(message, ID);
                            }
                        });
                    }
                    BeginRead();
                }
                catch (Exception e)
                {
                    ConsoleWriter.Error("Error while handling message from socket for client: " + ID + "\n Error message: \n" + e.ToString());
                    MessageInterpreter.ReadMessage("client disconnected", ID);
                }
            }
            else
            {
                ConsoleWriter.Warning("Client disconnected: " + ID);
                MessageInterpreter.ReadMessage("client disconnected", ID);
            }
        }

        public void BeginSend(string message)
        {
            message = message.Trim('\0');
            var bytes = Encoding.ASCII.GetBytes(message + (char)23);
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
