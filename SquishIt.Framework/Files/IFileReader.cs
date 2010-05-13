using System;

namespace SquishIt.Framework.Files
{
    public interface IFileReader: IDisposable
    {
        string ReadLine();
        string ReadToEnd();
    }
}