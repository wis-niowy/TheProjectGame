using System;
using System.Collections.Generic;
using System.Text;

namespace CommunicationServer.ServerObjects
{
    public interface IClientHandle
    {
        DateTime LastKeepAlive { get; set; }
        ulong ID { get; set; }
        IInterpreter MessageInterpreter { get; set; } //odpowiada za poprawny odczyt i wykonanie akcji na daną wiadomość
        bool IsAlive { get; }

        void BeginRead();
        void EndRead(IAsyncResult result);
        void BeginSend(string message);
        void EndSend(IAsyncResult result);
    }
}
