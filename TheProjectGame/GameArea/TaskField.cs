using System;
using System.Collections.Generic;
using System.Text;

namespace TheProjectGame.GameArea
{
    public class TaskField:Field
    {
        private Piece piece;
        public Piece GetPiece()
        {
            return piece;
        }

        public void SetPiece(Piece value)
        {
            piece = value;
        }

        public TaskField(int x, int y, Piece piece = null):base(x,y)
        {
            this.piece = piece;
        }

        public void RemovePiece()
        {
            piece = null;
        }

        public override FieldType GetFieldType()
        {
            return FieldType.Task;
        }
    }
}
