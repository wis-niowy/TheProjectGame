using Messages;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameArea
{
    public class TaskField : Field
    {
        private int distance;
        public int Distance
        {
            get
            {
                return distance;
            }
            set
            {
                this.distance = value;
            }
        }
        private Piece piece;    //--------- czy piece ma być tylko na task field, czy na dowolnym field?
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
            distance = 0;
        }

        public TaskField(uint x, uint y, Piece piece = null) : base(x, y)
        {
            this.piece = piece;
            distance = int.MaxValue;
        }

        public void RemovePiece()
        {
            piece = null;
        }

        public override FieldType GetFieldType
        {
            get
            {
                return FieldType.Task;
            }
        }
    }
}
