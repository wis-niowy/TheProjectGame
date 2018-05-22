using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameArea.GameObjects;

namespace GameArea.AppMessages
{
    public class SuggestActionResponseMessage : SuggestActionMessage, IToBase<Messages.SuggestActionResponse>
    {
        public SuggestActionResponseMessage(ulong id, ulong senderId, string guid, ulong gameId, TaskField[] tasks = null, GoalField[] goals = null) : base(id, senderId, guid, gameId, tasks, goals)
        {
        }

        Messages.SuggestActionResponse IToBase<Messages.SuggestActionResponse>.ToBase()
        {
           return new Messages.SuggestActionResponse()
           {
               gameId = GameId,
               GoalFields = Goals?.Select(q => q.ToBase()).ToArray(),
               playerGuid = PlayerGUID,
               playerId = PlayerId,
               senderPlayerId = SenderPlayerId,
               TaskFields = Tasks?.Select(q => q.ToBase()).ToArray()
           };
        }
    }
}
