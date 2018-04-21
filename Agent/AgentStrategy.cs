using GameArea;
using Messages;
using System;
using System.Collections.Generic;
using System.Text;

namespace Player
{
    public partial class Player
    {
        public void DoStrategy()
        {
             if (!HasValidPiece)
            {
                FindAndPickPiece();
            }
            if(HasValidPiece)
            {
                FullfillGoal();
            }
        }

        public void FindAndPickPiece()
        {
            if(!OnPiece && !HasPiece)
                GoToNearestPiece(); //move to piece
            else if (OnPiece && !HasPiece)
                TryPickPiece(); //possible that piece has gone
            else if(HasUnknownPiece)
                TryTestPiece(); //test if valid
            else if(HasShamPiece)
                DestroyPiece();
        }

        public void GoToNearestPiece() //makes moves until is not on piece
        {
            if (InGoalArea)
                GoToTaskArea(team);
            else
            {
                if (!OnPiece)
                {
                    if (gameFinished)
                    {
                        return;
                    }
                    Discover(gameMaster);
                    if (OnPiece)
                        return;
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
            
        }

        public bool TryPickPiece()
        {
            return PickUpPiece(gameMaster);
        }

        public bool TryTestPiece()
        {
            return TestPiece(gameMaster);
        }

        public bool DestroyPiece()
        {
            return Destroy(gameMaster);
        }

        public void FullfillGoal()
        {
            if (InTaskArea)
                GoToGoalArea(team);
            else if(InGoalArea && GetCurrentGoalField.GoalType != GoalFieldType.unknown)
                GoToNotFullfilledGoal();
            else if(InGoalArea && GetCurrentGoalField.GoalType == GoalFieldType.unknown)
                TryPlacePiece();
        }

        public void GoToNotFullfilledGoal()
        {
            var currentGoal = GetCurrentGoalField;
            if (currentGoal.GoalType == GoalFieldType.unknown)
                return;
            MoveType direction = GetClosestUnknownGoalDirection();
            var moved = TryMove(direction);
            if (!moved)
            {
                TryMove(direction,true);
            }
        }

        public bool TryPlacePiece()
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
