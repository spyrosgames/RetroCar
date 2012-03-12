using UnityEngine;
using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;

public class OpenFeintX : Gree.Unity.UnityToNative
{	
	
	/// <summary>
	/// Returns the version of OFX sdk.
	/// </summary>
	static public string OFXVersion
	{
		get {
			#if UNITY_ANDROID
				throwAndroidUnsupported("OpenFeintX.cs", "OFXVersion");
				return "";
			#else
				return (string)SendMessage("oFXVersion", new ArrayList());
			#endif
		}
	}

	/// <summary>
	/// Tells that the player is about to enter the store.
	/// This must be called before the player enters the store.
	/// This will cause certain UI screens to appear to keep the user form losing data
	/// </summary>
	static public void OFXUserWillEnterStore()
	{
		#if UNITY_ANDROID
			throwAndroidUnsupported("OpenFeintX.cs", "oFXUserWillEnterStore");
		#else
			SendMessage("oFXUserWillEnterStore", new ArrayList());
		#endif
	}

	/// <summary>
	/// Tells that the player is about to leave the store. This must be called when the player leaves the 
	/// store in order to serialize the inventory and a server synchronization.
	/// </summary>
	static public void OFXUserWillLeaveStore()
	{
		SendMessage("oFXUserWillLeaveStore", new ArrayList());
	}

	/// <summary>
	/// Attempts to restore all purchases items associated with the logged-in iTunes Store account.
	/// This will only restore items configured as non-consumable, unique, and paid for via StoreKit.
	/// </summary>
	/// <remarks>
	/// This method may ask the user to login with their iTunes Store account.
	/// Upon completion of the restore the event OnStoreKitNonconsumableRestoreFinished will be trigged.
	/// </remarks>
	static public void OFXRestoreStoreKitNonconsumablePurchases()
	{
		#if UNITY_ANDROID
			throwAndroidUnsupported("OpenFeintX.cs", "oFXRestoreStoreKitNonconsumablePurchases");
		#else
			SendMessage("oFXRestoreStoreKitNonconsumablePurchases", new ArrayList());
		#endif
	}

	/// <summary>
	/// Access the amount of a specific currency in the user's inventory
	/// </summary>
	/// <returns>
	/// Quantity of the given currency possessed by the user.
	/// </returns>
	/// <param name='currency'>
	/// he currency identifier to query the quantity of.
	/// </param>
	static public int OFXAmountOfCurrency(string currency)
	{
		ArrayList args = new ArrayList {
			currency
		};
		return System.Convert.ToInt32(SendMessage("oFXAmountOfCurrency", args));
	}

	/// <summary>
	/// Access the quantity possessed of a specific item in the user's inventory.
	/// </summary>
	/// <returns>
	/// uantity of the given item possessed by the user
	/// </returns>
	/// <param name='itemIdentifier'>
	/// The item identifier to query the quantity of.
	/// </param>
	static public int OFXNumberOfItem(string itemIdentifier)
	{
		ArrayList args = new ArrayList {
			itemIdentifier
		};
		
		return System.Convert.ToInt32(SendMessage("oFXNumberOfItem", args));
	}

	/// <summary>
	/// Stores arbitrary information associated with a player's use of the the items in their
	/// inventory. This information is stored alongside the inventory on the OpenFeint servers.
	/// For example, this could be used to track which items a player has equipped:
	/// - Mark that item identifier "wooden_bow" was equipped in "main_hand" slot:
	/// OFXAddToMetadata("wooden_bow", "main_hand");
	/// </summary>
	/// <param name='itemIdentifier'>
	/// Item identifier.
	/// </param>
	/// <param name='slot'>
	/// Slot.
	/// </param>
	static public void OFXAddToMetadata(string itemIdentifier, string slot)
	{
		ArrayList args = new ArrayList {
			itemIdentifier,
			slot
		};
		
		SendMessage("oFXAddToMetadata", args);
	}

