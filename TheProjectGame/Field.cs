using Messages;
using System;
using System.Collections.Generic;
using System.Text;


namespace GameArea
{
    public enum FieldType { Goal, Task}
    public abstract class Field:Location
    {
        private DateTime timestamp;
        private Player player;
        public Player Player
        {
            get
            {
                return player;
            }
            set
            {
                this.player = value;
            }
        }
        public Field(int x, int y):base(x,y)
        {
        }

        public DateTime TimeStamp { get { return timestamp; } }
        public void UpdateTimeStamp(DateTime newTimeStamp)
        {
            timestamp = newTimeStamp;
        }

        public bool HasPlayer()
        {
            return (player != null);
        }

        public abstract FieldType GetFieldType { get; }
    }
}
