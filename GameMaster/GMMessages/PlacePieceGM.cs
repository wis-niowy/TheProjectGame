using GameArea;
using GameArea.AppMessages;
using Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameMaster.GMMessages
{
    public class PlacePieceGM : PlacePieceMessage, IGMMessage
    {
        public PlacePieceGM(string guid, ulong gameId) : base(guid, gameId)
        {
        }

        public string[] Process(IGameMaster gameMaster)
        {
            var messages = new  string[] { gameMaster.HandlePlacePieceRequest(this).Serialize() };
            if(gameMaster.IsGameFinished && gameMaster.State == GameMasterState.GameResolved)
            {
                var newGameMessages = gameMaster.RestartGame();
                messages = messages.Union(newGameMessages).ToArray();
            }
            return messages;
        }
    }
}
