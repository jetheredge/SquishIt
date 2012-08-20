using System.Web.Mvc;
using SquishIt.Framework.CSS;
using SquishIt.Framework.JavaScript;

namespace SquishIt.Mvc
{
    public static class HtmlHelperExtensions
    {
        public static CSSBundle BundleCss(this HtmlHelper html)
        {
            return new CSSBundle();
        }

        public static JavaScriptBundle BundleJavaScript(this HtmlHelper html)
        {
            return new JavaScriptBundle();
        }
    }
}
