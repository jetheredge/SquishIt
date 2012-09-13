using Microsoft.Ajax.Utilities;

namespace SquishIt.Framework.Minifiers.CSS
{
    public class MsMinifier: ICSSMinifier
    {
        public CssSettings Settings { get; set; }

        public MsMinifier()
        {
        }

        public MsMinifier(CssSettings settings)
        {
            Settings = settings;
        }

        public string Minify(string content)
        {
            var minifier = new Minifier();
            var stylesheet = string.Empty;

            stylesheet = Settings != null 
                ? minifier.MinifyStyleSheet(content, Settings) 
                : minifier.MinifyStyleSheet(content);

            return stylesheet;
        }
    }
}