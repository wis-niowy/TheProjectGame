using System;
using System.Collections.Generic;
using System.Text;
using Messages;
using System.Linq;

namespace GameArea
{
    public class Board
    {
        private uint width;
        private uint taskAreaHeight;
        private uint goalAreaHeight;
        public uint BoardWidth
        {
            get
            {
                return width;
            }
        }
        public uint TaskAreaHeight
        {
            get
            {
                return taskAreaHeight;
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
                return taskAreaHeight + 2 * goalAreaHeight;
            }
        }

        public List<TaskField> TaskFields
        {
            get
            {
                var taskFields = new List<TaskField>();
                for(uint i = goalAreaHeight;i<goalAreaHeight + taskAreaHeight;i++)
                {
                    for(uint j = 0; j<BoardWidth;j++)
                    {
                        taskFields.Add((TaskField)fields[i, j]);
                    }
                }
                return taskFields;
            }
        }

        public Field GetField(uint x, uint y)
        {
            if (x >= BoardHeight || y >= BoardWidth)
                return null;
            return fields[x, y];
        }

        public TaskField GetTaskField(uint x, uint y)
        {
            if (x < GoalAreaHeight || x >=GoalAreaHeight+TaskAreaHeight || y >= BoardWidth)
                return null;
            return (TaskField)fields[x, y];
        }

        private Field[,] fields;
        public Board(uint width, uint pieceAreaHeight, uint goalAreaHeight)
        {
            this.width = width;
            this.taskAreaHeight = pieceAreaHeight;
            this.goalAreaHeight = goalAreaHeight;
            fields = new Field[pieceAreaHeight + 2 * goalAreaHeight, width];
            for (uint i = 0; i < GoalAreaHeight; i++)
            {
                for (uint j = 0; j < BoardWidth; j++)
                {
                    fields[i, j] = new GoalField(i, j, TeamColour.blue, GoalFieldType.nongoal);
                    fields[i + pieceAreaHeight + goalAreaHeight, j] = new GoalField(i + pieceAreaHeight + goalAreaHeight, j, TeamColour.red, GoalFieldType.nongoal);
                }
            }
            for (uint i = GoalAreaHeight; i < BoardHeight - GoalAreaHeight; i++)
            {
                for (uint j = 0; j < BoardWidth; j++)
                {
                    fields[i, j] = new TaskField(i, j);
                }
            }
        }
    }
}
