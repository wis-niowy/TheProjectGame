using System;
using System.Collections.Generic;
using System.Text;
using GameArea;
using GameArea.AppMessages;

namespace GameMaster.GMMessages
{
    public class ConfirmGameRegistrationGM : ConfirmGameRegistrationMessage, IGMMessage
    {
        public ConfirmGameRegistrationGM(ulong gameId) : base(gameId)
        {
        }

        public string[] Process(IGameMaster gameMaster)
        {
            gameMaster.HandleConfirmGameRegistration(this);
            return new string[] { };
        }
    }
}
