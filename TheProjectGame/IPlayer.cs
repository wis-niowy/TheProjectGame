using GameArea.AppConfiguration;
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
        Destroy,
        SearchingForGame,
        Joining
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

        ActionType ActionToComplete { get; set; }
        PlayerSettingsConfiguration Settings { get; set; }

        void DoStrategy();
        bool UpdateLocalBoard(DataMessage receivedData, ActionType action);
        void RegisteredGames(RegisteredGamesMessage messageObject);
        void ConfirmJoiningGame(ConfirmJoiningGameMessage messageObject);
        void GameStarted(AppMessages.GameMessage messageObject);
        void GameMasterDisconnected(AppMessages.GameMasterDisconnectedMessage messageObject);

        void ErrorMessage(AppMessages.ErrorMessage error);
    }
}
