using System.Collections.Generic;

namespace JavascriptBundler.FileResolvers
{
    public interface IFileResolver
    {        
        IEnumerable<string> TryResolve(string file);
    }
}