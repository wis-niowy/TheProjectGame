using System;
using System.Collections.Generic;
using System.Text;
using GameArea;
using GameArea.AppMessages;

namespace GameMaster.GMMessages
{
    public class AcceptExchangeRequestGM : AcceptExchangeRequestMessage, IGMMessage
    {
        public bool Prioritised => false;
        public string GUID => null;
        public AcceptExchangeRequestGM(ulong id, ulong senderId) : base(id, senderId)
        {
        }

        

        public string[] Process(IGameMaster gameMaster)
        {
            if (gameMaster.GameEndDate > ReceiveDate || gameMaster.GameStartDate > ReceiveDate || gameMaster.IsGameFinished)
            {
                return null;
            }
            return null;
            //return new string[] { gameMaster.HandleAcceptKnowledgeExchange(this)?.Serialize() };
        }
    }
}
