using PureMVC.Interfaces;
using PureMVC.Patterns.Command;
using UnityPureMVC.Core.Controller.Notes;
using UnityPureMVC.Core.Libraries.UnityLib.Utilities.Logging;
using UnityPureMVC.Core.Model.VO;
using UnityPureMVC.Modules.FileDownload.Controller.Notes;
using UnityPureMVC.Modules.FileDownload.Model.Enums;
using UnityPureMVC.Modules.FileDownload.Model.Proxies;
using UnityPureMVC.Modules.FileDownload.Model.VO;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

namespace UnityPureMVC.Modules.FileDownload.Controller.Commands.Request
{
    internal class RequestsProcessQueueCommand : SimpleCommand
    {
        private FileDownloadProxy fileDownloadSystemProxy;

        /// <summary>
        /// Start coroutine to load assetbundle
        /// </summary>
        /// <param name="notification"></param>
        public override void Execute(INotification notification)
        {
            DebugLogger.Log("RequestsProcessQueueCommand.cs::Execute");

            // Get the FileDownloadSystemProxy
            fileDownloadSystemProxy = Facade.RetrieveProxy(FileDownloadProxy.NAME) as FileDownloadProxy;

            fileDownloadSystemProxy.IsQueueProcessing = true;

            ProcessNextItemInQueue();
        }

        protected void ProcessNextItemInQueue()
        {
            // Get next item in queue
            FileDownloadVO fileDownloadVO = fileDownloadSystemProxy.GetNextItemInQueue();

            // Null check for end of queue
            if (fileDownloadVO == null)
            {
                fileDownloadSystemProxy.RemoveLoadingGameObject();
                fileDownloadSystemProxy.IsQueueProcessing = false;
                SendNotification(FileDownloadNote.QUEUE_FINISHED);
                return;
            }

            // Null check for invalid file path
            if (string.IsNullOrEmpty(fileDownloadVO.path))
            {
                ReportError(fileDownloadVO.path);
                return;
            }

            if (!string.IsNullOrEmpty(fileDownloadVO.loadingTemplate))
            {
                // Show loading screen
                fileDownloadSystemProxy.CreateLoadingGameObject(fileDownloadVO.loadingTemplate);
            }

            // Determine the file type and pass to appropriate Coroutine
            IEnumerator coroutine = null;
            switch (fileDownloadVO.fileType)
            {
                case FileType.ASSET_BUNDLE:
                    coroutine = DownloadAssetBundle(fileDownloadVO);
                    break;
                case FileType.PNG:
                case FileType.JPG:
                    coroutine = DownloadTexture(fileDownloadVO);
                    break;
                case FileType.MOV:
                case FileType.MP4:
                    coroutine = DownloadVideoClip(fileDownloadVO);
                    break;
            }

            if (coroutine == null)
            {
                ReportError(fileDownloadVO.path);
                return;
            }

            // Request starting the coroutine
            SendNotification(CoreNote.REQUEST_START_COROUTINE, new RequestStartCoroutineVO
            {
                coroutine = coroutine
            });
        }

        /// <summary>
        /// Load the texture from path via UnityWebRequest
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private IEnumerator DownloadTexture(FileDownloadVO fileDownloadVO, bool forceCached = false)
        {
            // Attempt to load cached version
            string fileName = Path.GetFileName(fileDownloadVO.path);
            string cachedFilePath = Path.Combine(UnityEngine.Application.persistentDataPath, fileName);
            string loadFromPath = fileDownloadVO.path;
            bool cacheExists = File.Exists(cachedFilePath);
            int cacheAge = -1;

            if (cacheExists)
            {
                cacheAge = System.DateTime.Now.Subtract(File.GetLastWriteTime(cachedFilePath)).Hours;

                // Check the age of file
                if (cacheAge <= 24 || forceCached)
                {
                    loadFromPath = cachedFilePath;

                    Texture2D texture = new Texture2D(10, 10);
                    texture.LoadImage(File.ReadAllBytes(cachedFilePath), true);

                    // Check for onComplete callback
                    fileDownloadVO.onComplete?.Invoke(texture);

                    // Notify the application
                    SendNotification(FileDownloadNote.FILE_DOWNLOADED, texture);

                    yield return new WaitForFixedUpdate();
                    ProcessNextItemInQueue();

                    yield break;
                }
            }

            UnityWebRequest uwr = UnityWebRequestTexture.GetTexture(loadFromPath, true);

            uwr.SendWebRequest();

            while (!uwr.isDone)
            {
                yield return null;
            }

            if (uwr.isNetworkError || uwr.isHttpError)
            {
                if (loadFromPath == fileDownloadVO.path && cacheExists && cacheAge > 24)
                {
                    // Request starting the coroutine with forced cache
                    SendNotification(CoreNote.REQUEST_START_COROUTINE, new RequestStartCoroutineVO
                    {
                        coroutine = DownloadTexture(fileDownloadVO, true)
                    });
                    yield break;
                }

                ReportError(fileDownloadVO.path);
                yield break;
            }
            else
            {
                // Store in cache
                if (!cacheExists || cacheAge > 24 && loadFromPath == fileDownloadVO.path)
                {
                    File.WriteAllBytes(cachedFilePath, uwr.downloadHandler.data);
                }

                Texture2D texture = DownloadHandlerTexture.GetContent(uwr);

                // Check for onComplete callback
                fileDownloadVO.onComplete?.Invoke(texture);

                // Notify the application
                SendNotification(FileDownloadNote.FILE_DOWNLOADED, texture);
            }

            yield return new WaitForFixedUpdate();
            ProcessNextItemInQueue();
        }

