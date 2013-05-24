namespace SquishIt.Framework.Invalidation
{
    public interface ICacheInvalidationStrategy
    {
        string GetOutputFileLocation(string outputFile, string hash);
        string GetOutputWebPath(string renderToPath, string hashKeyName, string hash);
    }
}
