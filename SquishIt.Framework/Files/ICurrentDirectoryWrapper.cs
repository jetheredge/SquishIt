using System;
namespace SquishIt.Framework.Files
{
    public interface ICurrentDirectoryWrapper
    {
        T UsingCurrentDirectory<T>(string directory, Func<T> function);
    }
}