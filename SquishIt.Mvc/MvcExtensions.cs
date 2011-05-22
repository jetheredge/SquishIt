using System.Web;
using System.Web.Mvc;
using SquishIt.Framework.CSS;
using SquishIt.Framework.JavaScript;

namespace SquishIt.Mvc
{
	public static class MvcExtensions
	{
		public static MvcHtmlString RenderMvc(this CSSBundle cssBundleBuilder, string renderTo)
		{
			return MvcHtmlString.Create(cssBundleBuilder.Render(renderTo));
		}

		public static MvcHtmlString RenderMvc(this JavaScriptBundle javaScriptBundleBuilder, string renderTo)
		{
			return MvcHtmlString.Create(javaScriptBundleBuilder.Render(renderTo));
		}

		public static MvcHtmlString RenderNamedMvc(this JavaScriptBundle javaScriptBundle, string name)
		{
			return MvcHtmlString.Create(javaScriptBundle.RenderNamed(name));
		}

		public static MvcHtmlString RenderNamedMvc(this CSSBundle cssBundle, string name)
		{
			return MvcHtmlString.Create(cssBundle.RenderNamed(name));
		}
	}
}