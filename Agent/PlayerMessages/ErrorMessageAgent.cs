using GameArea;
using GameArea.AppMessages;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Player.PlayerMessages
{
    public class ErrorMessageAgent : ErrorMessage, IAgentMessage
    {
        XmlDocument Document { get; }
        public ErrorMessageAgent(string type, string message, string causeName, XmlDocument doc) : base(type, message, causeName)
        {
            Document = doc;
        }

        public void Process(IPlayer player)
        {
            player.ErrorMessage(this);
        }
    }
}
