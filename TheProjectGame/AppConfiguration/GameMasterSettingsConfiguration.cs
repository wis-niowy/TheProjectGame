using System;
using System.Collections.Generic;
using System.Text;
using Configuration;

namespace GameArea.AppConfiguration
{
    public class GameMasterSettingsConfiguration: ConfigurationGA, IToBase<Configuration.GameMasterSettings>
    {
        public GameMasterSettingsGameDefinitionConfiguration GameDefinition { get; set; }
        public GameMasterSettingsActionCostsConfiguration ActionCosts { get; set; }
        public int RetryRegisterGameInterval { get; set; }


        public GameMasterSettingsConfiguration(Configuration.GameMasterSettings gameMasterSettings = null): base((int)gameMasterSettings.KeepAliveInterval)
        {
            if (gameMasterSettings == null)
                gameMasterSettings = Configuration.GameMasterSettings.GetDefaultGameMasterSettings();

            GameDefinition = new GameMasterSettingsGameDefinitionConfiguration(gameMasterSettings.GameDefinition);
            ActionCosts = new GameMasterSettingsActionCostsConfiguration(gameMasterSettings.ActionCosts);
            RetryRegisterGameInterval = (int)gameMasterSettings.RetryRegisterGameInterval;
        }


        public GameMasterSettings ToBase()
        {
            return new GameMasterSettings()
            {
                GameDefinition = GameDefinition.ToBase(),
                ActionCosts = ActionCosts.ToBase(),
            };
        }

        public string Serialize()
        {
            return MessageParser.Serialize(ToBase());
        }
    }
}
