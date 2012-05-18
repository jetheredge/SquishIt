using System.Web;

namespace SquishIt.Framework
{
    class HttpContext
    {
        internal static HttpContextBase contextBase;
        internal static HttpContextBase Current
        {
            get { return contextBase ??
                (System.Web.HttpContext.Current == null ? null : new HttpContextWrapper(System.Web.HttpContext.Current)); }
        }
    }
}
