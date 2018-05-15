using System;
using System.Collections.Generic;
using System.Text;
using GameArea;
using GameArea.AppMessages;

namespace Player.PlayerMessages
{
    public class RejectJoiningGameAgent : RejectJoiningGameMessage, IAgentMessage
    {
        public RejectJoiningGameAgent(string name, ulong playerId) : base(name, playerId)
        {
        }

        public string[] Process(IPlayer player)
        {
            player.ActionToComplete = ActionType.none;
            //nic nie robi, sprobuje połączyć się z inną grą
            return null;
        }
    }
}
