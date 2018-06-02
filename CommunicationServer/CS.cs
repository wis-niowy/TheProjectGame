using CommunicationServer.ServerObjects;
using GameArea;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Threading;

namespace CommunicationServer
{
    public class CS
    {
        public class TcpHelper
        {
            private static TcpListener listener { get; set; }
            private static bool accept { get; set; } = false;

            public static ClientManager manager; // only for test purposes

            public static void StartServer(IPAddress address, int port)
            {
                listener = new TcpListener(address, port);

                listener.Start();
                accept = true;

                ServerWriter.Show($"Server started. Listening to TCP clients at " + address + ":" + port);
            }

            private static volatile bool work = true;
            public static void Listen()
            {
                manager = new ClientManager();
                if (listener != null && accept)
                {
                    // Continue listening.  
                    while (work)
                    {
                        ServerWriter.Show("Waiting for client...");
                        try
                        {
                            var clientTask = listener.AcceptTcpClientAsync(); // Get the client  

                            if (clientTask.Result != null)
                            {
                                ServerWriter.Show("Client connected. Waiting for data.");
                                var client = clientTask.Result;

                                manager.AddClient(client);
                            }
                        }
                        catch (Exception e)
                        {
                            continue;
                        }
                    }
                }
            }

            public static void StopListening(Thread t)
            {
                work = false;
                t.Interrupt();
            }
        }
        public static void Main(string[] args)
        {
            IPAddress IP;
            int port;
            int year;
            string lang;
            var valid = ValidateArgs(args, out port, out IP, out year, out lang);
            if (!valid)
            {
                ServerWriter.Warning("Invalid args, loading default!");
                lang = "en";
                year = 18;
                IP = IPAddress.Parse("127.0.0.1");
                port = 5678;
                ServerWriter.StateOnly = true;
            }
            TcpHelper.StartServer(IP, port);
            TcpHelper.Listen(); // Start listening. 
        }

        private static bool ValidateArgs(string[] args, out int portNumber, out IPAddress serverAddress, out int year, out string lang)
        {
            bool validAddress = false;
            bool validPort = false;
            bool validBase = false;
            lang = "en";
            year = 18;
            serverAddress = IPAddress.Parse("127.0.0.1");
            portNumber = 5678;
            if (args.Length < 5)
            {
                ServerWriter.Error("Args too short: YY-LANG-XX-cs --port [portnumber] --address [server IPv4 address or IPv6 address or host name] --logStateOnly [1|0]");
                return false;
            }
            int portId = 0, addressId = 0, baseId = 0;
            try
            {
                for (int i = 0; i < args.Length; i++)
                {
                    switch (args[i])
                    {
                        case "--port":
                            portId = ++i;
                            portNumber = int.Parse(args[i]);
                            if (portNumber >= 0)
                                validPort = true;
                            else
                                portNumber = -1;
                            break;
                        case "--address":
                            addressId = ++i;
                            serverAddress = Dns.GetHostEntry(args[i]).AddressList[0];
                            if (serverAddress != null)
                                validAddress = true;
                            break;
                        case "--logStateOnly":
                            ServerWriter.StateOnly = args[++i] == "1" ? true : false;
                            break;
                        default:
                            baseId = i;
                            if (Regex.IsMatch(args[i], "[0-9]{2}-LANG-(pl|en)-cs"))
                            {
                                var baseArgs = args[i].Split('-');
                                year = int.Parse(baseArgs[0]);
                                lang = baseArgs[2];
                                validBase = true;
                            }
                            break;
                    }
                }
            }
            catch (Exception e)
            {
                ServerWriter.Error("Error during reading args: \n" + e.Message);
                return false;
            }
            if (!validAddress)
                ServerWriter.Error("IP adress is not valid: " + args[addressId]);
            if (!validPort)
                ServerWriter.Error("Socket number not valid: " + args[portId]);
            if (!validBase)
                ServerWriter.Error("Invalid year and language param: " + args[baseId]);
            return validAddress && validPort && validBase;
        }
    }
}
