using System.IO;

namespace SquishIt.Framework.JavaScript.Minifiers
{
    public class NullMinifier: IJavaScriptMinifier
    {
        public static string Identifier
        {
            get { return "null"; }
        }

        string IJavaScriptMinifier.Identifier
        {
            get { return Identifier; }
        }

        public string CompressContent(string content)
        {
            return content;
        }
    }
}