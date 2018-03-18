using Messages;
using Player;
using System;
using System.Collections.Generic;
using System.Linq;
using GameArea;
using Messages;
using Configuration;

namespace GameMaster
{
    public class GameMaster
    {
        private List<Agent> agents;

        public List<Agent> GetAgents
        {
            get
            {
                return agents;
            }
        }

        public List<Agent> GetAgentsByTeam(Messages.TeamColour team)
        {
            return agents.Where(q => q.GetTeam == team).ToList();
        }

        public Agent GetAgentByGuid(ulong guid)
        {
            return agents.Where(q => q.GUID == guid).FirstOrDefault();
        }

        public void RegisterAgent(Agent agent)
        {
            var newGuid = agents.Max(q => q.GUID) + 1;
            agent.SetGuid(newGuid);
            agents.Add(agent);
        }

        private Board actualBoard;

        public Board Board
        {
            get
            {
                return actualBoard;
            }
        }

        private GameMasterSettingsGameDefinition gameSettings;

        public GameMaster(GameMasterSettingsGameDefinition settings)
        {
            gameSettings = settings;
        }
    }
}
