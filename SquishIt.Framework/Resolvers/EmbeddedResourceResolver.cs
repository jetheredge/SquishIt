using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace SquishIt.Framework.Resolvers
{
    public class EmbeddedResourceResolver : IResolver
    {
        public string TryResolve(string file)
        {
            var split = file.Split(new[] { "://" }, StringSplitOptions.None);
            var assemblyName = split.ElementAt(0);
            var assembly = AppDomain.CurrentDomain.GetAssemblies().SingleOrDefault(x => x.GetName().Name == assemblyName);
            var resourceName = assemblyName + "." + split.ElementAt(1);
            using (var stream = assembly.GetManifestResourceStream(resourceName))
            {
                if (stream == null) throw new InvalidOperationException(String.Format("Embedded resource not found: {0}", file));

                string contents;
                using (var sr = new StreamReader(stream))
                {
                    contents = sr.ReadToEnd();
                }
                string fileName = Path.GetTempPath() + Path.GetRandomFileName();

                using (var sw = new StreamWriter(fileName))
                {
                    sw.Write(contents);
                }
                return fileName;
            }
        }

        public IEnumerable<string> TryResolveFolder(string path, IEnumerable<string> allowedExtensions) {
            throw new NotImplementedException("Adding entire directories only supported by FileSystemResolver.");
        }

        public virtual bool IsDirectory(string path) {
            return false;
        }
    }
}