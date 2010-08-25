using SquishIt.Framework.Utilities;

namespace SquishIt.Tests.Stubs
{
    public class StubDebugStatusReader: IDebugStatusReader
    {
        private bool isDebuggingEnabled;

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

        #region IDebugStatusReader Members


        public void ForceDebug()
        {
            isDebuggingEnabled = true;
        }

        public void ForceRelease()
        {
            isDebuggingEnabled = false;
        }

        #endregion
    }
}