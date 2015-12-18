using System.IO;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text;
using System.Threading;

namespace SquishIt.Framework.Files
{
    public class RetryableFileOpener : IRetryableFileOpener
    {
        /// <summary>
        /// File might be locked when attempting to open it. This will attempt to open the file the number of times specified by <paramref name="retry"/>
        /// </summary>
        /// <param name="fileInfo">The file to attempt to get a file stream for</param>
        /// <param name="retry">The number of times a file open should be attempted</param>
        /// <param name="fileMode">The file mode to use</param>
        /// <param name="fileAccess">The file access to use</param>
        /// <param name="fileShare">The file sharing to use</param>
        /// <returns>A file stream of the file</returns>
        /// <remarks>
        /// It attempt to open the file in increasingly longer periods and throw an exception if it cannot open it within the
        /// specified number of retries.
        /// Note - Directory creation only works if retry count is greater than one
        /// </remarks>
        public Stream OpenFileStream(FileInfo fileInfo, int retry, FileMode fileMode, FileAccess fileAccess, FileShare fileShare)
        {
            return OpenFileStream(fileInfo.FullName, retry, fileMode, fileAccess, fileShare);
        }

        /// <summary>
        /// File might be locked when attempting to open it. This will attempt to open the file the number of times specified by <paramref name="retry"/>
        /// </summary>
        /// <param name="filePath">The file to attempt to get a file stream for</param>
        /// <param name="retry">The number of times a file open should be attempted</param>
        /// <param name="fileMode">The file mode to use</param>
        /// <param name="fileAccess">The file access to use</param>
        /// <param name="fileShare">The file sharing to use</param>
        /// <returns>A file stream of the file</returns>
        /// <remarks>
        /// It attempt to open the file in increasingly longer periods and throw an exception if it cannot open it within the
        /// specified number of retries.
        /// </remarks>
        public Stream OpenFileStream(string filePath, int retry, FileMode fileMode, FileAccess fileAccess, FileShare fileShare)
        {
            var delay = 0;

            for(var i = 0; i < retry; i++)
            {
                try
                {
                    var stream = new FileStream(filePath, fileMode, fileAccess, fileShare);
                    return stream;
                }
                catch(DirectoryNotFoundException)
                {
                    CreateDirectoryStructure(filePath);
                }
                catch(FileNotFoundException)
                {
                    throw;
                }
                catch(IOException)
                {
                    delay += 100;
                    if(i == retry) throw;
                }
                Thread.Sleep(delay);
            }

            //We will never get here
            throw new IOException(string.Format("Unable to open file '{0}'", filePath));
        }

        /// <summary>
        /// Create folder structure for a given file path
        /// </summary>
        /// <param name="filePath"></param>
        /// <remarks>
        /// Will create folder with permissions inherited from parent
        /// </remarks>
        protected virtual void CreateDirectoryStructure(string filePath)
        {
            var file = new FileInfo(filePath);
            file.Directory.Create();
        }

        /// <summary>
        /// File might be locked when attempting to open it. This will attempt to open the file the number of times specified by <paramref name="retry"/>
        /// </summary>
        /// <param name="fileInfo">The file to attempt to get a text stream for</param>
        /// <param name="retry">The number of times a file open should be attempted</param>
        /// <returns>A text stream of the file</returns>
        /// <remarks>
        /// It attempt to open the file in increasingly longer periods and throw an exception if it cannot open it within the
        /// specified number of retries.
        /// </remarks>
        public StreamReader OpenTextStreamReader(FileInfo fileInfo, int retry)
        {
            return OpenTextStreamReader(fileInfo.FullName, retry);
        }

        /// <summary>
        /// File might be locked when attempting to open it. This will attempt to open the file the number of times specified by <paramref name="retry"/>
        /// </summary>
        /// <param name="filePath">The file to attempt to get a text stream for</param>
        /// <param name="retry">The number of times a file open should be attempted</param>
        /// <returns>A text stream of the file</returns>
        /// <remarks>
        /// It attempt to open the file in increasingly longer periods and throw an exception if it cannot open it within the
        /// specified number of retries.
        /// </remarks>
        public StreamReader OpenTextStreamReader(string filePath, int retry)
        {
            var delay = 0;

            for(var i = 0; i < retry; i++)
            {
                try
                {
                    var stream = new StreamReader(filePath, Encoding.UTF8);
                    return stream;
                }
                catch(DirectoryNotFoundException)
                {
                    throw;
                }
                catch(FileNotFoundException)
                {
                    throw;
                }
                catch(IOException)
                {
                    delay += 100;
                    if(i == retry) throw;
                }

                Thread.Sleep(delay);
            }

            //We will never get here
            throw new IOException(string.Format("Unable to open file '{0}'", filePath));
        }

        /// <summary>
        /// File might be locked when attempting to open it. This will attempt to open the file the number of times specified by <paramref name="retry"/>
        /// </summary>
        /// <param name="fileInfo">The file to attempt to get a text stream for</param>
        /// <param name="retry">The number of times a file open should be attempted</param>
        /// <param name="append">Should file be appended</param>
        /// <returns>A text stream of the file</returns>
        public StreamWriter OpenTextStreamWriter(FileInfo fileInfo, int retry, bool append)
        {
            return OpenTextStreamWriter(fileInfo.FullName, retry, append);
        }

        /// <summary>
        /// File might be locked when attempting to open it. This will attempt to open the file the number of times specified by <paramref name="retry"/>
        /// </summary>
        /// <param name="filePath">The file to attempt to get a text stream for</param>
        /// <param name="retry">The number of times a file open should be attempted</param>
        /// <param name="append">Should file be appended</param>
        /// <returns>A text stream of the file</returns>
        /// <remarks>
        /// Note - Directory creation only works if retry count is greater than one
        /// </remarks>
        public StreamWriter OpenTextStreamWriter(string filePath, int retry, bool append)
        {
            var delay = 0;

            for(var i = 0; i < retry; i++)
            {
                try
                {
                    var stream = new StreamWriter(filePath, append, Encoding.UTF8);
                    return stream;
                }
                catch (DirectoryNotFoundException)
                {
                    CreateDirectoryStructure(filePath);
                }
                catch(FileNotFoundException)
                {
                    throw;
                }
                catch(IOException)
                {
                    delay += 100;
                    if(i == retry) throw;
                }

                Thread.Sleep(delay);
            }

            //We will never get here
            throw new IOException(string.Format("Unable to open file '{0}'", filePath));
        }
    }
}
