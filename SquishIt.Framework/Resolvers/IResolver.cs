using System.Collections.Generic;

namespace SquishIt.Framework.Resolvers
{
    public interface IResolver
    {        
        IEnumerable<string> TryResolve(string file, string[] allowedExtensions);
    }
}