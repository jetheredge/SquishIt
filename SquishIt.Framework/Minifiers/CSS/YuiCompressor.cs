using Yahoo.Yui.Compressor;

namespace SquishIt.Framework.Minifiers.CSS
{
    public class YuiCompressor: ICSSMinifier
    {
        readonly CssCompressor compressor;

        internal YuiCompressor()
        {
            compressor = new CssCompressor();
        }

        internal YuiCompressor(int columnWidth)
        {
            compressor = new CssCompressor
            {
                LineBreakPosition = columnWidth,
            };
        }

        internal YuiCompressor(int columnWidth, CompressionType compressionType)
        {
            compressor = new CssCompressor
            {
                CompressionType = compressionType,
                LineBreakPosition = columnWidth,
            };
        }

        internal YuiCompressor(int columnWidth, CompressionType compressionType, bool removeComments)
        {
            compressor = new CssCompressor
                             {
                                 CompressionType = compressionType,
                                 LineBreakPosition = columnWidth,
                                 RemoveComments = removeComments
                             };
        }

        public string Minify(string content)
        {
            return compressor.Compress(content);
        }
    }
}