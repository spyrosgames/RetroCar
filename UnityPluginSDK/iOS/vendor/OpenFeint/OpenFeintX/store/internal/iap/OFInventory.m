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

#import "OFInventory.h"
#import "OpenFeintX.h"
#import "OpenFeint/OFResourceRequest.h"
#import "OpenFeint/OpenFeint+UserOptions.h"
#import "OpenFeint/OFJsonCoder.h"
#import "OFInAppPurchase.h"
#import "OpenFeint/OFEncryptedFile.h"
#import "OFDeliverable.h"
#import "OFInAppPurchaseCatalog.h"
#import "NSDictionary+NumericValues.h"
#import "OFXStore.h"
#import "OFXStore+Private.h"
#import "OFXDebug.h"
#import "OpenFeint/OFServerException.h"
#import "OpenFeint/OFUser.h"
#import "OFInventoryData.h"

static OFInventory* sMainInventory = nil;
static float sFakeInventoryDelay = 0.f;

@interface OFInventory()
@property (nonatomic, assign) BOOL syncIsInProgress;
@property (nonatomic, retain) OFInventoryData* syncClientData;
@property (nonatomic, retain) OFInventoryData* syncServerData;
@property (nonatomic, retain) NSMutableArray* syncTransactions;
@property (nonatomic, retain) NSMutableArray* syncStoreKitTransactions;
@property (nonatomic, assign) BOOL syncDidChangeDevices;
@property (nonatomic, assign) BOOL syncInventoryIsEmpty;
@property (nonatomic, assign) NSInteger syncLockVersion;
@property (nonatomic, assign) BOOL syncMetadataNeedsMerging;

@property (nonatomic, retain) NSMutableArray* pendingTransactions;
@property (nonatomic, retain) NSMutableArray* pendingStoreKitTransactions;
@property (nonatomic, retain) NSMutableDictionary* currencyDeltas;
@property (nonatomic, retain) NSMutableDictionary* itemDeltas;

@property (nonatomic, retain) OFInventoryData* data;
@property (nonatomic, retain) NSMutableDictionary* nonconsumableItems;

@property (nonatomic, retain) NSString* userId;
@property (nonatomic, retain) OFResourceRequest* currentRequest;

+ (OFInventory*)inventoryForUserId:(NSString*)userId;
- (void)convertDeltasToTransactions;
- (void)serializeToDisk;
- (void)deserializeFromDisk;
+(OFInventory*) instance;
-(void) writeNonConsumableToKeychain;
-(void) readNonConsumableFromKeychain;
- (void)signalUpdatesRequired:(NSArray*)deliverables;
- (void)synchronizeFinished:(OFInventorySynchronizationStatus)status;
- (void)lastUpdateResponse:(OFResourceRequest*)request;
- (void)getServerInventory;
- (void)inventoryGetResponse:(OFResourceRequest*)request;
- (void)putClientInventory;
- (void)inventoryPutResponse:(OFResourceRequest*)request;
- (void)checkStoreKitReceipts;
- (void)storeKitReceiptResponse:(OFResourceRequest*)request;
- (void)cancelCurrentRequests;
@end

@implementation OFInventory

@synthesize syncIsInProgress;
@synthesize syncClientData;
@synthesize syncServerData;
@synthesize syncTransactions;
@synthesize syncStoreKitTransactions;
@synthesize syncDidChangeDevices;
@synthesize syncInventoryIsEmpty;
@synthesize syncLockVersion;
@synthesize syncMetadataNeedsMerging;

@synthesize pendingTransactions;
@synthesize pendingStoreKitTransactions;
@synthesize currencyDeltas;
@synthesize itemDeltas;

@synthesize data;
@synthesize nonconsumableItems;

@synthesize userId;
@synthesize currentRequest;

+ (BOOL)canHaveMore:(NSString*)itemIdentifier
{
    OFDeliverable* deliverable = [[OFInAppPurchaseCatalog deliverablesInternal] objectForKey:itemIdentifier];
	if (!deliverable || !deliverable.unique)
        return YES;
		
	if (deliverable.unique && [self numberOfItem:deliverable.identifier] == 0)
		return YES;
		
	return NO;
}

+ (OFInventory*)instance
{
	OFAssert(sMainInventory, @"+(void)initializeInventory must be called before accessing the inventory!");
    return sMainInventory;
}

+ (OFInventory*)inventoryForUserId:(NSString*)userId
{
	OFInventory* inv = [[[OFInventory alloc] init] autorelease];
	inv.userId = userId;
	[inv deserializeFromDisk];
	return inv;
}

- (id)init
{
    self = [super init];
    if (self != nil)
    {
        self.pendingTransactions = [NSMutableArray arrayWithCapacity:10];
		self.pendingStoreKitTransactions = [NSMutableArray arrayWithCapacity:1];
		self.currencyDeltas = [NSMutableDictionary dictionaryWithCapacity:10];
		self.itemDeltas = [NSMutableDictionary dictionaryWithCapacity:10];
		
        data = [[OFInventoryData alloc] init];
		
        [self readNonConsumableFromKeychain];
    }
    return self;
}

