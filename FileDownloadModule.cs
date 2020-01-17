/*
 * 																					
 * CoreBehaviour.cs															
 * 																					
 * This behaviour should be attached directly to the main Application GameObject.
 * This is used as an entry point for establishing a concrete application facade.	
 * 																					
 * Boots up the Core system, which then loads up the Application Prepare commands in Application namespace
 * 
 */
namespace UnityPureMVC.Modules.FileDownload
{
    using PureMVC.Interfaces;
    using PureMVC.Patterns.Facade;
    using UnityPureMVC.Modules.FileDownload.Controller.Commands;
    using UnityPureMVC.Modules.FileDownload.Controller.Notes;
    using System;
    using UnityEngine;

    internal class FileDownloadModule : MonoBehaviour
    {
        /// <summary>
        /// The core facade.
        /// </summary>
        private IFacade facade;

        /// <summary>
        /// Start this instance.
        /// </summary>
        protected virtual void Awake()
        {
            try
            {
                facade = Facade.GetInstance("Core");
                facade.RegisterCommand(FileDownloadNote.START, typeof(FileDownloadStartCommand));
                facade.SendNotification(FileDownloadNote.START, this);
            }
            catch (Exception exception)
            {
                throw new UnityException("Unable to initiate Facade", exception);
            }
        }

        /// <summary>
        /// On destroy.
        /// </summary>
        protected virtual void OnDestroy()
        {
            if (facade != null)
            {
                facade.Dispose();
                facade = null;
            }
        }
    }
}