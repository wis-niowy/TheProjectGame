using System;
using System.Collections.Generic;
using System.Text;
using GameArea;
using GameArea.AppMessages;
using GameArea.GameObjects;

namespace GameMaster.GMMessages
{
    public class SuggestActionGM : SuggestActionMessage, IGMMessage
    {
        public bool Prioritised => false;
        public string GUID => PlayerGUID;
        public SuggestActionGM(ulong id, ulong senderId, string guid, ulong gameId, TaskField[] tasks = null, GoalField[] goals = null) : base(id, senderId, guid, gameId, tasks, goals)
        {
        }

        public string[] Process(IGameMaster gameMaster)
        {
            return new string[] { gameMaster.HandleSuggestAction(this)?.Serialize() };
        }
    }
}
