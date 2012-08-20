namespace SquishIt.Framework.CSS
{
    public interface ICSSAssetsFileHasher
    {
        string AppendFileHash(string cssFilePath, string url);
    }
}
