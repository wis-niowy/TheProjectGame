using GameArea;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameMaster
{
    public interface IGMMessage
    {
        string[] Process(IGameMaster gameMaster);
        bool Prioritised { get; }
        string GUID { get; }
    }
}
