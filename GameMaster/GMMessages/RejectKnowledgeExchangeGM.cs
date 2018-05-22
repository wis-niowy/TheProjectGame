using System;
using System.Collections.Generic;
using System.Text;
using GameArea;
using GameArea.AppMessages;

namespace GameMaster.GMMessages
{
    public class RejectKnowledgeExchangeGM : RejectKnowledgeExchangeMessage, IGMMessage
    {
        public bool Prioritised => false;
        public string GUID => PlayerGUID;
        public RejectKnowledgeExchangeGM(ulong id, ulong senderId, bool permanent, string playerGuid = null) : base(id, senderId, permanent, playerGuid)
        {
        }

        public string[] Process(IGameMaster gameMaster)
        {
            if (gameMaster.GameEndDate > ReceiveDate || gameMaster.GameStartDate > ReceiveDate || gameMaster.IsGameFinished)
            {
                return null;
            }
            return new string[] { gameMaster.HandleRejectKnowledgeExchange(this)?.Serialize() };
        }
    }
}
