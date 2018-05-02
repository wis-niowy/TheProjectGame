using GameArea.Parsers;
using Messages;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameArea.GameObjects
{
    public class Piece:IToBase<Messages.Piece>
    {
        public ulong ID { get; set; }
        public PieceType Type { get; set; }
        public long PlayerId { get; set; }
        public DateTime TimeStamp { get; set; }

        public Piece(Messages.Piece piece)
        {
            ID = piece.id;
            PlayerId = piece.playerIdSpecified ? (long)piece.playerId:-1;
            Type = piece.type;
            TimeStamp = piece.timestamp;
        }
        public Piece(ulong id,DateTime timeStamp, PieceType type = PieceType.unknown, long playerId = -1)
        {
            ID = id;
            TimeStamp = DateTime.Now;
            Type = type;
            PlayerId = playerId;
            TimeStamp = timeStamp;
        }
        public Messages.Piece ToBase()
        {
            return new Messages.Piece()
            {
                id = ID,
                playerId = (ulong)PlayerId,
                timestamp = TimeStamp,
                playerIdSpecified = PlayerId >= 0,
                type = Type
            };
        }

        public string Serialize()
        {
            return MessageParser.Serialize(ToBase());
        }
    }
}
