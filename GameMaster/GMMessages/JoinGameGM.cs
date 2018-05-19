using System;
using System.Collections.Generic;
using System.Text;
using GameArea;
using GameArea.AppMessages;
using Messages;

namespace GameMaster.GMMessages
{
    public class JoinGameGM : JoinGameMessage, IGMMessage
    {
        public bool Prioritised => true;
        public string GUID => null;
        public JoinGameGM(string name, TeamColour team, PlayerRole role, long playerId = -1) : base(name, team, role, playerId)
        {
        }

        public string[] Process(IGameMaster gameMaster)
        {
            return gameMaster.HandleJoinGameRequest(this);
        }
    }
}