	/// <summary>
	/// Restores the arbitrary information associated with a player's use of the the items in their
	/// inventory. This information was stored alongside the inventory on the OpenFeint servers.
	/// For example, to figure out what item identifier is equipped in the "head" slot:
	/// string equipped = OFXEsistsInMetadata("head");
	/// </summary>
	/// <returns>
	/// The exists in metadata.
	/// </returns>
	/// <param name='slot'>
	/// Slot.
	/// </param>
	static public string OFXExistsInMetadata(string slot)
	{
		ArrayList args = new ArrayList {
			slot
		};
		return (string)SendMessage("oFXExistsInMetadata", args);
	}

	/// <summary>
	/// Removes the arbitrary information associated with a player's use of the the items in their
	/// inventory.
	/// </summary>
	/// <param name='slot'>
	/// Slot.
	/// </param>
	static public void OFXRemoveFromMetadata(string slot)
	{
		#if UNITY_ANDROID
			throwAndroidUnsupported("OpenFeintX.cs", "oFXRemoveFromMetadata");
		#else
			ArrayList args = new ArrayList {
				slot
			};
			SendMessage("oFXRemoveFromMetadata", args);
		#endif
	}

	/// <summary>
	/// Returns the set of item identifiers which need to have their payloads downloaded
	/// </summary>
	/// <returns>
	/// The items with an outdated payload.
	/// </returns>
	static public ArrayList OFXItemsWithAnOutdatedPayload()
	{
		#if UNITY_ANDROID
			throwAndroidUnsupported("OpenFeintX.cs", "oFXItemsWithAnOutdatedPayload");
			return new ArrayList();
		#else
			return (ArrayList)SendMessage("oFXItemsWithAnOutdatedPayload", new ArrayList());
		#endif
	}

	/// <summary>
	/// Begin the download of the payload associated with the given item.
	/// </summary>
	/// <returns>
	/// True if loaded.
	/// </returns>
	/// <param name='itemIdentifier'>
	/// The item identifier whose payload to download
	/// </param>
	static public bool OFXLoadPayloadForItem(string itemIdentifier)
	{
		ArrayList args = new ArrayList {
			itemIdentifier
		};
		return (bool)SendMessage("oFXLoadPayloadForItem", args);
	}

	/// <summary>
	/// Returns the payload download status for a given item.
	/// </summary>
	/// <returns>
	/// The payload status for item.
	/// </returns>
	/// <param name='itemIdentifier'>
	/// Item identifier.
	/// </param>
	static public Gree.Unity.CallbackFetcherOpenFeintX.eItemPayloadStatus OFXPayloadStatusForItem(string itemIdentifier)
	{
		#if UNITY_ANDROID
			throwAndroidUnsupported("OpenFeintX.cs", "oFXPayloadStatusForItem");
			return Gree.Unity.CallbackFetcherOpenFeintX.eItemPayloadStatus.ItemPayloadStatus_None;
		#else
			ArrayList args = new ArrayList {
				itemIdentifier
			};
			return (Gree.Unity.CallbackFetcherOpenFeintX.eItemPayloadStatus)System.Convert.ToInt32(SendMessage("oFXPayloadStatusForItem", args));
		#endif
	}

	/// <summary>
	/// Returns the payload download progress for a given item.
	/// </summary>
	/// <remarks>
	/// This method can only return a non-zero value if payloadStatusForItem is
	/// ItemPayloadStatus_Loading, signifying that a download is in progress.
	/// </remarks>
	/// <returns>
	/// Current download progress for the given item identifier's payload download.
	/// </returns>
	/// <param name='itemIdentifier'>
	/// Item identifier.
	/// </param>
	static public double OFXPayloadProgressForItem(string itemIdentifier)
	{
		#if UNITY_ANDROID
			throwAndroidUnsupported("OpenFeintX.cs", "oFXPayloadProgressForItem");
			return 0D;
		#else
			ArrayList args = new ArrayList {
				itemIdentifier
			};
			return (double)SendMessage("oFXPayloadProgressForItem", args);
		#endif
	}

