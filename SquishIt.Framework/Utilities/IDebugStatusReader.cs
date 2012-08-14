using System;

namespace SquishIt.Framework.Utilities
{
    public interface IDebugStatusReader
    {
        bool IsDebuggingEnabled(Func<bool> debugPredicate = null);
        void ForceDebug();
        void ForceRelease();
    }
}