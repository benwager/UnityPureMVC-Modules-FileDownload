using PureMVC.Interfaces;
using PureMVC.Patterns.Command;
using UnityPureMVC.Core.Libraries.UnityLib.Utilities.Logging;
using UnityPureMVC.Modules.FileDownload.Controller.Commands.Request;
using UnityPureMVC.Modules.FileDownload.Controller.Notes;
using UnityPureMVC.Modules.FileDownload.Model.Proxies;

namespace UnityPureMVC.Modules.FileDownload.Controller.Commands
{
    internal class FileDownloadStartCommand : SimpleCommand
    {
        public override void Execute(INotification notification)
        {
            DebugLogger.Log("{0}::Execute", "FileDownloadStartCommand");

            // Register proxy
            Facade.RegisterProxy(new FileDownloadProxy());

            // Register Commands
            Facade.RegisterCommand(FileDownloadNote.REQUEST_FILE_DOWNLOAD, typeof(RequestFileDownloadCommand));
            Facade.RegisterCommand(FileDownloadNote.REQUEST_PROCESS_QUEUE, typeof(RequestsProcessQueueCommand));

            Facade.RemoveCommand(FileDownloadNote.START);
        }
    }
}
