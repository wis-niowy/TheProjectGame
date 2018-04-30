using GameArea;
using GameArea.AppMessages;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace GameMaster.GMMessages
{
    public class ErrorMessageGM : ErrorMessage, IGMMessage
    {
        public XmlDocument Document { get; }
        public ErrorMessageGM(string type, string message, string causeName, XmlDocument doc) : base(type, message, causeName)
        {
            Document = doc;
        }

        public string[] Process(IGameMaster gameMaster)
        {
            gameMaster.HandlerErrorMessage(this);
            return new string[] { };
        }
    }
}
