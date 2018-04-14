using CommunicationServer.ServerObjects;
using GameArea;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommunicationServer
{
    public class ObjectsManager
    {
        Dictionary<string, GameController> gameDefinitions;
        List<ClientHandle> clients;

        public ObjectsManager()
        {
            gameDefinitions = new Dictionary<string, GameController>();
            clients = new List<ClientHandle>();
        }

        public bool RegisterGame(string name, int clientID)
        {
            if(GameAvaiable(name))
            {
                var controller = new GameController();
                var client = clients.Where(q => q.ID == clientID).FirstOrDefault();
                client.MessageInterpreter = new GMInterpreter(controller);
                var GM = new GM(client);
                controller.SetGM(GM);
                gameDefinitions.Add(name, controller);
                ConsoleWriter.Show("Successful registration for game: " + name);
                return true;
            }
            ConsoleWriter.Show("Game not registered (possible that already exists): " + name);
            return false;
        }

        internal void RegisterAgentForGame(string gameName, int clientId)
        {
            var client = clients.Where(q => q.ID == clientId).FirstOrDefault();
            var game = gameDefinitions.Where(q => q.Key == gameName).Select(q => q.Value).FirstOrDefault();
            if (game == null)
                client.BeginSend("game not found");
            else
            {
                clients.Remove(client);
                game.AddClient(client, "join");
            }

        }

        private bool GameAvaiable(string name)
        {
            return !gameDefinitions.Where(q => q.Key == name).Any();
        }

        public void SendToClient(int clientId, string message)
        {
            var client = clients.Where(q => q.ID == clientId).FirstOrDefault();
            if(client != null)
            {
                client.BeginSend(message);
            }
        }

        public void AddClient(ClientHandle client)
        {
            clients.Add(client);
        }
    }
}
