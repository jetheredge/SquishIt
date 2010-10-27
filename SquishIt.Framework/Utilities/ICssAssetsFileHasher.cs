namespace SquishIt.Framework.Utilities
{
    public interface ICssAssetsFileHasher
    {
        string AppendFileHash(string cssFilePath, string url);
    }
}
