using System;
using Bundler.Framework.Utilities;

namespace Bundler.Framework.Tests.Mocks
{
    public class StubDebugStatusReader: IDebugStatusReader
    {
        private readonly bool isDebuggingEnabled;

        public StubDebugStatusReader()
        {
            isDebuggingEnabled = true;
        }

        public StubDebugStatusReader(bool isDebuggingEnabled)
        {
            this.isDebuggingEnabled = isDebuggingEnabled;
        }

        public bool IsDebuggingEnabled()
        {
            return isDebuggingEnabled;
        }
    }
}