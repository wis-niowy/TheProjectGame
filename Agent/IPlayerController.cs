using GameArea;
using GameArea.AppConfiguration;
using GameArea.AppMessages;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Player
{
    public interface IPlayerController
    {
        IPlayer Player { get; set; }
        AgentState State { get; set; }
        ActionType ActionToComplete { get; set; }
        PlayerSettingsConfiguration Settings { get; set; }

        bool ConnectToServer(IPAddress ip, Int32 port);
        void StartPerformance();
        void BeginRead();
        void EndRead(IAsyncResult result);
        void BeginSend(string message);
        void EndSend(IAsyncResult result);

        void RegisteredGames(RegisteredGamesMessage messageObject);
        void ConfirmJoiningGame(ConfirmJoiningGameMessage info);
    }
}
