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
        private readonly IPathTranslator pathTranslator;

        public override string[] Extensions
        {
            get { return new[] { ".less" }; }
        }

        public override string[] IgnoreExtensions
        {
            get
            {
                return new[] { ".css" };
            }
        }

        /// <summary>
        /// Can be overridden to build engine with specific configuration.
        /// </summary>
        public static Func<ILessEngine> EngineBuilder = () => new EngineFactory().GetEngine();

        public LessPreprocessor() : this(EngineBuilder, Configuration.Instance.DefaultPathTranslator()) { }

        /// <summary>
        /// Construct LESS preprocessor with custom engine construction.
        /// </summary>
        /// <param name="engineBuilder">Function for custom ILessEngine construction.</param>
        /// <param name="pathTranslator">Translator for moving between web and filesystem paths.</param>
        public LessPreprocessor(Func<ILessEngine> engineBuilder, IPathTranslator pathTranslator)
        {
            _engineBuilder = engineBuilder;
            this.pathTranslator = pathTranslator;
        }

        public override IProcessResult Process(string filePath, string content)
        {
            var engine = _engineBuilder();
            string css = engine.TransformToCss(content, filePath);

            string dir = Path.GetDirectoryName(filePath);
            string appPath = string.Empty;
            if(!string.IsNullOrEmpty(dir))
            {
                appPath = pathTranslator.ResolveFileSystemPathToAppRelative(dir);
            }

            var dependencies = engine.GetImports().Select(importPath => {
                return pathTranslator.ResolveAppRelativePathToFileSystem(Path.Combine(appPath, importPath));
            });

            return new ProcessResult(css, dependencies);
        }
    }
}
