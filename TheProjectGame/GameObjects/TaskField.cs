using GameArea.Parsers;
using Messages;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameArea.GameObjects
{
    public class TaskField:Field,IToBase<Messages.TaskField>
    {
        public int DistanceToPiece { get; set; }
        public Piece Piece { get; set; }

        public override FieldType GetFieldType => FieldType.Task;

        public TaskField(Messages.TaskField task):base(task)
        {
            DistanceToPiece = task.distanceToPiece;
            Piece = task.pieceIdSpecified ? new Piece(task.pieceId, DateTime.MinValue) : null;
            Player = task.playerIdSpecified ? new Player(task.playerId) : null;
        }

        public TaskField(TaskField task) : base(task.ToBase())
        {
            DistanceToPiece = task.DistanceToPiece;
            if (task.Piece != null)
                Piece = new Piece(task.Piece);
            if (task.Player != null)
                Player = new Player(task.Player);
        }

        public TaskField(int x, int y, DateTime timestamp, int distanceToPiece = int.MaxValue, Piece piece = null, Player player = null) : base(x, y)
        {
            DistanceToPiece = distanceToPiece;
            Piece = piece;
            Player = player;
            TimeStamp = timestamp;
        }

        public override string ToString()
        {
            StringBuilder value = new StringBuilder();
            if (Player != null)
            {
                value.Append(Player.Team == TeamColour.red ? "[R" : "[B");
            }
            else
                value.Append("[ ");
            if (Piece != null)
            {
                switch (Piece.Type)
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

        public Messages.TaskField ToBase()
        {
            return new Messages.TaskField()
            {
                distanceToPiece = DistanceToPiece,
                pieceId = Piece != null ? Piece.ID : 0,
                pieceIdSpecified = Piece != null,
                playerId = Player != null ? Player.ID : 0,
                playerIdSpecified = Player != null,
                timestamp = TimeStamp,
                x = (uint)X,
                y = (uint)Y
            };
        }

        public override string Serialize()
        {
            return MessageParser.Serialize(ToBase());
        }
    }
}
