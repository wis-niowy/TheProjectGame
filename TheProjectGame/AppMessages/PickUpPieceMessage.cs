using GameArea.Parsers;
using Messages;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameArea.AppMessages
{
    public class PickUpPieceMessage : GameAbstractMessage, IToBase<PickUpPiece>
    {
        public PickUpPieceMessage(PickUpPiece pick) : base(pick) { }
        public string Serialize()
        {
            return MessageParser.Serialize(ToBase());
        }

        public PickUpPiece ToBase()
        {
            return new PickUpPiece()
            {
                gameId = GameId,
                playerGuid = PlayerGUID
            };
        }
    }
}
