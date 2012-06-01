namespace SquishIt.Framework.Base
{
    public class Asset
    {
        internal string LocalPath { get; set; }
        internal string RemotePath { get; set; }
        internal int Order { get; set; }
        internal bool IsEmbeddedResource { get; set; }
        internal bool DownloadRemote { get; set; }
        internal bool IsRecursive { get; set; }
        internal string Content { get; set; }
        internal string Extension { get; set; }

        internal bool IsArbitrary
        {
            get { return !string.IsNullOrEmpty(Content); }
        }

        internal bool IsLocal
        {
            get { return string.IsNullOrEmpty(RemotePath) && !IsArbitrary; }
        }

        internal bool IsRemote
        {
            get { return !string.IsNullOrEmpty(RemotePath) && !IsEmbeddedResource && !DownloadRemote; }
        }

        internal bool IsRemoteDownload
        {
            get { return !string.IsNullOrEmpty(RemotePath) && DownloadRemote; }
        }


        //TODO : favor this constructor, object initializers
        internal Asset()
        {
        }

        internal Asset(string localPath, string remotePath = null, int order = 0, bool isEmbeddedResource = false, bool isRecursive = true, string content = null)
        {
            LocalPath = localPath;
            RemotePath = remotePath;
            Order = order;
            IsEmbeddedResource = isEmbeddedResource;
            IsRecursive = isRecursive;
            Content = content;
        }
    }
}