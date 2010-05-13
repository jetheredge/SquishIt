using System;

namespace SquishIt.Framework.Files
{
    public class FileWriterFactory: IFileWriterFactory
    {
        public IFileWriter GetFileWriter(string file)
        {
            return new FileWriter(file);
        }
    }
}