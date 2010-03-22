using System;
using System.IO;
using Bundler.Framework.Minifiers;
using Yahoo.Yui.Compressor;

namespace Bundler.Framework.CssCompressors
{
    public class YuiCompressor: ICssCompressor
    {
        public static string Identifier
        {
            get { return "YuiCompressor"; }
        }

        public string CompressContent(string content)
        {
            return CssCompressor.Compress(content, 0, CssCompressionType.StockYuiCompressor);
        }

        string ICssCompressor.Identifier
        {
            get { return Identifier; }
        }
    }
}