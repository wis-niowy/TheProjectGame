using System;
using System.Collections.Generic;
using System.Text;

namespace TheProjectGame.GameArea
{
    public class Point
    {
        public int X
        {
            get
            {
                return X;
            }
            set
            {
                X = value;
            }
        }

        public int Y
        {
            get
            {
                return Y;
            }
            set
            {
                Y = value;
            }
        }

        public Point(int x, int y)
        {
            X = x;
            Y = y;
        }
    }
}
