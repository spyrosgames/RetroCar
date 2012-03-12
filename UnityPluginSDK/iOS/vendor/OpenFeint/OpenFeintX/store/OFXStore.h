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

#import "OFXStore+Internal.h"

@interface OFXStore (Public) 

//////////////////////////////////////////////////////////////////////////////////////////
/// Tells OFXStore that the player is about to enter the store
/// This must be called before the player enters the store
/// This will cause certain UI screens to appear to keep the user form losing data
//////////////////////////////////////////////////////////////////////////////////////////
+ (void)userWillEnterStore;

//////////////////////////////////////////////////////////////////////////////////////////
/// Tells OFXStore that the player is about to leave the store. This must be called 
/// when the player leaves the store in order to serialize the inventory and attempt a
/// server synchronization.
//////////////////////////////////////////////////////////////////////////////////////////
+ (void)userWillLeaveStore;

//////////////////////////////////////////////////////////////////////////////////////////
/// Attempts to restore all OFInAppPurchase content associated with the logged-in iTunes 
/// Store account. This will only restore items configured as non-consumable, unique, and
/// paid for via StoreKit.
///
/// @note This method may ask the user to login with their iTunes Store account.
///
/// @note Upon completion of the restore the OFXStoreDelegate method
///		  - (void)storeKitNonconsumableRestoreFinished will be invoked.
//////////////////////////////////////////////////////////////////////////////////////////
+ (void)restoreStoreKitNonconsumablePurchases; 

//////////////////////////////////////////////////////////////////////////////////////////
/// Returns the root path to use for downloaded payloads.   Apple guidelines for iCloud 
/// suggest moving non user-generated data out of the Documents directory, so this may change
///
/// @note The delegate method moveDataToNonCloudStorage determines what this will return.
//////////////////////////////////////////////////////////////////////////////////////////
+ (NSString*)rootPathForPayloads;
@end
