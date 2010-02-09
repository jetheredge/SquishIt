using System.Collections.Generic;
using System.IO;
using System.Net;

namespace JavascriptBundler.FileResolvers
{
    public class HttpResolver: IFileResolver
    {
        public static string Type
        {
            get { return "http"; }
        }        

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