-(void)dealloc
{
    self.pendingTransactions = nil;
	self.pendingStoreKitTransactions = nil;
	self.currencyDeltas = nil;
	self.itemDeltas = nil;

    [data release];
    self.nonconsumableItems = nil;

    [syncClientData release];
    [syncServerData release];
	self.syncTransactions = nil;
	self.syncStoreKitTransactions = nil;
	
	self.userId = nil;
    [self cancelCurrentRequests];
    [currentRequest release]; 
    [super dealloc];
}

#pragma mark -
#pragma mark Payload Management
#pragma mark -

- (void)signalUpdatesRequired:(NSArray*)deliverables
{
	if (![deliverables count])
		return;
		
    BOOL update = YES;
    if([[OFXStore delegate] respondsToSelector:@selector(payloadUpdatesRequired:)]) 
	{
		NSMutableArray* itemIdentifiers = [[NSMutableArray alloc] initWithCapacity:[deliverables count]];
		for (OFDeliverable* deliverable in deliverables)
			[itemIdentifiers addObject:[deliverable identifier]];

        update = [[OFXStore delegate] payloadUpdatesRequired:itemIdentifiers];

		[itemIdentifiers release];
	}

    if (update)
    {
        for (OFDeliverable* deliverable in deliverables)
        {
            [deliverable loadPayload];
        }
    }
}

+ (NSArray*)itemsWithAnOutdatedPayload
{
	NSMutableArray* itemsToUpdate = [NSMutableArray arrayWithCapacity:10];
	for (NSString* itemIdentifier in [OFInventory itemsInInventory])
	{
		OFDeliverable* deliverable = [[OFInAppPurchaseCatalog deliverablesInternal] objectForKey:itemIdentifier];
		if (deliverable.payloadDownloadRequired)
			[itemsToUpdate addObject:[deliverable identifier]];
	}
	
	return itemsToUpdate;
}

+ (void)loadPayloadForItem:(NSString*)itemIdentifier
{
    OFDeliverable* deliverable = [[OFInAppPurchaseCatalog deliverablesInternal] objectForKey:itemIdentifier];
    [deliverable loadPayload];
}

+ (OFItemPayloadStatus)payloadStatusForItem:(NSString*)itemIdentifier
{
    OFDeliverable* deliverable = [[OFInAppPurchaseCatalog deliverablesInternal] objectForKey:itemIdentifier];
    return deliverable.payloadStatus;
}

+ (float)payloadProgressForItem:(NSString*)itemIdentifier
{
	OFDeliverable* deliverable = [[OFInAppPurchaseCatalog deliverablesInternal] objectForKey:itemIdentifier];
	return [deliverable payloadProgress];
}

#pragma mark -
#pragma mark Serialization
#pragma mark -

- (void)convertDeltasToTransactions
{
	for (NSString* identifier in currencyDeltas)
	{
		NSNumber* delta = [currencyDeltas objectForKey:identifier];
		if ([delta integerValue] != 0)
		{
			NSMutableDictionary* newTx = [NSMutableDictionary dictionaryWithObjectsAndKeys:
										  identifier, @"currency_id",
										  delta, @"currency_quantity",
										  nil];
			[[OpenFeintX eventLog] logEventNamed:@"wallet_modification" parameters:newTx];

            // deltas are inherently unordered, so in order to ensure correct
            // playback functionality we need to put positive deltas at the
            // beginning of the tx list and negative deltas at the end
            if ([delta integerValue] > 0)
                [pendingTransactions insertObject:newTx atIndex:0];
            else
			[pendingTransactions addObject:newTx];
		}
	}
	[currencyDeltas removeAllObjects];
	
	for (NSString* identifier in itemDeltas)
	{
		NSNumber* delta = [itemDeltas objectForKey:identifier];
		if ([delta integerValue] != 0)
		{
			NSMutableDictionary* newTx = [NSMutableDictionary dictionaryWithObjectsAndKeys:
										  identifier, @"item_id",
										  delta, @"item_quantity",
										  nil];
			[[OpenFeintX eventLog] logEventNamed:@"inventory_modification" parameters:newTx];

            // deltas are inherently unordered, so in order to ensure correct
            // playback functionality we need to put positive deltas at the
            // beginning of the tx list and negative deltas at the end
            if ([delta integerValue] > 0)
                [pendingTransactions insertObject:newTx atIndex:0];
            else
			[pendingTransactions addObject:newTx];
		}
	}
	[itemDeltas removeAllObjects];
}

- (void)writeNonConsumableToKeychain
		{
    OFEncryptedFile* file = [OFEncryptedFile backedUpEncryptedFileWithFilename:@"nonconsume.json"];
    file.plaintext = [OFJsonCoder encodeObjectToData:self.nonconsumableItems];
}

