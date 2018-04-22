using Messages;
using System;
using System.Collections.Generic;
using System.Text;
using GameArea;

namespace Player
{
    public class Leader : Player
    {
        public Leader(TeamColour team, string guid ) : base(team,_guid:guid)
        {
        }
    }
}
