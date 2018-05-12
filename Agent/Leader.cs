using Messages;
using System;
using System.Collections.Generic;
using System.Text;
using GameArea;

namespace Player
{
    public class Leader : Player
    {
        public Leader(TeamColour team,PlayerRole role, string guid ) : base(team, role,_guid:guid)
        {
        }
    }
}