- (void)readNonConsumableFromKeychain
{
    OFEncryptedFile* file = [OFEncryptedFile backedUpEncryptedFileWithFilename:@"nonconsume.json"];
    self.nonconsumableItems = [OFJsonCoder decodeJsonFromData:file.plaintext];
    if(!self.nonconsumableItems) 
        self.nonconsumableItems = [NSMutableDictionary dictionaryWithCapacity:10];
}

- (void)serializeToDisk
{
    NSMutableDictionary* storeData = [NSMutableDictionary dictionaryWithDictionary:[data dictionaryRepresentation]];
    [storeData setObject:pendingTransactions forKey:@"pending"];
    [storeData setObject:pendingStoreKitTransactions forKey:@"pending_store_kit"];
    [storeData setObject:[NSNumber numberWithBool:syncHasCompletedOnce] forKey:@"first_sync_completed"];

    OFEncryptedFile* file = [OFEncryptedFile backedUpEncryptedFileWithFilename:[NSString stringWithFormat:@"inv%@", userId]];
    file.plaintext = [OFJsonCoder encodeObjectToData:storeData];
}

- (void)deserializeFromDisk
{
    OFEncryptedFile* file = [OFEncryptedFile backedUpEncryptedFileWithFilename:[NSString stringWithFormat:@"inv%@", userId]];
    NSDictionary* storeData = [OFJsonCoder decodeJsonFromData:file.plaintext];

    NSDictionary* walletData = [storeData objectForKey:@"wallet"];
    NSDictionary* itemsData = [storeData objectForKey:@"items"];
	NSDictionary* metadataFromDisk = [storeData objectForKey:@"metadata"];
	
    OFInventoryData* newData = [[OFInventoryData alloc] initWithItems:itemsData wallet:walletData metadata:metadataFromDisk];
    self.data = newData;
    [newData release];

    NSArray* pendingData = [storeData objectForKey:@"pending"];
    if(pendingData)
        self.pendingTransactions =[NSMutableArray arrayWithArray:pendingData];
    else {
        [self.pendingTransactions removeAllObjects];
    }
	
	NSMutableArray* pendingStoreKitData = [storeData objectForKey:@"pending_store_kit"];
	if (pendingStoreKitData)
		self.pendingStoreKitTransactions = pendingStoreKitData;
	else
		[self.pendingStoreKitTransactions removeAllObjects];
	
	syncHasCompletedOnce = [[storeData objectForKey:@"first_sync_completed"] boolValue];
}

+ (void)initializeInventory 
{
    BOOL shouldFireInventorySwitched = NO;
	NSString* invalidUserId = [[OFUser invalidUser] userId];

	// initialize our main instance with the offline user if not already done
	if (!sMainInventory)
	{
		OFXLog(@"Inventory", @"Initializing inventory with userId %@", invalidUserId);
		sMainInventory = [[OFInventory inventoryForUserId:invalidUserId] retain];
        shouldFireInventorySwitched = YES;
	}

	// handle changing users
	NSString* userId = [[OpenFeint localUser] userId];
	if (![sMainInventory.userId isEqualToString:userId])
	{
		OFXLog(@"Inventory", @"Changed users from %@ to %@", sMainInventory.userId, userId);
		[sMainInventory serializeToDisk];
		OFSafeRelease(sMainInventory);
		sMainInventory = [[OFInventory inventoryForUserId:userId] retain];

		// if we're not the offline user anymore let's award all the offline user's stuff to the new user
		if (![userId isEqualToString:invalidUserId])
		{
			OFInventory* offlineInventory = [OFInventory inventoryForUserId:invalidUserId];

			OFXLog(@"Inventory", @"Awarding offline user's inventory to userId %@", sMainInventory.userId);			

            [sMainInventory.data combineWithItemsAndWalletFrom:offlineInventory.data];
            [sMainInventory.data combineWithMetadataFrom:offlineInventory.data];

			[sMainInventory.pendingTransactions addObjectsFromArray:offlineInventory.pendingTransactions];
			[sMainInventory.pendingStoreKitTransactions addObjectsFromArray:offlineInventory.pendingStoreKitTransactions];
			
			[sMainInventory serializeToDisk];
			
            [offlineInventory.data clearAll];
			[offlineInventory.pendingTransactions removeAllObjects];
			[offlineInventory.pendingStoreKitTransactions removeAllObjects];
			[offlineInventory serializeToDisk];
		}

        shouldFireInventorySwitched = YES;
	}
    
    if (shouldFireInventorySwitched && [[OFXStore delegate] respondsToSelector:@selector(inventorySwitched)])
        [[OFXStore delegate] inventorySwitched];

	if ([OpenFeint isOnline])
	{
		[self synchronizeInventory];
	}
}

+ (void)storeInventory
{
    OFInventory* instance = [self instance];
	[instance convertDeltasToTransactions];
    [instance.data sanitizeMetadata];
    [instance serializeToDisk];
}

#pragma mark -
#pragma mark Debug Methods
#pragma mark -

