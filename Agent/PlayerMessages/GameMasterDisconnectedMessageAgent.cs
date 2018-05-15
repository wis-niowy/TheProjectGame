using System;
using System.Collections.Generic;
using System.Text;
using GameArea;
using GameArea.AppMessages;

namespace Player.PlayerMessages
{
    public class GameMasterDisconnectedMessageAgent : GameMasterDisconnectedMessage, IAgentMessage
    {
        public GameMasterDisconnectedMessageAgent(ulong id) : base(id)
        {
        }

        public string[] Process(IPlayer player)
        {
            player.GameMasterDisconnected(this);
            return null;
        }
    }
}
