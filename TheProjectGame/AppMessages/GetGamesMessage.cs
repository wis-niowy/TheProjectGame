using Messages;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameArea.AppMessages
{
    public class GetGamesMessage : IToBase<GetGames>
    {
        public string Serialize()
        {
            return MessageParser.Serialize(ToBase());
        }

        public GetGames ToBase()
        {
            return new GetGames()
            {

            };
        }
    }
}
