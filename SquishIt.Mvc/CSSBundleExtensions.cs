using System;
using System.Web.Mvc;
using SquishIt.Framework.CSS;

namespace SquishIt.Mvc
{
	public static class CSSBundleExtensions
    {
        [Obsolete("Renamed to MvcRender() for concistency with other extension methods.")]
        public static MvcHtmlString RenderMvc(this CSSBundle cssBundle, string renderTo)
        {
            return cssBundle.MvcRender(renderTo);
        }

        public static MvcHtmlString MvcRender(this CSSBundle cssBundle, string renderTo)
        {
            return MvcHtmlString.Create(cssBundle.Render(renderTo));
        }

		public static MvcHtmlString MvcRenderNamed(this CSSBundle cssBundle, string name)
		{
			return MvcHtmlString.Create(cssBundle.RenderNamed(name));
		}

		public static MvcHtmlString MvcRenderCachedAssetTag(this CSSBundle cssBundle, string name)
		{
			return MvcHtmlString.Create(cssBundle.RenderCachedAssetTag(name));
		}
	}
}