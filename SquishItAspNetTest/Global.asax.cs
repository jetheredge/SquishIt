using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using SquishIt.Framework;
using SquishIt.Framework.Css;
using SquishIt.Less;

namespace SquishItAspNetTest
{
    public class Global : System.Web.HttpApplication
    {
        protected void Application_Start(object sender, EventArgs e)
        {
            Bundle.RegisterStylePreprocessor(new LessPreprocessor());

            Bundle.JavaScript()
                .ForceRelease()
                .Add("~/js/alert.js")
                .AsNamed("test", "~/js/output_#.js");
        }

        protected void Session_Start(object sender, EventArgs e)
        {

        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {

        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        protected void Application_Error(object sender, EventArgs e)
        {

        }

        protected void Session_End(object sender, EventArgs e)
        {

        }

        protected void Application_End(object sender, EventArgs e)
        {

        }
    }
}