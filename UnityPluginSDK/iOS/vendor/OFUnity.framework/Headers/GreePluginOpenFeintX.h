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

#import "GreePluginOpenFeint.h"
#import "GreePluginOpenFeintXDelegate.h"

#import <OFCurrency.h>
#import <OFInAppPurchase.h>
#import <OFInAppPurchaseCatalog.h>
#import <OFInventory.h>
#import <OFInventoryData.h>
#import <OFXStore.h>
#import <OFXStoreDelegate.h>
#import <OFXStoreSettings.h>


// OpenFeint class interface.
@interface GreePluginOpenFeintX : GreePluginOpenFeint <OFCurrencyDelegate, OFInAppPurchaseDelegate, OFXStoreDelegate> {
//@private
//    id<PluginOpenFeintXDelegate> _pluginDelegate;
}
@property (nonatomic, assign) id<GreePluginOpenFeintXDelegate> pluginOFXDelegate;


+ (GreePluginOpenFeintX *)sharedGreePluginOpenFeintX:(GreePluginOpenFeintX *(^)(Class allocClass))initBlock;



- (NSDictionary*)getOpenFeintSettings;

- (NSString *)oFXVersion;
- (void)oFXUserWillEnterStore;
- (void)oFXUserWillLeaveStore;
- (void)oFXRestoreStoreKitNonconsumablePurchases;
- (id)oFXItemsInInventory;
- (id)oFXCurrenciesInInventory;
- (NSInteger)oFXAmountOfCurrency:(NSString *)currencyIdentifier;
- (NSInteger)oFXNumberOfItem:(NSString *)itemIdentifier;
- (void)oFXAddToMetadata:(NSString *)itemIdentifier slot:(NSString *)slot;
- (id)oFXExistsInMetadata:(NSString *)slot;
- (void)oFXRemoveFromMetadata:(NSString *)slot;
- (id)oFXItemsWithAnOutdatedPayload;
- (void)oFXLoadPayloadForItem:(NSString *)itemIdentifier;
- (int)oFXPayloadStatusForItem:(NSString *)itemIdentifier;
- (float)oFXPayloadProgressForItem:(NSString *)itemIdentifier;
- (void)oFXStoreInventory;
- (void)oFXSynchronizeInventory;
- (void)oFXModifyCurrency:(NSString *)currencyId withAmount:(NSInteger)amount;
- (void)oFXModifyGameItem:(NSString *)itemIdentifier withQuantity:(NSInteger)quantity;
- (BOOL)oFXCanHaveMore:(NSString *)itemIdentifier;
- (void)oFXUpdateCatalogFromServer;
- (id)oFXInAppPurchasesAddedInLastUpdate;
- (id)oFXInAppPurchasesForCategory:(NSString *)category;
- (id)oFXCurrencies;
- (id)oFXCategories;
- (id)oFXInAppPurchases;
- (id)oFXDeliverables;
- (void)oFXCurrencyLoadIcon:(NSString *)currencyId;
- (id)oFXCurrencyProperties:(NSString *)currencyId;
- (BOOL)oFXPurchase:(NSString *)itemIdentifier;
- (BOOL)oFXPurchaseIsPurchasable:(NSString *)itemIdentifier;
- (id)oFXPurchaseFormattedPrice:(NSString *)itemIdentifier;
- (BOOL)oFXPurchaseIsFree:(NSString *)itemIdentifier;
- (BOOL)oFXPurchaseLoadIcon:(NSString *)itemIdentifier;
- (BOOL)oFXPurchaseLoadScreenshot:(NSString *)itemIdentifier;
- (BOOL)oFXPurchaseLoadStorePayload:(NSString *)itemIdentifier;
- (id)oFXPurchaseProperties:(NSString *)itemIdentifier;

@end
