using System;
using System.Collections.Generic;
using System.Text;

namespace CommunicationServer
{
    public interface IServerMessage<T>
    {
        ulong ClientId { get; }
        void Process(T controller);
    }
}
