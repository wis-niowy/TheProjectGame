using System;
using System.Collections.Generic;
using System.Text;

namespace GameArea
{
    public interface IToBase<T>
    {
        T ToBase();
        string Serialize();
    }
}
