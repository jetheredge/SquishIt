using System;

namespace Bundler.Framework.Utilities
{
    public interface IFileWriter: IDisposable
    {
        void Write(string value);
        void WriteLine(string value);
    }
}