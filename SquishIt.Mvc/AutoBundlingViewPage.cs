using System.Web;
using System.Web.Mvc;

namespace SquishIt.Mvc {

	public abstract class AutoBundlingViewPage<TModel> : ViewPage<TModel> {

		protected void AddResources(params string[] resources) {
			AutoBundler.Current.AddResources(AppRelativeVirtualPath, resources);
		}

		public HtmlString ResourceLinks {
			get { return AutoBundler.Current.ResourceLinks; }
		}

	}
}