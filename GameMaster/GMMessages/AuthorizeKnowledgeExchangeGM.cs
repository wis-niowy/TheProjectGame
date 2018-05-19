using System;
using System.Collections.Generic;
using System.Text;
using GameArea;
using GameArea.AppMessages;

namespace GameMaster.GMMessages
{
    public class AuthorizeKnowledgeExchangeGM : AuthorizeKnowledgeExchangeMessage, IGMMessage
    {
        public bool Prioritised => false;
        public string GUID => PlayerGUID;

        public AuthorizeKnowledgeExchangeGM(string guid, ulong gameId, ulong withPlayerId) : base(guid, gameId, withPlayerId)
        {

        }

        public string[] Process(IGameMaster gameMaster)
        {
            return new string[] { gameMaster.HandleAuthorizeKnowledgeExchange(this)?.Serialize() };
        }
    }
}
