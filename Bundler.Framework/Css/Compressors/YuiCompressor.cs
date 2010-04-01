using Yahoo.Yui.Compressor;

namespace Bundler.Framework.Css.Compressors
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