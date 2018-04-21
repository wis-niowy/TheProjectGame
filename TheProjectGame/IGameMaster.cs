using Messages;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameArea
{
    public interface IGameMaster
    {
        Data HandleTestPieceRequest(TestPiece msg);
        Data HandlePlacePieceRequest(PlacePiece msg);
        Data HandlePickUpPieceRequest(PickUpPiece msg);
        Data HandleMoveRequest(Move msg);
        Data HandleDiscoverRequest(Discover msg);
        Data HandleDestroyPieceRequest(string playerId, ulong gameGuid);

        string HandleActionRequest(string s);
    }
}
