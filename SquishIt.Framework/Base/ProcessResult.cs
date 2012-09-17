using System.Collections.Generic;
using System.Linq;

namespace SquishIt.Framework.Base
{
    public class ProcessResult : IProcessResult
    {
        public ProcessResult(string result)
        {
            Result = result;
            Dependencies = Enumerable.Empty<string>();
        }

        public ProcessResult(string result, IEnumerable<string> dependencies)
        {
            Result = result;
            Dependencies = dependencies;
        }

        public string Result { get; private set; }
        public IEnumerable<string> Dependencies { get; private set; }
    }
}
