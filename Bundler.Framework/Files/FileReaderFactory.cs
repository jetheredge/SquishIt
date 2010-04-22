using System;
using System.IO;

namespace Bundler.Framework.Files
{
    public class FileReaderFactory: IFileReaderFactory
    {
        public IFileReader GetFileReader(string file)
        {
            return new FileReader(file);
        }

        public bool FileExists(string file)
        {
            return File.Exists(file);
        }
    }
}