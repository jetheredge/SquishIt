using System;
using System.Collections.Generic;
using System.Linq;
using SquishIt.Framework.Invalidation;
using SquishIt.Framework.Renderers;

namespace SquishIt.Framework.Base
{
    internal class BundleState
    {
        internal List<Asset> Assets = new List<Asset>();
        internal Dictionary<string, string> Attributes = new Dictionary<string, string>();
        
        internal HashSet<string> AllowedExtensions = new HashSet<string>();
        internal IList<IPreprocessor> Preprocessors = new List<IPreprocessor>();
        internal List<string> DependentFiles = new List<string>();

        internal string HashKeyName { get; set; }
        internal string BaseOutputHref { get; set; }

        internal bool Typeless { get; set; }
        internal bool ShouldRenderOnlyIfOutputFileIsMissing { get; set; }

        internal bool ForceDebug { get; set; }
        internal bool ForceRelease { get; set; }
        internal string Path { get; set; }

        internal IRenderer ReleaseFileRenderer { get; set; }
        internal ICacheInvalidationStrategy CacheInvalidationStrategy { get; set; }
        internal Func<bool> DebugPredicate { get; set; }

        internal BundleState()
        { 
        }

        internal BundleState(Dictionary<string, string> attributes)
        {
            Attributes = attributes;
        }

        internal void AddPreprocessor(IPreprocessor instance)
        {
            if(Preprocessors.All(ipp => ipp.GetType() != instance.GetType()))
            {
                foreach(var extension in instance.Extensions)
                {
                    AllowedExtensions.Add(extension);
                }
                Preprocessors.Add(instance);
            }
            if (instance.IgnoreExtensions.NullSafeAny())
            {
                foreach (var extension in instance.IgnoreExtensions)
                {
                    AllowedExtensions.Add(extension);
                }
                Preprocessors.Add(new NullPreprocessor(instance.IgnoreExtensions));
            }
        }
    }
}