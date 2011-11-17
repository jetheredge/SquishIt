using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using SquishIt.Framework;
using SquishIt.Framework.Css;
using SquishIt.Framework.Files;
using dotless.Core;

namespace SquishIt.Less
{
    public class LessPreprocessor : IPreprocessor
    {
        public string FileMatchRegex
        {
            get { return @"(\.less)|(\.less.css)$"; }
        }

        public string Process(string filePath, string content)
        {
            var engineFactory = new EngineFactory();
            var engine = engineFactory.GetEngine();
            return engine.TransformToCss(content, filePath);
        }
    }
}
