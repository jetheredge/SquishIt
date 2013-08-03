using System.Collections.Generic;

namespace SquishIt.Framework.Resolvers 
{
    public interface IResolver 
    {
        bool IsDirectory(string path);
        string Resolve(string file);
        IEnumerable<string> ResolveFolder(string path, bool recursive, string debugFileExtension, IEnumerable<string> allowedExtensions, IEnumerable<string> disallowedExtensions);
    }
}