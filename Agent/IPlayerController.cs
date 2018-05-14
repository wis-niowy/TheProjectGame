using GameArea;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Player
{
    public interface IPlayerController
    {
        IPlayer Player { get; set; }

        bool ConnectToServer(IPAddress ip, Int32 port);
        void StartPerformance();
        void BeginRead();
        void EndRead(IAsyncResult result);
        void BeginSend(string message);
        void EndSend(IAsyncResult result);
    }
}
