using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Gree.Unity;

public class OpenFeintXSample : BaseUI 
{
	public GUISkin guiSkin;  
  
	public enum eOFXTopic
	{
		Currency,
		Item,
		Inventory,
		Catalog,
		Payload,
		InAppPurchase
	}

 	private string versionString;
	
	/// <summary>
	/// Scroll position.
	/// </summary>
	private Vector2 scrollPos = new Vector2();	
	
	/// <summary>
	/// It contains current values for all input fields present in this screen.
	/// </summary>
	private Dictionary<string, object[,]> inputValuesDictionary = new Dictionary<string, object[,]>();

	// Default leaderboard id
	private static string DEFAULT_CURRENCY_ID = (string)Properties.ConfigurationProperties["MY_APP_DEFAULT_CURRENCY_ID"]; // "1206612";
	private static string DEFAULT_ITEM_ID = (string)Properties.ConfigurationProperties["MY_APP_DEFAULT_ITEM_ID"]; // "1206612";
	private static string DEFAULT_SLOT = (string)Properties.ConfigurationProperties["MY_APP_DEFAULT_SLOT"]; // "head";	
	private static string DEFAULT_AMOUNT = (string)Properties.ConfigurationProperties["MY_APP_DEFAULT_AMOUNT"]; // "100";	
	private static string DEFAULT_QUANTITY = (string)Properties.ConfigurationProperties["MY_APP_DEFAULT_QUANTITY"]; // "10";	
	private static string DEFAULT_CATEGORY = (string)Properties.ConfigurationProperties["MY_APP_DEFAULT_CATEGORY"]; // "dcat";
	
	// Number of services present in this UI. This is needed so to calculate scroll view size.
	private eOFXTopic viewTopic = eOFXTopic.Currency;
	
	protected static IInputExecutor oFXAmountOfCurrencyExec	 	= new OFXAmountOfCurrencyExecutor();
	protected static IInputExecutor oFXModifyCurrencyExec	 	= new OFXModifyCurrencyExecutor();
	protected static IInputExecutor oFXCurrencyLoadIconExec	 	= new OFXCurrencyLoadIconExecutor();
	protected static IExecutor oFXCurrenciesInInventoryExec	 	= new OFXCurrenciesInInventoryExecutor();
	protected static IExecutor oFXCurrenciesExec			 	= new OFXCurrenciesExecutor();
	protected static IInputExecutor oFXCurrencyPropertiesExec 	= new OFXCurrencyPropertiesExecutor();
	
	protected static IInputExecutor oFXNumberOfItemExec			= new OFXNumberOfItemExecutor();	
	protected static IInputExecutor oFXAddToMetadataExec		= new OFXAddToMetadataExecutor();
	protected static IInputExecutor oFXExistsInMetadataExec		= new OFXExistsInMetadataExecutor();
	protected static IInputExecutor oFXRemoveFromMetadataExec	= new OFXRemoveFromMetadataExecutor();
	protected static IInputExecutor oFXModifyGameItemExec		= new OFXModifyGameItemExecutor();
	
	protected static IInputExecutor oFXCanHaveMoreExec			= new OFXCanHaveMoreExecutor();
	protected static IExecutor oFXItemsInInventoryExec			= new OFXItemsInInventoryExecutor();
	protected static IExecutor oFXStoreInventoryExec			= new OFXStoreInventoryExecutor();
	protected static IExecutor oFXSynchronizeInventoryExec		= new OFXSynchronizeInventoryExecutor();
	
	protected static IExecutor oFXItemsWithAnOutdatedPayloadExec	= new OFXItemsWithAnOutdatedPayloadExecutor();
	protected static IInputExecutor oFXLoadPayloadForItemExec		= new OFXLoadPayloadForItemExecutor();
	protected static IInputExecutor oFXPayloadStatusForItemExec		= new OFXPayloadStatusForItemExecutor();
	protected static IInputExecutor oFXPayloadProgressForItemExec	= new OFXPayloadProgressForItemExecutor();
	protected static IExecutor oFXDeliverablesExec					= new OFXDeliverablesExecutor();
	
	protected static IExecutor oFXUpdateCatalogFromServerExec			= new OFXUpdateCatalogFromServerExecutor();
	protected static IInputExecutor oFXInAppPurchasesForCategoryExec	= new OFXInAppPurchasesForCategoryExecutor();
	protected static IExecutor oFXCategoriesExec						= new OFXCategoriesExecutor();
	
	protected static IExecutor oFXInAppPurchasesExec					= new OFXInAppPurchasesExecutor();
	protected static IExecutor oFXInAppPurchasesAddedInLastUpdateExec	= new OFXInAppPurchasesAddedInLastUpdateExecutor();
	protected static IInputExecutor oFXPurchaseExec						= new OFXPurchaseExecutor();
	protected static IInputExecutor oFXPurchaseIsPurchasableExec		= new OFXPurchaseIsPurchasableExecutor();
	protected static IInputExecutor oFXPurchaseFormattedPriceExec		= new OFXPurchaseFormattedPriceExecutor();
	protected static IInputExecutor oFXPurchaseIsFreeExec				= new OFXPurchaseIsFreeExecutor();
	protected static IInputExecutor oFXPurchaseLoadIconExec				= new OFXPurchaseLoadIconExecutor();
	protected static IInputExecutor oFXPurchaseLoadScreenshotExec		= new OFXPurchaseLoadScreenshotExecutor();
	protected static IInputExecutor oFXPurchaseLoadStorePayloadExec		= new OFXPurchaseLoadStorePayloadExecutor();
	protected static IInputExecutor oFXPurchasePropertiesExec			= new OFXPurchasePropertiesExecutor();
	
