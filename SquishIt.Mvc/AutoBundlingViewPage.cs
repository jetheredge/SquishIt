using System.Web;
using System.Web.Mvc;

namespace SquishIt.Mvc
{

    public abstract class AutoBundlingViewPage<TModel> : ViewPage<TModel>
    {
        protected string ViewFileName
        {
            get { return this.AppRelativeVirtualPath; }
        }
        protected void AddResources(params string[] resources)
        {
            AutoBundler.Current.AddResources(ViewFileName, resources);
        }

        protected void AddStyleResources(params string[] resources)
        {
            AutoBundler.Current.AddStyleResources(ViewFileName, resources);
        }

        protected void AddScriptResources(params string[] resources)
        {
            AutoBundler.Current.AddScriptResources(ViewFileName, resources);
        }

        public HtmlString ResourceLinks
        {
            get { return new HtmlString(AutoBundler.Current.StyleResourceLinks + AutoBundler.Current.ScriptResourceLinks); }
        }

        public HtmlString StyleResourceLinks
        {
            get { return new HtmlString(AutoBundler.Current.StyleResourceLinks); }
        }

        public HtmlString ScriptResourceLinks
        {
            get { return new HtmlString(AutoBundler.Current.ScriptResourceLinks); }
        }
    }
}