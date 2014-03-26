using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SquishIt.Framework;
using SquishIt.Framework.Base;
using SquishIt.Framework.CSS;
using SquishIt.Framework.JavaScript;
using SquishIt.Framework.Utilities;
using HttpContext = System.Web.HttpContext;

namespace SquishIt.Mvc
{
    /// <summary>
    /// Manages a bundle of .js and/or .css specific to the specified ViewPage
    /// </summary>
    public class AutoBundler
    {
        public static AutoBundlingBehavior Behavior { get; set; }

        private IHasher _hasher = Configuration.Instance.DefaultHasher();

        static AutoBundler()
        {
            Behavior = new AutoBundlingBehavior
            {
                FilenameFormat = "{0}{1}{2}",
                ResourceLocation = "~/assets/",
                RenderingDelegate = (b, n) => b.Render(n),
                // Since some resource types contain paths relative to the current location,
                // we offer the option of splitting bundles on path boundaries.
                KeepScriptsInOriginalFolder = false,
                // Grouping bundle content by folder can result in content order changes
                // For example, [  "/a/css1", "/b/css2",   "/a/css3"  ]
                // may become   [ ["/a/css1", "/a/css3"], ["/b/css2"] ]
                // for that reason these should be off be default
                // Note that SquishIt does support relocating CSS url() paths.
                KeepStylesInOriginalFolder = true,
            };
        }

        // It might be nice to allow multiple link queues to be named,
        // for example one for the head and another for the tail of body
        /// <summary>
        /// Get the cached AutoBundler for this HTTP context,
        /// or create a new AutoBundler if this is first request for it.
        /// </summary>
        /// <returns>The bundler for operating on the current request.</returns>
        public static AutoBundler Current
        {
            get { return NewOrCached(HttpContext.Current.Items); }
        }

        /// <summary>
        /// Emits here the bundled resources declared with "AddStyleResources" on all child controls
        /// </summary>
        public string StyleResourceLinks
        {
            get
            {
                return string.Join("", styleResourceLinks);
            }
        }

        /// <summary>
        /// Emits here the bundled resources declared with "AddScriptResources" on all child controls
        /// </summary>
        public string ScriptResourceLinks
        {
            get
            {
                return string.Join("", scriptResourceLinks);
            }
        }

        /// <summary>
        /// Queues resources to be bundled and later emitted with the ResourceLinks directive
        /// </summary>
        /// <param name="resourceFiles">Project paths to JavaScript and/or CSS files</param>
        public void AddResources(string viewPath, params string[] resourceFiles)
        {
            var styles = FilterFileExtensions(resourceFiles, Bundle.AllowedStyleExtensions);
            AddStyleResources(viewPath, styles);
            var scripts = FilterFileExtensions(resourceFiles, Bundle.AllowedScriptExtensions);
            AddScriptResources(viewPath, scripts);
        }

        /// <summary>
        /// Bundles CSS files to be emitted with the ResourceLinks directive
        /// </summary>
        /// <param name="viewPath"></param>
        /// <param name="resourceFiles">Zero or more project paths to JavaScript files</param>
        public void AddStyleResources(string viewPath, params string[] resourceFiles)
        {
            AddBundles(Bundle.Css, viewPath, Behavior.KeepStylesInOriginalFolder, STYLE_BUNDLE_EXTENSION, resourceFiles);
        }

        /// <summary>
        /// Bundles JavaScript files to be emitted with the ResourceLinks directive
        /// </summary>
        /// <param name="resourceFiles">Zero or more project paths to JavaScript files</param>
        public void AddScriptResources(string viewPath, params string[] resourceFiles)
        {
            AddBundles(Bundle.JavaScript, viewPath, Behavior.KeepScriptsInOriginalFolder, SCRIPT_BUNDLE_EXTENSION, resourceFiles);
        }