	private Rect menuButtonRect0 = menuButtonRectForButton(0);
	private Rect menuButtonRect1 = menuButtonRectForButton(1);
	private Rect menuButtonRect2 = menuButtonRectForButton(2);
	private Rect menuButtonRect3 = menuButtonRectForButton(3);
	private Rect menuButtonRect4 = menuButtonRectForButton(4);
	private Rect menuButtonRect5 = menuButtonRectForButton(5);
					
	private static string hashTableToString(Hashtable hashTable) {
		string resultString = "";
		foreach (DictionaryEntry entry in hashTable) {
			resultString = resultString + String.Format("{0}: {1}\n", entry.Key, System.Convert.ToString(entry.Value));
		}
		return resultString;
	}
	
	private static string arrayListToString(ArrayList arrayList) {
		string resultString = "";
		foreach (string entry in arrayList) {
			resultString = resultString + String.Format("{0}\n", entry);
		}
		return resultString;
	}
		
    //At this script initialization  
    public void initializeInterface()  
    {  		 
		InitializeWithConfiguration(true);	
		
		Gree.Unity.CallbackFetcherOpenFeintX.OnPurchaseSucceeded						+= new	Gree.Unity.CallbackFetcherOpenFeintX.PurchaseSucceeded(CallbackPurchaseSucceeded);
		Gree.Unity.CallbackFetcherOpenFeintX.OnPurchaseFailedWithStatus				+= new	Gree.Unity.CallbackFetcherOpenFeintX.PurchaseFailedWithStatus(CallbackPurchaseFailed);
		Gree.Unity.CallbackFetcherOpenFeintX.OnPurchasePayloadStarted					+= new	Gree.Unity.CallbackFetcherOpenFeintX.PurchasePayloadStarted(CallbackPurchasePayloadStarted);
		Gree.Unity.CallbackFetcherOpenFeintX.OnPurchasePayloadProgress				+= new	Gree.Unity.CallbackFetcherOpenFeintX.PurchasePayloadProgress(CallbackPurchasePayloadProgress);
		Gree.Unity.CallbackFetcherOpenFeintX.OnPurchasePayloadLoaded					+= new	Gree.Unity.CallbackFetcherOpenFeintX.PurchasePayloadLoaded(CallbackPurchasePayloadLoaded);
		Gree.Unity.CallbackFetcherOpenFeintX.OnPurchasePayloadSucceeded				+= new	Gree.Unity.CallbackFetcherOpenFeintX.PurchasePayloadSucceeded(CallbackPurchasePayloadSucceeded);
		Gree.Unity.CallbackFetcherOpenFeintX.OnPurchaseCatalogUpdated					+= new	Gree.Unity.CallbackFetcherOpenFeintX.PurchaseCatalogUpdated(CallbackPurchaseCatalogUpdated);
		Gree.Unity.CallbackFetcherOpenFeintX.OnPurchaseScreenshotLoaded				+= new	Gree.Unity.CallbackFetcherOpenFeintX.PurchaseScreenshotLoaded(CallbackPurchaseScreenshotLoaded);
		Gree.Unity.CallbackFetcherOpenFeintX.OnPurchaseIconLoaded						+= new	Gree.Unity.CallbackFetcherOpenFeintX.PurchaseIconLoaded(CallbackPurchaseIconLoaded);
		Gree.Unity.CallbackFetcherOpenFeintX.OnCurrencyIconLoaded						+= new	Gree.Unity.CallbackFetcherOpenFeintX.CurrencyIconLoaded(CallbackCurrencyIconLoaded);
		Gree.Unity.CallbackFetcherOpenFeintX.OnInventorySwitched						+= new	Gree.Unity.CallbackFetcherOpenFeintX.InventorySwitched(CallbackInventorySwitched);
		Gree.Unity.CallbackFetcherOpenFeintX.OnInventorySynchronized					+= new	Gree.Unity.CallbackFetcherOpenFeintX.InventorySynchronized(CallbackInventorySynchronized);
		Gree.Unity.CallbackFetcherOpenFeintX.OnPayloadStartedLoading					+= new	Gree.Unity.CallbackFetcherOpenFeintX.PayloadStartedLoading(CallbackPayloadStartedLoading);
		Gree.Unity.CallbackFetcherOpenFeintX.OnPayloadUpdatedProgress					+= new	Gree.Unity.CallbackFetcherOpenFeintX.PayloadUpdatedProgress(CallbackPayloadUpdatedProgress);
		Gree.Unity.CallbackFetcherOpenFeintX.OnPayloadFinishedLoading					+= new	Gree.Unity.CallbackFetcherOpenFeintX.PayloadFinishedLoading(CallbackPayloadFinishedLoading);
		Gree.Unity.CallbackFetcherOpenFeintX.OnPayloadUpdatesRequired					+= new	Gree.Unity.CallbackFetcherOpenFeintX.PayloadUpdatesRequired(CallbackPayloadUpdatesRequired);
		Gree.Unity.CallbackFetcherOpenFeintX.OnStoreKitNonconsumableRestoreFinished 	+= new	Gree.Unity.CallbackFetcherOpenFeintX.StoreKitNonconsumableRestoreFinished(CallbackStoreKitNonconsumableRestoreFinished);
		
		initializeInputValuesDictionary();		
    }  
	
