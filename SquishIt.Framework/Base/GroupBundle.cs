using System.Collections.Generic;
using SquishIt.Framework.Minifiers;

namespace SquishIt.Framework.Base
{
    public class GroupBundle
    {
        internal List<Asset> Assets = new List<Asset>();
        internal Dictionary<string, string> Attributes = new Dictionary<string, string>();
        internal int Order { get; set; }

        internal GroupBundle()
        { 
        }

        internal GroupBundle(Dictionary<string, string> attributes)
        {
            Attributes = attributes;
        }
    }
}