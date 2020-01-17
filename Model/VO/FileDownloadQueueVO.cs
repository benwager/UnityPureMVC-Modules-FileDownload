// 
// Persistent data for the download system
// Download Q etc
//
using System.Collections.Generic;
using UnityEngine;

namespace UnityPureMVC.Modules.FileDownload.Model.VO
{
    internal class FileDownloadQueueVO
    {
        internal List<FileDownloadVO> queue;
        internal GameObject loadingGameObject;
        internal bool isQueueProcessing;
    }
}
