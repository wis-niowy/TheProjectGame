using Messages;
using System;
using System.Collections.Generic;
using System.Text;
namespace GameArea.AppMessages
{
    public class RegisterGameMessage : IToBase<RegisterGame>
    {
        public GameObjects.GameInfo NewGameInfo { get; set; }

        public RegisterGameMessage(string name, ulong red, ulong blue)
        {
            NewGameInfo = new GameObjects.GameInfo(name, red, blue);
        }
        public string Serialize()
        {
            return MessageParser.Serialize(ToBase());
        }

        public RegisterGame ToBase()
        {
            return new RegisterGame()
            {

                NewGameInfo = NewGameInfo?.ToBase()
            };
        }
    }
}
