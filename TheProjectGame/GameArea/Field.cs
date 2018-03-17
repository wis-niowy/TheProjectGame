using System;
using System.Collections.Generic;
using System.Text;

namespace TheProjectGame.GameArea
{
    public enum FieldType { Goal, Task}
    public abstract class Field
    {
        private int x;
        private int y;
        public int X { get { return x; } }
        public int Y { get { return y; } }

        public Field(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        private DateTime timestamp;

        public DateTime TimeStamp { get { return timestamp; } }
        public void UpdateTimeStamp(DateTime newTimeStamp)
        {
            timestamp = newTimeStamp;
        }

        public abstract FieldType GetFieldType();
    }
}