        private void AddBundles<bT>(Func<BundleBase<bT>> newBundleFunc, string viewPath, bool originalFolder, string bundleExtension, string[] resourceFiles) where bT : BundleBase<bT>
        {
            var filename = GetFilenameRepresentingResources(resourceFiles);
            if (originalFolder)
            {
                // Create a separate bundle for each resource path contained in the provided resourceFiles.
                foreach (var resourceFolder in resourceFiles.
                    // Note that on a typical MS "preserve-but-ignore-case" posix-compliant filesystem,
                    // this case-insenstive grouping does allow for resources in two different folders to be bundled together,
                    // however this case would require some frankly unsupportable behavior from the author to induce.
                    GroupBy(r => r.Substring(0, r.LastIndexOf('/') + 1), StringComparer.OrdinalIgnoreCase).
                    Reverse())
                {
                    AddBundle(newBundleFunc, viewPath, string.Format(Behavior.FilenameFormat, resourceFolder.Key, filename, bundleExtension), resourceFolder);
                }
            }
            else
            {
                if (resourceFiles.Any())
                {
                    AddBundle(newBundleFunc, viewPath, string.Format(Behavior.FilenameFormat, Behavior.ResourceLocation, filename, bundleExtension), resourceFiles);
                }
            }
        }

        private void AddBundle<bT>(Func<BundleBase<bT>> newBundleFunc, string viewPath, string bundlePath, IEnumerable<string> resourceFiles) where bT : BundleBase<bT>
        {
            var bundle = newBundleFunc();

            if (bundle.IsDebuggingEnabled())
            {
                bundle = bundle.WithAttribute("data-autobundle-declaring-view", viewPath);
            }

            foreach (var resourceFile in resourceFiles)
            {
                bundle.Add(resourceFile);
            }
            
            var renderedLinks = Behavior.RenderingDelegate(bundle, bundlePath);

            // WebViewPages (Views and Partials) render from the inside out (although branch order may not be guaranteed),
            // which is required for us to expose our resources declared in children to the parent where they are emitted.
            // However, it also means our resources naturally collect here in an order that is probably not what the site author intends.
            // We reverse the order with insert
            if (bundle is JavaScriptBundle)
            {
                scriptResourceLinks.Insert(0, renderedLinks);
            }
            else if (bundle is CSSBundle)
            {
                styleResourceLinks.Insert(0, renderedLinks);
            }
            else
            {
                throw new NotSupportedException(string.Format("Unknown bundle type: {0}.", bundle.GetType().FullName));
            }
        }

        private const string OBJECT_CONTEXT_ID = "39E42E21-553D-43C2-9A30-95C8492EEBF8";
        private const string SCRIPT_BUNDLE_EXTENSION = ".js";
        private const string STYLE_BUNDLE_EXTENSION = ".css";

        /// <summary>
        /// Retrieves the cached AutoBundler from the provided dictionary, creating and caching it if necessary.
        /// </summary>
        /// <param name="contextItems">The dictionary (typically HttpContext.Items) containing the cached AutoBundler.</param>
        private static AutoBundler NewOrCached(IDictionary contextItems)
        {
            AutoBundler newOrCachedSelf;
            if (contextItems.Contains(OBJECT_CONTEXT_ID))
            {
                newOrCachedSelf = contextItems[OBJECT_CONTEXT_ID] as AutoBundler;
            }
            else
            {
                newOrCachedSelf = new AutoBundler();
                contextItems.Add(OBJECT_CONTEXT_ID, newOrCachedSelf);
            }
            return newOrCachedSelf;
        }

        // this will spit out a nonsense name, but should make it easier to reuse bundles
        private string GetFilenameRepresentingResources(string[] resourcePaths)
        {
            return _hasher.GetHash(string.Join("-", resourcePaths));
        }

        private readonly List<string> styleResourceLinks = new List<string>();
        private readonly List<string> scriptResourceLinks = new List<string>();

        private string[] FilterFileExtensions(string[] filenames, IEnumerable<string> extensions)
        {
            var filtered =
                filenames.Where(r => extensions.Any(e => r.EndsWith(e, StringComparison.OrdinalIgnoreCase)));
            return filtered.ToArray();
        }
    }
}
