namespace SquishIt.Framework
{
    public interface IPathTranslator
    {
        string ResolveAppRelativePathToFileSystem(string file);
        string ResolveFileSystemPathToAppRelative(string file);
        string BuildAbsolutePath(string siteRelativePath);
    }
}