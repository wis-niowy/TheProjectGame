using System;
using System.Collections.Generic;
using System.Text;

namespace GameArea
{
    public interface IMessage<T>
    {
        ulong ClientId { get; }
        void Process(T controller);
    }
}
