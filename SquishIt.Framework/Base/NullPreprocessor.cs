using System;

namespace SquishIt.Framework.Base
{
    /// <summary>
    /// No-op preprocessor used to prevent extensions from blocking pipeline
    /// </summary>
    public class NullPreprocessor : Preprocessor
    {
        private readonly string[] _extensions;

        public NullPreprocessor(params string[] extensions)
        {
            _extensions = extensions;
        }

        public override IProcessResult Process(string filePath, string content)
        {
            throw new NotImplementedException();
        }

        public override string[] Extensions { get { return _extensions; } }
    }
}