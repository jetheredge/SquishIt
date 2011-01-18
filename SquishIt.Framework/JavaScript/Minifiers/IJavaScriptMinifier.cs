namespace SquishIt.Framework.JavaScript.Minifiers
{
    public interface IJavaScriptMinifier
    {
        string Identifier { get; }        
        string CompressContent(string content);
    }
}