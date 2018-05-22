using GameArea;
using GameArea.AppMessages;
using Messages;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameMaster.GMMessages
{
    public class PickUpPieceGM : PickUpPieceMessage, IGMMessage
    {
        public bool Prioritised => false;
        public string GUID => PlayerGUID;
        public PickUpPieceGM(string guid, ulong gameId) : base(guid, gameId)
        {
        }

        public string[] Process(IGameMaster gameMaster)
        {
            if (gameMaster.GameEndDate > ReceiveDate || gameMaster.GameStartDate > ReceiveDate || gameMaster.IsGameFinished)
            {
                return null;
            }
            return new string[] { gameMaster.HandlePickUpPieceRequest(this)?.Serialize() };
        }
    }
}
