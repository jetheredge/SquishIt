namespace SquishIt.Framework.Invalidation
{
    public class HashAsVirtualDirectoryCacheInvalidationStrategy : ICacheInvalidationStrategy
    {
        public string GetOutputFileLocation(string outputFile, string hash)
        {
            if (outputFile.Contains("#"))
            {
                return outputFile.Replace("#", string.Empty).Replace("//", "/");
            }
            return outputFile;
        }

        public string GetOutputWebPath(string renderToPath, string hashKeyName, string hash)
        {
            if (renderToPath.Contains("#"))
            {
                return renderToPath.Replace("#", hashKeyName + "-" + hash);
            }
            if (!string.IsNullOrEmpty(hashKeyName))
            {
                return renderToPath + (renderToPath.Contains("?") ? "&" : "?") + hashKeyName + "=" + hash;
            }
            return renderToPath;
        }
    }
}