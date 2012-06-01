using System.IO;

namespace SquishIt.Framework.Files
{
    public class FileReader: IFileReader
    {
        readonly StreamReader streamReader;

        public FileReader(IRetryableFileOpener retryableFileOpener, int numberOfRetries, string file)
        {
            streamReader = retryableFileOpener.OpenTextStreamReader(file, numberOfRetries);
        }

        public string ReadLine()
        {
            return streamReader.ReadLine();
        }

        public string ReadToEnd()
        {
            return streamReader.ReadToEnd();
        }

        public void Dispose()
        {
            if (streamReader != null)
            {
                streamReader.Dispose();
            }
        }
    }
}