	/// <summary>
	/// Writes the current inventory and unsent transaction logs to disk.
	/// Calling this insures that the data will not be lost in the case of the application
	/// stopping unexpectedly.
	/// </summary>
	static public void OFXStoreInventory()
	{
		SendMessage("oFXStoreInventory", new ArrayList());
	}

	/// <summary>
	/// Writes the current inventory to the server for permanent storage. Since this is an
	/// HTTP request, it is not recommended during gameplay.
	/// </summary>
	/// <remarks>
	/// This method may result in a user-facing UIAlertView when there are any inventory
	/// modifications on the server or if the user had last updated on a different device.
	/// Triggers OnInventorySynchronized when complete.
	/// </remarks>
	static public void OFXSynchronizeInventory()
	{
		SendMessage("oFXSynchronizeInventory", new ArrayList());
	}
	
	/// <summary>
	/// Records a change in currency.	/// </summary>
	/// <param name='currencyId'>
	/// Currency identifier.
	/// </param>
	/// <param name='amount'>
	/// Amount.
	/// </param>
	static public void OFXModifyCurrency(string currencyId, int amount)
	{
		ArrayList args = new ArrayList {
			currencyId,
			amount
		};
		SendMessage("oFXModifyCurrency", args);
	}

	/// <summary>
	/// Records a change in item counts.
	/// </summary>
	/// <param name='itemIdentifier'>
	/// Item identifier.
	/// </param>
	/// <param name='quantity'>
	/// Quantity.
	/// </param>
	static public void OFXModifyGameItem(string itemIdentifier, int quantity)
	{
		ArrayList args = new ArrayList {
			itemIdentifier,
			quantity
		};
		SendMessage("oFXModifyGameItem", args);
	}

	/// <summary>
	/// Determines if the inventory can hold any more of the given item.
	/// </summary>
	/// <returns>
	/// True if more of the specified item can be acquired, false otherwise.
	/// </returns>
	/// <param name='itemIdentifier'>
	/// Identifier of the item to check.
	/// </param>
	static public bool OFXCanHaveMore(string itemIdentifier)
	{
		ArrayList args = new ArrayList {
			itemIdentifier
		};
		return (bool)SendMessage("oFXCanHaveMore", args);
	}

	/// <summary>
	/// Downloads any updated catalog information from the OpenFeint servers.
	/// This is automatically called during application initialization.
	/// </summary>
	static public void OFXUpdateCatalogFromServer()
	{
		SendMessage("oFXUpdateCatalogFromServer", new ArrayList());
	}

	/// <summary>
	/// Retrieve a list of in app purchases by category
	/// </summary>
	/// <returns>
	/// A list of item identifiers within the given category
	/// </returns>
	/// <param name='category'>
	/// The category to filter by.
	/// </param>
	static public ArrayList OFXInAppPurchasesForCategory(string category)
	{
		ArrayList args = new ArrayList {
			category
		};
		return (ArrayList)SendMessage("oFXInAppPurchasesForCategory", args);
	}

	/// <summary>
	/// Request to load the icon image for this currency.
	/// </summary>
	/// <remarks>
	// This method may incur a network request to download the data.
	/// </remarks>
	/// <param name='currencyId'>
	/// If set to <c>true</c> currency identifier.
	/// </param>
	static public void OFXCurrencyLoadIcon(string currencyId)
	{
		ArrayList args = new ArrayList {
			currencyId
		};
		SendMessage("oFXCurrencyLoadIcon", args);
	}

	/// <summary>
	/// Returns a Dictionary<property, value> with all currency properties.
	/// Valid properties: identifier, name and iconUrl
	/// </summary>
	/// <returns>
	/// The currency properties.
	/// </returns>
	/// <param name='currencyId'>
	/// Currency identifier.
	/// </param>
	static public Hashtable OFXCurrencyProperties(string currencyId)
	{
		ArrayList args = new ArrayList {
			currencyId
		};
		return (Hashtable)SendMessage("oFXCurrencyProperties", args);
	}

