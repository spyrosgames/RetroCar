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

//////////////////////////////////////////////////////////////////////////////////////////
/// OFInventoryData is the underlying storage for items and currency. You should only
/// ever use this object during inventory conflict resolution.
///
/// @see -(void)resolveConflictingLocalInventory:andRemoteInventory:
/// @see +(void)commitResolvedInventory:
//////////////////////////////////////////////////////////////////////////////////////////
@interface OFInventoryData : NSObject< NSCopying >
{
    @private
    NSMutableDictionary* items;
    NSMutableDictionary* wallet;
    NSMutableDictionary* metadata;
}

//////////////////////////////////////////////////////////////////////////////////////////
/// Designated initializer
//////////////////////////////////////////////////////////////////////////////////////////
- (id)initWithItems:(NSDictionary*)_items wallet:(NSDictionary*)_wallet metadata:(NSDictionary*)_metadata;

- (NSArray*)itemsInInventory;
- (NSArray*)currenciesInInventory;
- (NSInteger)amountOfCurrency:(NSString*)currencyIdentifier;
- (NSInteger)numberOfItem:(NSString*)itemIdentifier;
- (void)modifyCurrency:(NSString*)currencyIdentifier amount:(NSInteger)delta;
- (void)modifyGameItem:(NSString*)itemIdentifier amount:(NSInteger)delta;
- (NSMutableDictionary*)metadata;

//////////////////////////////////////////////////////////////////////////////////////////
/// Validates the contents of the metadata. That is, verifies all keys are NSString and
/// all values are NSString, NSNumber, or NSNull.
//////////////////////////////////////////////////////////////////////////////////////////
- (void)sanitizeMetadata;

//////////////////////////////////////////////////////////////////////////////////////////
/// Removes all items, wallet, metadata from this inventory.
//////////////////////////////////////////////////////////////////////////////////////////
- (void)clearAll;

//////////////////////////////////////////////////////////////////////////////////////////
/// @return @c YES if the inventory data has no metadata, no currency (or 0 of all
///         currencies), and no items (or 0 quantity for all items)
//////////////////////////////////////////////////////////////////////////////////////////
- (BOOL)isEmpty;

//////////////////////////////////////////////////////////////////////////////////////////
/// @return @c YES if object is an OFInventoryData with identical contents
//////////////////////////////////////////////////////////////////////////////////////////
- (BOOL)isEqual:(id)object;

//////////////////////////////////////////////////////////////////////////////////////////
/// Convenience method to combine items and wallet data from another inventory with
/// this inventory.
//////////////////////////////////////////////////////////////////////////////////////////
- (void)combineWithItemsAndWalletFrom:(OFInventoryData*)other;

//////////////////////////////////////////////////////////////////////////////////////////
/// Convenience method to combine metadata from another inventory with this inventory.
/// The default implementation does nothing but ask the OFXStoreManager delegate to
/// perform the merge.
//////////////////////////////////////////////////////////////////////////////////////////
- (void)combineWithMetadataFrom:(OFInventoryData*)other;

- (NSDictionary*)dictionaryRepresentation;

@end
