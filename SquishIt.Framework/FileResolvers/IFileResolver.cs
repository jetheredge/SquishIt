using System.Collections.Generic;

namespace SquishIt.Framework.FileResolvers
{
    public interface IFileResolver
    {        
        IEnumerable<string> TryResolve(string file);
    }
}