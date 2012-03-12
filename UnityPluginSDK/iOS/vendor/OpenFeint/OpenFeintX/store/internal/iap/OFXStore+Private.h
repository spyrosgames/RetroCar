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

#import "OFInAppPurchase.h"
#import "OFInAppPurchaseCatalog.h"
#import "OFInventory.h"

//these might be useful to other parts of OFX, but should not be exposed as regular API

@interface OFInAppPurchaseCatalog (Private)
+(NSArray*)categoriesInternal;
+(NSDictionary*)deliverablesInternal;
@property (nonatomic, readonly, retain) NSArray* skus;
@property (nonatomic, readonly, retain) NSDictionary* currencies;
//@property (nonatomic, readonly, retain) NSDictionary* deliverables;
@property (nonatomic, readonly, retain) NSArray* categoriesInternal;
@property (nonatomic, readonly) BOOL serverLoaded;
+ (id)sharedCatalog;
+ (void)initializeCatalog;
+ (void)shutdownCatalog;
@end

@interface OFInventory (Private)
+ (void)applyInAppPurchase:(OFInAppPurchase*)purchase;
+(void) initializeInventory;
@end

@interface OFInAppPurchase (Private)
@property (nonatomic, readonly, retain) OFSkuCategory* category;
@property (nonatomic, retain) OFDeliverable* deliverable;
- (void)storeKitPurchaseCompleted:(OFStoreKitPurchaseStatus)status;
@property (nonatomic, assign) BOOL storeKitNonConsumable;
-(void)internalUpdateETags;
- (void)storeKitPurchaseCompleted:(OFStoreKitPurchaseStatus)status;
@property (nonatomic, retain) NSString* deliverableIdentifier;
@property (nonatomic, retain) NSString* purchaseCurrencyIdentifier;
@property (nonatomic, retain) NSString* startVersion;
@property (nonatomic, retain) NSString* endVersion;

@end

