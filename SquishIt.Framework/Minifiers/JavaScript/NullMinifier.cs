namespace SquishIt.Framework.Minifiers.JavaScript
{
    public class NullMinifier: IJavaScriptMinifier
    {
        public string Minify(string content)
        {
            return content + "\n";
        }
    }
}