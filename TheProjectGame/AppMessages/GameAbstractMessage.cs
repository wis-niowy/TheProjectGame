using GameArea.Parsers;
using Messages;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameArea.AppMessages
{
    public abstract class GameAbstractMessage
    {
        public string PlayerGUID { get; set; }
        public ulong GameId { get; set; }

        public GameAbstractMessage(Messages.GameMessage game)
        {
            PlayerGUID = game.playerGuid;
            GameId = game.gameId;
        }
    }
}
