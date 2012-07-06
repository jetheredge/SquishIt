using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SquishItAspNetMvcTest.Controllers
{
    public class FeaturesController : Controller
    {
        public ActionResult MinifyCss()
        {
            return View();
        }

        public ActionResult MinifyJs()
        {
            return View();
        }

        public ActionResult RenderNamed()
        {
            return View();
        }

        public ActionResult Imports()
        {
            return View();
        }
    }
}
