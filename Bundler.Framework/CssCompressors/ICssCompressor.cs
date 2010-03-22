namespace Bundler.Framework.CssCompressors
{
    public interface ICssCompressor
    {
        string Identifier { get; }
        string CompressContent(string content);
    }
}