- (NSString*)stringFromInventoryStatus:(OFInventorySynchronizationStatus)status
{
	switch (status)
	{
		case OFInventorySynchronization_Updated:		return @"Updated"; break;
		case OFInventorySynchronization_Stale:			return @"Stale"; break;
		case OFInventorySynchronization_NetworkError:	return @"General Network Failure"; break;
	}
	
	return @"Unknown";
}

#pragma mark -
#pragma mark Inventory Synchronization
#pragma mark -

+ (void)synchronizeInventory
{
	OFInventory* instance = [self instance];
    if (instance.syncIsInProgress)
	{
		OFXLog(@"Inventory", @"Attempting to synchronize multiple times concurrently.");
		return;
	}

	OFXLog(@"Inventory", @"Synchronization requested...");
    instance.syncIsInProgress = YES;    

	[instance.data sanitizeMetadata];
	[instance convertDeltasToTransactions];
	
    instance.syncLockVersion = -1;
	instance.syncDidChangeDevices = NO;
    instance.syncServerData = nil;
    instance.syncClientData = [instance.data copy];
	instance.syncTransactions = instance.pendingTransactions;
	instance.syncStoreKitTransactions = instance.pendingStoreKitTransactions;
    instance.syncInventoryIsEmpty = [instance.data isEmpty];
    instance.syncMetadataNeedsMerging = NO;
	instance.pendingTransactions = [NSMutableArray arrayWithCapacity:10];
	instance.pendingStoreKitTransactions = [NSMutableArray arrayWithCapacity:1];

    if ([[OFXStore delegate] respondsToSelector:@selector(isInventoryEmpty:)])
        instance.syncInventoryIsEmpty = [[OFXStore delegate] isInventoryEmpty:instance.syncClientData];

	// fail trivially here if we're offline
	if ([OpenFeint isOnline])
	{
		// first check server status...
		OFResourceRequest* r = [OFResourceRequest 
			getRequestWithPath:[NSString stringWithFormat:@"/xp/games/%@/inventory/status", [OpenFeint clientApplicationId]]];
        instance.currentRequest = r;
		[[r onRespondTarget:instance selector:@selector(lastUpdateResponseShim:)] execute];
	}
	else
	{
		[instance synchronizeFinished:OFInventorySynchronization_NetworkError];
	}
}

- (void)synchronizeFinished:(OFInventorySynchronizationStatus)status
{
	OFXLog(@"Inventory", @"Synchronization finished: %@", [self stringFromInventoryStatus:status]);

	switch (status)
	{
		case OFInventorySynchronization_Updated:
		{
			// after we've updated we must replay deltas and pending transactions
			// that occurred while synchronizing

            [self convertDeltasToTransactions];

			for (NSDictionary* transaction in pendingTransactions)
			{
				NSString* currencyId = [transaction objectForKey:@"currency_id"];
				if ([currencyId length])
                    [syncClientData modifyCurrency:currencyId amount:[[transaction objectForKey:@"currency_quantity"] intValue]];

				NSString* itemId = [transaction objectForKey:@"item_id"];
				if ([itemId length])
                    [syncClientData modifyGameItem:itemId amount:[[transaction objectForKey:@"item_quantity"] intValue]];
			}
			
            if (syncMetadataNeedsMerging)
                [syncClientData combineWithMetadataFrom:data];
			
            self.data = syncClientData;

			syncHasCompletedOnce = YES;
		} break;

		case OFInventorySynchronization_NetworkError:
		{
			// in the case of any sync error we need to merge sentTransactions 
			// and pending, as nothing happened.
            NSMutableArray* newPendingTransactions = [NSMutableArray arrayWithArray:syncTransactions];
            [newPendingTransactions addObjectsFromArray:pendingTransactions];
            self.pendingTransactions = newPendingTransactions;
			[self.pendingStoreKitTransactions addObjectsFromArray:self.syncStoreKitTransactions];
		} break;

		case OFInventorySynchronization_Stale:
		default:
			break;
	}

	if (syncDidChangeDevices)
		[self checkStoreKitReceipts];

    self.syncClientData = nil;
    self.syncServerData = nil;
	self.syncTransactions = nil;
	self.syncStoreKitTransactions = nil;
    syncIsInProgress = NO;
	
	// if our inventory sync results in a stale error we
	// need to reprocess the sync -- this means the developer
	// updated the inventory while we were in the process of syncing
	if (status == OFInventorySynchronization_Stale)
	{
		[OFInventory synchronizeInventory];
		return;
	}
	
	if ([[OFXStore delegate] respondsToSelector:@selector(inventorySynchronized:)])
		[[OFXStore delegate] inventorySynchronized:status];

	// scan for payload updates
    NSArray* items = [data itemsInInventory];
	NSMutableArray* payloadUpdates = [NSMutableArray arrayWithCapacity:[items count]];
	for (NSString* itemId in items)
	{
		OFDeliverable* deliverable = [[OFInAppPurchaseCatalog deliverablesInternal] objectForKey:itemId];
		if (deliverable.payloadDownloadRequired)
			[payloadUpdates addObject:deliverable];
	}
	[self signalUpdatesRequired:payloadUpdates];
}

