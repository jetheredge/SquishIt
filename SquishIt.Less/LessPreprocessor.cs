using SquishIt.Framework.Base;
using dotless.Core;
using System.Collections.Generic;
using SquishIt.Framework;
using System.IO;
using System.Linq;

namespace SquishIt.Less
{
    public class LessPreprocessor : Preprocessor
    {
        public override string[] Extensions
        {
            get { return new[] { ".less" }; }
        }

        public override IProcessResult Process(string filePath, string content)
        {
            var engineFactory = new EngineFactory();
            var engine = engineFactory.GetEngine();
            string css = engine.TransformToCss(content, filePath);

            string dir = Path.GetDirectoryName(filePath);
            string appPath = string.Empty;
            if (!string.IsNullOrEmpty(dir))
            {
                appPath = FileSystem.ResolveFileSystemPathToAppRelative(dir);
            }

            var dependencies = engine.GetImports().Select(importPath =>
            {
                return FileSystem.ResolveAppRelativePathToFileSystem(Path.Combine(appPath, importPath));
            });

            return new ProcessResult(css, dependencies);
        }
    }
}
