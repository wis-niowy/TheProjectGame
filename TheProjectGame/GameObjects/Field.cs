using System;
using System.Collections.Generic;
using System.Text;
using Messages;

namespace GameArea.GameObjects
{
    public enum FieldType { Goal, Task }
    public abstract class Field : Location
    { 
        public DateTime TimeStamp { get; set; }
        public long PlayerId { get; set; }
        public Player Player { get; set; }
        public bool HasPlayer()
        {
            return (Player != null);
        }

        public Field(int x, int y) : base(x, y) { }

        public Field(Messages.Field field):base(field)
        {
            PlayerId = field.playerIdSpecified ? (int)field.playerId : -1;
            TimeStamp = field.timestamp;
        }

        public Field(int playerId, DateTime timeStamp, int x, int y):base(x,y)
        {
            PlayerId = playerId;
            TimeStamp = timeStamp;
        }

        public abstract FieldType GetFieldType { get; }
    }
}