	private void initializeInputValuesDictionary() {
		inputValuesDictionary = new Dictionary<string, object[,]>();
		inputValuesDictionary.Add("oFXAmountOfCurrencyId", new object[,] { {"Currency Id :",DEFAULT_CURRENCY_ID,"textfield"}});
		inputValuesDictionary.Add("oFXNumberOfItemId", new object[,] { {"Item Id :",DEFAULT_ITEM_ID,"textfield"}});
		inputValuesDictionary.Add("oFXAddToMetadata", new object[,] { {"Item Id :",DEFAULT_ITEM_ID,"textfield"}, {"Slot :",DEFAULT_SLOT,"textfield"}});
		inputValuesDictionary.Add("oFXExistsInMetadata", new object[,] { {"Slot :",DEFAULT_SLOT,"textfield"}});
		inputValuesDictionary.Add("oFXRemoveFromMetadata", new object[,] { {"Slot :",DEFAULT_SLOT,"textfield"}});
		inputValuesDictionary.Add("oFXLoadPayloadForItem", new object[,] { {"Item Id :",DEFAULT_ITEM_ID,"textfield"}});
		inputValuesDictionary.Add("oFXPayloadStatusForItem", new object[,] { {"Item Id :",DEFAULT_ITEM_ID,"textfield"}});
		inputValuesDictionary.Add("oFXPayloadProgressForItem", new object[,] { {"Item Id :",DEFAULT_ITEM_ID,"textfield"}});
		inputValuesDictionary.Add("oFXModifyCurrency", new object[,] { {"Currency Id :",DEFAULT_CURRENCY_ID,"textfield"}, {"Amount :",DEFAULT_AMOUNT,"textfield"}});
		inputValuesDictionary.Add("oFXModifyGameItem", new object[,] { {"Item Id :",DEFAULT_ITEM_ID,"textfield"}, {"Quantity :",DEFAULT_QUANTITY,"textfield"}});
		inputValuesDictionary.Add("oFXCanHaveMore", new object[,] { {"Item Id :",DEFAULT_ITEM_ID,"textfield"}});
		inputValuesDictionary.Add("oFXInAppPurchasesForCategory", new object[,] { {"Category :",DEFAULT_CATEGORY,"textfield"}});
		inputValuesDictionary.Add("oFXCurrencyLoadIcon", new object[,] { {"Currency Id :",DEFAULT_CURRENCY_ID,"textfield"}});
		inputValuesDictionary.Add("oFXCurrencyProperties", new object[,] { {"Currency Id :",DEFAULT_CURRENCY_ID,"textfield"}});
		inputValuesDictionary.Add("oFXPurchase", new object[,] { {"Item Id :",DEFAULT_ITEM_ID,"textfield"}});
		inputValuesDictionary.Add("oFXPurchaseIsPurchasable", new object[,] { {"Item Id :",DEFAULT_ITEM_ID,"textfield"}});
		inputValuesDictionary.Add("oFXPurchaseFormattedPrice", new object[,] { {"Item Id :",DEFAULT_ITEM_ID,"textfield"}});
		inputValuesDictionary.Add("oFXPurchaseIsFree", new object[,] { {"Item Id :",DEFAULT_ITEM_ID,"textfield"}});
		inputValuesDictionary.Add("oFXPurchaseLoadIcon", new object[,] { {"Item Id :",DEFAULT_ITEM_ID,"textfield"}});
		inputValuesDictionary.Add("oFXPurchaseLoadScreenshot", new object[,] { {"Item Id :",DEFAULT_ITEM_ID,"textfield"}});
		inputValuesDictionary.Add("oFXPurchaseLoadStorePayload", new object[,] { {"Item Id :",DEFAULT_ITEM_ID,"textfield"}});
		inputValuesDictionary.Add("oFXPurchaseProperties", new object[,] { {"Item Id :",DEFAULT_ITEM_ID,"textfield"}});
	}
	
	Rect menuRect = new Rect(5.0f, backButtonSquareDimension + titleBarHeight + groupHeightSeparator, menuWindowWidth, 350.0f);
	Rect testRect = testWindowViewRect;
	Rect backButtonRect = new Rect (0, titleBarHeight + 5.0f, backButtonSquareDimension, backButtonSquareDimension);
	
    //Draws GUI elements  
    public bool DrawGUI()  
    {
        GUI.matrix = Matrix4x4.TRS(positionVector, Quaternion.identity, screenScale);  
		DrawTitleBar("Unity Bridge Test App", null, "OFX ver. " + versionString);
		
		if (GUI.Button(backButtonRect, "Back")) {
			return true;
		}
		
		menuRect = GUI.Window(0, menuRect, DrawMenu, "Menu");
		
		testRect = GUI.Window(1, testRect, DrawScrollView, viewTopicTitle);

		return false;
	}
	
	
	
	private void DrawMenu(int windowID) {
	
		if (GUI.Button(menuButtonRect0, "Currency")) {
			scrollPos = Vector2.zero;
			viewTopic = eOFXTopic.Currency;
		}

		if (GUI.Button(menuButtonRect1, "Items")) {
			scrollPos = Vector2.zero;
			viewTopic = eOFXTopic.Item;
		}
		
		if (GUI.Button(menuButtonRect2, "Inventory")) {
			scrollPos = Vector2.zero;
			viewTopic = eOFXTopic.Inventory;
		}


		if (GUI.Button(menuButtonRect3, "Catalog")) {
			scrollPos = Vector2.zero;
			viewTopic = eOFXTopic.Catalog;
		}


		if (GUI.Button(menuButtonRect4, "Payload")) {
			scrollPos = Vector2.zero;
			viewTopic = eOFXTopic.Payload;
		}


		if (GUI.Button(menuButtonRect5, "Purchase")) {
			scrollPos = Vector2.zero;
			viewTopic = eOFXTopic.InAppPurchase;
		}
	}
		
