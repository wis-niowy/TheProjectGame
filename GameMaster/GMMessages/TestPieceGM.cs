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
        public TestPieceGM(string guid, ulong gameID) : base(guid, gameID) { }

        public string[] Process(IGameMaster controller)
        {
            return new string[] { controller.HandleTestPieceRequest(this).Serialize() };
        }
    }
}
