using GameArea.Parsers;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameArea.AppMessages
{
    public abstract class PlayerMessage
    {
        public ulong PlayerId { get; set; }

        public PlayerMessage (Messages.PlayerMessage player)
        {
            PlayerId = player.playerId;
        }
        public PlayerMessage(ulong playerId)
        {
            PlayerId = playerId;
        }

        public abstract string Serialize();
    }
}
