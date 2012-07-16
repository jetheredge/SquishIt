using System.Collections.Generic;

namespace SquishIt.Framework.Resolvers 
{
    public interface IResolver 
    {
        bool IsDirectory(string path);
        string Resolve(string path);
        IEnumerable<string> ResolveFolder(string path, bool recursive, string debugFileExtension, IEnumerable<string> allowedExtensions, IEnumerable<string> disallowedExtensions);
    }
}