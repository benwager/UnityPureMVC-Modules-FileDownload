namespace UnityPureMVC.Modules.FileDownload.Controller.Notes
{
    internal class FileDownloadNote
    {
        /// <summary>
        /// Attempts to download a file from path. Pass FileDownloadVO as parameter
        /// </summary>
        internal const string START = "FileDownload/start";
        internal const string REQUEST_FILE_DOWNLOAD = "FileDownload/requestFileDownload";
        internal const string FILE_DOWNLOADED = "FileDownload/fileDownloaded";
        internal const string FILE_DOWNLOAD_ERROR = "FileDownload/fileDownloadError";
        internal const string REQUEST_PROCESS_QUEUE = "FileDownload/requestProcessQueue";
        internal const string QUEUE_FINISHED = "FileDownload/queueFinished";
    }
}