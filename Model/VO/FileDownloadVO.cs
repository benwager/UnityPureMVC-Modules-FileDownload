/*
 * 
 * Create a new instance of this VO class to pass through with any file download request

SendNotification(FileDownloadSystemNote.REQUEST_FILE_DOWNLOAD, new FileDownloadVO
{
    path = applicationDataProxy.GetCurrentSelectedItem().inlay,
    fileType = Modules.FileDownloadSystem.Model.Enums.FileType.JPG,
    onComplete = (object file) =>
    {
        material.mainTexture = file as Texture2D;
    }
});

*/


using UnityPureMVC.Modules.FileDownload.Model.Enums;

namespace UnityPureMVC.Modules.FileDownload.Model.VO
{
    internal delegate void OnCompleteCallback(object file);
    internal class FileDownloadVO
    {
        internal string path;
        internal int version;
        internal FileType fileType;
        internal OnCompleteCallback onComplete;
        internal string loadingTemplate;// = "FileDownloadSystem/FileDownloadSystemLoading";
        internal bool autoProcessQueue = true; // Automatically request that the File Download Queue is processed
    }
}
