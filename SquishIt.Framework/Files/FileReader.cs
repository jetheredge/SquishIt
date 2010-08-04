using System;
using System.IO;
using System.Text;

namespace SquishIt.Framework.Files
{
    public class FileReader: IFileReader
    {
        private readonly StreamReader streamReader;

        public FileReader(string file)
        {
            streamReader = new StreamReader(file, Encoding.UTF8);
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