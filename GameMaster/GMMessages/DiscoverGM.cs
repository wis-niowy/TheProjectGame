using GameArea;
using GameArea.AppMessages;
using Messages;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameMaster.GMMessages
{
    public class DiscoverGM : DiscoverMessage, IGMMessage
    {
        public DiscoverGM(string guid, ulong gameId) : base(guid, gameId)
        {
        }

        public string[] Process(IGameMaster gameMaster)
        {
            return new string[] { gameMaster.HandleDiscoverRequest(this).Serialize() };
        }
    }
}