	private void DrawScrollView(int windowID) {
		switch (viewTopic) {
			case eOFXTopic.Currency:
				viewTopicTitle = "Currency";
				scrollPos = scrollView(scrollPos, 6, scrollViewSize);
				scrollViewSize = DrawCurrencyGUI();				
				break;
			case eOFXTopic.Item:
				viewTopicTitle = "Items";
				scrollPos = scrollView(scrollPos, 5, scrollViewSize);
				scrollViewSize = DrawItemGUI();
				break;
			case eOFXTopic.Inventory:
				viewTopicTitle = "Inventory";
				scrollPos = scrollView(scrollPos, 4, scrollViewSize);
				scrollViewSize = DrawInventoryGUI();
				break;
			case eOFXTopic.Payload:
				viewTopicTitle = "Payload";
				scrollPos = scrollView(scrollPos, 5, scrollViewSize);
				scrollViewSize = DrawPayloadGUI();
				break;
			case eOFXTopic.Catalog:
				viewTopicTitle = "Catalog";
				scrollPos = scrollView(scrollPos, 3, scrollViewSize);
				scrollViewSize = DrawCatalogGUI();
				break;
			case eOFXTopic.InAppPurchase:
				viewTopicTitle = "Purchase";
				scrollPos = scrollView(scrollPos, 10, scrollViewSize);
				scrollViewSize = DrawInAppPurchaseGUI();
				break;
		}

		// End scroll view
		GUI.EndScrollView();
		
	}
	
	
	private float DrawCurrencyGUI() {
		// Currency
		float groupStartY = 0f + groupHeightSeparator;
		
		groupStartY += drawGroupWithInput("Currency for Currency Id", "Get Amount", groupStartY, oFXAmountOfCurrencyExec, inputValuesDictionary["oFXAmountOfCurrencyId"]);
		groupStartY += drawGroupWithInput("Modify Currency", "Modify Currency", groupStartY, oFXModifyCurrencyExec, inputValuesDictionary["oFXModifyCurrency"]);
		groupStartY += drawGroupWithInput("Load Currency Icon", "Load Currency Icon", groupStartY, oFXCurrencyLoadIconExec, inputValuesDictionary["oFXCurrencyLoadIcon"]);
		groupStartY += drawGroup("Currencies in Inventory", "Get List", groupStartY, oFXCurrenciesInInventoryExec);
		groupStartY += drawGroup("Currency Ids", "Get List", groupStartY, oFXCurrenciesExec);
		groupStartY += drawGroupWithInput("Currency Properties", "Get Properties", groupStartY, oFXCurrencyPropertiesExec, inputValuesDictionary["oFXCurrencyProperties"]);
		return groupStartY;
	}
	
	private float DrawItemGUI() {
		// Items
		float groupStartY = 0f + groupHeightSeparator;
		
		groupStartY += drawGroupWithInput("Items for Item Id", "Get Number of Items", groupStartY, oFXNumberOfItemExec, inputValuesDictionary["oFXNumberOfItemId"]);
		groupStartY += drawGroupWithInput("Add Metadata to Item", "Connect Item and Slot", groupStartY, oFXAddToMetadataExec, inputValuesDictionary["oFXAddToMetadata"]);
		groupStartY += drawGroupWithInput("Exists in Metadata", "Get Item for Slot", groupStartY, oFXExistsInMetadataExec, inputValuesDictionary["oFXExistsInMetadata"]);
		groupStartY += drawGroupWithInput("Remove from Metadata", "Remove from Slot", groupStartY, oFXRemoveFromMetadataExec, inputValuesDictionary["oFXRemoveFromMetadata"]);
		groupStartY += drawGroupWithInput("Item Quantity", "Change Item", groupStartY, oFXModifyGameItemExec, inputValuesDictionary["oFXModifyGameItem"]);
		return groupStartY;		
	}
	
	private float DrawInventoryGUI() {
		
		float groupStartY = 0f + groupHeightSeparator;
		
		// Inventory
		groupStartY += drawGroupWithInput("Can Inventory Hold More?", "Submit Item", groupStartY, oFXCanHaveMoreExec, inputValuesDictionary["oFXCanHaveMore"]);
		groupStartY += drawGroup("Item Inventory", "Get List", groupStartY, oFXItemsInInventoryExec);	
		groupStartY += drawGroup("Store Inventory", "Get Store Inventory", groupStartY, oFXStoreInventoryExec);
		groupStartY += drawGroup("Synchronize Inventory", "Sync Inventory", groupStartY, oFXSynchronizeInventoryExec);
		return groupStartY;		
	}

	private float DrawPayloadGUI() {
		// Payload
		float groupStartY = 0f + groupHeightSeparator;
		
		groupStartY += drawGroup("Outdated Payload", "Get Items", groupStartY, oFXItemsWithAnOutdatedPayloadExec);
		groupStartY += drawGroupWithInput("Load Payload for Item", "Load Payload", groupStartY, oFXLoadPayloadForItemExec, inputValuesDictionary["oFXLoadPayloadForItem"]);
		groupStartY += drawGroupWithInput("Payload Status for Item", "Get Status", groupStartY, oFXPayloadStatusForItemExec, inputValuesDictionary["oFXPayloadStatusForItem"]);
		groupStartY += drawGroupWithInput("Payload progress for item", "Get Payload Progress", groupStartY, oFXPayloadProgressForItemExec, inputValuesDictionary["oFXPayloadProgressForItem"]);
		groupStartY += drawGroup("Deliverable Payload IDs", "Get List", groupStartY, oFXDeliverablesExec);
		return groupStartY;		
	}


