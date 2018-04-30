using System;
using System.Collections.Generic;
using System.Text;
using GameArea.Parsers;
using Messages;

namespace GameArea.AppMessages
{
    public class PlacePieceMessage : GameAbstractMessage, IToBase<Messages.PlacePiece>
    {
        public PlacePieceMessage(PlacePiece place):base(place) { }
        public string Serialize()
        {
            return MessageParser.Serialize(ToBase());
        }

        public PlacePieceMessage(string guid, ulong gameId) : base(guid, gameId) { }

        public PlacePiece ToBase()
        {
            return new Messages.PlacePiece()
            {
                gameId = GameId,
                playerGuid = PlayerGUID
            };
        }
    }
}
