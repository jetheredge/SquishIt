using SquishIt.Framework.Files;

namespace SquishIt.Tests.Stubs
{
    public class StubFileReader: IFileReader
    {
        readonly string file;
        readonly string contents;

        public string File
        {
            get { return file; }
        }

        public StubFileReader(string file, string contents)
        {
            this.file = file;
            this.contents = contents;
        }

        public void Dispose()
        {            
        }

        public string ReadLine()
        {
            return contents;
        }

        public string ReadToEnd()
        {
            return contents;
        }
    }
}