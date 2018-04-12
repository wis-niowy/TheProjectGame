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
            if(value != null)
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

        public Messages.TaskField ConvertToMessageTaskField()
        {
            ulong playerid = 0;
            if (this.Player != null)
                playerid = this.Player.id;
            ulong pieceid = 0;
            if (this.Player != null)
                pieceid = this.Player.id;

            return new Messages.TaskField()
            {
                x = this.x,
                y = this.y,
                playerIdSpecified = this.Player != null,
                playerId = playerid,
                timestamp = this.TimeStamp,
                distanceToPiece = this.Distance,
                pieceIdSpecified = this.piece != null,
                pieceId = pieceid,
            };
        }

        public override string ToString()
        {
            StringBuilder value = new StringBuilder();
            if (Player != null)
            {
                value.Append(Player.team == TeamColour.red ? "[R" : "[B");
            }
            else
                value.Append("[ ");
            if (piece != null)
            {
                switch (piece.type)
                {
                    case PieceType.normal:
                        value.Append("NP");
                        break;
                    case PieceType.sham:
                        value.Append("SP");
                        break;
                    case PieceType.unknown:
                        value.Append("UP");
                        break;
                }
            }
            else
                value.Append("  ");
            value.Append("]");
            return value.ToString();
        }

    }
}
