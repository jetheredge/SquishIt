using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using SquishIt.Framework;

namespace SquishItAspNetMvcTest
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : HttpApplication
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                "Default", // Route name
                "{controller}/{action}/{id}", // URL with parameters
                new { controller = "Home", action = "Index", id = UrlParameter.Optional } // Parameter defaults
            );

        }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);

            Bundle.JavaScript()
                .Add("/assets/js/jquery_1.7.2.js")
                .Add("/assets/js/minifyjs_test.js")
                .ForceRelease()
                .AsNamed("RenderNamedTest", "/output/rendernamed_test_output.js");

            Bundle.JavaScript()
                .Add("assets/js/jquery_1.7.2.js")
                .Add("/assets/js/minifyjs_test.js")
                .ForceRelease()
                .AsCached("render-cached", "~/AssetCache/Js/render-cached");
        }
    }
}