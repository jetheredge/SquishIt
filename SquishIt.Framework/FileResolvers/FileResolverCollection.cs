using System.Collections.Generic;

namespace SquishIt.Framework.FileResolvers
{
    public class FileResolverCollection
    {
        private Dictionary<string,IFileResolver> fileResolvers = new Dictionary<string,IFileResolver>();

        public FileResolverCollection()
        {            
            fileResolvers.Add(FileResolver.Type, new FileResolver());
            fileResolvers.Add(DirectoryResolver.Type, new DirectoryResolver());
            fileResolvers.Add(HttpResolver.Type, new HttpResolver());
            fileResolvers.Add(EmbeddedResourceResolver.Type, new EmbeddedResourceResolver());
        }

        public IEnumerable<string> Resolve(string argument, string type)
        {
            return fileResolvers[type].TryResolve(argument);            
        }
    }
}