using Messages;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameArea
{
    public interface IGameMaster
    {
        Data HandleTestPieceRequest(ulong playerGuid, ulong gameId);
        Data HandlePlacePieceRequest(ulong playerGuid, ulong gameId);
        Data HandlePickUpPieceRequest(ulong playerGuid, ulong gameId);
        Data HandleMoveRequest(MoveType direction, ulong playerGuid, ulong gameId);
        Data HandleDiscoverRequest(ulong playerId, ulong gameGuid);
    }
}
