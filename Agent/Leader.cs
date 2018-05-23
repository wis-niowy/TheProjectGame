using Messages;
using System;
using System.Collections.Generic;
using System.Text;
using GameArea;
using GameArea.AppMessages;
using System.Linq;
using Player.PlayerMessages;

namespace Player
{
    public class Leader : Player
    {
        public Leader(TeamColour team,PlayerRole role, GameArea.AppConfiguration.PlayerSettingsConfiguration settings = null,
                                                                IPlayerController controller = null, string guid = "TEST_GUID", ulong id = 0) : base(team, role, settings, controller, guid, null, id)
        {
        }

        public override DataMessage HandleKnowledgeExchangeRequest(KnowledgeExchangeRequestMessage messageObject)
        {
            // Leader zawsze odpowiada, jezeli request jest z jego druzyny
            // wiadomosci od graczy z przeciwnego zespolu zapisuje sobie w kolejce

            DataMessage responseData = null;

            if (otherTeam.Select(p => p.ID).Contains(messageObject.SenderPlayerId))
                // wiadomosc od obcego playera - do kolejki
            {
                AddOtherPlayerExhangeKnowledgeRequest(messageObject as KnowledgeExchangeRequestAgent);
            }
            else if (myTeam.Select(p => p.ID).Contains(messageObject.SenderPlayerId))
                // wiadomosc od naszego player - natychmiastowa odpoweidz
            {
                responseData = PrepareKnowledgeExchangeMessage(messageObject);
            }

            return responseData;
        }

        public override void HandleRejectKnowledgeExchange(RejectKnowledgeExchangeMessage messageObject)
        {
            // nic nie robi
        }
    }
}
