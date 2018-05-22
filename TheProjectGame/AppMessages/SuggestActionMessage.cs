using GameArea.GameObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameArea.AppMessages
{
    public class SuggestActionMessage : BetweenPlayersAbstractMessage, IToBase<Messages.SuggestAction>
    {
        public TaskField[] Tasks { get; set; }
        public GoalField[] Goals { get; set; }
        public string PlayerGUID { get; set; }
        public ulong GameId { get; set; }
        public SuggestActionMessage(ulong id, ulong senderId, string guid, ulong gameId, TaskField[] tasks = null, GoalField[] goals = null) : base(id, senderId)
        {
            PlayerGUID = guid;
            Tasks = tasks;
            Goals = goals;
            GameId = gameId;

        }

        Messages.SuggestAction IToBase<Messages.SuggestAction>.ToBase()
        {
            return new Messages.SuggestAction()
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
