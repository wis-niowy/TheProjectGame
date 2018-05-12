using System;
using System.Collections.Generic;
using System.Text;
using GameArea;
using GameArea.AppMessages;

namespace GameMaster.GMMessages
{
    public class AcceptExchangeRequestGM : AcceptExchangeRequestMessage, IGMMessage
    {
        public AcceptExchangeRequestGM(ulong id, ulong senderId) : base(id, senderId)
        {
        }

        public string[] Process(IGameMaster gameMaster)
        {
            return new string[] { gameMaster.HandleAcceptKnowledgeExchange(this)?.Serialize() };
        }
    }
}
