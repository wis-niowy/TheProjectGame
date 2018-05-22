using GameArea.Parsers;
using Messages;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameArea.AppMessages
{
    public class DiscoverMessage : GameAbstractMessage, IToBase<Discover>
    {
        public DiscoverMessage(Discover discover):base(discover) { }
        public string Serialize()
        {
            return MessageParser.Serialize(ToBase());
        }

        public DiscoverMessage(string guid, ulong gameId) : base(guid, gameId) { }

        public Discover ToBase()
        {
            return new Discover()
            {
                gameId = GameId,
                playerGuid = PlayerGUID
            };
        }
    }
}
