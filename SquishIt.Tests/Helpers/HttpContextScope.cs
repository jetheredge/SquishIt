using System;
using System.Web;

namespace SquishIt.Tests.Helpers
{
    public class HttpContextScope : IDisposable
    {
        public HttpContextScope(HttpContextBase context)
        {
            Framework.HttpContext.contextBase = context;
        }

        public void Dispose()
        {
            Framework.HttpContext.contextBase = null;
        }
    }
}
