using System.Collections.Generic;
using SquishIt.Framework.Files;

namespace SquishIt.Tests.Stubs
{
    public class StubFileWriterFactory: IFileWriterFactory
    {
        readonly Dictionary<string, string> files = new Dictionary<string, string>();

        public Dictionary<string, string> Files
        {
            get { return files; }
        }

        public IFileWriter GetFileWriter(string file)
        {
            return new StubFileWriter(file, (f, contents) =>
                                                {
                                                    if (files.ContainsKey(f))
                                                    {
                                                        files[f] = files[f] + contents;
                                                    }
                                                    else
                                                    {
                                                        files[f] = contents;
                                                    }
                                                });
        }
    }
}