namespace SquishIt.Framework.Invalidation
{
    public class DefaultCacheInvalidationStrategy : ICacheInvalidationStrategy
    {
        public string GetOutputFileLocation(string outputFile, string hash)
        {
            if (outputFile.Contains("#"))
            {
                return outputFile.Replace("#", hash);
            }
            return outputFile;
        }

        public string GetOutputWebPath(string renderToPath, string hashKeyName, string hash)
        {
            if (renderToPath.Contains("#"))
            {
                return renderToPath.Replace("#", hash);
            }
            if (!string.IsNullOrEmpty(hashKeyName))
            {
                return renderToPath + (renderToPath.Contains("?") ? "&" : "?") + hashKeyName + "=" + hash;
            }
            return renderToPath;
        }
    }
}