using Messages;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameArea
{
    public class TaskField:Field
    {
        private Piece piece;
        public Piece GetPiece
        {
            get
            {
                return piece;
            }
        }

        public void SetPiece(Piece value)
        {
            piece = value;
        }

        public TaskField(uint x, uint y, Piece piece = null):base(x,y)
        {
            this.piece = piece;
        }

        public void RemovePiece()
        {
            piece = null;
        }

        public new FieldType GetFieldType
        {
            get
            {
                return FieldType.Task;
            }
        }
    }
}
