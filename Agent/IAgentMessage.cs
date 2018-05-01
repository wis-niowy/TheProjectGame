using GameArea;
using System;
using System.Collections.Generic;
using System.Text;

namespace Player
{
    public interface IAgentMessage
    {
        void Process(IPlayer player);
    }
}
