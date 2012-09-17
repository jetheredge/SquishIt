using System.Collections.Generic;

namespace SquishIt.Framework
{
    public interface IProcessResult
    {
        string Result { get; }
        IEnumerable<string> Dependencies { get; }
    }
}
