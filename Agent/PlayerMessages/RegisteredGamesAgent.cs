using System;
using System.Collections.Generic;
using System.Text;
using GameArea;
using GameArea.AppMessages;
using GameArea.GameObjects;

namespace Player.PlayerMessages
{
    public class RegisteredGamesAgent : RegisteredGamesMessage, IAgentMessage
    {
        public RegisteredGamesAgent(GameInfo[] games) : base(games)
        {
        }

        public void Process(IPlayer player)
        {
            player.RegisteredGames(this);
        }
    }
}
