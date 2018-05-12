using System;
using System.Collections.Generic;
using System.Text;
using GameArea;
using GameArea.AppMessages;

namespace GameMaster.GMMessages
{
    class RejectKnowledgeExchangeGM : RejectKnowledgeExchangeMessage, IGMMessage
    {
        public RejectKnowledgeExchangeGM(ulong id, ulong senderId, bool permanent, string playerGuid = null) : base(id, senderId, permanent, playerGuid)
        {
        }

        public string[] Process(IGameMaster gameMaster)
        {
            return new string[] { gameMaster.HandleRejectKnowledgeExchange(this)?.Serialize() };
        }
    }
}
