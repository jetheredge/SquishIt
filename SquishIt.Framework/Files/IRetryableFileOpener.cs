using System.IO;

namespace SquishIt.Framework.Files
{
    public interface IRetryableFileOpener
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
        /// </remarks>
        Stream OpenFileStream(FileInfo fileInfo, int retry, FileMode fileMode, FileAccess fileAccess, FileShare fileShare);

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
        Stream OpenFileStream(string filePath, int retry, FileMode fileMode, FileAccess fileAccess, FileShare fileShare);

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
        StreamReader OpenTextStreamReader(FileInfo fileInfo, int retry);

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
        StreamReader OpenTextStreamReader(string filePath, int retry);

        /// <summary>
        /// File might be locked when attempting to open it. This will attempt to open the file the number of times specified by <paramref name="retry"/>
        /// </summary>
        /// <param name="fileInfo">The file to attempt to get a text stream for</param>
        /// <param name="retry">The number of times a file open should be attempted</param>
        /// <param name="append">Should file be appended</param>
        /// <returns>A text stream of the file</returns>
        StreamWriter OpenTextStreamWriter(FileInfo fileInfo, int retry, bool append);

        /// <summary>
        /// File might be locked when attempting to open it. This will attempt to open the file the number of times specified by <paramref name="retry"/>
        /// </summary>
        /// <param name="filePath">The file to attempt to get a text stream for</param>
        /// <param name="retry">The number of times a file open should be attempted</param>
        /// <param name="append">Should file be appended</param>
        /// <returns>A text stream of the file</returns>
        StreamWriter OpenTextStreamWriter(string filePath, int retry, bool append);
    }
}
