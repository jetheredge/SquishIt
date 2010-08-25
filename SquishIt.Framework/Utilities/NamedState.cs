using System;

namespace SquishIt.Framework.Utilities
{
    public class NamedState
    {
        public bool Debug { get; protected set; }
        public string RenderTo { get; protected set; }

        public NamedState(bool debug, string renderTo)
        {
            Debug = debug;
            RenderTo = renderTo;
        }
    }
}