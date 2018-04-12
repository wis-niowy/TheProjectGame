using Messages;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameArea
{
    public interface IGameMaster
    {
        Data HandleTestPieceRequest(string playerGuid, ulong gameId);
        Data HandlePlacePieceRequest(string playerGuid, ulong gameId);
        Data HandlePickUpPieceRequest(string playerGuid, ulong gameId);
        Data HandleMoveRequest(MoveType direction, string playerGuid, ulong gameId);
        Data HandleDiscoverRequest(string playerId, ulong gameGuid);
        Data HandleDestroyPieceRequest(string playerId, ulong gameGuid);
    }
}