- (void)lastUpdateResponseShim:(OFResourceRequest*)request
{
    [self performSelector:@selector(lastUpdateResponse:) withObject:request afterDelay:sFakeInventoryDelay];
}

- (void)lastUpdateResponse:(OFResourceRequest*)request
{
    if(self.currentRequest != request)
    {
        OFXLog(@"Inventory", @"WARNING: update request does not match current request");
    }
    self.currentRequest = nil;
	if (request.httpResponseCode >= 200 && request.httpResponseCode < 300)
	{
		NSDictionary* lastUpdateInfo = [request.resources objectForKey:@"last_update"];
		OFXLog(@"Inventory", @"Received latest update information: \n%@", lastUpdateInfo);

		NSString* lastUpdatedUdid = [lastUpdateInfo objectForKey:@"device_identifier"];
		NSString* lastUpdatedMessage = [lastUpdateInfo objectForKey:@"message"];
		
		syncDidChangeDevices = [lastUpdatedUdid length] && ![[[UIDevice currentDevice] uniqueIdentifier] isEqualToString:lastUpdatedUdid];
		
		// show the last update message
		if ([lastUpdatedMessage length])
		{
			[[[[UIAlertView alloc] initWithTitle:nil
				message:lastUpdatedMessage 
				delegate:nil 
				cancelButtonTitle:OFLOCALSTRING(@"Ok")
				otherButtonTitles:nil] autorelease] show];
		}

        if ([lastUpdatedMessage length] || syncDidChangeDevices || !syncHasCompletedOnce)
			[self getServerInventory];
		else
			[self putClientInventory];
		}
	else
	{
		[self synchronizeFinished:OFInventorySynchronization_NetworkError];
	}
}

- (void)getServerInventory
{
	OFResourceRequest* r = [OFResourceRequest
		getRequestWithPath:[NSString stringWithFormat:@"/xp/games/%@/inventory", [OpenFeint clientApplicationId]]];
    self.currentRequest = r;
	[[r onRespondTarget:self selector:@selector(inventoryGetResponse:)] execute];
}

- (void)inventoryGetResponse:(OFResourceRequest*)request
{
    if(self.currentRequest != request)
    {
        OFXLog(@"Inventory", @"WARNING: inventory request does not match current request");
    }
    self.currentRequest = nil;
	if (request.httpResponseCode >= 200 && request.httpResponseCode < 300)
	{
        OFInventoryData* dataFromServer = [[OFInventoryData alloc] 
            initWithItems:[request.resources objectForKey:@"items"] 
            wallet:[request.resources objectForKey:@"wallet"] 
            metadata:[request.resources objectForKey:@"metadata"]];
        self.syncServerData = dataFromServer;
        [dataFromServer release];

		NSDictionary* lastUpdateInfo = [request.resources objectForKey:@"last_update"];
		NSString* lastUpdatedMessage = [lastUpdateInfo objectForKey:@"message"];

        syncLockVersion = [[request.resources objectForKey:@"lock_version"] integerValue];
		
        BOOL shouldMergeMetadata = NO;
		// if this is our first sync we should merge the server's stuff with ours
		if (!syncHasCompletedOnce)
		{
            [syncServerData combineWithItemsAndWalletFrom:syncClientData];
            [syncServerData combineWithMetadataFrom:syncClientData];
		}
		else
		{
            // since we're taking the server inventory the user might lose some stuff...
            // so if it's due to a developer message we'll apply all transactions since 
            // the last sync but if it's just a changed device we have to keep
            // only the store kit transactions since those were paid with RL$.
            NSArray* transactionsToApply = ([lastUpdatedMessage length] == 0) ? syncStoreKitTransactions : syncTransactions;

            for (NSDictionary* transaction in transactionsToApply)
			{
				NSString* currencyId = [transaction objectForKey:@"currency_id"];
				if ([currencyId length])
                    [syncServerData modifyCurrency:currencyId amount:[[transaction objectForKey:@"currency_quantity"] intValue]];

				NSString* itemId = [transaction objectForKey:@"item_id"];
				if ([itemId length])
                    [syncServerData modifyGameItem:itemId amount:[[transaction objectForKey:@"item_quantity"] intValue]];
            }

            shouldMergeMetadata = [transactionsToApply count] > 0;
			}

        if ([lastUpdatedMessage length] == 0 && syncDidChangeDevices && !syncInventoryIsEmpty && syncHasCompletedOnce)
        {
            if ([[OFXStore delegate] respondsToSelector:@selector(resolveConflictingLocalInventory:andRemoteInventory:)])
            {
                [[OFXStore delegate] resolveConflictingLocalInventory:syncClientData andRemoteInventory:syncServerData];
            }
            else
            {
                NSString* updatePrompt = [lastUpdateInfo objectForKey:@"device_change_message"];
                if ([updatePrompt length] == 0)
                {
                    updatePrompt = @"Your game progress was last saved to OpenFeint from another device.\n\n"
                                   @"Would you like to download your progress or overwrite it with your current game?";
                }

                NSString* updatePromptKeepLocal = [lastUpdateInfo objectForKey:@"device_change_keep_local_title"];
                if ([updatePromptKeepLocal length] == 0)
                    updatePromptKeepLocal = @"Overwrite";

                NSString* updatePromptKeepRemote = [lastUpdateInfo objectForKey:@"device_change_keep_remote_title"];
                if ([updatePromptKeepRemote length] == 0)
                    updatePromptKeepRemote = @"Download";

                [[[[UIAlertView alloc]
                    initWithTitle:nil 
                    message:updatePrompt 
                    delegate:self 
                    cancelButtonTitle:updatePromptKeepLocal 
                    otherButtonTitles:updatePromptKeepRemote, nil] autorelease] show];
            }
        }
        else
        {
            if (shouldMergeMetadata)
                syncMetadataNeedsMerging = YES;
            self.syncClientData = syncServerData;
            [self putClientInventory];
        }
	}
	else
	{
		[self synchronizeFinished:OFInventorySynchronization_NetworkError];
	}
	}
		
