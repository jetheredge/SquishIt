using System.Web;
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

		public static void AddResources(this HtmlHelper html, params string[] resourceFiles) {
			AutoBundler.Current.AddResources(resourceFiles);
		}

		public static void AddCssResources(this HtmlHelper html, params string[] resourceFiles) {
			AutoBundler.Current.AddCssResources(resourceFiles);
		}

		public static void AddJsResources(this HtmlHelper html, params string[] resourceFiles) {
			AutoBundler.Current.AddJsResources(resourceFiles);
		}

		public static HtmlString ResourceLinks(this HtmlHelper html) {
			return AutoBundler.Current.ResourceLinks;
		}
    }
}
