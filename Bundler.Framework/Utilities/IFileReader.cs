using System;

namespace Bundler.Framework.Utilities
{
    public interface IFileReader: IDisposable
    {
        string ReadLine();
        string ReadToEnd();
    }
}