namespace Bundler.Framework.Files
{
    public interface IFileReaderFactory
    {
        IFileReader GetFileReader(string file);
        bool FileExists(string file);
    }
}