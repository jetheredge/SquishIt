using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SquishIt.Framework.Base
{
    public class ProcessResult : IProcessResult
    {
        public ProcessResult(string result)
        {
            this.Result = result;
            this.Dependencies = Enumerable.Empty<string>();
        }

        public ProcessResult(string result, IEnumerable<string> dependencies)
        {
            this.Result = result;
            this.Dependencies = dependencies;
        }

        public string Result { get; private set; }
        public IEnumerable<string> Dependencies { get; private set; }
    }
}
