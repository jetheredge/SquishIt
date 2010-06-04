using System;
using System.Collections.Generic;
using SquishIt.Framework.Files;

namespace SquishIt.Framework.Tests.Mocks
{
    public class StubFileReaderFactory: IFileReaderFactory
    {
        private string contents;
        private bool fileExists;
        private Dictionary<string, string> contentsForFiles = new Dictionary<string, string>();

        public void SetContents(string contents)
        {
            this.contents = contents;
        }

        public void SetContentsForFile(string file, string contents)
        {
            contentsForFiles.Add(file, contents);
        }

        public void SetFileExists(bool fileExists)
        {
            this.fileExists = fileExists;
        }
        
        public IFileReader GetFileReader(string file)
        {
            if (contentsForFiles.ContainsKey(file))
            {
                return new StubFileReader(file, contentsForFiles[file]);
            }
            return new StubFileReader(file, contents);
        }

        public bool FileExists(string file)
        {
            return fileExists;
        }
    }
}