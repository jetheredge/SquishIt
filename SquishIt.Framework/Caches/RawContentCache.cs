namespace SquishIt.Framework.Caches
{
    public class RawContentCache : ContentCache
    {
        protected override string KeyPrefix
        {
            get { return "squishit_rawcontent_"; }
        }
    }
}