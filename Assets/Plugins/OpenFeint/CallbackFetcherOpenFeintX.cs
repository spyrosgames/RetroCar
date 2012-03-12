using System;
using System.Collections;
namespace Gree.Unity
{

	/// <summary>
    /// Callback fetcher for OpenFeint X events/methods.
    /// </summary>
	public class CallbackFetcherOpenFeintX : CallbackFetcherBase
    {
        #region Initialization
        public CallbackFetcherOpenFeintX()
            : base()
        {
            initializeCallbacks();
        }

        /// <summary>
        /// Initializes the callbacks for every method to register.
        /// </summary>
		private void initializeCallbacks()
        {
            // OpenFeintX
            registeredCallbacks.Add("TriggerInAppPurchaseSucceeded", new TriggerInAppPurchaseSucceeded());
            registeredCallbacks.Add("TriggerInAppPurchaseFailedWithStatus", new TriggerInAppPurchaseFailedWithStatus());
            registeredCallbacks.Add("TriggerInAppPurchasePayloadStarted", new TriggerInAppPurchasePayloadStarted());
            registeredCallbacks.Add("TriggerInAppPurchasePayloadProgress", new TriggerInAppPurchasePayloadProgress());
            registeredCallbacks.Add("TriggerInAppPurchasePayloadLoaded", new TriggerInAppPurchasePayloadLoaded());
            registeredCallbacks.Add("TriggerInAppPurchasePayloadSucceeded", new TriggerInAppPurchasePayloadSucceeded());
            registeredCallbacks.Add("TriggerInAppPurchaseCatalogUpdated", new TriggerInAppPurchaseCatalogUpdated());
            registeredCallbacks.Add("TriggerInAppPurchaseScreenshotLoaded", new TriggerInAppPurchaseScreenshotLoaded());
			registeredCallbacks.Add("TriggerInAppPurchaseIconLoaded", new TriggerInAppPurchaseIconLoaded());
			registeredCallbacks.Add("TriggerCurrencyIconLoaded", new TriggerCurrencyIconLoaded());
            registeredCallbacks.Add("TriggerInventorySwitched", new TriggerInventorySwitched());
            registeredCallbacks.Add("TriggerInventorySynchronized", new TriggerInventorySynchronized());
            registeredCallbacks.Add("TriggerPayloadForItemStartedLoading", new TriggerPayloadForItemStartedLoading());
            registeredCallbacks.Add("TriggerPayloadForItemFinishedLoading", new TriggerPayloadForItemFinishedLoading());
            registeredCallbacks.Add("TriggerPayloadForItemUpdatedProgress", new TriggerPayloadForItemUpdatedProgress());
            registeredCallbacks.Add("TriggerPayloadUpdatesRequired", new TriggerPayloadUpdatesRequired());
            registeredCallbacks.Add("TriggerStoreKitNonconsumableRestoreFinished", new TriggerStoreKitNonconsumableRestoreFinished());
        }
		
		public override Hashtable callBackHandlersList() {
			
			Hashtable _callbackHandlers = new Hashtable();
			_callbackHandlers.Add ("TriggerInAppPurchaseSucceeded", this);
			_callbackHandlers.Add ("TriggerInAppPurchaseFailedWithStatus", this);
			_callbackHandlers.Add ("TriggerInAppPurchasePayloadStarted", this);
			_callbackHandlers.Add ("TriggerInAppPurchasePayloadProgress", this);
			_callbackHandlers.Add ("TriggerInAppPurchasePayloadLoaded", this);
			_callbackHandlers.Add ("TriggerInAppPurchasePayloadSucceeded", this);
			_callbackHandlers.Add ("TriggerInAppPurchaseCatalogUpdated", this);
			_callbackHandlers.Add ("TriggerInAppPurchaseScreenshotLoaded", this);
			_callbackHandlers.Add ("TriggerInAppPurchaseIconLoaded", this);
			_callbackHandlers.Add ("TriggerCurrencyIconLoaded", this);
			_callbackHandlers.Add ("TriggerInventorySwitched", this);
			_callbackHandlers.Add ("TriggerInventorySynchronized", this);
			_callbackHandlers.Add ("TriggerPayloadForItemStartedLoading", this);
			_callbackHandlers.Add ("TriggerPayloadForItemFinishedLoading", this);
			_callbackHandlers.Add ("TriggerPayloadForItemUpdatedProgress", this);
			_callbackHandlers.Add ("TriggerPayloadUpdatesRequired", this);
			_callbackHandlers.Add ("TriggerStoreKitNonconsumableRestoreFinished", this);
			return _callbackHandlers;
		}
        #endregion

