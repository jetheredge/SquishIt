using System;
using System.Linq;

namespace SquishIt.Framework.Base
{
    public abstract class Preprocessor: IPreprocessor
    {
        public bool ValidFor(string extension)
        {
            return Extensions.Contains(extension.StartsWith(".") ? extension : ("." + extension), StringComparer.CurrentCultureIgnoreCase);
        }

		public abstract IProcessResult Process(string filePath, string content);
        public abstract string[] Extensions { get; }
    }
}