using System.IO;
using SquishIt.Framework.Minifiers.JavaScript.jsmin;

namespace SquishIt.Framework.Minifiers.JavaScript
{
    public class JsMinMinifier: IJavaScriptMinifier
    {
        string CompressFile(string file)
        {
            string outputFileName = Path.GetTempPath() + Path.GetRandomFileName();
            var minifier = new JavaScriptMinifier();
            minifier.Minify(file, outputFileName);
            try
            {
                string output;
                using (var sr = new StreamReader(outputFileName))
                {
                    output = sr.ReadToEnd();
                }
                return output;
            }
            finally
            {
                File.Delete(outputFileName);
            }
        }

        public string Minify(string content)
        {
            string inputFileName = Path.GetTempPath() + Path.GetRandomFileName();
            try
            {
                using (var sw = new StreamWriter(inputFileName))
                {
                    sw.Write(content);
                }
                return CompressFile(inputFileName);    
            }
            finally
            {
                File.Delete(inputFileName);   
            }
        }
    }
}