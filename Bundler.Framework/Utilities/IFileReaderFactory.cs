namespace Bundler.Framework.Utilities
{
    public interface IFileReaderFactory
    {
        IFileReader GetFileReader(string file);
    }
}