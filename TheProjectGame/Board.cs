﻿using System;
using System.Collections.Generic;
using System.Text;
using Messages;
using System.Linq;

namespace GameArea
{
    public class Board
    {
        private int width;
        private int taskAreaHeight;
        private int goalAreaHeight;
        public int BoardWidth
        {
            get
            {
                return width;
            }
        }
        public int TaskAreaHeight
        {
            get
            {
                return taskAreaHeight;
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
                for(int i = goalAreaHeight;i<goalAreaHeight + taskAreaHeight;i++)
                {
                    for(int j = 0; j<BoardWidth;j++)
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
            for (int i = 0; i < BoardHeight; i++)
            {
                for (int j = 0; j < BoardWidth; j++)
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
                for (int i = goalAreaHeight + taskAreaHeight; i < BoardHeight; i++)
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

        public Field GetField(int x, int y)
        {
            if (x >= BoardWidth || y >= BoardHeight || x < 0 || y < 0)
                return null;
            return fields[x, y];
        }

        public void SetGoalField(GameMasterGoalField goalField)
        {
            fields[goalField.x, goalField.y] = goalField;
        }

        public TaskField GetTaskField(int x, int y)
        {
            if (y < GoalAreaHeight || y >=GoalAreaHeight+TaskAreaHeight || x >= BoardWidth || x < 0)
                return null;
            return (TaskField)fields[x, y];
        }

        public TaskField GetTaskField(Location location)
        {
            if (location == null)
                return null;
            return GetTaskField(location.x,location.y);
        }

        public GoalField GetGoalField(int x, int y)
        {
            if ((y >= GoalAreaHeight && y <GoalAreaHeight + TaskAreaHeight) || x >= BoardWidth || x < 0)
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
        public Board(int width, int pieceAreaHeight, int goalAreaHeight, GoalFieldType defaultGoalType = GoalFieldType.unknown)
        {
            this.width = width;
            this.taskAreaHeight = pieceAreaHeight;
            this.goalAreaHeight = goalAreaHeight;
            fields = new Field[width,pieceAreaHeight + 2 * goalAreaHeight];
            for (int i = 0; i < GoalAreaHeight; i++)
            {
                for (int j = 0; j < BoardWidth; j++)
                {
                    fields[j, i] = new GoalField(j, i, TeamColour.blue, defaultGoalType);
                    fields[j, i + pieceAreaHeight + goalAreaHeight] = new GoalField(j, i + pieceAreaHeight + goalAreaHeight, TeamColour.red, defaultGoalType);
                }
            }
            for (int i = GoalAreaHeight; i < BoardHeight - GoalAreaHeight; i++)
            {
                for (int j = 0; j < BoardWidth; j++)
                {
                    fields[j, i] = new TaskField(j, i);
                }
            }
        }

        public void UpdatePieces(Piece[] pieceArray)
        {
            List<ulong> piecesIds = pieceArray.ToList().Select(p => p.id).ToList();
            List<TaskField> TaskFieldsList = TaskFields;
            
            foreach (var taskField in TaskFieldsList)
            {
                if (piecesIds.Contains(taskField.GetPiece.id))
                    // taskField has been received
                {
                    var receivedPiece = pieceArray.ToList().Where(p => p.id == taskField.GetPiece.id).FirstOrDefault();
                    if (receivedPiece.timestamp > taskField.GetPiece.timestamp)
                        // received version is more up to date
                    {
                        taskField.SetPiece(receivedPiece);
                    }
                }
            }
        }

        public void UpdateTaskFields(TaskField[] taskFieldsArray)
        {

            foreach(var field in taskFieldsArray.ToList())
            {
                int xCoord = field.x;
                int yCoord = field.y;
                var currentField = GetField(xCoord, yCoord) as TaskField;

                if (currentField != null && currentField.TimeStamp < field.TimeStamp)
                {
                    fields[xCoord, yCoord] = field;
                    

                    //GetTaskField(xCoord, yCoord).x = field.x;
                    //GetTaskField(xCoord, yCoord).y = field.y;
                    //GetTaskField(xCoord, yCoord).UpdateTimeStamp(field.TimeStamp);
                    //GetTaskField(xCoord, yCoord).Distance = field.Distance;
                }

            }
        }

        public void UpdateGoalFields(GoalField[] goalFieldsArray)
        {

            foreach (var field in goalFieldsArray.ToList())
            {
                int xCoord = field.x;
                int yCoord = field.y;
                var currentField = GetGoalField(xCoord, yCoord);

                if (currentField != null && currentField.TimeStamp < field.TimeStamp)
                {
                    fields[xCoord, yCoord] = field;
                }

            }
        }
    }
}
