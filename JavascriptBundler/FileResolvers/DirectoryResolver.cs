using System.Collections.Generic;
using JavascriptBundler.Directories;

namespace JavascriptBundler.FileResolvers
{
    public class DirectoryResolver: IFileResolver
    {
        public static string Type
        {
            get { return "dir"; }
        }
        
        private readonly IDirectoryEnumerator directoryEnumerator;

        public DirectoryResolver()
        {
            this.directoryEnumerator = new DirectoryEnumerator();
        }

        public DirectoryResolver(IDirectoryEnumerator directoryEnumerator)
        {
            this.directoryEnumerator = directoryEnumerator;
        }        

        public IEnumerable<string> TryResolve(string directory)
        {
            return directoryEnumerator.GetFiles(directory);            
        }        
    }
}