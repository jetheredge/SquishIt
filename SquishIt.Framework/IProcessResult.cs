using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SquishIt.Framework
{
    public interface IProcessResult
    {
        string Result { get; }
        IEnumerable<string> Dependencies { get; }
    }
}