        #region Declarations
        // ENUMERATIONS //
		/// <summary>
		/// Corresponds to OFItemPayloadStatus in OFXStoreEnums.h
		/// </summary>
        public enum eItemPayloadStatus
        {
            /// <summary>
            /// This purchase has not a downloadable payload.
            /// </summary>
			ItemPayloadStatus_None,
			
            /// <summary>
			/// This purchase has a payload, but it is not downloaded.
            /// </summary>
			ItemPayloadStatus_NotLoaded,
			
			/// <summary>
			/// This purchase payload is in progress.
			/// </summary>
            ItemPayloadStatus_Loading,
			
			/// <summary>
			/// This purchase payload is already on the device.
			/// </summary>
            ItemPayloadStatus_Loaded,
        }


        // DELEGATE DEFINITIONS //
        // Event handler delegates.
        public delegate void PurchaseSucceeded( string itemIdentifier );
        public delegate void PurchaseFailedWithStatus( string itemIdentifier, int status );
        public delegate void PurchasePayloadStarted( string itemIdentifier );
        public delegate void PurchasePayloadProgress( string itemIdentifier, double progress );
        public delegate void PurchasePayloadLoaded( string itemIdentifier, bool success );
        public delegate void PurchasePayloadSucceeded( string itemIdentifier );
        public delegate void PurchaseCatalogUpdated();
        public delegate void PurchaseScreenshotLoaded( string itemIdentifier, string filePath );
        public delegate void PurchaseIconLoaded( string itemIdentifier, string filePath );
        public delegate void CurrencyIconLoaded( string itemIdentifier, string filePath );
        public delegate void InventorySwitched();
        public delegate void InventorySynchronized( int status );
        public delegate void PayloadStartedLoading( string itemIdentifier );
        public delegate void PayloadUpdatedProgress( string itemIdentifier, double progress );
        public delegate void PayloadFinishedLoading( string itemIdentifier, bool success );
        public delegate void PayloadUpdatesRequired( string[] itemIdentifiers );
        public delegate void StoreKitNonconsumableRestoreFinished();

        static public event PurchaseSucceeded 						OnPurchaseSucceeded;
        static public event PurchaseFailedWithStatus 				OnPurchaseFailedWithStatus;
        static public event PurchasePayloadStarted 					OnPurchasePayloadStarted;
        static public event PurchasePayloadProgress 				OnPurchasePayloadProgress;
        static public event PurchasePayloadLoaded 					OnPurchasePayloadLoaded;
        static public event PurchasePayloadSucceeded 				OnPurchasePayloadSucceeded;
        static public event PurchaseCatalogUpdated 					OnPurchaseCatalogUpdated;
        static public event PurchaseScreenshotLoaded 				OnPurchaseScreenshotLoaded;
        static public event PurchaseIconLoaded 						OnPurchaseIconLoaded;
        static public event CurrencyIconLoaded 						OnCurrencyIconLoaded;
        static public event InventorySwitched 						OnInventorySwitched;
        static public event InventorySynchronized 					OnInventorySynchronized;
        static public event PayloadStartedLoading 					OnPayloadStartedLoading;
        static public event PayloadUpdatedProgress 					OnPayloadUpdatedProgress;
        static public event PayloadFinishedLoading 					OnPayloadFinishedLoading;
        static public event PayloadUpdatesRequired 					OnPayloadUpdatesRequired;
        static public event StoreKitNonconsumableRestoreFinished 	OnStoreKitNonconsumableRestoreFinished;

        #endregion

        #region OpenFeintX Callbacks
        private class TriggerInAppPurchaseSucceeded : ICallback
        {
            public void execute( ArrayList args )
            {
                string message = (string)args[0];
                if ( OnPurchaseSucceeded != null )
                {
                    OnPurchaseSucceeded(message);
                }
            }
        }

        private class TriggerInAppPurchaseFailedWithStatus : ICallback
        {
            public void execute( ArrayList args )
            {
                string itemIdentifier = (string)args[0];
                int status = System.Convert.ToInt32(args[1]);
                if ( OnPurchaseFailedWithStatus != null )
                {
                    OnPurchaseFailedWithStatus(itemIdentifier, status);
                }
            }
        }

        private class TriggerInAppPurchasePayloadStarted : ICallback
        {
            public void execute( ArrayList args )
            {
                string message = (string)args[0];
                if ( OnPurchasePayloadStarted != null )
                {
                    OnPurchasePayloadStarted(message);
                }
            }
        }

