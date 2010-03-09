namespace Bundler.Framework.CssCompressors
{
    public interface ICssCompressor
    {
        string Identifier { get; }
        string CompressFile(string file);
        string CompressContent(string content);
    }
}