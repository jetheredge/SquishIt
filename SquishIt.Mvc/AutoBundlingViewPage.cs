using System.Web;
using System.Web.Mvc;

namespace SquishIt.Mvc {

	public abstract class AutoBundlingViewPage<TModel> : ViewPage<TModel> {

        protected void AddResources(params string[] resources)
        {
            AutoBundler.Current.AddResources(resources);
        }

        protected void AddStyleResources(params string[] resources)
        {
            AutoBundler.Current.AddStyleResources(resources);
        }

        protected void AddScriptResources(params string[] resources)
        {
            AutoBundler.Current.AddScriptResources(resources);
        }

		public HtmlString ResourceLinks {
			get { return new HtmlString(AutoBundler.Current.StyleResourceLinks + AutoBundler.Current.ScriptResourceLinks); }
		}

	    public HtmlString StyleResourceLinks
	    {
	        get { return new HtmlString(AutoBundler.Current.StyleResourceLinks);}
	    }

	    public HtmlString ScriptResourceLinks
	    {
	        get { return new HtmlString(AutoBundler.Current.ScriptResourceLinks);}
	    }
	}
}