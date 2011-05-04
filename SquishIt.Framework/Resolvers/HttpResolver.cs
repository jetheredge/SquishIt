using System.Collections.Generic;
using System.IO;
using System.Net;

namespace SquishIt.Framework.Resolvers
{
    public class HttpResolver: IResolver
    {
        public IEnumerable<string> TryResolve(string file)
        {
            var webRequestObject = (HttpWebRequest)WebRequest.Create(file);
            var webResponse = webRequestObject.GetResponse();
            try
            {
                string contents;
                using (var sr = new StreamReader(webResponse.GetResponseStream()))
                {
                    contents = sr.ReadToEnd();
                }
                string fileName = Path.GetTempPath() + Path.GetRandomFileName();

                using (var sw = new StreamWriter(fileName))
                {
                    sw.Write(contents);
                }
                return new[] { fileName };
            }
            finally
            {
                webResponse.Close();
            }            
        }
    }
}