using Messages;
using System;
using System.Collections.Generic;
using System.Text;

namespace Player
{
    public partial class Agent
    {
        public void DoStrategy()
        {
            if (!HasPiece)
            {
                FindAndPickPiece();
            }
            if(HasPiece)
            {
                FullfillGoal();
            }
        }

        private void FindAndPickPiece()
        {
            while (!HasPiece)
            {
                if (gameFinished)
                {
                    gameFinished = true;
                    break;
                }
                GoToNearestPiece(); //move to piece
                if (gameFinished)
                {
                    gameFinished = true;
                    break;
                }
                if(OnPiece)
                    TryPickPiece(); //possible that piece has gone
                if (gameFinished)
                {
                    gameFinished = true;
                    break;
                }
                if (HasPiece)
                {
                    TryTestPiece();
                }
                if (gameFinished)
                {
                    if (gameFinished)
                    {
                        gameFinished = true;
                        break;
                    }
                    gameFinished = true;
                    break;
                }
            }

        }

        private void GoToNearestPiece() //makes moves until is not on piece
        {
            if (InGoalArea)
            {
                GoToTaskArea();
            }
            if(!OnPiece)
            {
                if (gameFinished)
                {
                    return;
                }
                Discover(gameMaster);
                MoveType direction = FindNearestPieceDirection();
                MoveType secondDirection = direction;
                var moved = TryMove(direction);
                if (!moved && !OnPiece)
                {
                    TryMove(direction); //try again
                }
                else 
                    return; //something is blocking action, repeat discovery and move
                MoveType possibleDirection = GetSecondClosestDirection(direction);
                var possibleTask = GetTaskFromDirection(possibleDirection);
                if (possibleTask != null && possibleTask.Distance < GetCurrentTaksField.Distance)
                    secondDirection = possibleDirection;
                moved = TryMove(secondDirection);
                if (!moved && !OnPiece)
                {
                    TryMove(secondDirection); //try again
                }
                //end of loop, try move to piece again, until not on piece
            }

        }

        private bool TryPickPiece()
        {
            return PickUpPiece(gameMaster);
        }

        private bool TryTestPiece()
        {
            return TestPiece(gameMaster);
        }

        private void FullfillGoal()
        {
            while(HasPiece)
            {
                if (gameFinished)
                {
                    gameFinished = true;
                    break;
                }
                GoToGoalArea();
                if (gameFinished)
                {
                    gameFinished = true;
                    break;
                }
                if(InGoalArea)
                    GoToNotFullfilledGoal();
                if (gameFinished)
                {
                    gameFinished = true;
                    break;
                }
                if (GetCurrentGoalField.GoalType == GoalFieldType.unknown)
                {
                    if (gameFinished)
                    {
                        gameFinished = true;
                        break;
                    }
                    TryPlacePiece();
                }
            }
        }

        private void GoToNotFullfilledGoal()
        {
            var currentGoal = GetCurrentGoalField;
            if (currentGoal.GoalType == GoalFieldType.unknown)
                return;
            MoveType direction = GetClosestUnknownGoalDirection();
            var moved = TryMove(direction);
            if (!moved)
            {
                TryMove(direction);
            }
        }

        private bool TryPlacePiece()
        {
            var goalFullfilled = PlacePiece(gameMaster);
            if(!goalFullfilled) //found non-goal field in goalarea
            {
                GetCurrentGoalField.GoalType = GoalFieldType.nongoal;
                GetCurrentGoalField.UpdateTimeStamp(DateTime.Now.AddYears(100));
            }
            return goalFullfilled;
        }
    }
}