	private float DrawCatalogGUI() {
		// Catalog
		float groupStartY = 0f + groupHeightSeparator;
		
		groupStartY += drawGroup("Updated Catalog", "Download Catalog", groupStartY, oFXUpdateCatalogFromServerExec);
		groupStartY += drawGroupWithInput("Purchases for Category", "Get List", groupStartY, oFXInAppPurchasesForCategoryExec, inputValuesDictionary["oFXInAppPurchasesForCategory"]);
		groupStartY += drawGroup("Purchasable Categories", "Get List", groupStartY, oFXCategoriesExec);
		return groupStartY;		
	}


	private float DrawInAppPurchaseGUI() {
		// in-app purchase
		float groupStartY = 0f + groupHeightSeparator;
		
		groupStartY += drawGroup("Purchases", "Get Purchases", groupStartY, oFXInAppPurchasesExec);
		groupStartY += drawGroup("Purchases Added", "Get List", groupStartY, oFXInAppPurchasesAddedInLastUpdateExec);
		groupStartY += drawGroupWithInput("Initiate Purchase", "Submit Item Id", groupStartY, oFXPurchaseExec, inputValuesDictionary["oFXPurchase"]);
		groupStartY += drawGroupWithInput("Item Is Purchasable?", "Submit Item Id", groupStartY, oFXPurchaseIsPurchasableExec, inputValuesDictionary["oFXPurchaseIsPurchasable"]);
		groupStartY += drawGroupWithInput("Item Price", "Get Formatted Price", groupStartY, oFXPurchaseFormattedPriceExec, inputValuesDictionary["oFXPurchaseFormattedPrice"]);
		groupStartY += drawGroupWithInput("Is Item a free download?", "Submit Item Id", groupStartY, oFXPurchaseIsFreeExec, inputValuesDictionary["oFXPurchaseIsFree"]);
		groupStartY += drawGroupWithInput("Start Loading Icon", "Submite Item Id", groupStartY, oFXPurchaseLoadIconExec, inputValuesDictionary["oFXPurchaseLoadIcon"]);
		groupStartY += drawGroupWithInput("Load Screenshot", "Submit Item Id", groupStartY, oFXPurchaseLoadScreenshotExec, inputValuesDictionary["oFXPurchaseLoadScreenshot"]);
		groupStartY += drawGroupWithInput("Load Store Payload", "Submit Item Id", groupStartY, oFXPurchaseLoadStorePayloadExec, inputValuesDictionary["oFXPurchaseLoadStorePayload"]);
		groupStartY += drawGroupWithInput("Load Purchase Properties", "Submit Item Id", groupStartY, oFXPurchasePropertiesExec, inputValuesDictionary["oFXPurchaseProperties"]);
		return groupStartY;		
	}


	private void CallbackPurchaseSucceeded(string itemId) {
		OpenFeint.InGameNotification("Purchase of item " + itemId + " succeeded.");
	}
	
	private void CallbackPurchaseFailed(string itemId, int status) {
		OpenFeint.InGameNotification("Purchase of item " + itemId + " failed with status: " + System.Convert.ToString(status));
	}

	private void CallbackPurchasePayloadStarted(string message) {
		OpenFeint.InGameNotification("Purchase payload started with message: " + message);
	}


	private void CallbackPurchasePayloadProgress(string itemId, double progress) {
		OpenFeint.InGameNotification("Check the debug log for purchase progress of item " + itemId);
		Debug.Log("Purchase progress for item " + itemId + ": " + System.Convert.ToString(progress));
	}
	
	private void CallbackPurchasePayloadLoaded(string itemId, bool result) {
		if (result)
			OpenFeint.InGameNotification("Purchase payload of item " + itemId + " succeeded.");
		else
			OpenFeint.InGameNotification("Purchase payload of item " + itemId + " failed.");		
	}
	
	private void CallbackPurchasePayloadSucceeded(string message) {
		OpenFeint.InGameNotification("Purchase payload succeeded.");
	}
	
	private void CallbackPurchaseCatalogUpdated() {
		OpenFeint.InGameNotification("Purchase catalog updated.");
	}
	
	private void CallbackPurchaseScreenshotLoaded(string itemId, string filePath) {
		OpenFeint.InGameNotification("Purchase screenshot loaded for item " + itemId + " at path: " + filePath);
	}
	
	private void CallbackPurchaseIconLoaded(string itemId, string filePath) {
		OpenFeint.InGameNotification("Purchase icon loaded for item " + itemId + " at path: " + filePath);
	}
	
	private void CallbackCurrencyIconLoaded(string itemId, string filePath) {
		OpenFeint.InGameNotification("Currency icon loaded for item " + itemId + " at path: " + filePath);
	}
	
	private void CallbackInventorySwitched() {
		OpenFeint.InGameNotification("Inventory switched.");
	}
	
	private void CallbackInventorySynchronized(int status) {
		OpenFeint.InGameNotification("Inventory synchronized with status: " + System.Convert.ToString(status));
	}
	
	private void CallbackPayloadStartedLoading(string message) {
		OpenFeint.InGameNotification("Payload started loading with message: " + message);
	}	
	
