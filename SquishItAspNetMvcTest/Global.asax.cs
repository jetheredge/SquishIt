using System.Web.Mvc;
using System.Web.Routing;
using SquishIt.Framework;
/*
//uncomment this (and comment out MsIe equivalents) to test jurassic-based preprocessors
using SquishIt.CoffeeScript;
using SquishIt.Hogan;
*/
using SquishIt.MsIeCoffeeScript;
using SquishIt.MsIeHogan;
using SquishIt.Less;
using SquishIt.Sass;

namespace SquishItAspNetMvcTest
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
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
            Bundle.RegisterScriptPreprocessor(new HoganPreprocessor());
            Bundle.RegisterScriptPreprocessor(new CoffeeScriptPreprocessor());

            Bundle.RegisterStylePreprocessor(new LessPreprocessor());

            Bundle.RegisterStylePreprocessor(new SassPreprocessor());

            Bundle.JavaScript()
                .Add("/assets/js/jquery_1.7.2.js")
                .Add("/assets/js/minifyjs_test.js")
                .ForceRelease()
                .AsNamed("RenderNamedTest", "/output/rendernamed_test_output.js");
        }
    }
}