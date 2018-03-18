using System;
using System.Collections.Generic;
using System.Text;

namespace GameArea
{
    public class Board
    {
        private uint width;
        private uint pieceAreaHeight;
        private uint goalAreaHeight;
        public uint BoardWidth
        {
            get
            {
                return width;
            }
        }
        public uint PieceAreaHeight
        {
            get
            {
                return pieceAreaHeight;
            }
        }

        public uint GoalAreaHeight
        {
            get { return goalAreaHeight; }
        }
        public uint BoardHeight
        {
            get
            {
                return pieceAreaHeight + 2 * goalAreaHeight;
            }
        }
        private Field[,] fields;
        public Board(uint width, uint pieceAreaHeight, uint goalAreaHeight)
        {
            this.width = width;
            this.pieceAreaHeight = pieceAreaHeight;
            this.goalAreaHeight = goalAreaHeight;
            fields = new Field[pieceAreaHeight + 2 * goalAreaHeight, width];
        }
    }
}
