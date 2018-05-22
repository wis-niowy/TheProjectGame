using Messages;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameArea.AppMessages
{
    public class RejectGameRegistrationMessage : IToBase<RejectGameRegistration>
    {
        public string GameName { get; set; }
        public RejectGameRegistrationMessage(string name)
        {
            GameName = name;
        }
        public string Serialize()
        {
            return MessageParser.Serialize(ToBase());
        }

        public RejectGameRegistration ToBase()
        {
            return new RejectGameRegistration()
            {
                gameName = GameName
            };
        }
    }
}
