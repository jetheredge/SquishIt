using System;
using SquishIt.Framework.Files;

namespace SquishIt.Framework.Tests.Mocks
{
    public class StubFileReaderFactory: IFileReaderFactory
    {
        private string contents;
        private bool fileExists;

        public void SetContents(string contents)
        {
            this.contents = contents;
        }

        public void SetFileExists(bool fileExists)
        {
            this.fileExists = fileExists;
        }
        
        public IFileReader GetFileReader(string file)
        {
            return new StubFileReader(file, contents);
        }

        public bool FileExists(string file)
        {
            return fileExists;
        }
    }
}