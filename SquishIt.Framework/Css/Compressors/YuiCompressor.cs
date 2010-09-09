using Yahoo.Yui.Compressor;

namespace SquishIt.Framework.Css.Compressors
{
    public class YuiCompressor: ICssCompressor
    {
        public static string Identifier
        {
            get { return "YuiCompressor"; }
        }

        public string CompressContent(string content, bool removeComments)
        {
            return CssCompressor.Compress(content, 0, CssCompressionType.StockYuiCompressor, removeComments);
        }

        public string CompressContent(string content)
        {
            return CompressContent(content, true);
        }

        string ICssCompressor.Identifier
        {
            get { return Identifier; }
        }
    }
}