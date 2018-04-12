using Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Player
{
    public partial class Agent
    {
        public GameArea.TaskField GetTaskFromDirection(MoveType direction)
        {
            switch (direction)
            {
                case MoveType.left:
                    return agentBoard.GetTaskField(location.x - 1, location.y);
                case MoveType.right:
                    return agentBoard.GetTaskField(location.x + 1, location.y);
                case MoveType.up:
                    return agentBoard.GetTaskField(location.x, location.y + 1);
                case MoveType.down:
                    return agentBoard.GetTaskField(location.x, location.y - 1);
            }

            return null;

        }

        public GameArea.TaskField GetCurrentTaksField
        {
            get
            {
                return agentBoard.GetTaskField(location);
            }
        }

        public GameArea.GoalField GetCurrentGoalField
        {
            get
            {
                return agentBoard.GetGoalField(location);
            }
        }

        public bool InGoalArea
        {
            get
            {
                return agentBoard.GetField(location.x, location.y) is GameArea.GoalField;
            }
        }

        public bool InTaskArea
        {
            get
            {
                return agentBoard.GetField(location.x, location.y) is GameArea.TaskField;
            }
        }

        public bool OnPiece
        {
            get
            {
                var field = agentBoard.GetField(location.x, location.y);
                if (field is GameArea.TaskField)
                {
                    return ((GameArea.TaskField)field).GetPiece != null;
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
            return (new GoalDirectionInfo(agentBoard.GoalFields(team),team,location).GetClosestDirection());
        }

       
    }


    public class TaskDirectionInfo
    {
        GameArea.TaskField Left { get; set; }
        GameArea.TaskField Right { get; set; }
        GameArea.TaskField Up { get; set; }
        GameArea.TaskField Down { get; set; }

        public TaskDirectionInfo(GameArea.TaskField left, GameArea.TaskField right, GameArea.TaskField up, GameArea.TaskField down)
        {
            Left = left;
            Right = right;
            Up = up;
            Down = down;
        }

        public MoveType GetClosestDirection()
        {
            MoveType closestDirection = MoveType.up;
            var minDistance = Up != null ? Up.Distance : int.MaxValue;
            if (Down != null && minDistance > Down.Distance)
                closestDirection = MoveType.down;
            if (Left != null && minDistance > Left.Distance)
                closestDirection = MoveType.left;
            if (Right != null && minDistance > Right.Distance)
                closestDirection = MoveType.right;
            return closestDirection;
        }

        public MoveType GetClosesUpDownDirection() //requires that Discovery was succesfull, last move was left/right
        {
            MoveType closestDirection = MoveType.up;
            var minDistance = Up != null ? Up.Distance : int.MaxValue;
            if (Down != null && minDistance > Down.Distance)
                closestDirection = MoveType.down;
            return closestDirection;
        }

        public MoveType GetClosesLeftRightDirection() //requires that Discovery was succesfull, last move was up/down
        {
            MoveType closestDirection = MoveType.left;
            var minDistance = Left != null ? Left.Distance : int.MaxValue;
            if (Right != null && minDistance > Right.Distance)
                closestDirection = MoveType.right;
            return closestDirection;
        }
    }

    public class GoalDirectionInfo
    {
        List<GameArea.GoalField> Goals { get; set; }
        Location AgentLocation { get; set; }
        TeamColour Team { get; set; }

        public GoalDirectionInfo(List<GameArea.GoalField> goals, TeamColour team, Location agentLocation)
        {
            AgentLocation = agentLocation;
            Team = team;
            if (team == TeamColour.red)
            {
                Goals = goals.OrderBy(q => q.y).ThenBy(q => q.x).Where(q => q.GoalType == GoalFieldType.unknown).ToList();
            }
            else
            {
                Goals = goals.OrderByDescending(q => q.y).ThenBy(q => q.x).Where(q => q.GoalType == GoalFieldType.unknown).ToList();
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
        private int GetDistance(Location goalLocation)
        {
            return (int)(Math.Abs((int)goalLocation.x - (int)AgentLocation.x) + Math.Abs((int)goalLocation.y - (int)AgentLocation.y));
        }

        private MoveType GetDirectionToGoal(GameArea.GoalField goal)
        {
            MoveType direction = MoveType.left;
            long xDiff = Math.Abs((int)goal.x - (int)AgentLocation.x);
            long yDiff = Math.Abs((int)goal.y - (int)AgentLocation.y);

            if (xDiff < yDiff) //move in y axis
            {
                if ((int)goal.y - (int)AgentLocation.y > 0)
                {
                    direction = MoveType.up;
                }
                else
                    direction = MoveType.down;
            }
            else
            {
                if ((int)goal.x - (int)AgentLocation.x > 0)
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
