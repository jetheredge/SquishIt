namespace SquishIt.Framework.Css.Compressors
{
    public interface ICssCompressor
    {
        string Identifier { get; }
        string CompressContent(string content);
        string CompressContent(string content, bool removeComments);
    }
}