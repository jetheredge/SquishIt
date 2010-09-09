using Microsoft.Ajax.Utilities;

namespace SquishIt.Framework.Css.Compressors
{
    public class MsCompressor: ICssCompressor
    {
        public static string Identifier
        {
            get { return "MsCompressor"; }
        }

        public string CompressContent(string content)
        {
            var minifier = new Minifier();
            return minifier.MinifyStyleSheet(content);
        }

        public string CompressContent(string content, bool removeComments)
        {

            var settings = new CssSettings();
            if(removeComments)
                settings.CommentMode = CssComment.None;

            var minifier = new Minifier();
            return minifier.MinifyStyleSheet(content, settings);
        }

        string ICssCompressor.Identifier
        {
            get { return Identifier; }
        }
    }
}