using System;

namespace Bundler.Framework.Files
{
    public interface IFileReader: IDisposable
    {
        string ReadLine();
        string ReadToEnd();
    }
}