using System.IO;
using SquishIt.Framework.Utilities;

namespace SquishIt.Framework.Files
{
    public class FileWriter: IFileWriter
    {
        readonly StreamWriter streamWriter;

        public FileWriter(IRetryableFileOpener retryableFileOpener, int numberOfRetries, string file)
        {
            streamWriter = retryableFileOpener.OpenTextStreamWriter(file, numberOfRetries, false);
        }

        public void Write(string value)
        {
            streamWriter.Write(value);
        }

        public void WriteLine(string value)
        {
            streamWriter.WriteLine(value);
        }

        public void Dispose()
        {
            if (streamWriter != null)
            {
                streamWriter.Dispose();    
            }
        }
    }
}