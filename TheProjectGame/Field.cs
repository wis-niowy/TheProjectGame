using Messages;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameArea
{
    public enum FieldType { Goal, Task}
    public abstract class Field:Location
    {

        public Field(uint x, uint y):base(x,y)
        {
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
