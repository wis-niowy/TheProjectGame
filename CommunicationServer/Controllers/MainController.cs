using CommunicationServer.Interpreters;
using CommunicationServer.ServerObjects;
using GameArea;
using GameArea.AppMessages;
using GameArea.ControllerInterfaces;
using Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace CommunicationServer
{
    public class MainController:IMainController
    {
        Dictionary<string, GameController> gameDefinitions;
        List<IClientHandle> clients;

        public MainController()
        {
            gameDefinitions = new Dictionary<string, GameController>();
            clients = new List<IClientHandle>();
        }

        public bool RegisterGame(string name, ulong red, ulong blue, ulong clientId)
        {
            if(GameAvaiable(name))
            {
                var gameInfo = new GameArea.GameObjects.GameInfo(name, red, blue);
                var controller = new GameController(gameInfo, GetNewGameId(),this);
                var client = clients.Where(q => q.ID == clientId).FirstOrDefault();
                clients.Remove(client);
                client.MessageInterpreter = new GMInterpreter(controller);
                var GM = new GM(client);
                controller.SetGM(GM);
                gameDefinitions.Add(name, controller);
                ConsoleWriter.Show("Successful registration for game: " + name);
                controller.SendMessageToGameMaster((new ConfirmGameRegistrationMessage(controller.gameId)).Serialize());
                return true;
            }
            else
            {
                SendToClient(clientId, (new RejectGameRegistrationMessage(name)).Serialize());
            }
            ConsoleWriter.Show("Game not registered (possible that already exists): " + name);
            return false;
        }

        public void JoinGame(string name, TeamColour team, PlayerRole role, ulong clientId)
        {
            var game = gameDefinitions.Where(q => q.Value.GameInfo.GameName == name).Select(q=>q.Value).FirstOrDefault();
            if(game == null)
            {
                SendToClient(clientId, (new RejectJoiningGameMessage(name, clientId).Serialize()));
            }
            else
            {
                var client = RemoveClient(clientId);
                game.AddClient(client, new JoinGameMessage(name,team,role,(long)clientId));
            }
        }

        public void GetGames(ulong clientId)
        {
            SendToClient(clientId, GetGamesMessage());
        }

        private string GetGamesMessage()
        {
            var games = new List<GameArea.GameObjects.GameInfo>();
            foreach(var game in gameDefinitions.Where(q=>q.Value.State == GameState.New))
            {
                games.Add(game.Value.GameInfo);
            }
            var message = new RegisteredGamesMessage(games.ToArray());
            return message.Serialize();
        }

        internal IClientHandle RemoveClient(ulong clientId)
        {
            var client = clients.Where(q => q.ID == clientId).FirstOrDefault();
            if(client != null)
            {
                clients.Remove(client);
            }
            return client;
        }

        private bool GameAvaiable(string name)
        {
            return !gameDefinitions.Where(q => q.Key == name).Any();
        }

        public void SendToClient(ulong clientId, string message)
        {
            var client = clients.Where(q => q.ID == clientId).FirstOrDefault();
            if(client != null && client.IsAlive)
            {
                client.BeginSend(message);
            }
        }

        public void AddClient(IClientHandle client)
        {
            clients.Add(client);
        }

        private ulong GetNewGameId()
        {
            var ids =  gameDefinitions.Select(q => q.Value.gameId).OrderBy(q=>q).ToList();
            if (ids.Count == 0)
                return 0;
            ulong newId = ids[0]+1;
            for(int i=1;i<ids.Count;i++)
            {
                if (newId < ids[i])
                    break;
                else
                    newId += 1;
            }
            return newId;
        }

        public void DoCleaning()
        {
            var gamesToDelete = gameDefinitions.Where(q => q.Value.State == GameState.Ended).Select(q => q.Key).ToList();
            foreach (var game in gamesToDelete)
            {
                var gameObj = gameDefinitions[game];
                if(gameObj != null)
                {
                    foreach (var agent in gameObj.Agents)
                        if (agent.Client.IsAlive)
                        {
                            agent.Client.MessageInterpreter = new NewClientInterpreter(this);
                            clients.Add(agent.Client);
                        }
                    if (gameObj.GameMaster != null && gameObj.GameMaster.Client.IsAlive)
                    {
                        gameObj.GameMaster.Client.MessageInterpreter = new NewClientInterpreter(this);
                        clients.Add(gameObj.GameMaster.Client);
                    }
                    foreach (var client in gameObj.JoiningAgents)
                        if (client.IsAlive)
                        {
                            client.MessageInterpreter = new NewClientInterpreter(this);
                            clients.Add(client);
                        }
                }
                gameDefinitions.Remove(game);
            }
        }

        public ulong GetNewClientId()
        {
            DoCleaning();
            var newClientId = 0ul;
            var clientIds = GetAllClientIds();
            if(clientIds.Any())
            {
                foreach(var id in clientIds)
                {
                    if (newClientId < id)
                        break;
                    else
                        newClientId = id + 1;
                }
            }

            return newClientId;
        }

        private IEnumerable<ulong> GetAllClientIds()
        {
            var clientIds = clients.Select(q => q.ID);
            foreach(var game in gameDefinitions.Select(q=>q.Value))
            {
                clientIds = clientIds.Union(game.Agents.Select(q => q.Client.ID));
                clientIds = clientIds.Union(new List<ulong>() { game.GameMaster.Client.ID });
            }

            return clientIds.Distinct().OrderBy(q=>q);
            

        }

        /// <summary>
        /// Test method
        /// </summary>
        /// <param name="name"></param>
        /// <returns>Game Controller for a given game's name</returns>
        public GameController GetGameControllerByName(string name)
        {
            return gameDefinitions[name];
        }
        /// <summary>
        /// Test method
        /// </summary>
        /// <returns></returns>
        public List<IClientHandle> GetClientsList()
        {
            return clients;
        }

        /// <summary>
        /// Test method
        /// </summary>
        /// <param name="client"></param>
        public void InsertClient(IClientHandle client)
        {
            clients.Add(client);
        }
    }
}
