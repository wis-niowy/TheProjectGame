using Messages;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameArea.ControllerInterfaces
{
    public interface IMainController
    {
        bool RegisterGame(string name, ulong red, ulong blue, ulong clientId);
        void JoinGame(string name, TeamColour team, PlayerRole role, ulong clientId);
        void GetGames(ulong clientId);
        void SendToClient(ulong clientId, string message);
        void SendKeepAlive(ulong clientId);
    }
}
