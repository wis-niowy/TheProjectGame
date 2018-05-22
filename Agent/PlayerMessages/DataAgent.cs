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

        public string[] Process(IPlayerController controller)
        {
            controller.Player.UpdateLocalBoard(this);//, (MoveType)player.LastMoveTaken);
            return null;
        }
    }
}