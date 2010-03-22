namespace Bundler.Framework.Minifiers
{
    public interface IJavaScriptCompressor
    {
        string Identifier { get; }        
        string CompressContent(string content);
    }
}