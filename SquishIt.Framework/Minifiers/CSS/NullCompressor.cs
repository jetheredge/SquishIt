namespace SquishIt.Framework.Minifiers.CSS
{
    public class NullCompressor: ICSSMinifier
    {
        public string Minify(string content)
        {
            return content + "\n";
        }     
    }
}