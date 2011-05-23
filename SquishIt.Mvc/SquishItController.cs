using System.Web.Mvc;
using SquishIt.Framework;

namespace SquishIt.Mvc
{
	public class SquishItController: Controller
	{
		public ActionResult Js(string id)
		{
			return Content(Bundle.JavaScript().RenderCached(id));
		}

		public ActionResult Css(string id)
		{
			return Content(Bundle.CSS().RenderCached(id));
		}
	}
}