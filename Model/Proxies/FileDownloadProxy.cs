using PureMVC.Patterns.Proxy;
using UnityPureMVC.Core.Libraries.UnityLib.Utilities.Logging;
using UnityPureMVC.Modules.FileDownload.Model.VO;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UnityPureMVC.Modules.FileDownload.Model.Proxies
{
    internal class FileDownloadProxy : Proxy
    {
        new internal const string NAME = "FileDownloadProxy";

        internal FileDownloadQueueVO FileDownloadQueueVO { get { return Data as FileDownloadQueueVO; } }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:UnityPureMVC.Core.Model.Proxies.FileDownloadSystemProxy"/> class.
        /// </summary>
        internal FileDownloadProxy() : base(NAME)
        {
            DebugLogger.Log("{0}::__Contstruct", NAME);

            Data = new FileDownloadQueueVO();

            FileDownloadQueueVO.queue = new List<FileDownloadVO>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        internal bool IsQueueProcessing
        {
            get
            {
                return FileDownloadQueueVO.isQueueProcessing;
            }
            set
            {
                FileDownloadQueueVO.isQueueProcessing = value;
            }
        }

        /// <summary>
        /// Add a new file download request to the queue
        /// </summary>
        /// <param name="fileDownloadVO"></param>
        internal void AddItemToQueue(FileDownloadVO fileDownloadVO)
        {
            FileDownloadQueueVO.queue.Add(fileDownloadVO);
        }

        /// <summary>
        /// Retrieve the next item from the queue
        /// This automatically removes the item from the queue
        /// </summary>
        /// <returns></returns>
        internal FileDownloadVO GetNextItemInQueue()
        {
            if (FileDownloadQueueVO.queue.Count <= 0)
            {
                return null;
            }
            FileDownloadVO fileDownloadVO = FileDownloadQueueVO.queue.First();
            FileDownloadQueueVO.queue.RemoveAt(0);
            return fileDownloadVO;
        }

        internal int GetQueueLength()
        {
            return FileDownloadQueueVO.queue.Count;
        }

        /// <summary>
        /// Creates an instance of the loader gameobject specified at template path
        /// </summary>
        /// <param name="templatePath"></param>
        internal void CreateLoadingGameObject(string templatePath)
        {
            if (FileDownloadQueueVO.loadingGameObject == null)
            {
                FileDownloadQueueVO.loadingGameObject = GameObject.Instantiate(Resources.Load(templatePath)) as GameObject;
            }
        }

        /// <summary>
        /// Destroy the loading GameObejct
        /// </summary>
        internal void RemoveLoadingGameObject()
        {
            GameObject.Destroy(FileDownloadQueueVO.loadingGameObject);
            FileDownloadQueueVO.loadingGameObject = null;
        }
    }
}
