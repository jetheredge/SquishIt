using System;
using SquishIt.Framework.Utilities;

namespace SquishIt.Framework.Tests.Mocks
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