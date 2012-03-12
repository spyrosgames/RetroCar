//  Copyright 2009-2010 Aurora Feint, Inc.
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

#pragma once

#import <StoreKit/StoreKit.h>
#import "OpenFeint/OFReachability.h"

@protocol OFStoreKitPurchasable
@required
- (NSString*)storeKitProductIdentifier;
- (SKProduct*)storeKitProduct;
- (void)receivedStoreKitProductInformation:(SKProduct*)product;
@end

extern NSString* const OFStoreKitPurchaseCompleted;				// Sent when a StoreKit purchase finishes. Purchase should be redeemed here.
extern NSString* const OFStoreKitPurchaseInvalidated;			// Send when a StoreKit purchase is found to be invalid. Purchase should be revoked here.
extern NSString* const OFStoreKitUserInfo_ProductIdentifierKey;	// NSString*, purchased productIdentifier
extern NSString* const OFStoreKitUserInfo_StatusKey;			// OFStoreKitPurchaseStatus, result of purchase attempt

// This notification is sent when a StoreKit restore finishes. Check OFStoreKitUserInfo_RestoreError
// to determine if the restorationw as successful or not.
extern NSString* const OFStoreKitRestoreFinished;
extern NSString* const OFStoreKitUserInfo_RestoreError;			// NSError*, valid only if the restoration failed

typedef enum
{
	OFStoreKitPurchaseStatus_Completed,
	OFStoreKitPurchaseStatus_Failed,
	OFStoreKitPurchaseStatus_Cancelled,
	OFStoreKitPurchaseStatus_Disabled,
	OFStoreKitPurchaseStatus_Invalidated,
} OFStoreKitPurchaseStatus;

@interface OFStoreKit : NSObject< SKPaymentTransactionObserver, SKProductsRequestDelegate, OFReachabilityObserver >
{
@private
	NSMutableArray* outstandingRequests;
	NSMutableSet* completedTransactions;
	NSMutableArray* validationQueue;
	NSMutableDictionary* productCatalog;
	BOOL restoreInProgress;
	BOOL validationInProgress;
}

// Provide an array of id<OFStoreKitPurchasable>
// Delegate method is -(void)storeKitProductInfoRequestFinished:(NSDictionary*)purchasables;
- (void)downloadProductInformation:(NSArray*)purchasables delegate:(id)delegate;

- (BOOL)canMakePurchases;
- (void)purchase:(id<OFStoreKitPurchasable>)purchasable;

- (void)restorePurchases;

@end