	private void CallbackPayloadUpdatedProgress(string itemId, double progress) {
		OpenFeint.InGameNotification("Check the debug log for payload updated progress of item " + itemId);
		Debug.Log("Payload updated progress for item " + itemId + ": " + System.Convert.ToString(progress));
	}
	
	private void CallbackPayloadFinishedLoading(string itemId, bool result) {
		if (result)
			OpenFeint.InGameNotification("Payload finished loading of item " + itemId + " succeeded.");
		else
			OpenFeint.InGameNotification("Payload finished loading of item " + itemId + " failed.");		
	}
	
	private void CallbackPayloadUpdatesRequired(string[] itemIds) {
		ArrayList itemIdList = new ArrayList(itemIds);
		OpenFeint.InGameNotification("Payload updates required on " + System.Convert.ToString(itemIdList.Count) + " items.");
	}
	
	private void CallbackStoreKitNonconsumableRestoreFinished() {
		OpenFeint.InGameNotification("StoreKit non-consumable restore finished.");
	}

	private void InitializeWithConfiguration(bool initSuccess) {
		Debug.Log("Properties Loaded");
		versionString = OpenFeintX.OFXVersion;		
	}

	/// <summary>
	/// Access the amount of a specific currency in the user's inventory
	/// </summary>
	private class OFXAmountOfCurrencyExecutor : IInputExecutor {
		
		public void execute(object[] input) {
			int result = OpenFeintX.OFXAmountOfCurrency((string) input[0]);
			OpenFeint.InGameNotification("Amount of currency: " + System.Convert.ToString(result));
		}
	}

	/// <summary>
	/// Access the quantity possessed of a specific item in the user's inventory.
	/// </summary>
	private class OFXNumberOfItemExecutor : IInputExecutor {
		
		public void execute(object[] input) {
			int result = OpenFeintX.OFXNumberOfItem((string) input[0]);
			OpenFeint.InGameNotification("Number of item: " + System.Convert.ToString(result));
		}
	}
	
	/// <summary>
	/// Stores arbitrary information associated with a player's use of the the items in their
	/// inventory. This information is stored alongside the inventory on the OpenFeint servers.
	/// For example, this could be used to track which items a player has equipped:
	/// - Mark that item identifier "wooden_bow" was equipped in "main_hand" slot:
	/// OFXAddToMetadata("wooden_bow", "main_hand");
	/// </summary>	
	private class OFXAddToMetadataExecutor : IInputExecutor {
		
		public void execute(object[] input) {
			OpenFeintX.OFXAddToMetadata((string) input[0], (string) input[1]);
		}
	}
	
	/// <summary>
	/// Restores the arbitrary information associated with a player's use of the the items in their
	/// inventory. This information was stored alongside the inventory on the OpenFeint servers.
	/// For example, to figure out what item identifier is equipped in the "head" slot:
	/// string equipped = OFXEsistsInMetadata("head");
	/// </summary>	
	private class OFXExistsInMetadataExecutor : IInputExecutor {
		
		public void execute(object[] input) {
			string result = OpenFeintX.OFXExistsInMetadata((string) input[0]);
			OpenFeint.InGameNotification("Item on slot: " + result);
		}
	}
	
	/// <summary>
	/// Removes the arbitrary information associated with a player's use of the the items in their
	/// inventory.
	/// </summary>	
	private class OFXRemoveFromMetadataExecutor : IInputExecutor {
		
		public void execute(object[] input) {
			OpenFeintX.OFXRemoveFromMetadata((string) input[0]);
		}
	}

	/// <summary>
	/// Returns the set of item identifiers which need to have their payloads downloaded
	/// </summary>
	private class OFXItemsWithAnOutdatedPayloadExecutor : IExecutor {
	
		public void execute() {
			ArrayList resultArrayList = OpenFeintX.OFXItemsWithAnOutdatedPayload();
			OpenFeint.InGameNotification(System.Convert.ToString(resultArrayList.Count) + " items with outdated payload.");
		}
	}

	/// <summary>
	/// Begin the download of the payload associated with the given item.
	/// </summary>
	private class OFXLoadPayloadForItemExecutor : IInputExecutor {
	
		public void execute(object[] input) {
			bool result = OpenFeintX.OFXLoadPayloadForItem((string) input[0]);
			if (result)
				OpenFeint.InGameNotification("Payload loaded.");
			else
				OpenFeint.InGameNotification("Payload not yet loaded.");
		}
	}


	/// <summary>
	/// Access the quantity possessed of a specific item in the user's inventory.
	/// </summary>
	private class OFXPayloadStatusForItemExecutor : IInputExecutor {
		
		public void execute(object[] input) {
			int result = (int)OpenFeintX.OFXPayloadStatusForItem((string) input[0]);
			string resultString;
			switch ((Gree.Unity.CallbackFetcherOpenFeintX.eItemPayloadStatus)result) {
				case Gree.Unity.CallbackFetcherOpenFeintX.eItemPayloadStatus.ItemPayloadStatus_None:
					resultString = "No Downloadable Payload";
					break;
				case Gree.Unity.CallbackFetcherOpenFeintX.eItemPayloadStatus.ItemPayloadStatus_NotLoaded:
					resultString = "Not Downloaded";
					break;
				case Gree.Unity.CallbackFetcherOpenFeintX.eItemPayloadStatus.ItemPayloadStatus_Loading:
					resultString = "Download in Progress";
					break;
				case Gree.Unity.CallbackFetcherOpenFeintX.eItemPayloadStatus.ItemPayloadStatus_Loaded:
					resultString = "Loaded";
					break;
				default:
					resultString = "status error";
					break;
			}
			OpenFeint.InGameNotification("Payload status : " + resultString);
		}
	}

