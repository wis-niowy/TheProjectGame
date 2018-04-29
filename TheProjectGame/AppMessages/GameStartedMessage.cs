using Messages;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameArea.AppMessages
{
    public class GameStartedMessage : IToBase<GameStarted>
    {
        public ulong GameId { get; set; }
        public GameStartedMessage(ulong id)
        {
            GameId = id;
        }
        public GameStartedMessage(GameStarted game) : this(game.gameId) { }
        public string Serialize()
        {
            return MessageParser.Serialize(ToBase());
        }

        public GameStarted ToBase()
        {
            return new GameStarted()
            {
                gameId = GameId
            };
        }
    }
}
