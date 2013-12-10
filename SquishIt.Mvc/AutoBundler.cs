using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using SquishIt.Framework;
using SquishIt.Framework.Base;
using SquishIt.Framework.CSS;
using SquishIt.Framework.JavaScript;
using SquishIt.Mvc;

namespace SquishIt.Mvc {
	/// <summary>
	/// Caches a bundle of .js and/or .css specific to the specified ViewPage,
	/// at a path similar to: shared_signinpartial_F3BD3CCE1DFCEA70F5524C57164EB48E.js
	/// </summary>
	public class AutoBundler {
		// Since resources can come from multiple folder and bundle to one file,
		// we just arbitrarily pick a folder to store them all in.
		// This also reduces the write-access footprint the web app requires.
		public static string AssetPath = "~/assets/";
		static AutoBundler() {

		}

		// Since some resource types contain paths relative to the current location,
		// we offer the option of splitting bundles on path boundaries.
		public static bool KeepJavascriptInOriginalFolder = false;

		// Grouping bundle content by folder can result in content order changes
		// For example, [  "/a/css1", "/b/css2",   "/a/css3"  ]
		// may become   [ ["/a/css1", "/a/css3"], ["/b/css2"] ]
		// for that reason these should be off be default
		// Note that SquishIt does support relocating CSS url() paths.
		public static bool KeepCssInOriginalFolder = true;

		// It might be nice to allow multiple link queues to be named,
		// for example one for the head and another for the tail of body
		/// <summary>
		/// Get the cached AutoBundler for this HTTP context,
		/// or create a new AutoBundler if this is first request for it.
		/// </summary>
		/// <returns>The bundler for operating on the current request.</returns>
		public static AutoBundler Current {
			get { return NewOrCached(HttpContext.Current.Items); }
		}

		/// <summary>
		/// Emits here the bundled resources declared with "AddResources" on all child controls
		/// </summary>
		public HtmlString ResourceLinks {
			get {
				return new HtmlString(string.Join("", resourceLinks));
			}
		}

		// This allows all resources to be specified in a single command,
		// which permits .css and .js resources to be declared in any order the site author prefers
		// It may be misleading, though, since obviously similar filetypes are grouped when bundled.
		// WARNING: This is the only place we determine type from extension
		// (although we do the converse, determine bundle extension from type, elsewhere).
		// If an author wishes to treat a .css file as JavaScript elsewhere, that must work.
		/// <summary>
		/// Queues resources to be bundled and later emitted with the ResourceLinks directive
		/// </summary>
		/// <param name="resourceFiles">Project paths to JavaScript and/or CSS files</param>
		public void AddResources(string viewPath, params string[] resourceFiles) {
			var css = FilterFileExtension(resourceFiles, CSS_EXTENSION);
			AddCssResources(viewPath, css);
			var js = FilterFileExtension(resourceFiles, JS_EXTENSION);
			AddJsResources(viewPath, js);
		}

		/// <summary>
		/// Bundles CSS files to be emitted with the ResourceLinks directive
		/// </summary>
		/// <param name="viewPath"></param>
		/// <param name="resourceFiles">Zero or more project paths to JavaScript files</param>
		public void AddCssResources(string viewPath, params string[] resourceFiles) {
			AddBundles<CSSBundle>(Bundle.Css, viewPath, KeepCssInOriginalFolder, CSS_EXTENSION, resourceFiles);
		}

		/// <summary>
		/// Bundles JavaScript files to be emitted with the ResourceLinks directive
		/// </summary>
		/// <param name="resourceFiles">Zero or more project paths to JavaScript files</param>
		public void AddJsResources(string viewPath, params string[] resourceFiles) {
			AddBundles<JavaScriptBundle>(Bundle.JavaScript, viewPath, KeepJavascriptInOriginalFolder, JS_EXTENSION, resourceFiles);
		}

