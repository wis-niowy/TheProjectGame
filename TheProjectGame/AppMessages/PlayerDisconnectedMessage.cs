using Messages;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameArea.AppMessages
{
    public class PlayerDisconnectedMessage : IToBase<PlayerDisconnected>
    {
        public ulong PlayerID { get; set; }
        public PlayerDisconnectedMessage(ulong id)
        {
            PlayerID = id;
        }
        public string Serialize()
        {
            return MessageParser.Serialize(ToBase());
        }

        public PlayerDisconnected ToBase()
        {
            return new PlayerDisconnected()
            {
                playerId = PlayerID
            };
        }
    }
}