- (void)putClientInventory
{
    NSDictionary* sync = [NSDictionary dictionaryWithObject:[syncClientData dictionaryRepresentation] forKey:@"inventory"];
	NSMutableDictionary* body = [NSMutableDictionary dictionaryWithObject:[OFJsonCoder encodeObject:sync] forKey:@"sync"];	
	if (syncLockVersion >= 0)
	{
		[body setObject:[NSNumber numberWithInteger:syncLockVersion] forKey:@"lock_version"];
	}

	OFResourceRequest* r = [OFResourceRequest 
						putRequestWithPath:[NSString stringWithFormat:@"/xp/games/%@/inventory", [OpenFeint clientApplicationId]] 
        andBody:body];
    self.currentRequest = r;
	[[r onRespondTarget:self selector:@selector(inventoryPutResponse:)] execute];
}

- (void)inventoryPutResponse:(OFResourceRequest*)request
{
    if(self.currentRequest != request)
    {
        OFXLog(@"Inventory", @"WARNING: client put request does not match current request");
    }
    self.currentRequest = nil;
	if (request.httpResponseCode >= 200 && request.httpResponseCode < 300)
	{
		[self synchronizeFinished:OFInventorySynchronization_Updated];
    }
    else 
	{
		if ([request.resources isKindOfClass:[OFServerException class]] &&
			[request.resources isStaleObjectException])
		{
			[self synchronizeFinished:OFInventorySynchronization_Stale];
		}
		else
		{
			[self synchronizeFinished:OFInventorySynchronization_NetworkError];
		}
    }
}

+ (void)commitResolvedInventory:(OFInventoryData*)resolved
{
    OFInventory* instance = [self instance];
    instance.syncClientData = resolved;
    [instance putClientInventory];
}

- (void)cancelCurrentRequests
{
    [self.currentRequest onRespondTarget:nil selector:nil];
}

#pragma mark -
#pragma mark Store Kit Restore
#pragma mark -

- (void)checkStoreKitReceipts
{
	OFXLog(@"Inventory", @"Fetching StoreKit receipts...");

	OFResourceRequest* r = [OFResourceRequest
		getRequestWithPath:[NSString stringWithFormat:@"/xp/games/%@/store_kit_receipts/products", [OpenFeint clientApplicationId]]
		andQuery:[NSDictionary dictionaryWithObject:[OpenFeint lastLoggedInUserId] forKey:@"user_id"]];
    self.currentRequest = r;
	[[r onRespondTarget:self selector:@selector(storeKitReceiptResponse:)] execute];
}

- (void)storeKitReceiptResponse:(OFResourceRequest*)request
{
    if(self.currentRequest != request)
    {
        OFXLog(@"Inventory", @"WARNING: store kit request does not match current request");
    }
    self.currentRequest = nil;
	BOOL needsRestore = YES;
	if (request.httpResponseCode >= 200 && request.httpResponseCode < 300)
	{
		OFXLog(@"Inventory", @"Received %d StoreKit receipts", [request.resources count]);
		needsRestore = NO;
		
		NSArray* iaps = [OFInAppPurchaseCatalog inAppPurchases];
		for (OFInAppPurchase* iap in iaps)
		{
			if (![iap.storeKitProductIdentifier length] || !iap.storeKitNonConsumable)
				continue;

			if ([request.resources containsObject:iap.storeKitProductIdentifier] &&
				[OFInventory numberOfItem:iap.deliverable.identifier] == 0)
			{
				needsRestore = YES;
				break;
			}
		}
	}
	
	if (needsRestore)
	{
		OFXLog(@"Inventory", @"Restoring StoreKit purchases");
		[[OFXStore storeKit] restorePurchases];
	}
}

#pragma mark -
#pragma mark UIAlertViewDelegate
#pragma mark -

