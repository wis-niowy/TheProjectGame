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

        Data HandleTestPieceRequest(TestPiece msg);
        Data HandlePlacePieceRequest(PlacePiece msg);
        Data HandlePickUpPieceRequest(PickUpPiece msg);
        Data HandleMoveRequest(Move msg);
        Data HandleDiscoverRequest(Discover msg);
        Data HandleDestroyPieceRequest(DestroyPiece msg);

        string[] HandleActionRequest(string s);
    }
}
