//
// Copyright 2011 GREE International, Inc.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//    http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//

#import <Foundation/Foundation.h>
#import <OFInAppPurchase.h>
#import <OFInventory.h>

@protocol GreePluginOpenFeintXDelegate

// 
- (void)triggerInAppPurchaseSucceeded:(NSString *)itemIdentifier;
- (void)triggerInAppPurchaseFailed:(NSString *)itemIdentifier withStatus:(OFInAppPurchaseStatus)status;
- (void)triggerInAppPurchasePayloadStarted:(NSString *)itemIdentifier;
- (void)triggerInAppPurchase:(NSString *)itemIdentifier storePayloadUpdatedProgress:(float)progress;
- (void)triggerInAppPurchase:(NSString *)itemIdentifier storePayloadLoaded:(BOOL)success;
- (void)triggerInAppPurchasePayloadSucceeded:(NSString *)itemIdentifier;
- (void)triggerInAppPurchaseCatalogUpdated;
- (void)triggerInAppPurchaseScreenshotLoaded:(NSString *)itemIdentifier withFilePath:(NSString *)filePath;
- (void)triggerInAppPurchaseIconLoaded:(NSString *)itemIdentifier withFilePath:(NSString *)filePath;
- (void)triggerCurrencyIconLoaded:(NSString *)itemIdentifier withFilePath:(NSString *)filePath;
- (void)triggerInventorySwitched;
- (void)triggerInventorySynchronized:(OFInventorySynchronizationStatus)status;
- (void)triggerPayloadForItemStartedLoading:(NSString*)itemIdentifier;
- (void)triggerPayloadForItem:(NSString*)itemIdentifier finishedLoading:(BOOL)success;
- (void)triggerPayloadForItem:(NSString*)itemIdentifier updatedProgress:(CGFloat)progress;
- (BOOL)triggerPayloadUpdatesRequired:(NSArray*)itemIdentifiers;
- (void)triggerStoreKitNonconsumableRestoreFinished;

@end