- (void)alertView:(UIAlertView *)alertView clickedButtonAtIndex:(NSInteger)buttonIndex;
{
    OFInventoryData* resolved = nil;

	if (buttonIndex == 0)
        resolved = [syncClientData retain];
	else if (buttonIndex == 1)
        resolved = [syncServerData retain];
    
    [OFInventory commitResolvedInventory:resolved];
    [resolved release];
}

#pragma mark -
#pragma mark Inventory management
#pragma mark -

+ (NSArray*)itemsInInventory
{
	OFInventory* instance = [OFInventory instance];
    return [[instance.data itemsInInventory] arrayByAddingObjectsFromArray:[instance.nonconsumableItems allKeys]];
}

+ (NSArray*)currenciesInInventory
{
    OFInventory* instance = [OFInventory instance];
    return [instance.data currenciesInInventory];
}

+ (NSInteger)amountOfCurrency:(NSString*)currencyId
{
    OFInventory* instance = [OFInventory instance];
    return [instance.data amountOfCurrency:currencyId];
}

+ (NSInteger)numberOfItem:(NSString*)itemIdentifier
{
    OFInventory* instance = [OFInventory instance];
    
    NSInteger count = [instance.data numberOfItem:itemIdentifier];
    if (count == 0)
        count = [[instance.nonconsumableItems objectForKey:itemIdentifier] intValue];
    
    return count;
}

+ (NSMutableDictionary*)metadata
{
    OFInventory* instance = [OFInventory instance];
    if (instance.syncIsInProgress)
        instance.syncMetadataNeedsMerging = YES;
    return [instance.data metadata];
}

+ (void)modifyCurrency:(NSString*)currencyId amount:(NSInteger)delta
{
    OFInventory* instance = [OFInventory instance];
    [instance.data modifyCurrency:currencyId amount:delta];
	[instance.currencyDeltas addInteger:delta toKey:currencyId];
}

+ (void)modifyGameItem:(NSString*)itemIdentifier quantity:(NSInteger)delta
{
    OFInventory* instance = [OFInventory instance];
    
    //don't allow modifications to a non-consumable item, these are controlled by Apple only
    if ([instance.nonconsumableItems objectForKey:itemIdentifier])
        return;
    
    OFDeliverable* deliverable = [[OFInAppPurchaseCatalog deliverablesInternal] objectForKey:itemIdentifier];

    //this is a deliverable for a currency pack, which isn't an "item"
    if (deliverable.currencyIdentifier)
        return;
    
    [instance.data modifyGameItem:itemIdentifier amount:delta];

    if (deliverable.payloadDownloadRequired)
    {
        NSArray* deliverables = [[NSArray alloc] initWithObjects:deliverable, nil];
        [instance signalUpdatesRequired:deliverables];
        [deliverables release];
    }

	[instance.itemDeltas addInteger:delta toKey:itemIdentifier];
}

+(void) applyInAppPurchase:(OFInAppPurchase*)purchase
{
	OFInventory* instance = [OFInventory instance];
	
	if (purchase.storeKitNonConsumable)
	{
		// StoreKit non-consumables go straight to a device-based store rather than the per-user store
		[instance.nonconsumableItems setObject:[NSNumber numberWithInteger:1] forKey:purchase.deliverable.identifier];
		[instance writeNonConsumableToKeychain];
	}
	else
	{
		NSMutableDictionary* transaction = [NSMutableDictionary dictionaryWithObjectsAndKeys:
			purchase.resourceId, @"sku_id",
			[[UIDevice currentDevice] uniqueIdentifier], @"udid",
			nil];

		// a deliverable with a currencyIdentifier signifies a currency pack
		// which by our restrictions cannot be purchased for anything but RL$
		// and cannot award items -- they can only modify the wallet
		if (purchase.deliverable.currencyIdentifier)
		{
            [instance.data modifyCurrency:purchase.deliverable.currencyIdentifier amount:purchase.quantity];
			[transaction setObject:purchase.deliverable.currencyIdentifier forKey:@"currency_id"];
			[transaction setObject:[NSNumber numberWithInteger:purchase.quantity] forKey:@"currency_quantity"];
		}
		// "normal" deliverable, meaning free item, virtual currency item, StoreKit paid item
		else
		{
			// if currency is used to purchase this item
			if (purchase.purchaseCurrency)
			{
				// ...deduct it
                [instance.data modifyCurrency:purchase.purchaseCurrency.identifier amount:-purchase.purchaseCurrencyAmount];
				[transaction setObject:purchase.purchaseCurrency.identifier forKey:@"currency_id"];
				[transaction setObject:[NSNumber numberWithInteger:-purchase.purchaseCurrencyAmount] forKey:@"currency_quantity"];
			}
			
			// award the deliverable in the given quantity
            [instance.data modifyGameItem:purchase.deliverable.identifier amount:purchase.quantity];
			[transaction setObject:purchase.deliverable.identifier forKey:@"item_id"];
			[transaction setObject:[NSNumber numberWithInteger:purchase.quantity] forKey:@"item_quantity"];
		}
		
		[[OpenFeintX eventLog] logEventNamed:@"in_app_purchase" parameters:transaction];

		// consumable StoreKit puchases must be saved off here as well.
		// we want to restore them if in the *next* sync there is a forced
		// developer update; meaning the user would lose his purchase.
		if ([purchase.storeKitProductIdentifier length] > 0)
		{
			[instance.pendingStoreKitTransactions addObject:[transaction copy]];
		}
		
		[instance.pendingTransactions addObject:transaction];
	}
    
    if(purchase.deliverable.payloadDownloadRequired) {
        [instance signalUpdatesRequired:[NSArray arrayWithObject:purchase.deliverable]];
    }

}

