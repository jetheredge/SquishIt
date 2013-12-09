using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SquishIt.Framework;
using SquishIt.Framework.CSS;
using SquishIt.Framework.JavaScript;
using SquishIt.Mvc;

namespace SquishIt.Mvc {

	public abstract class AutoBundlingViewPage<TModel> : WebViewPage<TModel> {

		protected void AddResources(params string[] resources) {
			AutoBundler.Current.AddResources(VirtualPath, resources);
		}

		public HtmlString ResourceLinks {
			get { return AutoBundler.Current.ResourceLinks; }
		}

	}
}