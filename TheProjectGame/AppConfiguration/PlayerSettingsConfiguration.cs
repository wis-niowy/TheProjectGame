using System;
using System.Collections.Generic;
using System.Text;
using Configuration;

namespace GameArea.AppConfiguration
{
    public class PlayerSettingsConfiguration: ConfigurationGA, IToBase<Configuration.PlayerSettings>
    {
        public int RetryJoinGameInterval { get; set; }


        public PlayerSettingsConfiguration(Configuration.PlayerSettings playerSettings): base(playerSettings)
        {
            RetryJoinGameInterval = (int)playerSettings.RetryJoinGameInterval;
        }
        public PlayerSettingsConfiguration(int interval) : base(interval)
        {
            RetryJoinGameInterval = interval;
        }

        public PlayerSettings ToBase()
        {
            return new Configuration.PlayerSettings()
            {
                KeepAliveInterval = (uint)this.KeepAliveInterval,
                RetryJoinGameInterval = (uint)this.RetryJoinGameInterval,
            };
        }

        public string Serialize()
        {
            return MessageParser.Serialize(ToBase());
        }
    }
}
