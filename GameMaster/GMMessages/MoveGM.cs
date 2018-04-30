using GameArea;
using GameArea.AppMessages;
using Messages;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameMaster.GMMessages
{
    public class MoveGM : MoveMessage, IGMMessage
    {
        public MoveGM(string guid, ulong gameId, MoveType? move = null) : base(guid, gameId, move)
        {
        }

        public string[] Process(IGameMaster gameMaster)
        {
            return new string[] { gameMaster.HandleMoveRequest(this).Serialize() };
        }
    }
}
