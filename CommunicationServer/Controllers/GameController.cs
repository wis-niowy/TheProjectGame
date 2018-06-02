using CommunicationServer.Interpreters;
using GameArea.AppMessages;
using GameArea.ControllerInterfaces;
using GameArea.GameObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommunicationServer.ServerObjects
{
    public enum GameState { New, InProgress, Ended };
    public class GameController : IGMController, IAgentController
    {
        public ulong gameId { get; }
        public GM GameMaster { get; set; }
        public List<AGENT> Agents { get; set; }
        public List<IClientHandle> JoiningAgents { get; set; }
        public GameInfo GameInfo { get; set; }
        public GameState State { get; set; }
        private MainController mainController;
        public GameController(GameInfo newGameInfo, ulong id, MainController controller)
        {
            Agents = new List<AGENT>();
            JoiningAgents = new List<IClientHandle>();
            GameInfo = newGameInfo;
            gameId = id;
            State = GameState.New;
            mainController = controller;
        }

        public bool SendMessageToAgent(ulong playerId, string message)
        {
            var agent = Agents.Where(q => q.PlayerId == playerId).FirstOrDefault();
            if (agent != null && agent.Client != null && agent.Client.IsAlive)
            {
                agent.SendMessage(message);
                return true;
            }
            return false;
        }

        public void SendMessageToGameMaster(string message)
        {
            if(GameMaster.Client != null && GameMaster.Client.IsAlive)
                GameMaster.SendMessage(message);
            else
            {
                CloseGame();
            }
        }

        public void SetGM(GM gm)
        {
            GameMaster = gm;
        }

        public void RegisterClientAsAgent(ulong clientId, string message)
        {
            var client = JoiningAgents.Where(q => q.ID == clientId).FirstOrDefault();
            JoiningAgents.Remove(client);

            client.MessageInterpreter = new AgentInterpreter(this);
            var agent = new AGENT(client);
            Agents.Add(agent);
            client.BeginSend(message);
            PrintServerState("Agent with ID: " + client.ID + " joins as Player to game: " + GameInfo.GameName);
        }

        public bool SendMessageToClient(ulong clientId, string message) //always first one in JoiningAgents
        {
            var client = JoiningAgents.Where(q => q.ID == clientId).FirstOrDefault();
            if (client != null && client.IsAlive)
            {
                client.BeginSend(message);
                return true;
            }
            else
            {
                RemoveClientOrAgent(clientId);
                return false;
            }
        }

        internal void AddClient(IClientHandle client, JoinGameMessage message)
        {
            JoiningAgents.Add(client);
            message.PlayerId = (long)client.ID;
            SendMessageToGameMaster(message.Serialize());
            PrintServerState("Agent with ID: " + client.ID + " tries to join game: " + GameInfo.GameName);
        }

        public void RemoveClientOrAgent(ulong clientId)
        {
            var client = JoiningAgents.Where(q => q.ID == clientId).FirstOrDefault();
            if (client != null)
            {
                JoiningAgents.Remove(client);
            }
            var agent = Agents.Where(q => q.Client.ID == clientId).FirstOrDefault();
            if (agent != null)
            {
                Agents.Remove(agent);
                var message = new PlayerDisconnectedMessage(agent.PlayerId);
                SendMessageToGameMaster(message.Serialize());
            }
            PrintServerState("Player disconnected: " + clientId + " from game: " + GameInfo.GameName);
        }

        public void CloseGame()
        {
            State = GameState.Ended;
            var message = new GameMasterDisconnectedMessage(gameId);
            foreach (var agent in Agents)
            {
                SendMessageToAgent(agent.PlayerId, message.Serialize());
            }
            mainController.DoCleaning();
        }

        public void GameFinished(string gameName, ulong redPlayers, ulong bluePlayers, ulong clientId)
        {
            State = GameState.Ended;
            mainController.DoCleaning();
            mainController.RegisterGame(gameName,redPlayers,bluePlayers,clientId);
        }

        public void RejectJoin(string name, ulong clientId)
        {
            SendMessageToClient(clientId,( new RejectJoiningGameMessage(name,clientId)).Serialize());
        }


        public void DataSend(string message, ulong clientId)
        {
            SendMessageToAgent(clientId, message);
        }

        public void BeginGame()
        {
            State = GameState.InProgress;
            PrintServerState("Game started: " + this.GameInfo.GameName);
        }

        public void SendKeepAlive(ulong clientId)
        {
            if (!SendMessageToAgent(clientId, ""))
                SendMessageToClient(clientId, "");
        }

        public void SendKeepAliveToGM()
        {
            SendMessageToGameMaster("");
        }

        public void PrintServerState(string message = null)
        {
            mainController.PrintServerState(message);
        }
    }
}
