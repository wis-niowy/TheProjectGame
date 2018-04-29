using GameArea.AppMessages;
using Messages;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameArea
{
   public enum ActionType
    {
        none,
        TestPiece,
        PlacePiece,
        PickUpPiece,
        Move,
        Discover,
        Destroy
    };

    public enum AgentState
    {
        SearchingForGame,
        Joining,
        AwaitingForStart,
        Playing,
        Dead
    }
    public interface IPlayer
    {
        AgentState State { get; set; }
        ActionType? LastActionTaken { get; set; }
        MoveType? LastMoveTaken { get; set; }

        void DoStrategy();
        bool UpdateLocalBoard(DataMessage receivedData, ActionType action, MoveType direction = MoveType.up);
        void RegisteredGames(RegisteredGamesMessage messageObject);
        void ConfirmJoiningGame(ConfirmJoiningGameMessage messageObject);
        void GameStarted(AppMessages.GameMessage messageObject);
        void GameMasterDisconnected(AppMessages.GameMasterDisconnectedMessage messageObject);
    }
}
