using System;
using System.Collections.Generic;
using System.Text;
using GameArea;
using GameArea.AppMessages;
using Messages;

namespace GameMaster.GMMessages
{
    public class DataGM : DataMessage, IGMMessage
    {
        public bool Prioritised => false;
        public string GUID => PlayerGUID;
        public DataGM(ulong playerId) : base(playerId)
        {
        }

        public DataGM(Data data) : base(data)
        {
        }

        public string[] Process(IGameMaster gameMaster)
        {
            return gameMaster.HandleData(this);
        }
    }
}
