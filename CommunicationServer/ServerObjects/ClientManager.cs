using CommunicationServer.Interpreters;
using GameArea;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CommunicationServer.ServerObjects
{
    public class ClientManager
    {
        NewClientInterpreter defaultInterpreter;
        public MainController defaultController;


        public ClientManager()
        {
            defaultController = new MainController();
            defaultInterpreter = new NewClientInterpreter(defaultController);
        }

        public bool AddClient(TcpClient client)
        {
            var newClient = new ClientHandle(client, defaultController.GetNewClientId(), defaultInterpreter);
            defaultController.AddClient(newClient);
            newClient.BeginRead();
            return true;
        }
    }

    
}
