namespace SquishIt.Framework.Utilities
{
    public interface IDebugStatusReader
    {
        bool IsDebuggingEnabled();
        void ForceDebug();
        void ForceRelease();
    }
}