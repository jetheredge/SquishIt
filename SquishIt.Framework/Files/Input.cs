using System.Collections.Generic;

namespace SquishIt.Framework.Files
{
    public class Input
    {
        public string Path { get; private set; }
        public Resolvers.IResolver Resolver { get; private set; }
	
        public Input(string filePath, Resolvers.IResolver resolver)
        {
            Path = filePath;
            Resolver = resolver;
        }

        public bool IsDirectory {
            get { return Resolver.IsDirectory(Path); }
        }

        public IEnumerable<string> TryResolve(IEnumerable<string> allowedExtensions) 
        {
            if (IsDirectory) 
            {
                return Resolver.TryResolveFolder(Path, allowedExtensions);
            }
            else 
            {
                return new [] { Resolver.TryResolve(Path) };
            }
        }
    }
}