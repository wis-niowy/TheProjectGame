using System;
using System.Collections.Generic;
using System.Text;
using GameArea;
using GameArea.AppMessages;

namespace GameMaster.GMMessages
{
    public class PlayerDisconnectedGM : PlayerDisconnectedMessage, IGMMessage
    {
        public PlayerDisconnectedGM(ulong id) : base(id)
        {
        }

        public string[] Process(IGameMaster gameMaster)
        {
            gameMaster.HandlePlayerDisconnectedRequest(this);
            return new string[] { };
        }
    }
}
