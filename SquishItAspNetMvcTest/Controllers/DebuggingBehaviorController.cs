using System.Web.Mvc;

namespace SquishItAspNetMvcTest.Controllers
{
    public class DebuggingBehaviorController : Controller
    {
        //
        // GET: /DebuggingBehavior/

        public ActionResult ForceViaPredicate()
        {
            return View();
        }

    }
}
