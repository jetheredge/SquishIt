namespace SquishIt.Framework.Files
{
    public interface ICurrentDirectoryWrapper
    {
        void SetCurrentDirectory(string directory);
        void Revert();
    }
}