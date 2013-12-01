using System;
using SquishIt.Framework.Base;
using dotless.Core;
using SquishIt.Framework;
using System.IO;
using System.Linq;

namespace SquishIt.Less
{
    /// <summary>
    /// LESS preprocessor.
    /// </summary>
    public class LessPreprocessor : Preprocessor
    {
        private readonly Func<ILessEngine> _engineBuilder;

        public override string[] Extensions
        {
            get { return new[] { ".less" }; }
        }

        /// <summary>
        /// Can be overridden to build engine with specific configuration.
        /// </summary>
        public static Func<ILessEngine> EngineBuilder = () => new EngineFactory().GetEngine();

        public LessPreprocessor() : this(EngineBuilder){ }
        
        /// <summary>
        /// Construct LESS preprocessor with custom engine construction.
        /// </summary>
        /// <param name="engineBuilder">Function for custom ILessEngine construction.</param>
        public LessPreprocessor(Func<ILessEngine> engineBuilder)
        {
            _engineBuilder = engineBuilder;
        }

        public override IProcessResult Process(string filePath, string content)
        {
            var engine = _engineBuilder();
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
