using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Reflection;
using SquishIt.Framework.Utilities;

namespace SquishIt.Framework.Resolvers
{
    public abstract class EmbeddedResourceResolver : IResolver
    {
        private readonly ITempPathProvider tempPathProvider = Configuration.Instance.DefaultTempPathProvider();

        protected abstract string CalculateResourceName(string assemblyName, string resourceName); 

        public string Resolve(string file)
        {
            var split = file.Split(new[] { "://" }, StringSplitOptions.None);
            var assemblyName = split.ElementAt(0);
            var assembly = AppDomain.CurrentDomain.GetAssemblies().SingleOrDefault(x => x.GetName().Name == assemblyName);
            var resourceName = CalculateResourceName(assemblyName, split.ElementAt(1));

            string resolved;
            if (TempFileResolutionCache.TryGetValue(resourceName, out resolved))
            {
                return resolved;
            }
            return ResolveFile(file, assembly, resourceName);
        }

        private string ResolveFile(string file, Assembly assembly, string resourceName)
        {
            using (var stream = assembly.GetManifestResourceStream(resourceName))
            {
                if (stream == null)
                    throw new InvalidOperationException(String.Format("Embedded resource not found: {0}", file));

                string contents;
                using (var sr = new StreamReader(stream))
                {
                    contents = sr.ReadToEnd();
                }
                string fileName = tempPathProvider.ForFile();

                using (var sw = new StreamWriter(fileName))
                {
                    sw.Write(contents);
                }
                TempFileResolutionCache.Add(resourceName, fileName);
                return fileName;
            }
        }

        public IEnumerable<string> ResolveFolder(string path, bool recursive, string debugFileExtension, IEnumerable<string> allowedExtensions, IEnumerable<string> disallowedExtensions)
        {
            throw new NotImplementedException("Adding entire directories only supported by FileSystemResolver.");
        }

        public virtual bool IsDirectory(string path)
        {
            return false;
        }
    }

    public class StandardEmbeddedResourceResolver : EmbeddedResourceResolver
    {
        protected override string CalculateResourceName(string assemblyName, string resourceName)
        {
            return assemblyName + "." + resourceName;
        }
    }

    public class RootEmbeddedResourceResolver : EmbeddedResourceResolver
    {
        protected override string CalculateResourceName(string assemblyName, string resourceName)
        {
            return resourceName;
        }
    }
}