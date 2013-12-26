using System.Web;
using System.Web.Mvc;
using System.Web.WebPages;
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

        public static string ViewName(this HtmlHelper html)
        {
            var webPage = html.ViewDataContainer as WebPageBase;
            return webPage.VirtualPath;
        }
        public static void AddResources(this HtmlHelper html, params string[] resourceFiles)
        {
            AutoBundler.Current.AddResources(html.ViewName(),resourceFiles);
        }

        public static void AddStyleResources(this HtmlHelper html, params string[] resourceFiles)
        {
            AutoBundler.Current.AddStyleResources(html.ViewName(), resourceFiles);
        }

        public static void AddScriptResources(this HtmlHelper html, params string[] resourceFiles)
        {
            AutoBundler.Current.AddScriptResources(html.ViewName(), resourceFiles);
        }

        public static HtmlString ResourceLinks(this HtmlHelper html)
        {
            return new HtmlString(AutoBundler.Current.StyleResourceLinks + AutoBundler.Current.ScriptResourceLinks);
        }

        public static HtmlString StyleResourceLinks(this HtmlHelper html)
        {
            return new HtmlString(AutoBundler.Current.StyleResourceLinks);
        }

        public static HtmlString ScriptResourceLinks(this HtmlHelper html)
        {
            return new HtmlString(AutoBundler.Current.ScriptResourceLinks);
        }
    }
}
