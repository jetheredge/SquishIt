using System;
using SquishIt.Framework.Base;

namespace SquishIt.Mvc
{
    public class AutoBundlingBehavior
    {
        public string ResourceLocation { get; set; }
        public string FilenameFormat { get; set; }
        public Func<IRenderable, string, string> RenderingDelegate { get; set; }
        public bool KeepScriptsInOriginalFolder { get; set; }
        public bool KeepStylesInOriginalFolder { get; set; }
    }
}