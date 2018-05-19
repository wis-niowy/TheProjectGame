using GameArea;
using GameArea.AppMessages;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameMaster.GMMessages
{
    public class DestroyPieceGM : DestroyPieceMessage, IGMMessage
    {
        public bool Prioritised => false;
        public string GUID => PlayerGUID;
        public DestroyPieceGM(string guid, ulong gameId) : base(guid, gameId)
        {
        }

        public string[] Process(IGameMaster gameMaster)
        {
            return new string[] { gameMaster.HandleDestroyPieceRequest(this)?.Serialize() };
        }
    }
}
