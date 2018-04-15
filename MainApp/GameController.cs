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
        public List<IPlayer> Players { get; set; }
        private bool TestMode;

        public GameController(GameMasterSettings settings,bool testing = false)
        {
            GM = new GameMaster(settings);
            Players = new List<IPlayer>();
            TestMode = testing;
        }

        public void RegisterPlayers()
        {

            while (Players.Count != 2 * GM.GetGameDefinition.NumberOfPlayersPerTeam)
            {
                var PlayerBlue = new Player.Player(Messages.TeamColour.blue, gm: GM);
                Players.Add(PlayerBlue);
                GM.RegisterPlayer(PlayerBlue);
                var PlayerRed = new Player.Player(Messages.TeamColour.red, gm: GM);
                Players.Add(PlayerRed);
                GM.RegisterPlayer(PlayerRed);
            }
            ConsoleWriter.Show("Players registeration succesfull.");
        }

        public void HandleGame()
        {
            var tasks = new Task[Players.Count];
            int actionsCount = 2000000;
            while (GM.GetState != GameMasterState.GameOver)
            {
                for (int i = 0; i < Players.Count; i++)
                {
                    if (GM.GetState == GameMasterState.GameOver)
                        break;
                    var task = tasks[i];
                    var Player = Players[i];
                    if (task == null)
                    {
                        tasks[i] = Task<string>.Run(() => Player.DoStrategy());
                        while (TestMode && !tasks[i].IsCompleted)
                        {
                            //waiting for task to do its job
                        }
                            
                        actionsCount--;
                    }
                    else if (task.IsCompleted)
                    {
                        task.Dispose();
                        tasks[i] = Task<string>.Run(() => Player.DoStrategy());
                        while (TestMode && !tasks[i].IsCompleted)
                        {
                            //waiting for task to do its job
                        }
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
            var PlayersCount = Players.Count;
            while (PlayersCount > 0)
            {
                for (int i = 0; i < tasks.Length; i++)
                {
                    var task = tasks[i];
                    if (task != null && task.IsCompletedSuccessfully)
                    {
                        PlayersCount--;
                        tasks[i] = null;
                    }
                    if (PlayersCount == 0)
                        break;
                }
            }
        }
    }
}
