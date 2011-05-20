namespace SquishIt.Framework.Base
{
    internal class Asset
    {
        internal string LocalPath { get; set; }
        internal string RemotePath { get; set; }
        internal int Order { get; set; }
        internal bool IsEmbeddedResource { get; set; }

        internal Asset()
        {
        }

        internal Asset(string localPath, string remotePath = null, int order = 0, bool isEmbeddedResource = false)
        {
            LocalPath = localPath;
            RemotePath = null;
            Order = order;
            IsEmbeddedResource = isEmbeddedResource;
        }
    }
}