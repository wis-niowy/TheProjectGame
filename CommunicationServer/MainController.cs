using CommunicationServer.ServerObjects;
using GameArea;
using Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace CommunicationServer
{
    public class MainController
    {
        Dictionary<string, GameController> gameDefinitions;
        List<ClientHandle> clients;

        public MainController()
        {
            gameDefinitions = new Dictionary<string, GameController>();
            clients = new List<ClientHandle>();
        }

        public bool RegisterGame(RegisterGame game, ulong clientId)
        {
            if(GameAvaiable(game.NewGameInfo.gameName))
            {
                var controller = new GameController(game.NewGameInfo, GetNewGameId(),this);
                var client = clients.Where(q => q.ID == clientId).FirstOrDefault();
                clients.Remove(client);
                client.MessageInterpreter = new GMInterpreter(controller);
                var GM = new GM(client);
                controller.SetGM(GM);
                gameDefinitions.Add(game.NewGameInfo.gameName, controller);
                ConsoleWriter.Show("Successful registration for game: " + game.NewGameInfo.gameName);
                controller.SendMessageToGameMaster(GameRegisterConfirmation(controller));
                return true;
            }
            else
            {
                SendToClient(clientId, GameRegisterRejection(game.NewGameInfo.gameName));
            }
            ConsoleWriter.Show("Game not registered (possible that already exists): " + game.NewGameInfo.gameName);
            return false;
        }

        private string GameRegisterRejection(string name)
        {
            return MessageReader.Serialize(new RejectGameRegistration() { gameName = name });
        }

        internal void JoinGame(JoinGame message, ulong clientId)
        {
            var game = gameDefinitions.Where(q => q.Value.GameInfo.gameName == message.gameName).Select(q=>q.Value).FirstOrDefault();
            if(game == null)
            {
                SendToClient(clientId, RejectJoiningGameMessage(message.gameName));
            }
            else
            {
                var client = RemoveClient(clientId);
                game.AddClient(client, message);
            }
        }

        private string RejectJoiningGameMessage(string name)
        {
            var obj = new RejectJoiningGame()
            {
                gameName = name
            };
            return MessageReader.Serialize(obj);
        }

        internal void GetGames(ulong clientId)
        {
            SendToClient(clientId, GetGamesMessage());
        }

        private string GetGamesMessage()
        {
            var games = new List<GameInfo>();
            foreach(var game in gameDefinitions.Where(q=>q.Value.State == GameState.New))
            {
                games.Add(game.Value.GameInfo);
            }
            var obj = new RegisteredGames()
            {
                GameInfo = games.ToArray()
            };
            return MessageReader.Serialize(obj);
        }

        private string GameRegisterConfirmation(GameController controller)
        {
            var stringwriter = new System.IO.StringWriter();
            var serializer = new XmlSerializer(typeof(ConfirmGameRegistration));
            serializer.Serialize(stringwriter,new ConfirmGameRegistration()
            {
                gameId = controller.gameId
            });
            return stringwriter.ToString();
        }

        internal ClientHandle RemoveClient(ulong clientId)
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
            if(client != null)
            {
                client.BeginSend(message);
            }
        }

        public void AddClient(ClientHandle client)
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
                            clients.Add(agent.Client);
                    if (gameObj.GameMaster != null && gameObj.GameMaster.Client.IsAlive)
                        clients.Add(gameObj.GameMaster.Client);
                    foreach (var client in gameObj.JoiningAgents)
                        if (client.IsAlive)
                            clients.Add(client);
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
    }
}
