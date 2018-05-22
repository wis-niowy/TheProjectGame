using Messages;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameArea.AppMessages
{
    public class AuthorizeKnowledgeExchangeMessage : GameAbstractMessage, IToBase<AuthorizeKnowledgeExchange>
    {
        public ulong WithPlayerId { get; set; }
        public AuthorizeKnowledgeExchangeMessage(string guid, ulong gameId, ulong withPlayerId) : base(guid, gameId)
        {
            WithPlayerId = withPlayerId;
        }

        public string Serialize()
        {
            return MessageParser.Serialize(ToBase());
        }

        public AuthorizeKnowledgeExchange ToBase()
        {
            return new AuthorizeKnowledgeExchange()
            {
                gameId = GameId,
                playerGuid = PlayerGUID,
                withPlayerId = WithPlayerId
            };
        }
    }
}
