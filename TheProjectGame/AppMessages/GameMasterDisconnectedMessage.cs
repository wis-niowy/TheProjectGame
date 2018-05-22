using Messages;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameArea.AppMessages
{
    public class GameMasterDisconnectedMessage : IToBase<Messages.GameMasterDisconnectedMessage>
    {
        public ulong GameId { get; set; }
        public GameMasterDisconnectedMessage(ulong id)
        {
            GameId = id;
        }
        public string Serialize()
        {
            return MessageParser.Serialize(ToBase());
        }

        public Messages.GameMasterDisconnectedMessage ToBase()
        {
            return new Messages.GameMasterDisconnectedMessage()
            {
                gameId = GameId
            };
        }
    }
}
