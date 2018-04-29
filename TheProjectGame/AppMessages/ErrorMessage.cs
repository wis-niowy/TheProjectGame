using System;
using System.Collections.Generic;
using System.Text;
using Messages;

namespace GameArea.AppMessages
{
    public class ErrorMessage : IToBase<Messages.ErrorMessage>
    {
        public string Type { get; set; }
        public string Message { get; set; }
        public string CauseParameterName { get; set; }
        public ErrorMessage(string type, string message, string causeName)
        {
            Type = type;
            Message = message;
            CauseParameterName = causeName;
        }

        public ErrorMessage(Messages.ErrorMessage error) : this(error.ExceptionType, error.ExceptionMessage, error.ExceptionCauseParameterName) { }
        public string Serialize()
        {
            return MessageParser.Serialize(ToBase());
        }

        public Messages.ErrorMessage ToBase()
        {
            return new Messages.ErrorMessage()
            {
                ExceptionCauseParameterName = CauseParameterName,
                ExceptionMessage = Message,
                ExceptionType = Type
            };
        }
    }
}
