using System;
using System.Collections.Generic;
using System.Text;

namespace TheProjectGame.GameArea
{
    public class Board
    {
        private int width;
        private int pieceAreaHeight;
        private int goalAreaHeight;
        public int BoardWidth
        {
            get
            {
                return width;
            }
        }
        public int PieceAreaHeight
        {
            get
            {
                return pieceAreaHeight;
            }
        }

        public int GoalAreaHeight
        {
            get { return goalAreaHeight; }
        }
        public int BoardHeight
        {
            get
            {
                return pieceAreaHeight + 2 * goalAreaHeight;
            }
        }
        private Field[,] fields;
        public Board(int width, int pieceAreaHeight, int goalAreaHeight)
        {
            this.width = width;
            this.pieceAreaHeight = pieceAreaHeight;
            this.goalAreaHeight = goalAreaHeight;
            fields = new Field[pieceAreaHeight + 2 * goalAreaHeight, width];
        }
    }
}
