using System;
using System.Collections.Generic;
using System.Text;
using Configuration;
using GameArea;
namespace MainApp
{
    public class GameObjectContainer
    {
        public GameMaster GameMaster { get; set; }

        public GameObjectContainer(GameMasterSettings settings)
        {
            GameMaster = new GameMaster(settings);
        }
    }
}
