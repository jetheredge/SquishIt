using System.Collections.Generic;
using System.IO;

namespace SquishIt.Framework.Resolvers
{
    public class FileResolver: IResolver
    {
        public static string Type
        {
            get { return "file"; }
        }        

        public IEnumerable<string> TryResolve(string file)
        {
            yield return Path.GetFullPath(file);            
        }        
    }
}