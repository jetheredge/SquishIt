using System;
using System.Web;

namespace Bundler.Framework.Utilities
{
    public class DebugStatusReader: IDebugStatusReader
    {
        public bool IsDebuggingEnabled()
        {
            return HttpContext.Current != null && HttpContext.Current.IsDebuggingEnabled;
        }
    }
}