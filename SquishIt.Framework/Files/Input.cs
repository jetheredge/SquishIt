using System.Collections.Generic;

namespace SquishIt.Framework.Files
{
    public class Input
    {
        public string Path { get; private set; }
        public Resolvers.IResolver Resolver { get; private set; }
        public bool IsRecursive { get; private set; }

        public Input(string filePath, bool recursive, Resolvers.IResolver resolver)
        {
            Path = filePath;
            IsRecursive = recursive;
            Resolver = resolver;
        }

        public bool IsDirectory
        {
            get { return Resolver.IsDirectory(Path); }
        }

        public IEnumerable<string> Resolve(IEnumerable<string> allowedExtensions, IEnumerable<string> disallowedExtensions, string debugFileExtension)
        {
            if(IsDirectory)
            {
                return Resolver.ResolveFolder(Path, IsRecursive, debugFileExtension, allowedExtensions, disallowedExtensions);
            }
            else
            {
                return new[] { Resolver.Resolve(Path) };
            }
        }
    }
}