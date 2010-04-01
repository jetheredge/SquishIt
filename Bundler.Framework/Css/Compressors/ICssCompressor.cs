namespace Bundler.Framework.Css.Compressors
{
    public interface ICssCompressor
    {
        string Identifier { get; }
        string CompressContent(string content);
    }
}