namespace Bundler.Framework.Utilities
{
    public interface IFileWriterFactory
    {
        IFileWriter GetFileWriter(string file);
    }
}