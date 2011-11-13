using System.Web.Mvc;
using SquishIt.Framework.JavaScript;

namespace SquishIt.Mvc
{
	public static class JavaScriptBundleExtensions
    {
		public static MvcHtmlString MvcRender(this JavaScriptBundle javaScriptBundle, string renderTo)
		{
			return MvcHtmlString.Create(javaScriptBundle.Render(renderTo));
		}

		public static MvcHtmlString MvcRenderNamed(this JavaScriptBundle javaScriptBundle, string name)
		{
			return MvcHtmlString.Create(javaScriptBundle.RenderNamed(name));
		}

		public static MvcHtmlString MvcRenderCachedAssetTag(this JavaScriptBundle javaScriptBundle, string name)
		{
			return MvcHtmlString.Create(javaScriptBundle.RenderCachedAssetTag(name));
		}
	}
}