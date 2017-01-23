using System.Web.Mvc;
using SquishIt.Framework;

namespace SquishIt.Mvc
{
	public class SquishItController: Controller
	{
		public ActionResult Js(string id)
		{
            return Content(Bundle.JavaScript().RenderCached(id), Configuration.Instance.DefaultJavascriptMimeType);
		}

		public ActionResult Css(string id)
		{
            return Content(Bundle.Css().RenderCached(id), Configuration.Instance.DefaultCssMimeType);
		}
	}
}