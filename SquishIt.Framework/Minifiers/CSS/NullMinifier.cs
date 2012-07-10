namespace SquishIt.Framework.Minifiers.CSS
{
    public class NullMinifier: ICSSMinifier
    {
        public string Minify(string content)
        {
            return content + "\n";
        }     
    }
}