	/// <summary>
	/// Initiate the purchase process.
	/// </summary>
	/// <remarks>
	// Triggers OnPurchaseSucceeded: on success, OnPurchaseFailedWithStatus: on failure.
	/// </remarks>
	/// <returns>
	/// The purchase.
	/// </returns>
	/// <param name='itemIdentifier'>
	/// Item identifier.
	/// </param>
	static public bool OFXPurchase(string itemIdentifier)
	{
		ArrayList args = new ArrayList {
			itemIdentifier
		};
		return (bool)SendMessage("oFXPurchase", args);
	}

	// OFXPurchaseIsPurchasable //
	//
	// Determines if this item is purchasable.
	//
	// Returns:
	// true if the content purchase is possible, false otherwise.
	/// <summary>
	/// Determines if this item is purchasable.
	/// </summary>
	/// <returns>
	/// True if the content purchase is possible, false otherwise.
	/// </returns>
	/// <param name='itemIdentifier'>
	/// Item identifier.
	/// </param>
	static public bool OFXPurchaseIsPurchasable(string itemIdentifier)
	{
		ArrayList args = new ArrayList {
			itemIdentifier
		};
		return (bool)SendMessage("oFXPurchaseIsPurchasable", args);
	}

	/// <summary>
	/// A string containing the formatted price for this item. If this item is sold for hard
	/// currency ($) the string will be localized to the logged-in StoreKit account's locale.
	/// If this item is sold for virtual currency the string is simply a concatenation of
	/// purchaseCurrencyAmount and purchaseCurrency.name.
	/// </summary>
	/// <returns>
	/// The purchase formatted price.
	/// </returns>
	/// <param name='itemIdentifier'>
	/// Item identifier.
	/// </param>
	static public string OFXPurchaseFormattedPrice(string itemIdentifier)
	{
		ArrayList args = new ArrayList {
			itemIdentifier
		};
		return (string)SendMessage("oFXPurchaseFormattedPrice", args);
	}

	/// <summary>
	/// Returns true if the item is a free download. This will always return false for paid
	/// items even if the player already owns the item and can re-download it for free.
	/// </summary>
	/// <returns>
	/// The purchase is free.
	/// </returns>
	/// <param name='itemIdentifier'>
	/// Item identifier.
	/// </param>
	static public bool OFXPurchaseIsFree(string itemIdentifier)
	{
		ArrayList args = new ArrayList {
			itemIdentifier
		};
		return (bool)SendMessage("oFXPurchaseIsFree", args);
	}

	/// <summary>
	/// Request to load the icon image for this item.
	/// </summary>
	/// <remarks>
	// This method may incur a network request to download the data.
	/// </remarks>
	/// <returns>
	/// The purchase load icon.
	/// </returns>
	/// <param name='itemIdentifier'>
	/// Item identifier.
	/// </param>
	static public bool OFXPurchaseLoadIcon(string itemIdentifier)
	{
		ArrayList args = new ArrayList {
			itemIdentifier
		};
		return (bool)SendMessage("oFXPurchaseLoadIcon", args);
	}

	/// <summary>
	/// Request to load the screenshot image for this item.
	/// </summary>
	/// <remarks>
	/// This method may incur a network request to download the data.
	/// </remarks>
	/// <returns>
	/// The purchase load screenshot.
	/// </returns>
	/// <param name='itemIdentifier'>
	/// If set to <c>true</c> item identifier.
	/// </param>
	static public bool OFXPurchaseLoadScreenshot(string itemIdentifier)
	{
		ArrayList args = new ArrayList {
			itemIdentifier
		};
		return (bool)SendMessage("oFXPurchaseLoadScreenshot", args);
	}

