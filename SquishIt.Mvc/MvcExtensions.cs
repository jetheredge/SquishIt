using System.Web;
using System.Web.Mvc;
using SquishIt.Framework.Css;
using SquishIt.Framework.JavaScript;

namespace SquishIt.Mvc
{
	public static class MvcExtensions
	{
		public static IHtmlString RenderMvc(this ICssBundleBuilder cssBundleBuilder, string renderTo)
		{
			return new MvcHtmlString(cssBundleBuilder.Render(renderTo));
		}

		public static IHtmlString RenderMvc(this IJavaScriptBundleBuilder javaScriptBundleBuilder, string renderTo)
		{
			return new MvcHtmlString(javaScriptBundleBuilder.Render(renderTo));
		}

		public static IHtmlString RenderNamedMvc(this IJavaScriptBundle javaScriptBundle, string name)
		{
			return new MvcHtmlString(javaScriptBundle.RenderNamed(name));
		}

		public static IHtmlString RenderNamedMvc(this ICssBundle cssBundle, string name)
		{
			return new MvcHtmlString(cssBundle.RenderNamed(name));
		}
	}
}