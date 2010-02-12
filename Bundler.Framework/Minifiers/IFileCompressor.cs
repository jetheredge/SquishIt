namespace Bundler.Framework.Minifiers
{
    public interface IFileCompressor
    {
        string Compress(string file);
    }
}