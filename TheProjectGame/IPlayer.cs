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
        bool UpdateLocalBoard(Data receivedData, ActionType action);
        void RegisteredGames(RegisteredGames messageObject);
        void ConfirmJoiningGame(ConfirmJoiningGame messageObject);
        void GameStarted(Game messageObject);
        void GameMasterDisconnected(GameMasterDisconnected messageObject);
    }
}
