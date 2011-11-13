using System;

namespace SquishIt.Framework.Base
{
    internal class Asset
    {
        internal string LocalPath { get; set; }
        internal string RemotePath { get; set; }
        internal int Order { get; set; }
        internal bool IsEmbeddedResource { get; set; }
        internal bool DownloadRemote { get; set; }

        internal bool IsLocal
        {
            get
            {
                return String.IsNullOrEmpty(RemotePath);
            }
        }

        internal bool IsRemote
        {
            get
            {
                return !String.IsNullOrEmpty(RemotePath) && !IsEmbeddedResource && !DownloadRemote;
            }
        }

        internal bool IsRemoteDownload { 
            get { return !String.IsNullOrEmpty(RemotePath) && DownloadRemote; }
        }

        internal Asset()
        {
        }

        internal Asset(string localPath, string remotePath = null, int order = 0, bool isEmbeddedResource = false)
        {
            LocalPath = localPath;
            RemotePath = remotePath;
            Order = order;
            IsEmbeddedResource = isEmbeddedResource;
        }
    }
}