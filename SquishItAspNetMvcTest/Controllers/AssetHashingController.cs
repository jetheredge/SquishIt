using System.Web.Mvc;

namespace SquishItAspNetMvcTest.Controllers
{
    public class AssetHashingController : Controller
    {
        public ActionResult LocalRelative()
        {
            return View();
        }

        public ActionResult RootRelative()
        {
            return View();
        }
    }
}
