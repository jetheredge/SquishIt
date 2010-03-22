using System;

namespace Bundler.Framework.Utilities
{
    public class FileWriterFactory: IFileWriterFactory
    {
        public IFileWriter GetFileWriter(string file)
        {
            return new FileWriter(file);
        }
    }
}