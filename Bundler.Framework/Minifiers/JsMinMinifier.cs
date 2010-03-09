using System;
using System.IO;
using Bundler.Framework.jsmin;

namespace Bundler.Framework.Minifiers
{
    public class JsMinMinifier: IJavaScriptCompressor
    {
        public static string Identifier
        {
            get { return "jsmin"; }
        }

        string IJavaScriptCompressor.Identifier
        {
            get { return Identifier; }
        }

        public string CompressFile(string file)
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

        public string CompressContent(string content)
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