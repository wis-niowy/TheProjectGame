﻿using Messages;
using System;
using System.Collections.Generic;
using System.Text;
using GameArea;

namespace Player
{
    public class Leader : Agent
    {
        public Leader(TeamColour team, ulong guid = 0) : base(team, guid)
        {
        }
    }
}
