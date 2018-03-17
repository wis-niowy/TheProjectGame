using System;
using System.Collections.Generic;
using System.Text;

namespace TheProjectGame.GameArea
{
    public enum PieceType { Sham, Valid, Unknown };
    public class Piece
    {
        private PieceType type;
        public Piece()
        {
            type = PieceType.Unknown;
        }
        public Piece(PieceType type)
        {
            this.type = type;
        }
        public PieceType Type
        {
            get
            {
                return type;
            }
            set
            {
                type = value;
            }
        }
    }
}