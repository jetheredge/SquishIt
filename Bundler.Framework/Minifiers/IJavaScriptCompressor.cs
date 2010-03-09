namespace Bundler.Framework.Minifiers
{
    public interface IJavaScriptCompressor
    {
        string Identifier { get; }
        string CompressFile(string file);
        string CompressContent(string content);
    }
}