+ (NSNumber*) applyAdWallPoints:(NSNumber*) pointsObj
{
    NSDictionary* currencies = [OFInAppPurchaseCatalog currencies];
    for(NSString* currencyIdentifier in currencies)
    {
        OFCurrency* currency = [currencies objectForKey:currencyIdentifier];
        if(currency.usedForOffer) 
        {
            OFLog(@"Inventory applying %@ points to %@", pointsObj, currency.name);
            [self modifyCurrency:currencyIdentifier amount:[pointsObj intValue]];
            return [NSNumber numberWithBool:YES];
        }
    }
    return [NSNumber numberWithBool:NO];
}

#pragma mark -
#pragma mark Testing Support
#pragma mark -

+ (void)setFakeInventoryDelay:(NSNumber*)delay
{
    sFakeInventoryDelay = [delay floatValue];
}

@end

#pragma mark -
#pragma mark Testing
#pragma mark -

@interface OFInventory(Tests)
+(void)resetTest;
+ (void)fakeStore;
+(void)fakeSync;
+(void)fakeSyncResult:(BOOL)valid;
+(void)fakeServerAddCurrency:(NSInteger) deltaCurrency item:(NSInteger) deltaItem;
+ (BOOL)testNumGeneratedTransactions:(NSInteger)expectedTxCount forIdentifier:(NSString*)identifier;
+ (BOOL)testDelta:(NSInteger)expectedDelta forIdentifier:(NSString*)identifier;
@end

static NSUInteger fakeCurrencyValue;
static NSUInteger fakeItemValue;

@implementation OFInventory (Tests)

+ (void)resetTest
{
    NSDictionary* items = [NSDictionary dictionaryWithObject:[NSNumber numberWithInt:100] forKey:@"TESTITEM"];
    NSDictionary* wallet = [NSDictionary dictionaryWithObject:[NSNumber numberWithInt:100] forKey:@"TESTCURRENCY"];    
    sMainInventory.data = [[[OFInventoryData alloc] initWithItems:items wallet:wallet metadata:nil] autorelease];
	[sMainInventory.pendingTransactions removeAllObjects];
	sMainInventory.syncTransactions = nil;
	[sMainInventory.itemDeltas removeAllObjects];
	[sMainInventory.currencyDeltas removeAllObjects];
}

+ (void)fakeStore
{
	[sMainInventory convertDeltasToTransactions];
}

+(void)fakeSync {
    sMainInventory.syncTransactions = sMainInventory.pendingTransactions;
    [sMainInventory.pendingTransactions removeAllObjects];
    fakeItemValue = [sMainInventory.data numberOfItem:@"TESTITEM"];
    fakeCurrencyValue = [sMainInventory.data amountOfCurrency:@"TESTCURRENCY"];
}

+(void)fakeSyncResult:(BOOL) valid {
    if(valid) {
		[[self instance] synchronizeFinished:OFInventorySynchronization_Updated];
    }
    else {
		[[self instance] synchronizeFinished:OFInventorySynchronization_NetworkError];
    }
}
                                  
+(void)fakeServerAddCurrency:(NSInteger) deltaCurrency item:(NSInteger) deltaItem {
    fakeCurrencyValue += deltaCurrency;
    fakeItemValue += deltaItem;
}

+ (BOOL)testNumGeneratedTransactions:(NSInteger)expectedTxCount forIdentifier:(NSString*)identifier
{
	NSInteger numActualTx = 0;
	for (NSDictionary* tx in sMainInventory.pendingTransactions)
	{
		if ([[tx objectForKey:@"item_id"] isEqualToString:identifier] || [[tx objectForKey:@"currency_id"] isEqualToString:identifier])
		{
			++numActualTx;
		}
	}
	
	return numActualTx == expectedTxCount;
}

+ (BOOL)testDelta:(NSInteger)expectedDelta forIdentifier:(NSString*)identifier
{
	NSInteger itemDelta = [[sMainInventory.itemDeltas objectForKey:identifier] integerValue];
	NSInteger currencyDelta = [[sMainInventory.currencyDeltas objectForKey:identifier] integerValue];
	
	return (itemDelta == expectedDelta) || (currencyDelta == expectedDelta);
}

@end
