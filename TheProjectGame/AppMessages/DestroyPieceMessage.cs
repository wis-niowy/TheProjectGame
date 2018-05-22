using System;
using System.Collections.Generic;
using System.Text;
using GameArea.Parsers;
using Messages;

namespace GameArea.AppMessages
{
    public class DestroyPieceMessage :GameAbstractMessage, IToBase<Messages.DestroyPiece>
    {
        public string Serialize()
        {
            return MessageParser.Serialize(ToBase());
        }

        public DestroyPieceMessage(Messages.DestroyPiece destroy) : base(destroy) { }

        public DestroyPieceMessage(string guid, ulong gameId):base(guid,gameId) { }
        public DestroyPiece ToBase()
        {
            return new Messages.DestroyPiece()
            {
                gameId = GameId,
                playerGuid = PlayerGUID
            };
        }
    }
}
