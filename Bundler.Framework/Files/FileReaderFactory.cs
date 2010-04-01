namespace Bundler.Framework.Files
{
    public class FileReaderFactory: IFileReaderFactory
    {
        public IFileReader GetFileReader(string file)
        {
            return new FileReader(file);
        }
    }
}