using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using GameArea;

namespace Player.Tests
{
    public class PlayerControllerMock : IPlayerController
    {
        public IPlayer Player { get; set; }

        public PlayerControllerMock()
        {

        }

        public void BeginRead()
        {

        }

        public void BeginSend(string message)
        {
            if (Player.LastActionTaken == ActionType.Move)
            {
                (Player as Player).Location = (Player as Player).CalculateFutureLocation((Player as Player).Location, (Player as Player).LastMoveTaken.Value);
            }
        }

        public bool ConnectToServer(IPAddress ip, int port)
        {
            return true;
        }

        public void EndRead(IAsyncResult result)
        {

        }

        public void EndSend(IAsyncResult result)
        {

        }

        public void StartPerformance()
        {

        }
    }
}
