﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameArea.Parsers;
using Messages;

namespace GameArea.AppMessages
{
    public class DataMessage : PlayerMessage, IToBase<Messages.Data>
    {
        public GameObjects.TaskField[] Tasks { get; set; }
        public GameObjects.GoalField[] Goals { get; set; }
        public GameObjects.Piece[] Pieces { get; set; }
        public bool GameFinished { get; set; }
        public string PlayerGUID { get; set; }
        public GameObjects.Location PlayerLocation { get; set; }

        public DataMessage(ulong playerId):base(playerId)
        { }
        public DataMessage(Messages.Data data):base(data.playerId)
        {
            Tasks = data.TaskFields.Select(q=>new GameObjects.TaskField(q)).ToArray();
            Goals = data.GoalFields.Select(q => new GameObjects.GoalField(q)).ToArray();
            GameFinished = data.gameFinished;
            PlayerGUID = data.playerGuid;
            PlayerLocation = new GameObjects.Location(data.PlayerLocation);
            Pieces = data.Pieces.Select(q=>new GameObjects.Piece(q)).ToArray();
        }
        public Data ToBase()
        {
            return new Data()
            {
                gameFinished = GameFinished,
                GoalFields = Goals?.Select(q => (Messages.GoalField)q.ToBase()).ToArray(),
                Pieces = Pieces?.Select(q => q.ToBase()).ToArray(),
                playerGuid = PlayerGUID,
                playerId = PlayerId,
                PlayerLocation = PlayerLocation?.ToBase(),
                TaskFields = Tasks?.Select(q => (Messages.TaskField)q.ToBase()).ToArray()
            };
        }

        Data IToBase<Data>.ToBase()
        {
            throw new NotImplementedException();
        }

        public override string Serialize()
        {
            return MessageParser.Serialize(ToBase());
        }
    }
}
