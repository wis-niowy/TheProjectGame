using GameArea.GameObjects;
using Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Player
{
    public partial class Player
    {
        public GameArea.GameObjects.TaskField GetTaskFromDirection(MoveType direction)
        {
            switch (direction)
            {
                case MoveType.left:
                    return PlayerBoard.GetTaskField(location.X - 1, location.Y);
                case MoveType.right:
                    return PlayerBoard.GetTaskField(location.X + 1, location.Y);
                case MoveType.up:
                    return PlayerBoard.GetTaskField(location.X, location.Y + 1);
                case MoveType.down:
                    return PlayerBoard.GetTaskField(location.X, location.Y - 1);
            }

            return null;

        }

        public GameArea.GameObjects.TaskField GetCurrentTaksField
        {
            get
            {
                return PlayerBoard.GetTaskField(location);
            }
        }

        public GameArea.GameObjects.GoalField GetCurrentGoalField
        {
            get
            {
                return PlayerBoard.GetGoalField(location);
            }
        }

        public bool InGoalArea
        {
            get
            {
                return PlayerBoard.GetField(location.X, location.Y) is GameArea.GameObjects.GoalField;
            }
        }

        public bool InTaskArea
        {
            get
            {
                return PlayerBoard.GetField(location.X, location.Y) is GameArea.GameObjects.TaskField;
            }
        }

        public bool OnPiece
        {
            get
            {
                var field = PlayerBoard.GetField(location.X, location.Y);
                if (field is GameArea.GameObjects.TaskField)
                {
                    return ((GameArea.GameObjects.TaskField)field).Piece != null;
                }
                else
                    return false;
            }
        }


        public void GoToTaskArea(TeamColour color)
        {
            if (!InTaskArea)
                switch (color)
            {
                case TeamColour.red:
                    TryMove(MoveType.down,true);
                    break;
                case TeamColour.blue:
                    TryMove(MoveType.up,true);
                    break;
            }

        }

        public void GoToGoalArea(TeamColour color)
        {
            if (!InGoalArea)
            {
                switch (color)
                {
                    case TeamColour.red:
                        TryMove(MoveType.up, true);
                        break;
                    case TeamColour.blue:
                        TryMove(MoveType.down, true);
                        break;
                }
            }
        }

        public bool TryMove(MoveType direction, bool untilSucces = false)
        {
            if (untilSucces) //try to move until succes; if can't move - go Alternative1 or Alternative2 and then try move 3 times, if fail then go back and return
            {
                bool moved = Move(gameMaster, direction);
                for (int i = 0; i < 3 && !moved; i++)
                {
                    
                    if (!Move(gameMaster, Alternative1(direction)))
                        Move(gameMaster, Alternative2(direction));
                    moved = Move(gameMaster, direction);
                }
                if (!moved)
                    Move(gameMaster, OppositeDirection(direction));
                return moved;
            }
            else //only try, if not possible - don't try to force move
            {
                return Move(gameMaster, direction);
            }
        }

        public MoveType OppositeDirection(MoveType direction)
        {
            switch (direction)
            {
                case MoveType.up: return MoveType.down;
                case MoveType.down: return MoveType.up;

                case MoveType.left: return MoveType.right;
                case MoveType.right: 
                default: return MoveType.left;
            }
        }

        public MoveType Alternative1(MoveType direction)
        {
            switch (direction)
            {
                case MoveType.up:
                case MoveType.down: return MoveType.left;

                case MoveType.left:
                case MoveType.right:
                default: return MoveType.up;
            }
        }

        public MoveType Alternative2(MoveType direction)
        {
            switch(direction)
            {
                case MoveType.up:
                case MoveType.down: return MoveType.right;

                case MoveType.left:
                case MoveType.right:
                default: return MoveType.down;
            }
        }

        public MoveType FindNearestPieceDirection()
        {
            var directionInfo = GetTaskDirectionInfo();
            return directionInfo.GetClosestDirection();
        }
        public MoveType FindNearestPieceUpDownDirection()
        {
            var directionInfo = GetTaskDirectionInfo();
            return directionInfo.GetClosesUpDownDirection();
        }
        public MoveType FindNearestPieceLeftRightDirection()
        {
            var directionInfo = GetTaskDirectionInfo();
            return directionInfo.GetClosesLeftRightDirection();
        }

        public TaskDirectionInfo GetTaskDirectionInfo()
        {
            return new TaskDirectionInfo(GetTaskFromDirection(MoveType.left),
                                                  GetTaskFromDirection(MoveType.right),
                                                  GetTaskFromDirection(MoveType.up),
                                                  GetTaskFromDirection(MoveType.down));
        }

        public MoveType GetSecondClosestDirection(MoveType firstMove)
        {
            if (firstMove == MoveType.down || firstMove == MoveType.up)
                return FindNearestPieceLeftRightDirection();
            else
                return FindNearestPieceUpDownDirection();
        }

        public MoveType GetClosestUnknownGoalDirection()
        {
            return (new GoalDirectionInfo(PlayerBoard.GoalFields(team),team,location).GetClosestDirection());
        }

       
    }


    public class TaskDirectionInfo
    {
        GameArea.GameObjects.TaskField Left { get; set; }
        GameArea.GameObjects.TaskField Right { get; set; }
        GameArea.GameObjects.TaskField Up { get; set; }
        GameArea.GameObjects.TaskField Down { get; set; }

        public TaskDirectionInfo(GameArea.GameObjects.TaskField left, GameArea.GameObjects.TaskField right, GameArea.GameObjects.TaskField up, GameArea.GameObjects.TaskField down)
        {
            Left = left;
            Right = right;
            Up = up;
            Down = down;
        }

        public MoveType GetClosestDirection()
        {
            MoveType closestDirection = MoveType.up;
            var minDistance = Up != null ? Up.DistanceToPiece : int.MaxValue;
            if (Down != null && minDistance > Down.DistanceToPiece)
                closestDirection = MoveType.down;
            if (Left != null && minDistance > Left.DistanceToPiece)
                closestDirection = MoveType.left;
            if (Right != null && minDistance > Right.DistanceToPiece)
                closestDirection = MoveType.right;
            return closestDirection;
        }

        public MoveType GetClosesUpDownDirection() //requires that Discovery was succesfull, last move was left/right
        {
            MoveType closestDirection = MoveType.up;
            var minDistance = Up != null ? Up.DistanceToPiece : int.MaxValue;
            if (Down != null && minDistance > Down.DistanceToPiece)
                closestDirection = MoveType.down;
            return closestDirection;
        }

        public MoveType GetClosesLeftRightDirection() //requires that Discovery was succesfull, last move was up/down
        {
            MoveType closestDirection = MoveType.left;
            var minDistance = Left != null ? Left.DistanceToPiece : int.MaxValue;
            if (Right != null && minDistance > Right.DistanceToPiece)
                closestDirection = MoveType.right;
            return closestDirection;
        }
    }

    public class GoalDirectionInfo
    {
        List<GameArea.GameObjects.GoalField> Goals { get; set; }
        GameArea.GameObjects.Location PlayerLocation { get; set; }
        TeamColour Team { get; set; }

        public GoalDirectionInfo(List<GameArea.GameObjects.GoalField> goals, TeamColour team, GameArea.GameObjects.Location _PlayerLocation)
        {
            PlayerLocation = _PlayerLocation;
            Team = team;
            if (team == TeamColour.red)
            {
                Goals = goals.OrderBy(q => q.Y).ThenBy(q => q.X).Where(q => q.Type == GoalFieldType.unknown).ToList();
            }
            else
            {
                Goals = goals.OrderByDescending(q => q.Y).ThenBy(q => q.X).Where(q => q.Type == GoalFieldType.unknown).ToList();
            }
        }

        public MoveType GetClosestDirection()
        {
            var closestGoal = Goals.Select(q => new
            {
                goal = q,
                distance = GetDistance(q)
            }).OrderBy(q => q.distance).Select(q=>q.goal).FirstOrDefault();
            return GetDirectionToGoal(closestGoal);
        }
        private int GetDistance(GameArea.GameObjects.Location goalLocation)
        {
            return (int)(Math.Abs(goalLocation.X - PlayerLocation.X) + Math.Abs(goalLocation.Y - PlayerLocation.Y));
        }

        private MoveType GetDirectionToGoal(GameArea.GameObjects.GoalField goal)
        {
            MoveType direction = MoveType.left;
            long xDiff = Math.Abs(goal.X - PlayerLocation.X);
            long yDiff = Math.Abs(goal.Y - PlayerLocation.Y);

            if (xDiff < yDiff) //move in y axis
            {
                if (goal.Y - PlayerLocation.Y > 0)
                {
                    direction = MoveType.up;
                }
                else
                    direction = MoveType.down;
            }
            else
            {
                if (goal.X - PlayerLocation.X > 0)
                {
                    direction = MoveType.right;
                }
                else
                    direction = MoveType.left;
            }

            return direction;
        }

        private MoveType GetOppositeDirection(MoveType direction)
        {
            switch (direction)
            {
                case MoveType.left:
                    return MoveType.right;
                case MoveType.right:
                    return MoveType.left;
                case MoveType.up:
                    return MoveType.down;
                case MoveType.down:
                    return MoveType.up;
            }
            return direction;
        }
    }

}
