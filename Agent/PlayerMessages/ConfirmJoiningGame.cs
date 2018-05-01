using System;
using System.Collections.Generic;
using System.Text;
using GameArea;
using GameArea.AppMessages;
using GameArea.GameObjects;

namespace Player.PlayerMessages
{
    public class ConfirmJoiningGameAgent : ConfirmJoiningGameMessage, IAgentMessage
    {
        public ConfirmJoiningGameAgent(ulong gameId, GameArea.GameObjects.Player playerDef, string guid, ulong playerId) : base(gameId, playerDef, guid, playerId)
        {
        }

        public void Process(IPlayer player)
        {
            player.ConfirmJoiningGame(this);
        }
    }
}