	/// <summary>
	/// Request to load the store payload for this item.
	/// </summary>
	/// <remarks>
	// This method may incur a network request to download the data.
	/// </remarks>
	/// <returns>
	/// The purchase load store payload.
	/// </returns>
	/// <param name='itemIdentifier'>
	/// Item identifier.
	/// </param>
	static public bool OFXPurchaseLoadStorePayload(string itemIdentifier)
	{
		ArrayList args = new ArrayList {
			itemIdentifier
		};
		return (bool)SendMessage("oFXPurchaseLoadStorePayload", args);
	}

	/// <summary>
	/// Returns a Dictionary<property, value> with all purchase properties.
	/// Valid properties: category, storeKitProduct, storeKitPurchaseStarted,
	/// cachedStoreKitPrice, purchaseCurrencyIdentifier, deliverableIdentifier,
	/// position
	/// </summary>
	/// <returns>
	/// The purchase properties.
	/// </returns>
	/// <param name='itemIdentifier'>
	/// Item identifier.
	/// </param>
	static public Hashtable OFXPurchaseProperties(string itemIdentifier)
	{
		ArrayList args = new ArrayList {
			itemIdentifier
		};
		return (Hashtable)SendMessage("oFXPurchaseProperties", args);
	}		


	// PROPERTIES //

	/// <summary>
	/// Retrieve a list of all purchased items, unordered.
	/// </summary>
	/// <value>
	/// The OFX in app purchases.
	/// </value>
	static public ArrayList OFXInAppPurchases
	{
		get
		{
			return (ArrayList)SendMessage("oFXInAppPurchases", new ArrayList());
		}
	}

	/// <summary>
	/// Access a flat list of all items in the inventory
	/// </summary>
	/// <value>
	/// A list of item identifiers that are present in the inventory.
	/// </value>
	static public ArrayList OFXItemsInInventory
	{
		get
		{
			return (ArrayList)SendMessage("oFXItemsInInventory", new ArrayList());
		}
	}

	/// <summary>
	/// Access a flat list of all currency identifiers in the inventory
	/// </summary>
	/// <value>
	/// A list of currency identifiers that are present in the inventory.
	/// </value>
	static public ArrayList OFXCurrenciesInInventory
	{
		get
		{
			#if UNITY_ANDROID
				throwAndroidUnsupported("OpenFeintX.cs", "oFXCurrenciesInInventory");
				return new ArrayList();
			#else
				return (ArrayList)SendMessage("oFXCurrenciesInInventory", new ArrayList());
			#endif
		}
	}

	/// <summary>
	/// Retrieve an unordered list of in app purchases which have been added to the store
	/// between the previous server catalog update and the latest server catalog update.
	/// </summary>
	/// <value>
	/// A list of items identifiers
	/// </value>
	static public ArrayList OFXInAppPurchasesAddedInLastUpdate
	{
		get
		{
			#if UNITY_ANDROID
				throwAndroidUnsupported("OpenFeintX.cs", "oFXInAppPurchasesAddedInLastUpdate");
				return new ArrayList();
			#else
				return (ArrayList)SendMessage("oFXInAppPurchasesAddedInLastUpdate", new ArrayList());
			#endif
		}
	}

	/// <summary>
	/// Retrieve a list of all virtual currencies identifiers
	/// </summary>
	/// <value>
	/// The OFX currencies.
	/// </value>
	static public ArrayList OFXCurrencies
	{
		get
		{
			return (ArrayList)SendMessage("oFXCurrencies", new ArrayList());
		}
	}

	/// <summary>
	/// Retrive a list of all purchase categories
	/// </summary>
	/// <value>
	/// The OFX categories.
	/// </value>
	static public ArrayList OFXCategories
	{
		get
		{
			return (ArrayList)SendMessage("oFXCategories", new ArrayList());
		}
	}

	/// <summary>
	/// Returns all deliverable payload identifiers
	/// </summary>
	/// <value>
	/// The OFX deliverables.
	/// </value>
	static public ArrayList OFXDeliverables
	{
		get
		{
			return (ArrayList)SendMessage("oFXDeliverables", new ArrayList());
		}
	}
}
