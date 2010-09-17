using Microsoft.Ajax.Utilities;

namespace SquishIt.Framework.Css.Compressors
{
    public class MsCompressor: ICssCompressor
    {
        private readonly CssSettings settings;

        public static string Identifier
        {
            get { return "MsCompressor"; }
        }

        public MsCompressor()
        {
        }

        public MsCompressor(CssSettings settings)
        {
            this.settings = settings;
        }

        public string CompressContent(string content)
        {
            var minifier = new Minifier();
            if (settings != null)
            {
                return minifier.MinifyStyleSheet(content, settings);
            }
            return minifier.MinifyStyleSheet(content);
        }

        string ICssCompressor.Identifier
        {
            get { return Identifier; }
        }
    }
}