using GameArea;
using GameArea.AppMessages;
using Messages;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Player.PlayerMessages
{
    public class DataAgent : DataMessage, IAgentMessage
    {
        public DataAgent(ulong playerId) : base(playerId)
        {
        }

        public void Process(IPlayer player)
        {
            player.UpdateLocalBoard(this, (ActionType)player.LastActionTaken, (MoveType)player.LastMoveTaken);
        }
    }
}