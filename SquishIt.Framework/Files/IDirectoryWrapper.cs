using System;
namespace SquishIt.Framework.Files
{
    public interface IDirectoryWrapper
    {
        T ExecuteInDirectory<T>(string directory, Func<T> function);
    }
}