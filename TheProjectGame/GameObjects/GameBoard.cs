using Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameArea.GameObjects
{
    public class GameBoard : IToBase<Messages.GameBoard>
    {
        public int Width { get; set; }
        public int TaskAreaHeight { get; set; }
        public int GoalAreaHeight { get; set; }
        public int Height { get { return TaskAreaHeight + 2 * GoalAreaHeight; }  }

        public List<TaskField> TaskFields
        {
            get
            {
                var taskFields = new List<TaskField>();
                for (int i = GoalAreaHeight; i < GoalAreaHeight + TaskAreaHeight; i++)
                {
                    for (int j = 0; j < Width; j++)
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
            for (int i = 0; i < Height; i++)
            {
                for (int j = 0; j < Width; j++)
                {
                    if (fields[j, i] is GoalField && ((GoalField)fields[j, i]).Team == team)
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
                for (int i = 0; i < GoalAreaHeight; i++)
                {
                    for (int j = 0; j < Width; j++)
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
                for (int i = GoalAreaHeight + TaskAreaHeight; i < Height; i++)
                {
                    for (int j = 0; j < Width; j++)
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
            if (x >= Width || y >= Height || x < 0 || y < 0)
                return null;
            return fields[x, y];
        }

        public void SetGoalField(GameMasterGoalField goalField)
        {
            fields[goalField.X, goalField.Y] = goalField;
        }

        public TaskField GetTaskField(int x, int y)
        {
            if (y < GoalAreaHeight || y >= GoalAreaHeight + TaskAreaHeight || x >= Width || x < 0)
                return null;
            return (TaskField)fields[x, y];
        }

        public TaskField GetTaskField(Location location)
        {
            if (location == null)
                return null;
            return GetTaskField(location.X, location.Y);
        }

        public GoalField GetGoalField(int x, int y)
        {
            if ((y >= GoalAreaHeight && y < GoalAreaHeight + TaskAreaHeight) || x >= Width || x < 0)
                return null;
            return (GoalField)fields[x, y];
        }

        public GoalField GetGoalField(Location location)
        {
            if (location == null)
                return null;
            return GetGoalField(location.X, location.Y);
        }

        public Messages.GameBoard ToBase()
        {
            return new Messages.GameBoard()
            {
                goalsHeight = (uint)GoalAreaHeight,
                tasksHeight = (uint)TaskAreaHeight,
                width = (uint)Width
            };
        }

        public string Serialize()
        {
            return MessageParser.Serialize(ToBase());
        }

        private Field[,] fields;
        public GameBoard(int width, int taskAreaHeight, int goalAreaHeight, GoalFieldType defaultGoalType = GoalFieldType.unknown)
        {
            Width = width;
            TaskAreaHeight = taskAreaHeight;
            GoalAreaHeight = goalAreaHeight;
            fields = new Field[width, taskAreaHeight + 2 * goalAreaHeight];
            for (int i = 0; i < GoalAreaHeight; i++)
            {
                for (int j = 0; j < Width; j++)
                {
                    fields[j, i] = new GoalField(j, i, DateTime.Now, TeamColour.blue, defaultGoalType);
                    fields[j, i + taskAreaHeight + goalAreaHeight] = new GoalField(j, i + taskAreaHeight + goalAreaHeight, DateTime.Now, TeamColour.red, defaultGoalType);
                }
            }
            for (int i = GoalAreaHeight; i < Height - GoalAreaHeight; i++)
            {
                for (int j = 0; j < Width; j++)
                {
                    fields[j, i] = new TaskField(j, i,DateTime.Now);
                }
            }
        }

        public void UpdatePieces(Piece[] pieceArray)
        {
            // array with one 'null' element is received, when Player attempts to pick up piece from an empty field
            if (pieceArray == null || (pieceArray.Length == 1 && pieceArray[0] == null))
                return;

                List<ulong> piecesIds = pieceArray.Select(p => p.ID).ToList();
                List<TaskField> TaskFieldsList = TaskFields;

                foreach (var taskField in TaskFieldsList)
                {
                    if (taskField.Piece != null && piecesIds.Contains(taskField.Piece.ID))
                    // taskField has been received
                    {
                        var receivedPiece = pieceArray.Where(p => p.ID == taskField.Piece.ID).FirstOrDefault();
                        if (receivedPiece.TimeStamp > taskField.Piece.TimeStamp)
                        // received version is more up to date
                        {
                            taskField.Piece = receivedPiece;
                        }
                    }
                }
        }

        public void UpdateTaskFields(TaskField[] taskFieldsArray)
        {
            // array with one 'null' element is received, when Player attempts to place piece on occupied field
            if (taskFieldsArray == null || (taskFieldsArray.Length == 1 && taskFieldsArray[0] == null))
                return;

            
                foreach (var field in taskFieldsArray)
                {
                    int xCoord = field.X;
                    int yCoord = field.Y;
                    var currentField = GetField(xCoord, yCoord) as TaskField;

                    if (currentField != null && currentField.TimeStamp < field.TimeStamp)
                    {
                        fields[xCoord, yCoord] = field;
                    }
                }
        }

        public void UpdateGoalFields(GameObjects.GoalField[] goalFieldsArray)
        {
            if (goalFieldsArray == null)
                return;
                foreach (var field in goalFieldsArray)
                {
                    int xCoord = field.X;
                    int yCoord = field.Y;
                    var currentField = GetGoalField(xCoord, yCoord);

                    if (currentField != null && currentField.TimeStamp < field.TimeStamp)
                    {
                        fields[xCoord, yCoord] = field;
                    }
                }
        }

    }
}
