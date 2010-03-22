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