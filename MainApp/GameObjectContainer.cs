using System;
using System.Collections.Generic;
using System.Text;
using Configuration;
using GameArea;
using GameArea.AppConfiguration;

namespace MainApp
{
    public class GameObjectContainer
    {
        public GameArea.GameMaster GameMaster { get; set; }

        public GameObjectContainer(GameMasterSettingsConfiguration settings)
        {
            GameMaster = new GameArea.GameMaster(settings);
        }
    }
}
