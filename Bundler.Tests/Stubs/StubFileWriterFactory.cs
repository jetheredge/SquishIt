using System.Collections.Generic;
using Bundler.Framework.Utilities;
using Bundler.Tests.Stubs;

namespace Bundler.Framework.Tests.Mocks
{
    public class StubFileWriterFactory: IFileWriterFactory
    {
        private readonly Dictionary<string, string> files = new Dictionary<string, string>();

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