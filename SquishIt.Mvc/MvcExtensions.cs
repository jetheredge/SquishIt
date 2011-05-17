using System.Web;
using System.Web.Mvc;
using SquishIt.Framework.CSS;
using SquishIt.Framework.JavaScript;

namespace SquishIt.Mvc
{
	public static class MvcExtensions
	{
		public static IHtmlString RenderMvc(this CSSBundle cssBundleBuilder, string renderTo)
		{
			return new MvcHtmlString(cssBundleBuilder.Render(renderTo));
		}

		public static IHtmlString RenderMvc(this JavaScriptBundle javaScriptBundleBuilder, string renderTo)
		{
			return new MvcHtmlString(javaScriptBundleBuilder.Render(renderTo));
		}

        public static IHtmlString RenderNamedMvc(this JavaScriptBundle javaScriptBundle, string name)
		{
			return new MvcHtmlString(javaScriptBundle.RenderNamed(name));
		}

		public static IHtmlString RenderNamedMvc(this CSSBundle cssBundle, string name)
		{
			return new MvcHtmlString(cssBundle.RenderNamed(name));
		}
	}
}