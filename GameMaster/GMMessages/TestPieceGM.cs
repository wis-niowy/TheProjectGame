using GameArea;
using GameArea.AppMessages;
using Messages;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameMaster.GMMessages
{
    public class TestPieceGM : TestPieceMessage, IGMMessage
    {
        public bool Prioritised => false;
        public string GUID => PlayerGUID;
        public TestPieceGM(string guid, ulong gameID) : base(guid, gameID) { }

        public string[] Process(IGameMaster gameMaster)
        {
            if (gameMaster.GameEndDate > ReceiveDate || gameMaster.GameStartDate > ReceiveDate || gameMaster.IsGameFinished)
            {
                return null;
            }
            return new string[] { gameMaster.HandleTestPieceRequest(this)?.Serialize() };
        }
    }
}
