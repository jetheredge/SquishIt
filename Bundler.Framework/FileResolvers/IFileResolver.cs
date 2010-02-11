using System.Collections.Generic;

namespace Bundler.Framework.FileResolvers
{
    public interface IFileResolver
    {        
        IEnumerable<string> TryResolve(string file);
    }
}