	/// <summary>
	/// Returns the payload download progress for a given item.
	/// </summary>
	private class OFXPayloadProgressForItemExecutor : IInputExecutor {
		
		public void execute(object[] input) {
			double result = OpenFeintX.OFXPayloadProgressForItem((string) input[0]);
			OpenFeint.InGameNotification("Payload download progress : " + System.Convert.ToString(result));
		}
	}

	/// <summary>
	/// Writes the current inventory and unsent transaction logs to disk.
	/// Calling this insures that the data will not be lost in the case of the application
	/// stopping unexpectedly.
	/// </summary>
	private class OFXStoreInventoryExecutor : IExecutor {
		
		public void execute() {
			OpenFeintX.OFXStoreInventory();
			OpenFeint.InGameNotification("Inventory written to disk");
		}
	}

	/// <summary>
	/// Writes the current inventory to the server for permanent storage. Since this is an
	/// HTTP request, it is not recommended during gameplay.
	/// </summary>
	private class OFXSynchronizeInventoryExecutor : IExecutor {
		
		public void execute() {
			OpenFeintX.OFXSynchronizeInventory();
			OpenFeint.InGameNotification("Inventory synchronized");
		}
	}

	/// <summary>
	/// Records a change in currency.	/// </summary>
	private class OFXModifyCurrencyExecutor : IInputExecutor {
		
		public void execute(object[] input) {
			OpenFeintX.OFXModifyCurrency((string) input[0], System.Convert.ToInt32(input[1]));
		}
		
	}

	/// <summary>
	/// Records a change in item counts.
	/// </summary>
	private class OFXModifyGameItemExecutor : IInputExecutor {
		
		public void execute(object[] input) {
			OpenFeintX.OFXModifyGameItem((string) input[0], System.Convert.ToInt32(input[1]));
		}
		
	}

	/// <summary>
	/// Determines if the inventory can hold any more of the given item.
	/// </summary>
	private class OFXCanHaveMoreExecutor : IInputExecutor {
	
		public void execute(object[] input) {
			bool result = OpenFeintX.OFXCanHaveMore((string) input[0]);
			if (result)
				OpenFeint.InGameNotification("Inventory can hold more of item:" + (string) input[0]);
			else
				OpenFeint.InGameNotification("Inventory cannot hold more of item:" + (string) input[0]);
		}
	}


	/// <summary>
	/// Downloads any updated catalog information from the OpenFeint servers.
	/// This is automatically called during application initialization.
	/// </summary>
	private class OFXUpdateCatalogFromServerExecutor : IExecutor {
		
		public void execute() {
			OpenFeintX.OFXUpdateCatalogFromServer();
		}
	}

	/// <summary>
	/// Retrieve a list of in app purchases by category
	/// </summary>
	private class OFXInAppPurchasesForCategoryExecutor : IInputExecutor {
	
		public void execute(object[] input) {
			ArrayList resultArrayList = OpenFeintX.OFXInAppPurchasesForCategory((string) input[0]);
			OpenFeint.InGameNotification("In-App purchases for " + System.Convert.ToString(resultArrayList.Count) + " categories.");
		}
	}

	/// <summary>
	/// Request to load the icon image for this currency.
	/// </summary>
	private class OFXCurrencyLoadIconExecutor : IInputExecutor {
		
		public void execute(object[] input) {
			OpenFeintX.OFXCurrencyLoadIcon((string) input[0]);
		}
	}

	/// <summary>
	/// Returns a Dictionary<property, value> with all currency properties.
	/// Valid properties: identifier, name and iconUrl
	/// </summary>
	private class OFXCurrencyPropertiesExecutor : IInputExecutor {
	
		public void execute(object[] input) {
			Hashtable resultHashTable = OpenFeintX.OFXCurrencyProperties((string) input[0]);
			OpenFeint.InGameNotification("Currency properties:\n" + hashTableToString(resultHashTable));
		}
	}
	
	/// <summary>
	/// Initiate the purchase process.
	/// </summary>
	private class OFXPurchaseExecutor : IInputExecutor {
	
		public void execute(object[] input) {
			bool result = OpenFeintX.OFXPurchase((string) input[0]);
			if (result)
				OpenFeint.InGameNotification("Purchase initiated");
			else
				OpenFeint.InGameNotification("Purchase failed to initiatite.");
		}
	}

	// OFXPurchaseIsPurchasable //
	//
	// Determines if this item is purchasable.
	//
	// Returns:
	// true if the content purchase is possible, false otherwise.
	/// <summary>
	private class OFXPurchaseIsPurchasableExecutor : IInputExecutor {
	
		public void execute(object[] input) {
			bool result = OpenFeintX.OFXPurchaseIsPurchasable((string) input[0]);
			if (result)
				OpenFeint.InGameNotification("Item is purchasable");
			else
				OpenFeint.InGameNotification("Item is not purchaseable");
		}
	}

	/// <summary>
	/// A string containing the formatted price for this item. If this item is sold for hard
	/// currency ($) the string will be localized to the logged-in StoreKit account's locale.
	/// If this item is sold for virtual currency the string is simply a concatenation of
	/// purchaseCurrencyAmount and purchaseCurrency.name.
	/// </summary>
	private class OFXPurchaseFormattedPriceExecutor : IInputExecutor {
		
		public void execute(object[] input) {
			string result = OpenFeintX.OFXPurchaseFormattedPrice((string) input[0]);
			OpenFeint.InGameNotification("Formatted price: " + result);
		}
	}