        private class TriggerInAppPurchasePayloadProgress : ICallback
        {
            public void execute( ArrayList args )
            {
                string itemIdentifier = (string)args[0];
                double progress = (double)args[1];
                if ( OnPurchasePayloadProgress != null )
                {
                    OnPurchasePayloadProgress(itemIdentifier, progress);
                }
            }
        }

        private class TriggerInAppPurchasePayloadLoaded : ICallback
        {
            public void execute( ArrayList args )
            {
                string itemIdentifier = (string)args[0];
                bool success = System.Convert.ToBoolean(args[1]);
                if ( OnPurchasePayloadLoaded != null )
                {
                    OnPurchasePayloadLoaded(itemIdentifier, success);
                }
            }
        }

        private class TriggerInAppPurchasePayloadSucceeded : ICallback
        {
            public void execute( ArrayList args )
            {
                string message = (string)args[0];
                if ( OnPurchasePayloadSucceeded != null )
                {
                    OnPurchasePayloadSucceeded(message);
                }
            }
        }

        private class TriggerInAppPurchaseCatalogUpdated : ICallback
        {
            public void execute( ArrayList args )
            {
                if ( OnPurchaseCatalogUpdated != null )
                {
                    OnPurchaseCatalogUpdated();
                }
            }
        }

        private class TriggerInAppPurchaseScreenshotLoaded : ICallback
        {
            public void execute( ArrayList args )
            {
                string itemIdentifier = (string)args[0];
                string filePath = (string)args[1];
                if ( OnPurchaseScreenshotLoaded != null )
                {
                    OnPurchaseScreenshotLoaded(itemIdentifier, filePath);
                }
            }
        }

        private class TriggerInAppPurchaseIconLoaded : ICallback
        {
            public void execute( ArrayList args )
            {
                string itemIdentifier = (string)args[0];
                string filePath = (string)args[1];
                if ( OnPurchaseIconLoaded != null )
                {
                    OnPurchaseIconLoaded(itemIdentifier, filePath);
                }
            }
        }

        private class TriggerCurrencyIconLoaded : ICallback
        {
            public void execute( ArrayList args )
            {
                string itemIdentifier = (string)args[0];
                string filePath = (string)args[1];
                if ( OnCurrencyIconLoaded != null )
                {
                    OnCurrencyIconLoaded(itemIdentifier, filePath);
                }
            }
        }

        private class TriggerInventorySwitched : ICallback
        {
            public void execute( ArrayList args )
            {
                if ( OnInventorySwitched != null )
                {
                    OnInventorySwitched();
                }
            }
        }

        private class TriggerInventorySynchronized : ICallback
        {
            public void execute( ArrayList args )
            {
                int status = System.Convert.ToInt32(args[0]);
                if ( OnInventorySynchronized != null )
                {
                    OnInventorySynchronized(status);
                }
            }
        }

        private class TriggerPayloadForItemStartedLoading : ICallback
        {
            public void execute( ArrayList args )
            {
                string message = (string)args[0];
                if ( OnPayloadStartedLoading != null )
                {
                    OnPayloadStartedLoading(message);
                }
            }
        }

        private class TriggerPayloadForItemUpdatedProgress : ICallback
        {
            public void execute( ArrayList args )
            {
                string itemIdentifier = (string)args[0];
                double progress = (double)args[1];
                if ( OnPayloadUpdatedProgress != null )
                {
                    OnPayloadUpdatedProgress(itemIdentifier, progress);
                }
            }
        }

        private class TriggerPayloadForItemFinishedLoading : ICallback
        {
            public void execute( ArrayList args )
            {
                string itemIdentifier = (string)args[0];
                bool success = System.Convert.ToBoolean(args[1]);
                if ( OnPayloadFinishedLoading != null )
                {
                    OnPayloadFinishedLoading(itemIdentifier, success);
                }
            }
        }

        private class TriggerPayloadUpdatesRequired : ICallback
        {
            public void execute( ArrayList args )
            {
                string[] itemIdentifiers = (string[])args[0];
                if ( OnPayloadUpdatesRequired != null )
                {
                    OnPayloadUpdatesRequired(itemIdentifiers);
                }
            }
        }

        private class TriggerStoreKitNonconsumableRestoreFinished : ICallback
        {
            public void execute( ArrayList args )
            {
                if ( OnStoreKitNonconsumableRestoreFinished != null )
                {
                    OnStoreKitNonconsumableRestoreFinished();
                }
            }
        }
        #endregion
    }
}

