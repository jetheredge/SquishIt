namespace SquishIt.Framework.Caches
{
    public class BundleCache: ContentCache
    {
        protected override string KeyPrefix
        {
            get { return "squishit_"; }
        }
    }
}