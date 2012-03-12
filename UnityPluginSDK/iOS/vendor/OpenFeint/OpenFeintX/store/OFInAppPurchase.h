// 
//  Copyright 2010 Aurora Feint, Inc.
// 
//  Licensed under the Apache License, Version 2.0 (the "License");
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//  
//  	http://www.apache.org/licenses/LICENSE-2.0
//  	
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.
// 

#pragma once
#import "OFXStore.h"
#import "OpenFeint/OFResource.h"
#import "OFCurrency.h"
#import "OFStoreKit.h"
#import "OFContentDownloader.h"

@class SKProduct;
@class OFSkuCategory;
@protocol OFInAppPurchaseDelegate;

typedef enum
{
	OFInAppPurchase_PurchaseSucceeded,		/// Purchase completed successfully
	OFInAppPurchase_PurchaseFailed,			/// Purchase failed for some reason (network error, etc)
	OFInAppPurchase_PurchaseCancelled,		/// Purchase was cancelled by the user
	OFInAppPurchase_PurchasingDisabled,		/// Purchasing disabled on this device
	OFInAppPurchase_InsufficientFunds,		/// Not enough virtual currency to make the purchase
	OFInAppPurchase_ProductUnavailable,		/// Not enough product information is available to purchase
											/// (Apple product request outstanding/failed)
	OFInAppPurchase_AlreadyOwned,			/// Purchase cannot happen because this item is unique and already owned
	OFInAppPurchase_MustBeOnline,			/// Purchase failed because this item requires a payload download
											/// and there is no internet connection 
} OFInAppPurchaseStatus;


//////////////////////////////////////////////////////////////////////////////////////////
/// OFInAppPurchase is the primary interface for managing a virtual good (or DLC)'s
/// in-game store representation.
//////////////////////////////////////////////////////////////////////////////////////////
@interface OFInAppPurchase : OFResource< OFStoreKitPurchasable, OFContentDownloadDelegate >
{
	NSString* storeItemTitle;
	NSString* storeItemDescription;	
	NSInteger quantity;
	NSString* categoryIdentifier;
    OFSkuCategory* category;

	NSString* storeKitProductIdentifier;
	SKProduct* storeKitProduct;
	BOOL storeKitPurchaseStarted;
    BOOL storeKitNonConsumable;
	NSString* cachedStoreKitPrice;
	
	NSInteger purchaseCurrencyAmount;
    NSString* purchaseCurrencyIdentifier;
    OFCurrency* purchaseCurrency;

	NSString* deliverableIdentifier;
    NSUInteger position;

	NSURL* iconUrl;
	NSURL* screenshotUrl;
	NSURL* storePayloadUrl;
    
    NSString* startVersion;
    NSString* endVersion;
}

//////////////////////////////////////////////////////////////////////////////////////////
/// Set a delegate for all OFInAppPurchase related actions. Must adopt the 
/// OFInAppPurchaseDelegate protocol.
///
/// @note Defaults to nil. Weak reference
//////////////////////////////////////////////////////////////////////////////////////////
+ (void)setDelegate:(id<OFInAppPurchaseDelegate>)delegate;

@property (nonatomic, readonly, retain) NSString* storeItemTitle;
@property (nonatomic, readonly, retain) NSString* storeItemDescription;
@property (nonatomic, readonly, assign) NSInteger quantity;
@property (nonatomic, readonly, retain) NSString* categoryIdentifier;

@property (nonatomic, readonly, retain) NSString* storeKitProductIdentifier;
@property (nonatomic, readonly, assign) BOOL storeKitNonConsumable;

@property (nonatomic, readonly, assign) NSInteger purchaseCurrencyAmount;
@property (nonatomic, readonly, retain) OFCurrency* purchaseCurrency;

@property (nonatomic, readonly, retain) NSURL* iconUrl;
@property (nonatomic, readonly, retain) NSURL* screenshotUrl;
@property (nonatomic, readonly, retain) NSURL* storePayloadUrl;

@property (nonatomic, readonly, assign) OFItemPayloadStatus itemPayloadStatus;

//////////////////////////////////////////////////////////////////////////////////////////
/// Initiate the purchase process.
///
/// @note Invokes -(void)inAppPurchaseSucceeded: on success,
///				  -(void)inAppPurchase:purchaseFailedWithStatus: on failure.
//////////////////////////////////////////////////////////////////////////////////////////
- (void)purchase;

//////////////////////////////////////////////////////////////////////////////////////////
/// Determines if this item is purchasable. If it is not purchasable the reason will be
/// put into the given OFInAppPurchaseStatus parameter.
///
/// @param  reason  If provided this will give a more detailed reason why content is not
///                 purchasable.
///
/// @return @c YES if the content purchase is possible, @c NO otherwise.
//////////////////////////////////////////////////////////////////////////////////////////
- (BOOL)isPurchasable:(OFInAppPurchaseStatus*)reason;

