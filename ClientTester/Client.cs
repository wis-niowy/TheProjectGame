using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ClientTester
{
    class Client
    {
        public static TcpClient client;
        static void Main(string[] args)
        {
            IPAddress address = null;
            int port = -1;
            while (address == null)
            {
                Console.WriteLine("Give me IP:");
                var ip = Console.ReadLine();
                address = IPAddress.Parse(ip);
                if (address == null)
                    Console.WriteLine("Invalid IP address!!!");
            }

            while (port <= 0)
            {
                Console.WriteLine("Give me IP:");
                var parsed = int.TryParse(Console.ReadLine(),out port);
                if (!parsed || port <=0)
                    Console.WriteLine("Invalid port number!!!");
            }

            client = new TcpClient(address.ToString(), port);
            if(client == null)
            {
                Console.WriteLine("No connection to host");
                return;
            }
            NetworkStream stream = client.GetStream();


            BeginRead();
            string input = null;

            do
            {
                input = Console.ReadLine();
                Byte[] dataToSend = System.Text.Encoding.ASCII.GetBytes(input);
                stream.Write(dataToSend, 0, dataToSend.Length);
            }
            while (input != "quit");
        }

        public static void BeginRead()
        {
            var buffer = new byte[4096];
            var ns = client.GetStream();
            ns.BeginRead(buffer, 0, buffer.Length, EndRead, buffer);
        }

        public static void EndRead(IAsyncResult result)
        {
            var buffer = (byte[])result.AsyncState;
            var ns = client.GetStream();
            var bytesAvailable = ns.EndRead(result);
            var message = Encoding.ASCII.GetString(buffer);
            message = message.Trim('\0');

            Console.WriteLine(message);
            BeginRead();
        }
    }


}
