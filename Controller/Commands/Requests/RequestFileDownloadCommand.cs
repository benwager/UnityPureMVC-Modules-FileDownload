using PureMVC.Interfaces;
using PureMVC.Patterns.Command;
using UnityPureMVC.Modules.FileDownload.Controller.Notes;
using UnityPureMVC.Modules.FileDownload.Model.Proxies;
using UnityPureMVC.Modules.FileDownload.Model.VO;

namespace UnityPureMVC.Modules.FileDownload.Controller.Commands.Request
{
    internal class RequestFileDownloadCommand : SimpleCommand
    {
        private FileDownloadProxy fileDownloadSystemProxy;

        /// <summary>
        /// Start coroutine to load assetbundle
        /// </summary>
        /// <param name="notification"></param>
        public override void Execute(INotification notification)
        {
            // Get the FileDownloadSystemProxy
            fileDownloadSystemProxy = Facade.RetrieveProxy(FileDownloadProxy.NAME) as FileDownloadProxy;

            // Get the bundle VO
            FileDownloadVO fileDownloadVO = notification.Body as FileDownloadVO;

            // Add the VO to queue
            fileDownloadSystemProxy.AddItemToQueue(fileDownloadVO);

            // Request process queue if auto AND is not already processing
            if (fileDownloadVO.autoProcessQueue && !fileDownloadSystemProxy.IsQueueProcessing)
            {
                SendNotification(FileDownloadNote.REQUEST_PROCESS_QUEUE);
            }
        }
    }
}
