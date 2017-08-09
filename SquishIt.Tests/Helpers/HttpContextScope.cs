using System;
using System.Web;
using HttpContext = SquishIt.AspNet.HttpContext;

namespace SquishIt.Tests.Helpers
{
    public class HttpContextScope : IDisposable
    {
        public HttpContextScope(HttpContextBase context)
        {
            HttpContext.contextBase = context;
        }

        public void Dispose()
        {
            HttpContext.contextBase = null;
        }
    }
}
