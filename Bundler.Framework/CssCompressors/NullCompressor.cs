using System;
using System.IO;
using Bundler.Framework.Minifiers;

namespace Bundler.Framework.CssCompressors
{
    public class NullCompressor: ICssCompressor
    {
        public static string Identifier
        {
            get { return "NullCompressor"; }
        }

        public string CompressFile(string file)
        {
            using (var sr = new StreamReader(file))
            {
                return sr.ReadToEnd();
            }
        }

        public string CompressContent(string content)
        {
            return content;
        }

        string ICssCompressor.Identifier
        {
            get { return Identifier; }
        }        
    }
}