	/// <summary>
	/// Returns true if the item is a free download. This will always return false for paid
	/// items even if the player already owns the item and can re-download it for free.
	/// </summary>
	private class OFXPurchaseIsFreeExecutor : IInputExecutor {
	
		public void execute(object[] input) {
			bool result = OpenFeintX.OFXPurchaseIsFree((string) input[0]);
			if (result)
				OpenFeint.InGameNotification("Item is free.");
			else
				OpenFeint.InGameNotification("Item is not free.");
		}
	}


	/// <summary>
	/// Request to load the icon image for this item.
	/// </summary>
	private class OFXPurchaseLoadIconExecutor : IInputExecutor {
	
		public void execute(object[] input) {
			bool result = OpenFeintX.OFXPurchaseLoadIcon((string) input[0]);
			if (result)
				OpenFeint.InGameNotification("Item found and Icon can be loaded.");
			else
				OpenFeint.InGameNotification("Item not found and Icon cannot be loaded.");
		}
	}


	/// <summary>
	/// Request to load the screenshot image for this item.
	/// </summary>
	private class OFXPurchaseLoadScreenshotExecutor : IInputExecutor {
	
		public void execute(object[] input) {
			bool result = OpenFeintX.OFXPurchaseLoadScreenshot((string) input[0]);
			if (result)
				OpenFeint.InGameNotification("Item found and screenshot can be loaded.");
			else
				OpenFeint.InGameNotification("Item not found and screenshot cannot be loaded.");
		}
	}


	/// <summary>
	/// Request to load the store payload for this item.
	/// </summary>
	private class OFXPurchaseLoadStorePayloadExecutor : IInputExecutor {
	
		public void execute(object[] input) {
			bool result = OpenFeintX.OFXPurchaseLoadStorePayload((string) input[0]);
			if (result)
				OpenFeint.InGameNotification("Item found and store payload can be loaded.");
			else
				OpenFeint.InGameNotification("Item not found and store payload cannot be loaded.");
		}
	}


	/// <summary>
	/// Returns a Dictionary<property, value> with all purchase properties.
	/// Valid properties: category, storeKitProduct, storeKitPurchaseStarted,
	/// cachedStoreKitPrice, purchaseCurrencyIdentifier, deliverableIdentifier,
	/// position
	/// </summary>
	private class OFXPurchasePropertiesExecutor : IInputExecutor {
	
		public void execute(object[] input) {
			Hashtable resultHashTable = OpenFeintX.OFXPurchaseProperties((string) input[0]);
			OpenFeint.InGameNotification("Purchase properties:\n" + hashTableToString(resultHashTable));
		}
	}
	
	/// <summary>
	/// Retrieve a list of all purchased items, unordered.
	/// </summary>
	private class OFXInAppPurchasesExecutor : IExecutor {
	
		public void execute() {
			ArrayList resultArrayList = OpenFeintX.OFXInAppPurchases;
			OpenFeint.InGameNotification(System.Convert.ToString(resultArrayList.Count) + " purchased items.");
		}
	}
	
	/// <summary>
	/// Access a flat list of all items in the inventory
	/// </summary>
	private class OFXItemsInInventoryExecutor : IExecutor {
	
		public void execute() {
			ArrayList resultArrayList = OpenFeintX.OFXItemsInInventory;
			OpenFeint.InGameNotification(System.Convert.ToString(resultArrayList.Count) + " items in inventory:\n");
		}
	}

	/// <summary>
	/// Access a flat list of all currency identifiers in the inventory
	/// </summary>
	private class OFXCurrenciesInInventoryExecutor : IExecutor {
	
		public void execute() {
			ArrayList resultArrayList = OpenFeintX.OFXCurrenciesInInventory;
			OpenFeint.InGameNotification(System.Convert.ToString(resultArrayList.Count) + " currency ids in inventory");
		}
	}

	/// <summary>
	/// Retrieve an unordered list of in app purchases which have been added to the store
	/// between the previous server catalog update and the latest server catalog update.
	/// </summary>
	private class OFXInAppPurchasesAddedInLastUpdateExecutor : IExecutor {
	
		public void execute() {
			ArrayList resultArrayList = OpenFeintX.OFXInAppPurchasesAddedInLastUpdate;
			OpenFeint.InGameNotification(System.Convert.ToString(resultArrayList.Count) + " purchases since last update.");
		}
	}


	/// <summary>
	/// Retrieve a list of all virtual currencies identifiers
	/// </summary>
	private class OFXCurrenciesExecutor : IExecutor {
	
		public void execute() {
			ArrayList resultArrayList = OpenFeintX.OFXCurrencies;
			OpenFeint.InGameNotification(System.Convert.ToString(resultArrayList.Count) + " virtual currency ids.");
		}
	}

	/// <summary>
	/// Retrive a list of all purchase categories
	/// </summary>
	private class OFXCategoriesExecutor : IExecutor {
	
		public void execute() {
			ArrayList resultArrayList = OpenFeintX.OFXCategories;
			OpenFeint.InGameNotification(System.Convert.ToString(resultArrayList.Count) + " purchasable categories.");
		}
	}
	
	/// <summary>
	/// Returns all deliverable payload identifiers
	/// </summary>
	private class OFXDeliverablesExecutor : IExecutor {
	
		public void execute() {
			ArrayList resultArrayList = OpenFeintX.OFXDeliverables;
			OpenFeint.InGameNotification(System.Convert.ToString(resultArrayList.Count) + " deliverable payload ids.");
		}
	}
	
}