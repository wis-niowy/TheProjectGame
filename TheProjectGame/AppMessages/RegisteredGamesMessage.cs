using Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameArea.AppMessages
{
    public class RegisteredGamesMessage : IToBase<RegisteredGames>
    {
        public GameObjects.GameInfo[] Games { get; set; }
        public string Serialize()
        {
            return MessageParser.Serialize(ToBase());
        }

        public RegisteredGamesMessage(GameObjects.GameInfo[] games)
        {
            Games = games;
        }

        public RegisteredGames ToBase()
        {
            return new RegisteredGames()
            {
                GameInfo = Games?.Select(q => q.ToBase()).ToArray()
            };
        }
    }
}
