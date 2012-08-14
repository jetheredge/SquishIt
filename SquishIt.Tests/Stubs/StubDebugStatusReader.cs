using System;
using SquishIt.Framework;
using SquishIt.Framework.Utilities;

namespace SquishIt.Tests.Stubs
{
    public class StubDebugStatusReader: IDebugStatusReader
    {
        bool isDebuggingEnabled;

        public StubDebugStatusReader()
        {
            isDebuggingEnabled = true;
        }

        public StubDebugStatusReader(bool isDebuggingEnabled)
        {
            this.isDebuggingEnabled = isDebuggingEnabled;
        }

        public bool IsDebuggingEnabled(Func<bool> debugPredicate = null)
        {
            return (isDebuggingEnabled || debugPredicate.SafeExecute());
        }

        public void ForceDebug()
        {
            isDebuggingEnabled = true;
        }

        public void ForceRelease()
        {
            isDebuggingEnabled = false;
        }
    }
}