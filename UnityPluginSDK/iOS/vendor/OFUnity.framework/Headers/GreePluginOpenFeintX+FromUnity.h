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

#import "GreePluginOpenFeintX.h"

@interface GreePluginOpenFeintX (FromUnity)

- (NSString *)bridge_oFXVersion:(NSMutableArray *)args;
- (void)bridge_oFXUserWillEnterStore:(NSMutableArray *)args;
- (void)bridge_oFXUserWillLeaveStore:(NSMutableArray *)args;
- (void)bridge_oFXRestoreStoreKitNonconsumablePurchases:(NSMutableArray *)args;
- (id)bridge_oFXItemsInInventory:(NSMutableArray *)args;
- (id)bridge_oFXCurrenciesInInventory:(NSMutableArray *)args;
- (id)bridge_oFXAmountOfCurrency:(NSMutableArray *)args;
- (id)bridge_oFXNumberOfItem:(NSMutableArray *)args;
- (void)bridge_oFXAddToMetadata:(NSMutableArray *)args;
- (id)bridge_oFXExistsInMetadata:(NSMutableArray *)args;
- (void)bridge_oFXRemoveFromMetadata:(NSMutableArray *)args;
- (id)bridge_oFXItemsWithAnOutdatedPayload:(NSMutableArray *)args;
- (void)bridge_oFXLoadPayloadForItem:(NSMutableArray *)args;
- (id)bridge_oFXPayloadStatusForItem:(NSMutableArray *)args;
- (id)bridge_oFXPayloadProgressForItem:(NSMutableArray *)args;
- (void)bridge_oFXStoreInventory:(NSMutableArray *)args;
- (void)bridge_oFXSynchronizeInventory:(NSMutableArray *)args;
- (void)bridge_oFXModifyCurrency:(NSMutableArray *)args;
- (void)bridge_oFXModifyGameItem:(NSMutableArray *)args;
- (id)bridge_oFXCanHaveMore:(NSMutableArray *)args;
- (void)bridge_oFXUpdateCatalogFromServer:(NSMutableArray *)args;
- (id)bridge_oFXInAppPurchasesAddedInLastUpdate:(NSMutableArray *)args;
- (id)bridge_oFXInAppPurchasesForCategory:(NSMutableArray *)args;
- (id)bridge_oFXCurrencies:(NSMutableArray *)args;
- (id)bridge_oFXCategories:(NSMutableArray *)args;
- (id)bridge_oFXInAppPurchases:(NSMutableArray *)args;
- (id)bridge_oFXDeliverables:(NSMutableArray *)args;
- (void)bridge_oFXCurrencyLoadIcon:(NSMutableArray *)args;
- (id)bridge_oFXCurrencyProperties:(NSMutableArray *)args;
- (id)bridge_oFXPurchase:(NSMutableArray *)args;
- (id)bridge_oFXPurchaseIsPurchasable:(NSMutableArray *)args;
- (id)bridge_oFXPurchaseFormattedPrice:(NSMutableArray *)args;
- (id)bridge_oFXPurchaseIsFree:(NSMutableArray *)args;
- (id)bridge_oFXPurchaseLoadIcon:(NSMutableArray *)args;
- (id)bridge_oFXPurchaseLoadScreenshot:(NSMutableArray *)args;
- (id)bridge_oFXPurchaseLoadStorePayload:(NSMutableArray *)args;
- (id)bridge_oFXPurchaseProperties:(NSMutableArray *)args;
@end
