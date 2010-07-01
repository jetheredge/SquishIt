using System;
using System.IO;

namespace SquishIt.Framework.Files
{
    public class FileWriter: IFileWriter
    {
        private StreamWriter sw;

        public FileWriter(string file)
        {
            sw = new StreamWriter(file, false, System.Text.Encoding.UTF8);
        }

        public void Write(string value)
        {
            sw.Write(value);
        }

        public void WriteLine(string value)
        {
            sw.WriteLine(value);
        }

        public void Dispose()
        {
            if (sw != null)
            {
                sw.Dispose();    
            }
        }
    }
}