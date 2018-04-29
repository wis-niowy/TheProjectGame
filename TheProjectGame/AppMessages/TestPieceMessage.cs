using GameArea.ControllerInterfaces;
using GameArea.MessageInterfaces;
using GameArea.Parsers;
using Messages;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameArea.AppMessages
{
    public class TestPieceMessage : GameAbstractMessage, IToBase<TestPiece>
    {
        public TestPieceMessage(TestPiece test):base(test) { }
        public virtual string Serialize()
        {
            return MessageParser.Serialize(ToBase());
        }

        public TestPiece ToBase()
        {
            return new Messages.TestPiece()
            {
                gameId = GameId,
                playerGuid = PlayerGUID
            };
        }
    }
}
