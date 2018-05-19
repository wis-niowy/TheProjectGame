using GameArea;
using GameArea.AppMessages;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameMaster.GMMessages
{
    public class RejectGameRegistrationGM : RejectGameRegistrationMessage, IGMMessage
    {
        public bool Prioritised => true;
        public string GUID => null;
        public RejectGameRegistrationGM(string name) : base(name)
        {
        }

        public string[] Process(IGameMaster gameMaster)
        {
            gameMaster.State = GameMasterState.GameOver;
            return new string[] { };
        }
    }
}
