using System.IO;
using JavascriptBundler.jsmin;

namespace JavascriptBundler.Minifiers
{
    public class JsMinMinifier: IFileCompressor
    {
        public string Compress(string file)
        {
            string outputFileName = Path.GetTempPath() + Path.GetRandomFileName();
            var minifier = new JavaScriptMinifier();            
            minifier.Minify(file, outputFileName);

            string output;
            using (var sr = new StreamReader(outputFileName))
            {
                output = sr.ReadToEnd();
            }
            return output;
        }
    }
}