		private void AddBundles<bT>(Func<BundleBase<bT>> newBundleFunc, string viewPath, bool originalFolder, string bundleExtension, string[] resourceFiles) where bT : BundleBase<bT> {
			StringBuilder sb = new StringBuilder();
			string filename = GetFilenameRepresentingPath(viewPath) + "_#" + bundleExtension;
			if (originalFolder) {
				// Create a separate bundle for each resource path contained in the provided resourceFiles.
				foreach (var resourceFolder in resourceFiles.
					// Note that on a typical MS "preserve-but-ignore-case" posix-compliant filesystem,
					// this case-insenstive grouping does allow for resources in two different folders to be bundled together,
					// however this case would require some frankly unsupportable behavior from the author to induce.
					GroupBy(r => r.Substring(0, r.LastIndexOf('/') + 1), StringComparer.OrdinalIgnoreCase).
					Reverse()) {
					AddBundle(newBundleFunc, resourceFolder.Key + filename, resourceFolder);
				}
			} else {
				if (resourceFiles.Any()) {
					AddBundle(newBundleFunc, AssetPath + filename, resourceFiles);
				}
			}
		}

		private void AddBundle<bT>(Func<BundleBase<bT>> newBundleFunc, string bundlePath, IEnumerable<string> resourceFiles) where bT : BundleBase<bT> {
			BundleBase<bT> bundle = newBundleFunc();
			foreach (string resourceFile in resourceFiles) {
				bundle.Add(resourceFile);
			}
			string renderedLinks = bundle.Render(bundlePath);
			// WebViewPages (Views and Partials) render from the inside out (although branch order may not be guaranteed),
			// which is required for us to expose our resources declared in children to the parent where they are emitted.
			// However, it also means our resources naturally collect here in an order that is probably not what the site author intends.
			// We reverse the order with insert
			resourceLinks.Insert(0, renderedLinks);
		}

		#region private implementation
		private const string OBJECT_CONTEXT_ID = "39E42E21-553D-43C2-9A30-95C8492EEBF8";
		private const string JS_EXTENSION = ".js";
		private const string CSS_EXTENSION = ".css";

		/// <summary>
		/// Retrieves the cached AutoBundler from the provided dictionary, creating and caching it if necessary.
		/// </summary>
		/// <param name="contextItems">The dictionary (typically HttpContext.Items) containing the cached AutoBundler.</param>
		private static AutoBundler NewOrCached(IDictionary contextItems) {
			AutoBundler newOrCachedSelf;
			if (contextItems.Contains(OBJECT_CONTEXT_ID)) {
				newOrCachedSelf = contextItems[OBJECT_CONTEXT_ID] as AutoBundler;
			} else {
				newOrCachedSelf = new AutoBundler();
				contextItems.Add(OBJECT_CONTEXT_ID, newOrCachedSelf);
			}
			return newOrCachedSelf;
		}

		// ViewIdentifier returns a site-unique name for the current control, such as "shared_signinpartial"
		// Some security wonks may take issue with exposing folder structure here, perhaps it should obfuscate/hash
		private string GetFilenameRepresentingPath(string componentPath) {
			return
				// VirtualPath uniquely identifies the currently rendering View or Partial,
				// such as "~/Views/Shared/SignInPartial.cshtml"
				Path.GetFileNameWithoutExtension(componentPath).

				Substring(2).
				// It's assumed all of these bundles will be output to a single folder,
				// to keep filesystem write-access minimal, so we flatten them here.
				Replace("/", "_").
				// We collapse bundles in scenarios where the view was specified in an unprecise letter-case
				// although this probably originates from "WebViewPage.VirtualPath", we can't guarantee that.
				ToLowerInvariant();
			// Note that without the MD5 hash calculated later to address changing file content,
			// all three of the above string transformations could cause bundles to conflict
		}

		private List<string> resourceLinks = new List<string>();

		private string[] FilterFileExtension(string[] filenames, string mustEndWith) {
			IEnumerable<string> filtered =
				filenames.Where(r => r.EndsWith(mustEndWith, StringComparison.OrdinalIgnoreCase));
			return filtered.ToArray();
		}
		#endregion private implementation



		//// TODO: This hacked addition of less files places them out of order relative to neighbors.
		//foreach (var lessFile in resourceFiles.Where(r => r.EndsWith(".less", StringComparison.InvariantCultureIgnoreCase)))
		//	CssResourceLinks.Insert(0, MvcHtmlString.Create(string.Format("<link rel='stylesheet' type='text/css' href='{0}' />", lessFile)));

	}
}
