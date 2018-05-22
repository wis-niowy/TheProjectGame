using System;
using System.Collections.Generic;
using System.Text;
using Configuration;
using GameArea;
using GameArea.AppConfiguration;

namespace Player.PlayerConfiguration
{
    public class PlayerSettingsAgent : PlayerSettingsConfiguration, IAgentMessage
    {

        public PlayerSettingsAgent(Configuration.PlayerSettings playerSettings) : base(playerSettings)
        {
        }

        public string[] Process(IPlayerController controller)
        {
            throw new NotImplementedException();
        }
    }
}
