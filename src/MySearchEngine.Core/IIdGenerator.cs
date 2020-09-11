using System;
using System.Collections.Generic;
using System.Text;

namespace MySearchEngine.Core
{
    public interface IIdGenerator<TId>
    {
        TId Next();
    }
}
