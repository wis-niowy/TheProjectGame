using Messages;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameArea.AppMessages
{
    public class ConfirmJoiningGameMessage : PlayerMessage, IToBase<ConfirmJoiningGame>
    {
        public ulong GameId { get; set; }
        public GameObjects.Player PlayerDefinition { get; set; }
        public string GUID { get; set; }
        public override string Serialize()
        {
            return MessageParser.Serialize(ToBase());
        }

        public ConfirmJoiningGameMessage(ulong gameId, GameObjects.Player playerDef, string guid, ulong playerId):base(playerId)
        {
            GameId = gameId;
            PlayerDefinition = playerDef;
            GUID = guid;
        }

        public ConfirmJoiningGameMessage(ConfirmJoiningGame confirm):this(confirm.gameId,new GameObjects.Player(confirm.PlayerDefinition), confirm.privateGuid, confirm.playerId) { }

        public ConfirmJoiningGame ToBase()
        {
            return new ConfirmJoiningGame()
            {
                gameId = GameId,
                PlayerDefinition = PlayerDefinition?.ToBase(),
                playerId = PlayerId,
                privateGuid = GUID
            };
        }
    }
}
