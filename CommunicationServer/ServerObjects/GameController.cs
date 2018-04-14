using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommunicationServer.ServerObjects
{
    public class GameController:IGMController, IAgentController
    {
        private ulong gameId { get; }
        private GM GameMaster;
        private List<AGENT> Agents;

        private List<ClientHandle> JoiningAgents;

        public GameController()
        {
            Agents = new List<AGENT>();
            JoiningAgents = new List<ClientHandle>();
        }

        public bool SendMessageToAgent(uint playerId, string message)
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

        public void RegisterClientAsAgent(string message, ulong playerId)
        {
            var client = JoiningAgents.First();
            JoiningAgents.Remove(client);

            client.MessageInterpreter = new AgentInterpreter(this);
            var agent = new AGENT(client, playerId);
            Agents.Add(agent);
            client.BeginSend(message);
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

        internal void AddClient(ClientHandle client, string message)
        {
            JoiningAgents.Add(client);
            SendMessageToGameMaster(message);
        }
    }
}
