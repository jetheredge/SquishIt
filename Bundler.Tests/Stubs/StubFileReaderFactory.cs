using System;
using Bundler.Framework.Utilities;

namespace Bundler.Framework.Tests.Mocks
{
    public class StubFileReaderFactory: IFileReaderFactory
    {
        private string contents;

        public void SetContents(string contents)
        {
            this.contents = contents;
        }
        
        public IFileReader GetFileReader(string file)
        {
            return new StubFileReader(file, contents);
        }
    }
}