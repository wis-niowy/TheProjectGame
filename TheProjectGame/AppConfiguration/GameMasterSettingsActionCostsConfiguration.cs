using System;
using System.Collections.Generic;
using System.Text;
using Configuration;

namespace GameArea.AppConfiguration
{
    public class GameMasterSettingsActionCostsConfiguration : IToBase<Configuration.GameMasterSettingsActionCosts>
    {
        public int MoveDelay;
        public int DiscoverDelay;
        public int TestDelay;
        public int DestroyDelay;
        public int PickUpDelay;
        public int PlacingDelay;
        public int KnowledgeExchangeDelay;
        public int SuggestActionDelay;
    

        public GameMasterSettingsActionCostsConfiguration(Configuration.GameMasterSettingsActionCosts costs)
        {
            if (costs == null)
                costs = new Configuration.GameMasterSettingsActionCosts();

            MoveDelay = (int)costs.MoveDelay;
            DiscoverDelay = (int)costs.DiscoverDelay;
            TestDelay = (int)costs.TestDelay;
            DestroyDelay = (int)costs.DestroyDelay;
            PickUpDelay = (int)costs.PickUpDelay;
            PlacingDelay = (int)costs.PlacingDelay;
            KnowledgeExchangeDelay = (int)costs.KnowledgeExchangeDelay;
            SuggestActionDelay = (int)costs.SuggestActionDelay;
        }

        public string Serialize()
        {
            return MessageParser.Serialize(ToBase());
        }

        public GameMasterSettingsActionCosts ToBase()
        {
            return new GameMasterSettingsActionCosts()
            {
                MoveDelay = (uint)MoveDelay,
                DiscoverDelay = (uint)DiscoverDelay,
                TestDelay = (uint)TestDelay,
                DestroyDelay = (uint)DestroyDelay,
                PickUpDelay = (uint)PickUpDelay,
                PlacingDelay = (uint)PlacingDelay,
                KnowledgeExchangeDelay = (uint)KnowledgeExchangeDelay,
                SuggestActionDelay = (uint)SuggestActionDelay,
            };
        }
    }
}
