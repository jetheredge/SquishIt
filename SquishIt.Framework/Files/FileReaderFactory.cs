using System.IO;

namespace SquishIt.Framework.Files
{
    public class FileReaderFactory: IFileReaderFactory
    {
        protected IRetryableFileOpener RetryableFileOpener;
        protected int NumberOfRetries;

        public FileReaderFactory(IRetryableFileOpener retryableFileOpener, int numberOfRetries)
        {
            RetryableFileOpener = retryableFileOpener;
            NumberOfRetries = numberOfRetries;
        }

        public IFileReader GetFileReader(string file)
        {
            return new FileReader(RetryableFileOpener,NumberOfRetries, file);
        }

        public bool FileExists(string file)
        {
            return File.Exists(file);
        }
    }
}