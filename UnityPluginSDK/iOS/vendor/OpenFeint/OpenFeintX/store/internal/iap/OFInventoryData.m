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

#import "OFInventoryData.h"
#import "OFInAppPurchaseCatalog.h"
#import "OFDeliverable.h"
#import "OFXDebug.h"
#import "NSDictionary+NumericValues.h"
#import "OFXStore+Private.h"

#pragma mark -
#pragma mark Internal additions
#pragma mark -

@interface OFInventoryData ()
@property (nonatomic, readonly) NSMutableDictionary* items;
@property (nonatomic, readonly) NSMutableDictionary* wallet;
@property (nonatomic, readonly) NSMutableDictionary* metadata;
@end

@implementation OFInventoryData

@synthesize items;
@synthesize wallet;
@synthesize metadata;

#pragma mark -
#pragma mark Life-cycle
#pragma mark -

- (id)init
{
    return [self initWithItems:nil wallet:nil metadata:nil];
}

- (id)initWithItems:(NSDictionary*)_items wallet:(NSDictionary*)_wallet metadata:(NSDictionary*)_metadata
{
    self = [super init];
    if (self != nil)
    {
        items = [_items mutableCopy];
        if (!items)
            items = [[NSMutableDictionary alloc] initWithCapacity:8];

        wallet = [_wallet mutableCopy];
        if (!wallet)
            wallet = [[NSMutableDictionary alloc] initWithCapacity:8];

        metadata = [_metadata mutableCopy];
        if (!metadata)
            metadata = [[NSMutableDictionary alloc] initWithCapacity:8];
    }
    
    return self;
}

- (void)dealloc
{
    [items release];
    [wallet release];
    [metadata release];
    [super dealloc];
}

#pragma mark -
#pragma mark NSCopying
#pragma mark -

- (id)copyWithZone:(NSZone*)zone
{
    return [[OFInventoryData alloc] initWithItems:items wallet:wallet metadata:metadata];
}

#pragma mark -
#pragma mark Interface methods
#pragma mark -

- (NSArray*)itemsInInventory
{
    return [items allKeys];
}

- (NSArray*)currenciesInInventory
{
    return [wallet allKeys];
}

- (NSInteger)amountOfCurrency:(NSString*)currencyIdentifier
{
    return [[wallet objectForKey:currencyIdentifier] intValue];
}

- (NSInteger)numberOfItem:(NSString*)itemIdentifier
{
    return [[items objectForKey:itemIdentifier] intValue];
}

- (void)modifyCurrency:(NSString*)currencyIdentifier amount:(NSInteger)delta
{
    NSInteger currentAmount = [[wallet objectForKey:currencyIdentifier] intValue];
    NSInteger newAmount = currentAmount + delta;
    newAmount = MAX(0, newAmount);
    
    if (newAmount > 0)
    {
        NSNumber* amount = [[NSNumber alloc] initWithInteger:newAmount];
        [wallet setObject:amount forKey:currencyIdentifier];
        [amount release];
    }
    else
    {
        [wallet removeObjectForKey:currencyIdentifier];
    }
}

- (void)modifyGameItem:(NSString*)itemIdentifier amount:(NSInteger)delta
{
    OFDeliverable* deliverable = [[OFInAppPurchaseCatalog deliverablesInternal] objectForKey:itemIdentifier];

    //this is a deliverable for a currency pack, which isn't an "item"
    if (deliverable.currencyIdentifier)
        return;

    NSInteger currentAmount = [[items objectForKey:itemIdentifier] intValue];
    NSInteger newAmount = currentAmount + delta;
    
    if (deliverable.unique && newAmount > 1)
        newAmount = 1;
    else if (newAmount < 0)
        newAmount = 0;
        
    if (newAmount)
    {
        NSNumber* amount = [[NSNumber alloc] initWithInteger:newAmount];
        [items setObject:amount forKey:itemIdentifier];
        [amount release];
    }
    else
    {
        [items removeObjectForKey:itemIdentifier];
    }
}

- (NSMutableDictionary*)metadata
{
    return metadata;
}

- (void)sanitizeMetadata
{
	NSArray* metadataKeys = [metadata allKeys];
	for (id key in metadataKeys)
	{
		if (![key isKindOfClass:[NSString class]])
		{
			OFXLog(@"Inventory", @"Encountered metadata key with invalid type! Removing key/value pair for key: %@", key);
			[metadata removeObjectForKey:key];
			continue;
		}
		
		id object = [metadata objectForKey:key];
		if (![object isKindOfClass:[NSString class]] &&
			![object isKindOfClass:[NSNumber class]] &&
			![object isKindOfClass:[NSNull class]])
		{
			OFXLog(@"Inventory", @"Encountered metadata value with invalid type! Removing key/value pair for key: %@", key);
			[metadata removeObjectForKey:key];
			continue;
		}
	}
}

- (void)clearAll
{
    [wallet removeAllObjects];
    [items removeAllObjects];
    [metadata removeAllObjects];
}

- (BOOL)isEmpty
{
    for (NSString* key in wallet)
    {
        if ([[wallet objectForKey:key] intValue] > 0)
            return NO;
    }
    
    for (NSString* key in items)
    {
        if ([[items objectForKey:key] intValue] > 0)
            return NO;
    }

    return [metadata count] == 0;
}

- (BOOL)isEqual:(id)object
{
    if (![object isKindOfClass:[OFInventoryData class]])
        return NO;
    
    OFInventoryData* other = object;
    return [wallet isEqualToDictionary:other.wallet] && 
           [items isEqualToDictionary:other.items] &&
           [metadata isEqualToDictionary:other.metadata];
}

- (void)combineWithItemsAndWalletFrom:(OFInventoryData*)other
{
    [wallet mergeIntegerDictionary:other.wallet];
    [items mergeIntegerDictionary:other.items];
    
    // uniquify
    NSArray* otherItemKeys = [other itemsInInventory];
    for (NSString* itemIdentifier in otherItemKeys)
    {
        if ([[[OFInAppPurchaseCatalog deliverablesInternal] objectForKey:itemIdentifier] unique])
            [items setObject:[NSNumber numberWithInt:1] forKey:itemIdentifier];
    }
    
    // disallow negative currency amounts
    for (NSString* currencyIdentifier in wallet)
    {
        NSNumber* amount = [wallet objectForKey:currencyIdentifier];
        if ([amount integerValue] < 0)
            [wallet setObject:[NSNumber numberWithInteger:0] forKey:currencyIdentifier];
    }
}

- (void)combineWithMetadataFrom:(OFInventoryData*)other
{
    if ([[OFXStore delegate] respondsToSelector:@selector(mergeMetadataFromInventory:withMetadata:)])
    {
        OFInventoryData* inventoryData = [self copy];
        NSDictionary* metadataTwo = [other.metadata copy];

        NSMutableDictionary* merged = [[OFXStore delegate] mergeMetadataFromInventory:inventoryData withMetadata:metadataTwo];
        [metadata release];
        metadata = [merged retain];

        [inventoryData release];
        [metadataTwo release];
    }
}

- (NSDictionary*)dictionaryRepresentation
{
    return [NSDictionary dictionaryWithObjectsAndKeys:
        items, @"items",
        wallet, @"wallet",
        metadata, @"metadata",
        nil];
}

@end
