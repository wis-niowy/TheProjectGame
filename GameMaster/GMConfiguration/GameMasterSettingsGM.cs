using System;
using System.Collections.Generic;
using System.Text;
using Configuration;
using GameArea;
using GameArea.AppConfiguration;

namespace GameMaster.GMConfiguration
{
    public class GameMasterSettingsGM : GameMasterSettingsConfiguration, IGMMessage
    {


        public GameMasterSettingsGM(GameMasterSettings gameMasterSettings = null) : base(gameMasterSettings)
        {

        }

        public bool Prioritised => true;

        public string GUID => null;

        public string[] Process(IGameMaster gameMaster)
        {
            throw new NotImplementedException();
        }
    }
}
