using Yahoo.Yui.Compressor;

namespace SquishIt.Framework.Css.Compressors
{
    public class YuiCompressor: ICssCompressor
    {
        private readonly int columnWidth = 0;
        private readonly CssCompressionType compressionType = CssCompressionType.StockYuiCompressor;
        private readonly bool removeComments = true;

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

        public YuiCompressor(int columnWidth, CssCompressionType compressionType, bool removeComments)
        {
            this.columnWidth = columnWidth;
            this.compressionType = compressionType;
            this.removeComments = removeComments;
        }

        public string CompressContent(string content)
        {
            return CssCompressor.Compress(content, columnWidth, compressionType, removeComments);
        }

        string ICssCompressor.Identifier
        {
            get { return Identifier; }
        }
    }
}