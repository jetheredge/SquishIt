using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using SquishIt.Framework.Utilities;

namespace SquishIt.Framework.Resolvers
{
    public class HttpResolver: IResolver
    {
        private readonly ITempPathProvider tempPathProvider = Configuration.Instance.DefaultTempPathProvider();

        public string Resolve(string file)
        {
            string resolved;
            if (TempFileResolutionCache.TryGetValue(file, out resolved))
            {
                return resolved;
            }
            return ResolveWebResource(file);
        }

        private string ResolveWebResource(string path)
        {
            var webRequestObject = (HttpWebRequest) WebRequest.Create(path);
            var webResponse = webRequestObject.GetResponse();
            try
            {
                string contents;
                using (var sr = new StreamReader(webResponse.GetResponseStream()))
                {
                    contents = sr.ReadToEnd();
                }
                string fileName = tempPathProvider.ForFile();

                using (var sw = new StreamWriter(fileName))
                {
                    sw.Write(contents);
                }
                TempFileResolutionCache.Add(path, fileName);
                return fileName;
            }
            finally
            {
                webResponse.Close();
            }
        }

        public IEnumerable<string> ResolveFolder(string path, bool recursive, string debugFileExtension, IEnumerable<string> allowedExtensions, IEnumerable<string> disallowedExtensions) {
            throw new NotImplementedException("Adding entire directories only supported by FileSystemResolver.");
        }

        public virtual bool IsDirectory(string path) 
        {
            return false;
        }
    }
}