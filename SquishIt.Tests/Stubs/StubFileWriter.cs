using System;
using SquishIt.Framework.Files;

namespace SquishIt.Tests.Stubs
{
    public class StubFileWriter: IFileWriter
    {
        readonly string file;
        readonly Action<string,string> writeDelegate;

        public StubFileWriter(string file, Action<string,string> writeDelegate)
        {
            this.file = file;
            this.writeDelegate = writeDelegate;
        }

        public void Dispose()
        {
        }

        public void Write(string value)
        {
            writeDelegate(file, value);
        }

        public void WriteLine(string value)
        {
            writeDelegate(file, value + Environment.NewLine);
        }
    }
}