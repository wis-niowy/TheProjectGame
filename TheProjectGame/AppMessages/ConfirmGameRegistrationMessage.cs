using Messages;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameArea.AppMessages
{
    public class ConfirmGameRegistrationMessage : IToBase<ConfirmGameRegistration>
    {
        public ulong GameId { get; set; }
        public ConfirmGameRegistrationMessage(ConfirmGameRegistration confirmation)
        {
            GameId = confirmation.gameId;
        }
        public ConfirmGameRegistrationMessage(ulong gameId)
        {
            GameId = gameId;
        }
        public string Serialize()
        {
            return MessageParser.Serialize(ToBase());
        }

        public ConfirmGameRegistration ToBase()
        {
            return new ConfirmGameRegistration()
            {
                gameId = GameId
            };
        }
    }
}
