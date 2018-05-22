using GameArea.Parsers;
using Messages;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameArea.AppMessages
{
    public abstract class GameAbstractMessage
    {
        public DateTime ReceiveDate { get; }
        public string PlayerGUID { get; set; }
        public ulong GameId { get; set; }

        public GameAbstractMessage(string guid, ulong gameId)
        {
            PlayerGUID = guid;
            GameId = gameId;
            ReceiveDate = DateTime.Now;
        }
        public GameAbstractMessage(Messages.GameMessage game)
        {
            PlayerGUID = game.playerGuid;
            GameId = game.gameId;
        }
    }
}
