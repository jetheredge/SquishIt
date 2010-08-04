using System;
using System.IO;
using System.Text;

namespace SquishIt.Framework.Files
{
    public class FileWriter: IFileWriter
    {
        private readonly StreamWriter streamWriter;

        public FileWriter(string file)
        {
            streamWriter = new StreamWriter(file, false, Encoding.UTF8);
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