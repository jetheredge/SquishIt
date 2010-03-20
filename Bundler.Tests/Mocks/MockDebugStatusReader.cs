using System;
using Bundler.Framework.Utilities;

namespace Bundler.Framework.Tests.Mocks
{
    public class MockDebugStatusReader: IDebugStatusReader
    {
        public bool IsDebuggingEnabled()
        {
            return true;
        }
    }
}