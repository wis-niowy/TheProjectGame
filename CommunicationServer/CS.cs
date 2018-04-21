using CommunicationServer.ServerObjects;
using GameArea;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace CommunicationServer
{
    class CS
    {
        class TcpHelper
        {
            private static TcpListener listener { get; set; }
            private static bool accept { get; set; } = false;

            public static void StartServer(IPAddress address, int port)
            {
                listener = new TcpListener(address, port);

                listener.Start();
                accept = true;

                Console.WriteLine($"Server started. Listening to TCP clients at "+ address + ":"+port);
            }

            public static void Listen()
            {
                ClientManager manager = new ClientManager();
                if (listener != null && accept)
                {
                    // Continue listening.  
                    while (true)
                    {
                        Console.WriteLine("Waiting for client...");
                        var clientTask = listener.AcceptTcpClientAsync(); // Get the client  

                        if (clientTask.Result != null)
                        {
                            
                            Console.WriteLine("Client connected. Waiting for data.");
                            var client = clientTask.Result;

                            manager.AddClient(client);

                            //string message = "";

                            //while (message != null && !message.StartsWith("quit"))
                            //{
                            //    byte[] data = Encoding.ASCII.GetBytes("Send next data: [enter 'quit' to terminate] ");
                            //    NetworkStream nwStream = client.GetStream();
                            //    nwStream.Write(data, 0, data.Length);

                                
                            //    byte[] buffer = new byte[client.ReceiveBufferSize];
                            //    nwStream.Read(buffer, 0, client.ReceiveBufferSize);


                            //    message = Encoding.ASCII.GetString(buffer);
                            //    Console.WriteLine(message);
                            //}
                            //Console.WriteLine("Closing connection.");
                            //client.GetStream().Dispose();
                        }
                    }
                }
            }
        }
        static void Main(string[] args)
        {
            var valid = ValidateArgs(args);
            if (!valid)
                ConsoleWriter.Warning("Invalid args, laoding default!");
            IPAddress IP;
            int socket;
            if (valid)
            {
                IP = IPAddress.Parse(args[0]);
                socket = Int32.Parse(args[1]);
            }
            else
            {
                IP = IPAddress.Parse("127.0.0.1");
                socket = Int32.Parse("5678");
            }
            TcpHelper.StartServer(IP, socket);
            TcpHelper.Listen(); // Start listening. 
        }

        private static bool ValidateArgs(string[] args)
        {
            var valid = true;
            if (args.Length < 2)
            {
                ConsoleWriter.Error("Args too short, must be provided: IP adress, socket number");
                return false;
            }
            var ip = IPAddress.Parse(args[0]);
            if (ip == null)
            {
                ConsoleWriter.Error("IP adress is not valid: " + args[0]);
                valid = false;
            }
            var socket = Int32.Parse(args[1]);
            if(socket <=0)
            {
                ConsoleWriter.Error("Socket number not valid: " + args[1]);
                valid = false;
            }
            return valid;
        }
    }
}
