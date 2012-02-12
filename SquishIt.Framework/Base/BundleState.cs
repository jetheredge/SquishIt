using System.Collections.Generic;
using SquishIt.Framework.Minifiers;

namespace SquishIt.Framework.Base
{
    internal class BundleState
    {
        internal List<Asset> Assets = new List<Asset>();
        internal Dictionary<string, string> Attributes = new Dictionary<string, string>();
        internal int Order { get; set; }
        public bool ForceDebug { get; set; }
        public bool ForceRelease { get; set; }
        public string Path { get; set; }

        internal BundleState()
        { 
        }

        internal BundleState(Dictionary<string, string> attributes)
        {
            Attributes = attributes;
        }
    }
}