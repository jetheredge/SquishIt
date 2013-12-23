using System.Web.Mvc;

namespace SquishItAspNetMvcTest.Controllers
{
    public class AutoBundlingController : Controller
    {
        public ActionResult Javascript()
        {
            return View();
        }

        public ActionResult Css()
        {
            return View();
        }
    }
}
