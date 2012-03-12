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

#import "OFXStoreEnums.h"

@class OFInventoryData;

@protocol OFXStoreDelegate <NSObject>
@required
//////////////////////////////////////////////////////////////////////////////////////////
/// Invoked when the contents of the in app purchase catalog are modified.
///
/// @note Your catalog is not available until this method is invoked once
//////////////////////////////////////////////////////////////////////////////////////////
- (void)inAppPurchaseCatalogUpdated;

@optional
//////////////////////////////////////////////////////////////////////////////////////////
/// Invoked after any inventory synchronization, including calls to 
/// [OFInventory synchronizeInventory].
//////////////////////////////////////////////////////////////////////////////////////////
- (void)inventorySynchronized:(OFInventorySynchronizationStatus)status;

//////////////////////////////////////////////////////////////////////////////////////////
/// Invoked when the inventory is completely switched because the player changed 
/// OpenFeint account or logged in/out of OpenFeint
//////////////////////////////////////////////////////////////////////////////////////////
- (void)inventorySwitched;

//////////////////////////////////////////////////////////////////////////////////////////
/// Invoked during inventory synchronization when the user's inventory was updated
/// on another device. If we know the locally stored inventory is considered empty
/// (as defined by this method) then we can trivially accept the more recent remote
/// inventory.
///
/// @return @c YES if the inventory is considered empty
///
/// @note   If this method is not implemented an empty inventory is one that has
///         no items (or 0 for all items), no currencies (or 0 for all currencies), 
///         and no metadata
//////////////////////////////////////////////////////////////////////////////////////////
- (BOOL)isInventoryEmpty:(OFInventoryData*)inventory;

//////////////////////////////////////////////////////////////////////////////////////////
/// Invoked whenever there is a potential metadata conflict.
///
/// Your implementation should examine the metadata of the given inventory as well as
/// the potentially conflicting metadata and return a new metadata dictionary.
///
/// @param  inventory   Inventory data
/// @param  metadataTwo The potentially conflicting metadata dictionary
///
/// @return A new metadata dictionary for the inventory.
//////////////////////////////////////////////////////////////////////////////////////////
- (NSMutableDictionary*)mergeMetadataFromInventory:(OFInventoryData*)inventory withMetadata:(NSDictionary*)metadata;

//////////////////////////////////////////////////////////////////////////////////////////
/// Invoked during inventory synchronization when the user's inventory was updated
/// on another device. 
///
/// If you choose to implement this method you *MUST* invoke
///  [OFInventory commitResolvedInventory:]
/// in order to complete the synchronization process.
///
/// If you do not implement this method then the user will be asked via UIAlertView
/// to choose which inventory he wants to keep.
///
/// @param  local   Contents of the user's inventory local to this device
/// @param  remote  Contents of the user's inventory stored on OpenFeint servers
//////////////////////////////////////////////////////////////////////////////////////////
- (void)resolveConflictingLocalInventory:(OFInventoryData*)local andRemoteInventory:(OFInventoryData*)remote;

//////////////////////////////////////////////////////////////////////////////////////////
/// If you are converting from another inventory storage system to OpenFeint X you
/// should use this method as the place to transfer existing items and currency into
/// OpenFeint X.
///
/// The inventory is serialized to disk immediately following this method.
//////////////////////////////////////////////////////////////////////////////////////////
- (void)migrateLegacyInventoryData;

//////////////////////////////////////////////////////////////////////////////////////////
/// Invoked when the restoration of StoreKit nonconsumable content has been completed.
//////////////////////////////////////////////////////////////////////////////////////////
- (void)storeKitNonconsumableRestoreFinished;

//////////////////////////////////////////////////////////////////////////////////////////
/// Invoked when OFXStore determines that an item owned by the user requires an
/// additional payload download. This could be a new, undownloaded item; an update to
/// an existing item; etc.
///
/// @param  itemIdentifiers An array of item identifiers that need a payload download
///
/// @return If @c YES the system will begin downloading the payloads, if @c NO then your
///         application must decide when to download the items by invoking 
///         OFInventory's + (void)loadPayloadForItem: on each item in the array.
//////////////////////////////////////////////////////////////////////////////////////////
- (BOOL)payloadUpdatesRequired:(NSArray*)itemIdentifiers;

//////////////////////////////////////////////////////////////////////////////////////////
/// Invoked when the download for an item's payload begins
///
/// @param  itemIdentifier  The identifier for the game item that requires this payload
//////////////////////////////////////////////////////////////////////////////////////////
- (void)payloadForItemStartedLoading:(NSString*)itemIdentifier;

//////////////////////////////////////////////////////////////////////////////////////////
/// Invoked periodically as download progress updates for an item's payload
///
/// @param  itemIdentifier  The identifier for the game item that requires this payload
/// @param  progress        The progress (from 0.0 to 1.0) of the download
//////////////////////////////////////////////////////////////////////////////////////////
- (void)payloadForItem:(NSString*)itemIdentifier updatedProgress:(CGFloat)progress;

//////////////////////////////////////////////////////////////////////////////////////////
/// Invoked when the payload for a game item has been downloaded
///
/// @param  itemIdentifier  The identifier for the game item that requires this payload
/// @param  success         @c YES if the download was successful, @c NO if the download
///                         failed for whatever reason.
//////////////////////////////////////////////////////////////////////////////////////////
- (void)payloadForItem:(NSString*)itemIdentifier finishedLoading:(BOOL)success;

@end
