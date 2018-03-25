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

        public Messages.GameBoard ConvertToMessageGameBoard()
        {
            return new GameBoard()
            {
                goalsHeight = this.GoalAreaHeight,
                tasksHeight = this.TaskAreaHeight,
                width = this.BoardWidth,
            };
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
                        taskFields.Add((TaskField)fields[j, i]);
                    }
                }
                return taskFields;
            }
        }

        public List<GoalField> GoalFields(TeamColour team)
        {
            var goalFields = new List<GoalField>();
            for (uint i = 0; i < BoardHeight; i++)
            {
                for (uint j = 0; j < BoardWidth; j++)
                {
                    if (fields[j, i] is GoalField && ((GoalField)fields[j, i]).GetOwner == team)
                        goalFields.Add((GoalField)fields[j, i]);
                }
            }
            return goalFields;
        }

        public List<GoalField> GetBlueGoalAreaFields
        {
            get
            {
                var goalFields = new List<GoalField>();
                for(int i=0;i<goalAreaHeight;i++)
                {
                    for (int j = 0; j < BoardWidth; j++)
                    {
                        var field = (GoalField)fields[j, i];
                        goalFields.Add(field);
                    }
                }
                return goalFields;
            }
        }

        public List<GoalField> GetRedGoalAreaFields
        {
            get
            {
                var goalFields = new List<GoalField>();
                for (uint i = goalAreaHeight + taskAreaHeight; i < BoardHeight; i++)
                {
                    for (uint j = 0; j < BoardWidth; j++)
                    {
                        var field = (GoalField)fields[j, i];
                        goalFields.Add(field);
                    }
                }
                return goalFields;
            }
        }

        public Field GetField(uint x, uint y)
        {
            if (x >= BoardWidth || y >= BoardHeight)
                return null;
            return fields[x, y];
        }

        public void SetGoalField(GameMasterGoalField goalField)
        {
            fields[goalField.x, goalField.y] = goalField;
        }

        public TaskField GetTaskField(uint x, uint y)
        {
            if (y < GoalAreaHeight || y >=GoalAreaHeight+TaskAreaHeight || x >= BoardWidth)
                return null;
            return (TaskField)fields[x, y];
        }

        public TaskField GetTaskField(Location location)
        {
            if (location == null)
                return null;
            return GetTaskField(location.x,location.y);
        }

        public GoalField GetGoalField(uint x, uint y)
        {
            if ((y >= GoalAreaHeight && y <GoalAreaHeight + TaskAreaHeight) || x >= BoardWidth)
                return null;
            return (GoalField)fields[x, y];
        }

        public GoalField GetGoalField(Location location)
        {
            if (location == null)
                return null;
            return GetGoalField(location.x, location.y);
        }

        private Field[,] fields;
        public Board(uint width, uint pieceAreaHeight, uint goalAreaHeight, GoalFieldType defaultGoalType = GoalFieldType.unknown)
        {
            this.width = width;
            this.taskAreaHeight = pieceAreaHeight;
            this.goalAreaHeight = goalAreaHeight;
            fields = new Field[width,pieceAreaHeight + 2 * goalAreaHeight];
            for (uint i = 0; i < GoalAreaHeight; i++)
            {
                for (uint j = 0; j < BoardWidth; j++)
                {
                    fields[j, i] = new GoalField(j, i, TeamColour.blue, defaultGoalType);
                    fields[j, i + pieceAreaHeight + goalAreaHeight] = new GoalField(j, i + pieceAreaHeight + goalAreaHeight, TeamColour.red, defaultGoalType);
                }
            }
            for (uint i = GoalAreaHeight; i < BoardHeight - GoalAreaHeight; i++)
            {
                for (uint j = 0; j < BoardWidth; j++)
                {
                    fields[j, i] = new TaskField(j, i);
                }
            }
        }
    }
}
