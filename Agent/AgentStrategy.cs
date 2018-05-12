using GameArea;
using GameArea.AppMessages;
using GameArea.Parsers;
using Messages;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Player
{
    public partial class Player
    {
        public void DoStrategy()
        {
            while (State != AgentState.Dead)
            {
                switch (State)
                {
                    case AgentState.SearchingForGame:
                        ActionToComplete = ActionType.SearchingForGame;
                        Controller.BeginSend(new GetGamesMessage().Serialize());
                        break;
                    case AgentState.Joining:
                        TryJoinGame();
                        break;
                    case AgentState.AwaitingForStart:
                        //nic nie rób, czekaj na wiadomość Game
                        break;
                    case AgentState.Playing:
                        if (ActionToComplete == ActionType.none)
                        {
                            if (!HasValidPiece)
                            {
                                FindAndPickPiece();
                            }
                            if (HasValidPiece)
                            {
                                FullfillGoal();
                            }
                        }
                        break;
                    case AgentState.Dead:
                        //agent martwy
                        break;
                }
                WaitForActionComplete();
            }
        }

        private void TryJoinGame()
        {
            if (GamesList == null || GamesList.Count == 0)
            {
                State = AgentState.SearchingForGame;
                Thread.Sleep(Settings.RetryJoinGameInterval);
                //nie ustawiamy akcji, strategia sama dojdzie do tego co ma zrobić
            }
            else
            {
                var game = GamesList[0];
                GamesList.RemoveAt(0);
                Controller.BeginSend(new JoinGameMessage(game.GameName,Team, Role).Serialize());
                ActionToComplete = ActionType.Joining;
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
                GoToTaskArea(Team);
            else
            {
                if (!OnPiece)
                {
                    if (gameFinished)
                    {
                        return;
                    }
                    Discover();
                    if (OnPiece)
                        return;
                    MoveType direction = FindNearestPieceDirection();
                    var moved = TryMove(direction);
                    if (!moved)
                    {
                        TryMove(direction); //try again
                    }
                    if (OnPiece)
                        return;
                    MoveType possibleDirection = GetSecondClosestDirection(direction);
                    var possibleTask = GetTaskFromDirection(possibleDirection);
                    if (possibleTask != null && possibleTask.DistanceToPiece < GetCurrentTaksField.DistanceToPiece)
                    {
                        moved = TryMove(possibleDirection);
                        if (!moved)
                        {
                            TryMove(possibleDirection); //try again
                        }
                    }
                    //end of loop, try move to piece again, until not on piece
                }
            }
        }

        public bool TryPickPiece()
        {
            return PickUpPiece();
        }

        public bool TryTestPiece()
        {
            return TestPiece();
        }

        public bool DestroyPiece()
        {
            return Destroy();
        }

        public void FullfillGoal()
        {
            if (InTaskArea)
                GoToGoalArea(Team);
            else if(InGoalArea && GetCurrentGoalField.Type != GoalFieldType.unknown)
                GoToNotFullfilledGoal();
            else if(InGoalArea && GetCurrentGoalField.Type == GoalFieldType.unknown)
                TryPlacePiece();
        }

        public void GoToNotFullfilledGoal()
        {
            var currentGoal = GetCurrentGoalField;
            if (currentGoal.Type == GoalFieldType.unknown)
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
            var goalFullfilled = PlacePiece();
            if(!goalFullfilled && GetCurrentGoalField != null) //found non-goal field in goalarea
            {
                GetCurrentGoalField.Type = GoalFieldType.nongoal;
                GetCurrentGoalField.TimeStamp = DateTime.Now.AddYears(100);
            }
            return goalFullfilled;
        }
    }
}
