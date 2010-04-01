namespace Bundler.Framework.Files
{
    public interface IFileWriterFactory
    {
        IFileWriter GetFileWriter(string file);
    }
}