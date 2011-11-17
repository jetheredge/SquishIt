using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SquishIt.Framework
{
    public interface IPreprocessor
    {
        string FileMatchRegex { get; }
        string Process(string filePath, string content);
    }
}
