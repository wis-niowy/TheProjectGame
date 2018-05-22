using System;
using System.Collections.Generic;
using System.Text;
using GameArea;
using GameArea.AppMessages;
using GameArea.GameObjects;

namespace GameMaster.GMMessages
{
    public class SuggestActionResponseGM : SuggestActionResponseMessage, IGMMessage
    {
        public bool Prioritised => false;
        public string GUID => PlayerGUID;
        public SuggestActionResponseGM(ulong id, ulong senderId, string guid, ulong gameId, TaskField[] tasks = null, GoalField[] goals = null) : base(id, senderId, guid, gameId, tasks, goals)
        {
        }

        public string[] Process(IGameMaster gameMaster)
        {
            if (gameMaster.GameEndDate > ReceiveDate || gameMaster.GameStartDate > ReceiveDate || gameMaster.IsGameFinished)
            {
                return null;
            }
            return new string[] { gameMaster.HandleSuggestActionResponse(this)?.Serialize() };
        }
    }
}
