using GameArea.Parsers;
using Messages;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameArea.AppMessages
{
    public class MoveMessage : GameAbstractMessage, IToBase<Move>
    {
        public MoveType? Direction { get; set; }
        public MoveMessage(Move move):base(move)
        {
            Direction = move.directionSpecified ? (MoveType?)move.direction : null;
        }
        public string Serialize()
        {
            return MessageParser.Serialize(ToBase());
        }

        public Move ToBase()
        {
            return new Move()
            {
                directionSpecified = Direction != null,
                direction = (MoveType)Direction,
                gameId = GameId,
                playerGuid = PlayerGUID
            };
        }
    }
}
