using Yahoo.Yui.Compressor;

namespace SquishIt.Framework.Minifiers.CSS
{
    public class YuiCompressor: ICSSMinifier
    {
        private readonly int columnWidth = 0;
        private readonly CssCompressionType compressionType = CssCompressionType.Hybrid;

        internal YuiCompressor()
        {
        }

        internal YuiCompressor(int columnWidth)
        {
            this.columnWidth = columnWidth;
        }

        internal YuiCompressor(int columnWidth, CssCompressionType compressionType)
        {
            this.columnWidth = columnWidth;
            this.compressionType = compressionType;
        }

        public string Minify(string content)
        {
            return CssCompressor.Compress(content, columnWidth, compressionType, true);
        }
    }
}