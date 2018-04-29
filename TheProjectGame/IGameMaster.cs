using GameArea.AppMessages;
using Messages;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameArea
{
    public enum GameMasterState {RegisteringGame, AwaitingPlayers, GameInprogress, GameOver };
    public interface IGameMaster
    {
        GameMasterState State { get; set; }

        DataMessage HandleTestPieceRequest(TestPieceMessage msg);
        DataMessage HandlePlacePieceRequest(PlacePieceMessage msg);
        DataMessage HandlePickUpPieceRequest(PickUpPieceMessage msg);
        DataMessage HandleMoveRequest(MoveMessage msg);
        DataMessage HandleDiscoverRequest(DiscoverMessage msg);
        DataMessage HandleDestroyPieceRequest(DestroyPieceMessage msg);

        string[] HandleActionRequest(string s);
        RegisterGameMessage RegisterGame();
    }
}