        /// <summary>
        /// Load the texture from path via UnityWebRequest
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private IEnumerator DownloadVideoClip(FileDownloadVO fileDownloadVO, bool forceCached = false)
        {
            // Attempt to load cached version
            string fileName = Path.GetFileName(fileDownloadVO.path);
            string cachedFilePath = Path.Combine(UnityEngine.Application.persistentDataPath, fileName);
            string loadFromPath = fileDownloadVO.path;
            bool cacheExists = File.Exists(cachedFilePath);
            int cacheAge = -1;

            if (cacheExists)
            {
                cacheAge = System.DateTime.Now.Subtract(File.GetLastWriteTime(cachedFilePath)).Hours;

                // Check the age of file
                if (cacheAge <= 24 || forceCached)
                {
                    loadFromPath = cachedFilePath;

                    // Check for onComplete callback
                    fileDownloadVO.onComplete?.Invoke(cachedFilePath);

                    // Notify the application
                    SendNotification(FileDownloadNote.FILE_DOWNLOADED, cachedFilePath);

                    yield return new WaitForFixedUpdate();
                    ProcessNextItemInQueue();

                    yield break;
                }
            }


            UnityWebRequest uwr = UnityWebRequest.Get(loadFromPath);

            uwr.SendWebRequest();
            ProcessNextItemInQueue();

            while (!uwr.isDone)
            {
                yield return null;
            }

            if (uwr.isNetworkError || uwr.isHttpError)
            {
                if (loadFromPath == fileDownloadVO.path && cacheExists && cacheAge > 24)
                {
                    // Request starting the coroutine with forced cache
                    SendNotification(CoreNote.REQUEST_START_COROUTINE, new RequestStartCoroutineVO
                    {
                        coroutine = DownloadVideoClip(fileDownloadVO, true)
                    });
                    yield break;
                }

                ReportError(fileDownloadVO.path);
                yield break;
            }
            else
            {
                // Store in cache
                if (!cacheExists || cacheAge > 24 && loadFromPath == fileDownloadVO.path)
                {
                    File.WriteAllBytes(cachedFilePath, uwr.downloadHandler.data);
                }

                // Check for onComplete callback
                fileDownloadVO.onComplete?.Invoke(cachedFilePath);

                // Notify the application
                SendNotification(FileDownloadNote.FILE_DOWNLOADED, cachedFilePath);
            }
            yield return new WaitForFixedUpdate();
            // ProcessNextItemInQueue();
        }

        /// <summary>
        /// Load the assetbundle from path via UnityWebRequest
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private IEnumerator DownloadAssetBundle(FileDownloadVO fileDownloadVO)
        {
            UnityWebRequest uwr = UnityWebRequestAssetBundle.GetAssetBundle(fileDownloadVO.path, (uint)fileDownloadVO.version, 0);

            uwr.SendWebRequest();

            while (!uwr.isDone)
            {
                yield return null;
            }

            if (uwr.isNetworkError || uwr.isHttpError)
            {
                ReportError(fileDownloadVO.path);
                yield break;
            }
            else
            {
                AssetBundle bundle = DownloadHandlerAssetBundle.GetContent(uwr);

                // Check for onComplete callback
                fileDownloadVO.onComplete?.Invoke(bundle);

                // Notify system
                SendNotification(FileDownloadNote.FILE_DOWNLOADED, bundle);
            }

            yield return new WaitForFixedUpdate();
            ProcessNextItemInQueue();

        }

        /// <summary>
        /// Request an error dialog
        /// Create a button calback to try to load data again
        /// </summary>
        protected void ReportError(string filename = "")
        {
            ProcessNextItemInQueue();

            SendNotification(CoreNote.ERROR, new CoreErrorVO
            {
                errorType = FileDownloadNote.FILE_DOWNLOAD_ERROR,
                title = "ERROR",
                message = "THERE WAS AN ERROR DOWNLOADING A FILE : \n\n " + filename,
                buttonText = "OK"
            });
        }
    }
}
