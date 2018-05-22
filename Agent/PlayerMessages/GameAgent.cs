using System;
using System.Collections.Generic;
using System.Text;
using GameArea;

namespace Player.PlayerMessages
{
    public class GameAgent : GameArea.AppMessages.GameMessage, IAgentMessage
    {
        public GameAgent(ulong playerId) : base(playerId)
        {
        }

        public string[] Process(IPlayerController controller)
        {
            controller.Player.GameStarted(this);
            return null;
        }
    }
}
