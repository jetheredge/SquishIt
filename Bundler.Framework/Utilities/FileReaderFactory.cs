namespace Bundler.Framework.Utilities
{
    public class FileReaderFactory: IFileReaderFactory
    {
        public IFileReader GetFileReader(string file)
        {
            return new FileReader(file);
        }
    }
}