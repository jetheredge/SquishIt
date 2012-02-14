using System.Collections.Generic;

namespace SquishIt.Framework.Resolvers 
{
    public interface IResolver 
    {
        bool IsDirectory(string path);
        string TryResolve(string path);
        IEnumerable<string> TryResolveFolder(string path, IEnumerable<string> allowedFileExtensions, IEnumerable<string> disallowedFileExtensions);
    }
}