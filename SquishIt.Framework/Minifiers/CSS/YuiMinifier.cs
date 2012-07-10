using Yahoo.Yui.Compressor;

namespace SquishIt.Framework.Minifiers.CSS
{
    public class YuiMinifier: ICSSMinifier
    {
        readonly CssCompressor compressor;

        internal YuiMinifier()
        {
            compressor = new CssCompressor();
        }

        internal YuiMinifier(int columnWidth)
        {
            compressor = new CssCompressor
            {
                LineBreakPosition = columnWidth,
            };
        }

        internal YuiMinifier(int columnWidth, CompressionType compressionType)
        {
            compressor = new CssCompressor
            {
                CompressionType = compressionType,
                LineBreakPosition = columnWidth,
            };
        }

        internal YuiMinifier(int columnWidth, CompressionType compressionType, bool removeComments)
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