using Yahoo.Yui.Compressor;

namespace SquishIt.Framework.Css.Compressors
{
    public class YuiCompressor: ICssCompressor
    {
        private readonly int columnWidth = 0;
        private readonly CssCompressionType compressionType = CssCompressionType.StockYuiCompressor;

        public static string Identifier
        {
            get { return "YuiCompressor"; }
        }

        public YuiCompressor()
        {
        }

        public YuiCompressor(int columnWidth)
        {
            this.columnWidth = columnWidth;
        }

        public YuiCompressor(int columnWidth, CssCompressionType compressionType)
        {
            this.columnWidth = columnWidth;
            this.compressionType = compressionType;
        }

        public string CompressContent(string content)
        {
            return CssCompressor.Compress(content, columnWidth, compressionType);
        }

        string ICssCompressor.Identifier
        {
            get { return Identifier; }
        }
    }
}