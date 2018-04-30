using GameArea;
using GameArea.AppMessages;
using Messages;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameMaster.GMMessages
{
    public class PlacePieceGM : PlacePieceMessage, IGMMessage
    {
        public PlacePieceGM(string guid, ulong gameId) : base(guid, gameId)
        {
        }

        public string[] Process(IGameMaster gameMaster)
        {
            return new string[] { gameMaster.HandlePlacePieceRequest(this).Serialize() };
        }
    }
}
