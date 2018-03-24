using Configuration;
using GameArea;
using Player;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MainApp
{
    public class GameController
    {
        public GameMaster GM { get; set; }
        public List<Agent> Agents { get; set; }

        public GameController(GameMasterSettings settings)
        {
            GM = new GameMaster(settings);
            Agents = new List<Agent>();
        }

        public void RegisterAgents()
        {

            while (Agents.Count != 2 * GM.GetGameDefinition.NumberOfPlayersPerTeam)
            {
                var agentBlue = new Agent(Messages.TeamColour.blue, gm: GM);
                Agents.Add(agentBlue);
                GM.RegisterAgent(agentBlue);
                var agentRed = new Agent(Messages.TeamColour.red, gm: GM);
                Agents.Add(agentRed);
                GM.RegisterAgent(agentRed);
            }
            ConsoleWriter.Show("Agents registeration succesfull.");
        }

        public void HandleGame()
        {
            var tasks = new Task[Agents.Count];
            int actionsCount = 2000000;
            while (GM.GetState != GameMasterState.GameOver)
            {
                for (int i = 0; i < Agents.Count; i++)
                {
                    if (GM.GetState == GameMasterState.GameOver)
                        break;
                    var task = tasks[i];
                    var agent = Agents[i];
                    if (task == null)
                    {
                        tasks[i] = Task<string>.Run(() => agent.DoStrategy());
                        actionsCount--;
                    }
                    else if (task.IsCompleted)
                    {
                        task.Dispose();
                        tasks[i] = Task<string>.Run(() => agent.DoStrategy());
                        actionsCount--;
                    }
                    else
                    {
                        continue;
                    }
                }

                if(actionsCount<= 0)
                {
                    break;
                }
            }
            var agentsCount = Agents.Count;
            while (agentsCount > 0)
            {
                for (int i = 0; i < tasks.Length; i++)
                {
                    var task = tasks[i];
                    if (task != null && task.IsCompletedSuccessfully)
                    {
                        agentsCount--;
                        tasks[i] = null;
                    }
                    if (agentsCount == 0)
                        break;
                }
            }
        }
    }
}