//////////////////////////////////////////////////////////////////////////////////////////
/// @return An NSString containing the formatted price for this item. If this IAP is sold
///         for hard currency ($) the string will be localized to the logged-in StoreKit
///         account's locale. If this IAP is sold for virtual currency the string is
///         simply a concatenation of purchaseCurrencyAmount and purchaseCurrency.name.
//////////////////////////////////////////////////////////////////////////////////////////
- (NSString*)formattedPrice;

//////////////////////////////////////////////////////////////////////////////////////////
/// @return YES if the item is a free download. This will always return NO for paid items 
///			even if the player already owns the item and can re-download it for free.
//////////////////////////////////////////////////////////////////////////////////////////
- (BOOL)isFree;

//////////////////////////////////////////////////////////////////////////////////////////
/// @return The item identifier awarded when purchasing this IAP. If this is a virtual
///         currency pack then the itemIdentifier is the identifier of the awarded
///         currency.
//////////////////////////////////////////////////////////////////////////////////////////
- (NSString*)itemIdentifier;

//////////////////////////////////////////////////////////////////////////////////////////
/// Request to load the icon image for this in app purchase.
///
/// @note This method may incur a network request to download the data.
//////////////////////////////////////////////////////////////////////////////////////////
- (void)loadIcon;

//////////////////////////////////////////////////////////////////////////////////////////
/// Request to load the screenshot image for this in app purchase.
///
/// @note This method may incur a network request to download the data.
//////////////////////////////////////////////////////////////////////////////////////////
- (void)loadScreenshot;

//////////////////////////////////////////////////////////////////////////////////////////
/// Request to load the store payload for this in app purchase.
///
/// @note This method may incur a network request to download the data.
//////////////////////////////////////////////////////////////////////////////////////////
- (void)loadStorePayload;

- (NSInteger)sortByPosition:(OFInAppPurchase*)rhs;
@end

@protocol OFInAppPurchaseDelegate
@optional
//////////////////////////////////////////////////////////////////////////////////////////
/// Invoked when an in app purchase is purchased successfully.
///
/// @param iap      The in app purchase that was purchased.
//////////////////////////////////////////////////////////////////////////////////////////
- (void)inAppPurchaseSucceeded:(OFInAppPurchase*)iap;

//////////////////////////////////////////////////////////////////////////////////////////
/// Invoked when an in app purchase fails purchasing.
///
/// @param iap      The in app purchase that failed purchasing.
/// @param status	The purchase status providing more information on the failure
//////////////////////////////////////////////////////////////////////////////////////////
- (void)inAppPurchase:(OFInAppPurchase*)iap purchaseFailedWithStatus:(OFInAppPurchaseStatus)status;

//////////////////////////////////////////////////////////////////////////////////////////
/// Invoked when the icon for an in app purchase is loaded
///
/// @param iap          The in app purchase whose icon was loaded
/// @param iconImage    A UIImage* containing the icon image data
//////////////////////////////////////////////////////////////////////////////////////////
- (void)inAppPurchase:(OFInAppPurchase*)iap iconLoaded:(UIImage*)iconImage;

//////////////////////////////////////////////////////////////////////////////////////////
/// Invoked when the screenshot for an in app purchase is loaded
///
/// @param iap          The in app purchase whose screenshot was loaded
/// @param iconImage    A UIImage* containing the screenshot image data
//////////////////////////////////////////////////////////////////////////////////////////
- (void)inAppPurchase:(OFInAppPurchase*)iap screenshotLoaded:(UIImage*)screenshotImage;

//////////////////////////////////////////////////////////////////////////////////////////
/// Invoked when the store payload for an in app purchase begins downloading
///
/// @param iap          The in app purchase whose store payload is downloading
///
/// @note This method will not be invoked if the store payload has already been cached
///       and does not require any updates.
//////////////////////////////////////////////////////////////////////////////////////////
- (void)inAppPurchaseStorePayloadStartedLoading:(OFInAppPurchase*)iap;

//////////////////////////////////////////////////////////////////////////////////////////
/// Invoked periodically as download progress updates for an in app purchases store payload
///
/// @param iap          The in app purchase whose store payload is downloading
/// @param progress     The progress (from 0.0 to 1.0) of the download
///
/// @note This method will not be invoked if the store payload has already been cached
///       and does not require any updates.
//////////////////////////////////////////////////////////////////////////////////////////
- (void)inAppPurchase:(OFInAppPurchase*)iap storePayloadUpdatedProgress:(CGFloat)progress;

//////////////////////////////////////////////////////////////////////////////////////////
/// Invoked when the store payload for an in app purchase has been downloaded
///
/// @param iap          The in app purchase whose store payload finished downloading
/// @param success      @c YES if the download was successful, @c NO if the download
///                     failed for whatever reason.
//////////////////////////////////////////////////////////////////////////////////////////
- (void)inAppPurchase:(OFInAppPurchase*)iap storePayloadFinishedLoading:(BOOL)success;

@end
