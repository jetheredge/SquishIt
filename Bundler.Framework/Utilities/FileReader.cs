using System;
using System.IO;

namespace Bundler.Framework.Utilities
{
    public class FileReader: IFileReader
    {
        private StreamReader streamReader;
        
        public FileReader(string file)
        {
            streamReader = new StreamReader(file);
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