using System;
using System.Collections.Generic;
using System.Text;
using GameArea.Parsers;
using Messages;

namespace GameArea.GameObjects
{
    public class Location:IToBase<Messages.Location>
    {
        public int X { get; set; }
        public int Y { get; set; }

        public Location (Messages.Location loc)
        {
            X = (int)loc.x;
            Y = (int)loc.y;
        }
        public Location(int x, int y)
        {
            X = x;
            Y = y;
        }
        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            var location = (Location)obj;
            return location.X == X && location.Y == Y;
        }

        public override string ToString()
        {
            return "(" + X + "," + Y + ")";
        }

        public virtual Messages.Location ToBase()
        {
            return new Messages.Location()
            {
                x = (uint)X,
                y = (uint)Y
            };
        }

        public virtual string Serialize()
        {
            return MessageParser.Serialize(ToBase());
        }
    }
}
