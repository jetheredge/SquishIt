using SquishIt.Framework.Utilities;

namespace SquishIt.Framework.Files
{
    public class FileWriterFactory: IFileWriterFactory
    {
        protected IRetryableFileOpener RetryableFileOpener;
        protected int NumberOfRetries;

        public FileWriterFactory(IRetryableFileOpener retryableFileOpener, int numberOfRetries)
        {
            RetryableFileOpener = retryableFileOpener;
            NumberOfRetries = numberOfRetries;
        }

        public IFileWriter GetFileWriter(string file)
        {
            return new FileWriter(RetryableFileOpener, NumberOfRetries, file);
        }
    }
}