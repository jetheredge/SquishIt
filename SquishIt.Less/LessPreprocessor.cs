using SquishIt.Framework.Base;
using dotless.Core;
using System.Collections.Generic;
using SquishIt.Framework;
using System.IO;
using System.Linq;
using dotless.Core.configuration;

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
            var engine = GetEngine();
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

        ILessEngine GetEngine()
        {
            try
            {
                return new EngineFactory(new WebConfigConfigurationLoader().GetConfiguration()).GetEngine();
            }
            catch
            {
                return new EngineFactory().GetEngine();
            }
        }
    }
}
