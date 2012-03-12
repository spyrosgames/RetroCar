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
#import "OFInventoryData.h"

//////////////////////////////////////////////////////////////////////////////////////////
/// OFInventory is the interface for accessing items and currency possessed by the
/// current user.
//////////////////////////////////////////////////////////////////////////////////////////
@interface OFInventory : NSObject
{
    @private
    OFInventoryData* data;
    NSMutableDictionary* nonconsumableItems;  //these are not sent to the OF server and are controlled per device, not user

    NSMutableArray* pendingTransactions;
	NSMutableArray* pendingStoreKitTransactions;
	NSMutableDictionary* currencyDeltas;
	NSMutableDictionary* itemDeltas;

	BOOL syncIsInProgress;
    OFInventoryData* syncClientData;
    OFInventoryData* syncServerData;
	NSMutableArray* syncTransactions;
	NSMutableArray* syncStoreKitTransactions;
    NSInteger syncLockVersion;
	BOOL syncDidChangeDevices;
	BOOL syncHasCompletedOnce;
    BOOL syncInventoryIsEmpty;
    BOOL syncMetadataNeedsMerging;
	
	NSString* userId;
}

//////////////////////////////////////////////////////////////////////////////////////////
/// Access a flat list of all items in the inventory
///
/// @return NSArray* of NSString* item identifiers that are present in the inventory.
//////////////////////////////////////////////////////////////////////////////////////////
+ (NSArray*)itemsInInventory;

//////////////////////////////////////////////////////////////////////////////////////////
/// Access a flat list of all currency identifiers in the inventory
///
/// @return NSArray* of NSString* currency identifiers that are present in the inventory.
//////////////////////////////////////////////////////////////////////////////////////////
+ (NSArray*)currenciesInInventory;

//////////////////////////////////////////////////////////////////////////////////////////
/// Access the amount of a specific currency in the user's inventory
///
/// @param currencyIdentifier   The currency identifier to query the quantity of
///
/// @return Quantity of the given currency possessed by the user
//////////////////////////////////////////////////////////////////////////////////////////
+ (NSInteger)amountOfCurrency:(NSString*)currencyIdentifier;

//////////////////////////////////////////////////////////////////////////////////////////
/// Access the quantity possessed of a specific item in the user's inventory
/// 
/// @param itemIdentifier       The item identifier to query the quantity of
///
/// @return Quantity of the given item possessed by the user
//////////////////////////////////////////////////////////////////////////////////////////
+ (NSInteger)numberOfItem:(NSString*)itemIdentifier;

//////////////////////////////////////////////////////////////////////////////////////////
/// Access the metadata dictionary for the inventory. This dictionary can be used to
/// store arbitrary information associated with a player's use of the the items in their
/// inventory. This dictionary is stored alongside the inventory on the OpenFeint servers.
///
/// For example, this could be used to track which items a player has equipped:
///
/// Mark that item identifier "wooden_bow" was equipped in "main_hand" slot:
///  [[OFInventory metadata] setObject:@"wooden_bow" forKey:@"main_hand"];
///
/// Figure out what item identifier is equipped in the "head" slot:
///  NSString* equippedHat = [[OFInventory metadata] objectForKey:@"head"];
///
/// @note The keys *must* be NSString objects and the values *must* be one 
///		  of: NSString, NSNumber, NSNull.
///
/// @return Metadata dictionary
//////////////////////////////////////////////////////////////////////////////////////////
+ (NSMutableDictionary*)metadata;

//////////////////////////////////////////////////////////////////////////////////////////
/// Returns the set of item identifiers which need to have their payloads downloaded
///
/// @return NSSet of NSString item identifiers.
//////////////////////////////////////////////////////////////////////////////////////////
+ (NSArray*)itemsWithAnOutdatedPayload;

//////////////////////////////////////////////////////////////////////////////////////////
/// Begin the download of the payload associated with the given item. This should 
/// typically be invoked at some point after you return @c NO from 
/// -(BOOL)payloadUpdatesRequired in your OFXStoreDelegate.
///
/// @param itemIdentifier       The item identifier whose payload to download
//////////////////////////////////////////////////////////////////////////////////////////
+ (void)loadPayloadForItem:(NSString*)itemIdentifier;

//////////////////////////////////////////////////////////////////////////////////////////
/// Returns the payload download status for a given item.
///
/// @return OFItemPayloadStatus (defined in OpenFeintXEnums.h) for a game item identifier
//////////////////////////////////////////////////////////////////////////////////////////
+ (OFItemPayloadStatus)payloadStatusForItem:(NSString*)itemIdentifier;

//////////////////////////////////////////////////////////////////////////////////////////
/// Returns the payload download progress for a given item.
///
/// @return Current download progress for the given item identifier's payload download.
///
/// @note This method can only return a non-zero value if payloadStatusForItem is 
///	      OFItemPayloadStatus_Loading, signifying that a download is in progress.
//////////////////////////////////////////////////////////////////////////////////////////
+ (float)payloadProgressForItem:(NSString*)itemIdentifier;

//////////////////////////////////////////////////////////////////////////////////////////
/// Writes the current inventory and unsent transaction logs to disk.
///
/// Calling this insures that the data will not be lost in the case of the application 
/// stopping unexpectedly.
//////////////////////////////////////////////////////////////////////////////////////////
+ (void)storeInventory; 

//////////////////////////////////////////////////////////////////////////////////////////
/// Writes the current inventory to the server for permanent storage.
/// Since this is an HTTP request, it is not recommended during gameplay.
///
/// @note This method may result in a user-facing UIAlertView when there are any 
///       inventory modifications on the server or if the user had last updated on a 
///       different device.
///
/// @note Invokes -(void)inventorySynchronized: on the OFXStore delegate when complete.
//////////////////////////////////////////////////////////////////////////////////////////
+ (void)synchronizeInventory; 

//////////////////////////////////////////////////////////////////////////////////////////
/// Used to provide OpenFeintX with the inventory that should be stored remotely.
///
/// @note This method should only be invoked as a response to the OFXStoreManager 
///       delegate method -(void)resolveConflictingLocalInventory:andRemoteInventory:
//////////////////////////////////////////////////////////////////////////////////////////
+ (void)commitResolvedInventory:(OFInventoryData*)resolved;

//////////////////////////////////////////////////////////////////////////////////////////
/// Records a change in currency.
//////////////////////////////////////////////////////////////////////////////////////////
+ (void)modifyCurrency:(NSString*)currencyId amount:(NSInteger)delta;

//////////////////////////////////////////////////////////////////////////////////////////
/// Records a change in item counts.
//////////////////////////////////////////////////////////////////////////////////////////
+ (void)modifyGameItem:(NSString*)itemIdentifier quantity:(NSInteger)delta;

//////////////////////////////////////////////////////////////////////////////////////////
/// Determines if the inventory can hold any more of the given item.
///
/// @param itemIdentifier   Identifier of the item to check.
///
/// @return @c YES if more of the specified item can be acquired, @c NO otherwise.
//////////////////////////////////////////////////////////////////////////////////////////
+ (BOOL)canHaveMore:(NSString*)itemIdentifier;
@end
