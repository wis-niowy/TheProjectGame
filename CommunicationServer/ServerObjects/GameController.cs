using Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommunicationServer.ServerObjects
{
    public enum GameState { New, InProgress, Ended };
    public class GameController:IGMController, IAgentController
    {
        public ulong gameId { get; }
        public GM GameMaster { get; set; }
        public List<AGENT> Agents { get; set; }
        public  List<ClientHandle> JoiningAgents { get; set; }
        public GameInfo GameInfo { get; set; }
        public GameState State { get; set; }
        private MainController mainController;
        public GameController(GameInfo newGameInfo,ulong id, MainController controller)
        {
            Agents = new List<AGENT>();
            JoiningAgents = new List<ClientHandle>();
            GameInfo = newGameInfo;
            gameId = id;
            State = GameState.New;
            mainController = controller;
        }

        public bool SendMessageToAgent(ulong playerId, string message)
        {
            var agent = Agents.Where(q => q.PlayerId == playerId).FirstOrDefault();
            if (agent != null)
            {
                agent.SendMessage(message);
                return true;
            }
            return false;
        }

        public void SendMessageToGameMaster(string message)
        {
            GameMaster.SendMessage(message);
        }

        public void SetGM(GM gm)
        {
            GameMaster = gm;
        }

        public void RegisterClientAsAgent(ConfirmJoiningGame message)
        {
            var client = JoiningAgents.Where(q => q.ID == message.playerId).FirstOrDefault();
            JoiningAgents.Remove(client);

            client.MessageInterpreter = new AgentInterpreter(this);
            var agent = new AGENT(client);
            Agents.Add(agent);
            client.BeginSend(MessageReader.Serialize(message));
        }

        public bool SendMessageToClient(string message) //always first one in JoiningAgents
        {
            var client = JoiningAgents.FirstOrDefault();
            if(client != null)
            {
                client.BeginSend(message);
                return true;
            }
            return false;
        }

        internal void AddClient(ClientHandle client, JoinGame message)
        {
            JoiningAgents.Add(client);
            message.playerId = client.ID;
            SendMessageToGameMaster(MessageReader.Serialize(message));
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
                var message = MessageReader.Serialize(new PlayerDisconnected()
                {
                    playerId = agent.PlayerId
                });
                SendMessageToGameMaster(message);
            }
        }

        public void CloseGame()
        {
            State = GameState.Ended;
            var message = MessageReader.Serialize(new GameMasterDisconnected()
            {
                gameId = this.gameId
            });
            foreach(var agent in Agents)
            {
                SendMessageToAgent(agent.PlayerId, message);
            }
            mainController.DoCleaning();
        }

        public void RejectJoin(RejectJoiningGame message)
        {
            SendMessageToAgent(message.playerId,MessageReader.Serialize(message));
        }


        public void DataSend(Data messageObject)
        {
            SendMessageToAgent(messageObject.playerId, MessageReader.Serialize(messageObject));
        }

        public void BeginGame()
        {
            State = GameState.InProgress;
        }
    }
}
