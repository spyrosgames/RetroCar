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

#import "OFInAppPurchaseCatalog.h"
#import "OpenFeint/OFZipArchive.h"
#import "OpenFeint/OFJsonCoder.h"
#import "OpenFeint/OFResourceRequest.h"
#import "OpenFeint/OpenFeint+UserOptions.h"
#import "OpenFeint/OFKeychainWrapper.h"
#import "OpenFeint/OFEncryptedFile.h"
#import "OFInAppPurchase.h"
#import "OFCurrency.h"
#import "OFDeliverable.h"
#import "OFXStore.h"
#import "OFStoreKit.h"
#import "OFSkuCategory.h"
#import "OFInventory.h"
#import "OFXStoreTestCategories.h"
#import "OFXStore+Private.h"
#import "OFXDebug.h"
#import "OpenFeint/OFSettings.h"

#ifdef _DEBUG
//uncomment to add fixture data to the store
//#define USE_FIXTURES
#endif

static NSString* skuKey = @"sku.json";
static NSString* deliverableKey = @"deliverable.json";
static NSString* currencyKey = @"currency.json";
static NSString* categoryKey = @"category.json";
static OFInAppPurchaseCatalog* sharedCatalog = nil;
    
    //assumes that the values in the dictionary have an identifier selector
static NSMutableDictionary* convertArrayToDictionary(const NSArray* array) {
        NSMutableDictionary*tempDict = [NSMutableDictionary dictionaryWithCapacity:[array count]];
        for(id value in array) {
            [tempDict setObject:value forKey:[value identifier]];
        }
    return tempDict;
    }

static NSString* OFXStoreUserDefault_OfflineDate = @"OFXStoreUserDefault_OfflineDate";
static NSString* OFXStoreUserDefault_UpdateTimestamp = @"OFXStoreUserDefault_UpdateTimestamp";




@interface OFInAppPurchaseCatalog (Tests)
+(void)addFixtures;
- (void)versionChecks;
- (void)startAndEndChecks;
@end

@interface OFInAppPurchaseCatalog ()
- (void)invokeCatalogUpdatedDelegate;
- (void)showOfflinePackageError:(NSString*)errorMessage;
- (void)extractOfflinePackage;
- (void)commitExtractedPackage:(NSDictionary*)storeData;
- (void)storeKitPurchaseCompleted:(NSNotification*)notification;
- (OFInAppPurchase*)iapForStoreKitProductIdentifier:(NSString*)storeKitProductIdentifier;
- (void)storeKitProductInfoRequestFinished:(NSDictionary*)purchasables;
- (id)resourcesFromJsonAtPath:(NSString*)path;
-(void)retrieveFromServer;
-(void)storeResponse:(OFResourceRequest*)data;
-(void)writeToStore:(NSString*)tag  withObject:(id) data;
-(id)retrieveFromStore:(NSString*)tag;
-(NSArray*)addNullCategory:(NSArray*)skus categories:(NSArray*)categories;
-(void)setSkus:(NSArray*)skus currencies:(NSDictionary*) currencies deliverables:(NSDictionary*) deliverables 
    categories:(NSArray*) categories fixCategories:(BOOL) fixCategories;
- (void)migrateDownloadableContent;

- (NSComparisonResult) compareVersion:(NSString*)oldVersion to:(NSString*)newVersion;

@property (nonatomic, readwrite, retain) NSArray* skus;
@property (nonatomic, readwrite, retain) NSDictionary* currencies;
@property (nonatomic, readwrite, retain) NSDictionary* deliverablesInternal;
@property (nonatomic, readwrite, retain) NSArray* categoriesInternal;
@property (nonatomic, readwrite, retain) NSArray* skusAddedInLastUpdate;
@property (nonatomic, readonly) BOOL serverLoaded;
@end

@implementation OFInAppPurchaseCatalog
@synthesize skus;
@synthesize currencies;
@synthesize deliverablesInternal;
@synthesize categoriesInternal;
@synthesize skusAddedInLastUpdate;
@synthesize serverLoaded;
#pragma mark Singleton Methods

+ (void)initializeCatalog
{
	if (!sharedCatalog)
	{
		sharedCatalog = [[OFInAppPurchaseCatalog alloc] init];
		[sharedCatalog migrateDownloadableContent];
	}
}

+ (void)shutdownCatalog
{
	OFSafeRelease(sharedCatalog);
}

+ (id)sharedCatalog
{
	return sharedCatalog;
}

+ (void)updateCatalogFromServer
{
    [sharedCatalog retrieveFromServer];
}

+ (NSArray*)inAppPurchasesAddedInLastUpdate
{
	return [sharedCatalog skusAddedInLastUpdate];
}

#pragma mark Life-cycle
- (id)init
{
	self = [super init];
	if (self != nil)
	{
        [[NSNotificationCenter defaultCenter] 
            addObserver:self 
            selector:@selector(storeKitPurchaseCompleted:) 
            name:OFStoreKitPurchaseCompleted 
            object:nil];

        
        //for offline, check it if the modified date has been changed.
        NSDate* lastLoadedOffline = [[NSUserDefaults standardUserDefaults] objectForKey:OFXStoreUserDefault_OfflineDate];
        NSString* offlinePath = [[NSBundle mainBundle] pathForResource:@"ofx_offline_config" ofType:@"zip"];
        NSDate* fileDate = [[[NSFileManager defaultManager] attributesOfItemAtPath:offlinePath error:NULL] fileModificationDate];
        
        if(![fileDate isEqualToDate:lastLoadedOffline])
        {
            //read the offline
			initializationFlags.hasExtractedOfflinePackage = NO;
			[self performSelectorInBackground:@selector(extractOfflinePackage) withObject:nil];
		}
        else
		{
            //trust the loaded store
            [self setSkus:[self retrieveFromStore:skuKey]
               currencies:[self retrieveFromStore:currencyKey]
             deliverables:[self retrieveFromStore:deliverableKey]
               categories:[self retrieveFromStore:categoryKey]
            fixCategories:NO];
			self.skusAddedInLastUpdate = nil;
			initializationFlags.hasExtractedOfflinePackage = YES;
			[self performSelector:@selector(invokeCatalogUpdatedDelegate) withObject:nil afterDelay:0.f];
        }
    }
	return self;
}

- (void)dealloc
{
	[[NSNotificationCenter defaultCenter] removeObserver:self];
	self.skus = nil;
	self.currencies = nil;
	self.deliverablesInternal = nil;
	self.categoriesInternal = nil;
	self.skusAddedInLastUpdate = nil;
	[super dealloc];
}

#pragma mark Offline Package Handling
- (NSComparisonResult) compareVersion:(NSString*)oldVersion to:(NSString*)newVersion
{
    NSScanner* oldScanner = [NSScanner scannerWithString:oldVersion];
    NSScanner* newScanner = [NSScanner scannerWithString:newVersion];
    [oldScanner setCharactersToBeSkipped:[NSCharacterSet characterSetWithCharactersInString:@"."]];
    [newScanner setCharactersToBeSkipped:[NSCharacterSet characterSetWithCharactersInString:@"."]];
    
    while(1)
    {
        int oldPiece = 0;
        int newPiece = 0;
        BOOL found = NO;
        found |= [oldScanner scanInt:&oldPiece];
        found |= [newScanner scanInt:&newPiece];
        if(found)
        {
            if(oldPiece < newPiece) return NSOrderedAscending;
            if(oldPiece > newPiece) return NSOrderedDescending;
        }
        else
        {            
            break;
        }
    }
    return NSOrderedSame;
}

- (void)showOfflinePackageError:(NSString*)errorMessage
{
	[[[[UIAlertView alloc] 
		initWithTitle:OFLOCALSTRING(@"Offline Package")
		message:errorMessage
		delegate:nil 
		cancelButtonTitle:@"OK" 
		otherButtonTitles:nil] autorelease] show];	
}

- (BOOL)sku:(OFInAppPurchase*) sku validForVersion:(NSString*)bundleVersion 
{
    BOOL valid = YES;
    valid = valid && [self compareVersion:sku.startVersion to:bundleVersion] != NSOrderedDescending;
    valid = valid && [self compareVersion:sku.endVersion to:bundleVersion] != NSOrderedAscending;
    return valid;
}
             

- (void)extractOfflinePackage
{
	NSAutoreleasePool* pool = [NSAutoreleasePool new];

    NSString* documentsPath = [OFXStore _rootPathForPayloads];
    NSString* catalogPath = [[OFXStore _rootPathForPayloads] stringByAppendingPathComponent:@"ofx_offline_cache"];
	
	OFZipArchive* zip = [[OFZipArchive alloc] init];
	if ([zip UnzipOpenFile:[[NSBundle mainBundle] pathForResource:@"ofx_offline_config" ofType:@"zip"]])
	{
		[zip UnzipFileTo:catalogPath overWrite:YES];
		[zip UnzipCloseFile];
	}
	else
	{
		/*
        NSString* message = OFLOCALSTRING(@"You have not included an offline package! You can download your offline package from the developer dashboard. This is *required*!");
		[self performSelectorOnMainThread:@selector(showOfflinePackageError:) withObject:message waitUntilDone:NO];
        */
	}

    NSString* timestamp = [NSString stringWithContentsOfFile:[catalogPath stringByAppendingPathComponent:@"timestamp.txt"] encoding:NSUTF8StringEncoding error:nil];
    int timestampValue = [timestamp intValue];
    
    NSString* bundleVersion = [OFSettings instance].clientBundleVersion;
	NSDictionary* storeData = [self resourcesFromJsonAtPath:[catalogPath stringByAppendingPathComponent:@"ios_store.json"]];

    if(!storeData)
    {
        storeData = [self resourcesFromJsonAtPath:[catalogPath stringByAppendingPathComponent:@"store.json"]]; 
	}
        
    //list of all skus added this round
    NSMutableDictionary* offlineSkus = [NSMutableDictionary dictionary];
    for(OFInAppPurchase* sku in [storeData objectForKey:@"skus"])
    {
        if([self sku:sku validForVersion:bundleVersion])
        {
            [offlineSkus setObject:sku forKey:sku.resourceId];
        }
    }

    //these hold the values to send
    NSArray* totalSkus;
    NSMutableDictionary* totalCurrencies;
    NSMutableDictionary* totalDeliverables;
    NSArray* totalCategories;
    NSMutableDictionary* totalFingerprints = [NSMutableDictionary dictionary];
    
    BOOL fixCategories = YES;
    
    int currentTimestamp = [[NSUserDefaults standardUserDefaults] integerForKey:OFXStoreUserDefault_UpdateTimestamp];
    if(currentTimestamp > timestampValue)
    {
        fixCategories = NO;
        
        //add any skus that don't already exist
        NSArray* storeSkus = [self retrieveFromStore:skuKey];
        for(OFInAppPurchase* sku in storeSkus)
        {
            [offlineSkus removeObjectForKey:sku.resourceId];
        }
        //what's left gets added to the store
        totalSkus = [storeSkus arrayByAddingObjectsFromArray:[offlineSkus allValues]];

        //these are already dictionaries, so they are easier
        totalCurrencies = convertArrayToDictionary([storeData objectForKey:@"currencies"]);
        [totalCurrencies addEntriesFromDictionary:[self retrieveFromStore:currencyKey]];  //which replaces in the case of collisions

        totalDeliverables = convertArrayToDictionary([storeData objectForKey:@"deliverables"]);
        [totalDeliverables addEntriesFromDictionary:[self retrieveFromStore:deliverableKey]];
        
        //categories, sadly not dictionaries
        NSMutableDictionary* tempCategories = [NSMutableDictionary dictionary];
        for(OFSkuCategory* cat in [storeData objectForKey:@"sku_categories"])
        {
            [tempCategories setObject:cat forKey:cat.name];
        }
        NSMutableDictionary* storeCategories = [NSMutableDictionary dictionary];
        for(OFSkuCategory* cat in [self retrieveFromStore:categoryKey])
        {
            [storeCategories setObject:cat forKey:cat.name];
        }
        [tempCategories addEntriesFromDictionary:storeCategories];
        totalCategories = [tempCategories allValues];
    }
    else
    {
        totalSkus = [offlineSkus allValues];
        totalCurrencies = convertArrayToDictionary([storeData objectForKey:@"currencies"]);
        totalDeliverables = convertArrayToDictionary([storeData objectForKey:@"deliverables"]);
        totalCategories = [storeData objectForKey:@"sku_categories"];
    }
	
	for (OFInAppPurchase* sku in offlineSkus.objectEnumerator)
	{
		if (sku.storePayloadUrl)
		{
			NSString* path = [catalogPath stringByAppendingPathComponent:sku.storePayloadUrl.path];
			if ([zip UnzipOpenFile:path])
			{
				[zip UnzipFileTo:documentsPath overWrite:YES];
				[zip UnzipCloseFile];
			}
            //and add the appropriate fingerprints
            //store payload
            NSString* fingerprintPath = sku.storePayloadUrl.relativePath;
            if(fingerprintPath)
            {
                id fingerprint = [[storeData objectForKey:@"store_resources_assets"] objectForKey:fingerprintPath];
                if(fingerprint)
                {
                    [totalFingerprints setObject:fingerprint forKey:fingerprintPath];
                }
                else
                {
                    OFLog(@"Warning: eTag hash for %@ was not found in the offline package.", fingerprintPath);
                }
            }            
            //icon
            fingerprintPath = sku.iconUrl.relativePath;
            if(fingerprintPath)
            {
                id fingerprint = [[storeData objectForKey:@"store_resources_assets"] objectForKey:fingerprintPath];
                if(fingerprint)
                {
                    [totalFingerprints setObject:fingerprint forKey:fingerprintPath];
                }
                else
                {
                    OFLog(@"Warning: eTag hash for %@ was not found in the offline package.", fingerprintPath);
		}
	}
	
            //screenshot
            fingerprintPath = sku.screenshotUrl.relativePath;
            if(fingerprintPath)
            {
                id fingerprint = [[storeData objectForKey:@"store_resources_assets"] objectForKey:fingerprintPath];
                if(fingerprint)
                {
                    [totalFingerprints setObject:fingerprint forKey:fingerprintPath];
                }
                else
                {
                    OFLog(@"Warning: eTag hash for %@ was not found in the offline package.", fingerprintPath);
                }
		}
	}
	}
    
    NSDictionary* sendData = [NSDictionary dictionaryWithObjectsAndKeys:
                              totalSkus, @"skus",
                              totalCurrencies, @"currencies",
                              totalDeliverables, @"deliverables",
                              totalCategories, @"sku_categories",
                              totalFingerprints, @"store_resources_assets", 
                              [NSNumber numberWithBool:fixCategories], @"fix_categories",
                              nil];
	
	[self performSelectorOnMainThread:@selector(commitExtractedPackage:) withObject:sendData waitUntilDone:NO];
	[pool release];
}

- (void)commitExtractedPackage:(NSDictionary*)storeData
{
	[[OFXStore downloader] batchSetFingerprints:[storeData objectForKey:@"store_resources_assets"]];
	[self setSkus:[storeData objectForKey:@"skus"]
	   currencies:[storeData objectForKey:@"currencies"]
	 deliverables:[storeData objectForKey:@"deliverables"]
	   categories:[storeData objectForKey:@"sku_categories"] 
	fixCategories:[[storeData objectForKey:@"fix_categories"] boolValue]];
	self.skusAddedInLastUpdate = nil;  //this is questionable when merging into a new version, for compatibility I am leaving this as is.

	[self invokeCatalogUpdatedDelegate];
	
	initializationFlags.hasExtractedOfflinePackage = YES;

	if (initializationFlags.shouldMigrateDLC)
		[self migrateDownloadableContent];

	if (initializationFlags.shouldUpdateFromServer)
		[self retrieveFromServer];
		
	initializationFlags.shouldMigrateDLC = NO;
	initializationFlags.shouldUpdateFromServer = NO;
    
    NSString* offlinePath = [[NSBundle mainBundle] pathForResource:@"ofx_offline_config" ofType:@"zip"];
    NSDate* fileDate = [[[NSFileManager defaultManager] attributesOfItemAtPath:offlinePath error:NULL] fileModificationDate];
    [[NSUserDefaults standardUserDefaults] setObject:fileDate forKey:OFXStoreUserDefault_OfflineDate];
}

#pragma mark Internal Methods

- (void)invokeCatalogUpdatedDelegate
{
	[[OFXStore delegate] inAppPurchaseCatalogUpdated];
}

- (void)migrateDownloadableContent
{
	if (!initializationFlags.hasExtractedOfflinePackage)
	{
		initializationFlags.shouldMigrateDLC = YES;
		return;
	}
	
	if (![(NSNumber*)[[NSUserDefaults standardUserDefaults] objectForKey:@"OFXHasMigratedDLC"] boolValue])
	{
		for (OFInAppPurchase* iap in skus)
		{
			if (!iap.storeKitNonConsumable)
				continue;

			if ([[OFKeychainWrapper keychainValueWithIdentifier:iap.storeKitProductIdentifier] data] != nil)
				[OFInventory applyInAppPurchase:iap];
		}
		[OFInventory storeInventory];
		[[NSUserDefaults standardUserDefaults] setObject:[NSNumber numberWithBool:YES] forKey:@"OFXHasMigratedDLC"];
	}
}

#pragma mark StoreKit

- (OFInAppPurchase*)iapForStoreKitProductIdentifier:(NSString*)storeKitProductIdentifier
{
    for (OFInAppPurchase* iap in skus)
    {
        if ([[iap storeKitProductIdentifier] isEqualToString:storeKitProductIdentifier])
			return iap;
    }
	
	return nil;
}

- (void)storeKitPurchaseCompleted:(NSNotification*)notification
{
	OFInAppPurchase* iapPurchased = [self iapForStoreKitProductIdentifier:[[notification userInfo] objectForKey:OFStoreKitUserInfo_ProductIdentifierKey]];
    if (iapPurchased != nil)
    {
		OFStoreKitPurchaseStatus storeKitStatus = (OFStoreKitPurchaseStatus)[(NSNumber*)[[notification userInfo] objectForKey:OFStoreKitUserInfo_StatusKey] integerValue];
		if (storeKitStatus == OFStoreKitPurchaseStatus_Completed)
		{
			[OFInventory applyInAppPurchase:iapPurchased];
			[OFInventory storeInventory];
			[OFInventory synchronizeInventory];
		}
		
		[iapPurchased storeKitPurchaseCompleted:storeKitStatus];
    }
}

- (void)storeKitProductInfoRequestFinished:(NSDictionary*)purchasables
{
    if ([purchasables count] > 0)
    {
	// [adill] serialize the skus here as we may have updated our cached store kit pricing info
	[self writeToStore:skuKey withObject:skus];
	[self invokeCatalogUpdatedDelegate];
}
}

#pragma mark Propert Methods

-(void)setSkus:(NSArray*)_skus 
	currencies:(NSDictionary*)_currencies 
	deliverables:(NSDictionary*)_deliverables 
	categories:(NSArray*)_categories 
	fixCategories:(BOOL)fixCategories
{
	NSMutableArray* skusAdded = [NSMutableArray arrayWithArray:_skus]; 
	[skusAdded removeObjectsInArray:skus];
	self.skusAddedInLastUpdate = skusAdded;
	
    self.skus = _skus;
    [self writeToStore:skuKey withObject:self.skus]; 
    
    self.deliverablesInternal = _deliverables;
    [self writeToStore:deliverableKey withObject:self.deliverablesInternal]; 
    
    self.currencies = _currencies;
    [self writeToStore:currencyKey withObject:self.currencies]; 

    if(fixCategories)
        self.categoriesInternal = [[self addNullCategory:_skus categories:_categories] sortedArrayUsingSelector:@selector(positionSort:)];
    else 
        self.categoriesInternal = _categories;
    [self writeToStore:categoryKey withObject:self.categoriesInternal];
    
#ifdef USE_FIXTURES
    if(![OFInAppPurchaseCatalog.categories containsObject:@"FIXTURES"])
        [OFInAppPurchaseCatalog addFixtures];
#endif

	[[OFXStore storeKit] downloadProductInformation:_skus delegate:self];
}

-(NSArray*)addNullCategory:(NSArray*)_skus categories:(NSArray*)_categories {
    if(![_categories containsObject:[OFSkuCategory uncategorized]]) {
        for(OFInAppPurchase*sku in _skus) {
            if([sku.category.name isEqualToString:[OFSkuCategory uncategorized].name]) {
                return [_categories arrayByAddingObject:[OFSkuCategory uncategorized]];
            }
        }
    }
    return _categories;
}

- (id)resourcesFromJsonAtPath:(NSString*)path
{
	id resources = nil;
	
	if ([[NSFileManager defaultManager] fileExistsAtPath:path])
	{
		NSString* json = [NSString stringWithContentsOfFile:path encoding:NSUTF8StringEncoding error:nil];
		resources = [OFJsonCoder decodeJson:json];
		
		if ([resources isKindOfClass:[NSDictionary class]] && [resources count] == 1)
		{
			resources = [[resources objectEnumerator] nextObject];
		}

		[[NSFileManager defaultManager] removeItemAtPath:path error:nil];
	}
	
	return resources;
}


-(void)writeToStore:(NSString*)tag withObject:(id) data {
    OFEncryptedFile* file = [OFEncryptedFile encryptedFileWithFilename:tag];
    file.plaintext = [OFJsonCoder encodeObjectToData:data];
}

-(id)retrieveFromStore:(NSString*)tag {
    OFEncryptedFile* file = [OFEncryptedFile encryptedFileWithFilename:tag];
    return [OFJsonCoder decodeJsonFromData:file.plaintext];
}

- (void) retrieveFromServer {
	if (!initializationFlags.hasExtractedOfflinePackage)
	{
		initializationFlags.shouldUpdateFromServer = YES;
		return;
	}

	OFXLog(@"InAppPurchaseCatalog", @"Requesting latest catalog data...");
    NSString* path = [NSString stringWithFormat:@"/xp/games/%@/store.json", [OpenFeint clientApplicationId]];
    NSDictionary* params = [NSDictionary dictionaryWithObject:[OFSettings instance].clientBundleVersion forKey:@"app-version"];
    OFResourceRequest* totalReq = [OFResourceRequest getRequestWithPath:path andQuery:params];
    totalReq.requiresUserSession = NO;
    [[totalReq onRespondTarget:sharedCatalog selector:@selector(storeResponse:)] execute];
}

-(void)storeResponse:(OFResourceRequest*)data {
    if(data && data.httpResponseCode == 200) {
		OFXLog(@"InAppPurchaseCatalog", @"Received latest catalog data");
        [[OFXStore downloader] cancelAllDownloads];
        [[OFXStore downloader] batchUpdateFingerprints:[data.resources objectForKey:@"store_resources_assets"]];
        [self setSkus:[data.resources objectForKey:@"skus"]
           currencies:convertArrayToDictionary([data.resources objectForKey:@"currencies"])
         deliverables:convertArrayToDictionary([data.resources objectForKey:@"deliverables"])
           categories:[data.resources objectForKey:@"sku_categories"]
        fixCategories:YES];
        [[NSUserDefaults standardUserDefaults] setInteger:[[data.resources objectForKey:@"timestamp"] intValue] forKey:OFXStoreUserDefault_UpdateTimestamp];
    }
	else
	{
		OFXLog(@"InAppPurchaseCatalog", @"Failed downloading latest catalog data. Code=%d, body=%@", data.httpResponseCode, data.resources);
	}

    [self invokeCatalogUpdatedDelegate];
    serverLoaded = YES;
}


#pragma mark Data Access

+ (NSArray*)inAppPurchasesForCategory:(NSString*)category{
    //find category
    for(OFSkuCategory*cat in sharedCatalog.categoriesInternal) {
        if([cat.name isEqualToString:category]) {
            NSMutableArray *skuList = [NSMutableArray arrayWithCapacity:sharedCatalog.skus.count + 1];    
            for(OFInAppPurchase* sku in sharedCatalog.skus) {
                if([cat.name isEqualToString:sku.categoryIdentifier]) [skuList addObject:sku];
            }    
            return [skuList sortedArrayUsingSelector:@selector(sortByPosition:)];    
        }
    }
    return nil;
}

+ (NSArray*)categories {
    NSMutableArray* categoryNames = [NSMutableArray arrayWithCapacity:sharedCatalog.categoriesInternal.count];
    for(OFSkuCategory* cat in sharedCatalog.categoriesInternal) {
        [categoryNames addObject:cat.name];
    }
    return [NSArray arrayWithArray:categoryNames];
}

+ (NSArray*)deliverables {
    return [NSArray arrayWithArray:sharedCatalog.deliverablesInternal.allKeys];
}

+ (NSDictionary*)deliverablesInternal {
    return sharedCatalog.deliverablesInternal;
}

+ (NSDictionary*)currencies {
    return sharedCatalog.currencies;    
}

+ (NSArray*)inAppPurchases {
    return sharedCatalog.skus;    
}

+(NSArray*)categoriesInternal {
    return sharedCatalog.categoriesInternal;
}

#pragma mark Testing

#define CHECK(lhs, rhs, expect) ++testCount; if([self compareVersion:lhs to:rhs] != expect) NSLog(@"Version check FAIL %@ > %@ expected %d got %d", lhs, rhs, expect, [self compareVersion:lhs to:rhs]); 

- (void)versionChecks
{ 
    int testCount = 0;
    CHECK(@"1.1", @"1.0", NSOrderedDescending);
    CHECK(@"1.0", @"2.0", NSOrderedAscending); 
    CHECK(@"1.0", @"0.9", NSOrderedDescending);
    CHECK(@"1.0.1", @"1.0.0", NSOrderedDescending);
    CHECK(@"2.3", @"3.0.1", NSOrderedAscending);
    CHECK(@"4", @"3.9.9", NSOrderedDescending);
    CHECK(@"4", @"4.0.1", NSOrderedAscending);
    CHECK(@"4", @"5.2", NSOrderedAscending);
    CHECK(@"4.0.1", @"3", NSOrderedDescending);
    CHECK(@"4.4.4", @"4.4.4", NSOrderedSame);
    CHECK(@"4.4.4", @"4.4.3", NSOrderedDescending);
    CHECK(@"4.4.4", @"4.4.5", NSOrderedAscending);
    CHECK(@"4.4.0", @"4.4", NSOrderedSame);
    CHECK(@"4.0.0", @"4", NSOrderedSame);
    CHECK(@"4", @"4.0.0", NSOrderedSame);
    CHECK(@"10.0", @"9.1", NSOrderedDescending);
    CHECK(@"10.2", @"10.1", NSOrderedDescending);
    CHECK(@"CHARLIE", @"FOXTROT", NSOrderedSame);
    CHECK(@"1.CHARLIE", @"1.FOXTROT", NSOrderedSame);
    CHECK(@"1.2.CHARLIE", @"1.2.FOXTROT", NSOrderedSame);
    CHECK(@"CHARLIE.FACTORY", @"CHARLIRFOXTROT", NSOrderedSame);
    CHECK(@"", @"", NSOrderedSame);
    NSLog(@"Version checks done %d", testCount);
}

#define CHECK2(bundleVersion, expect) if([self sku:testSku validForVersion:bundleVersion] != expect) NSLog(@"Start/End Check failed for %@ expected %d", bundleVersion, expect);

- (void)startAndEndChecks
{
    OFInAppPurchase* testSku = [OFInAppPurchase new];
    testSku.startVersion = @"1.0.0";
    testSku.endVersion = @"2.0.0";
    CHECK2(@"0.0.0", NO);
    CHECK2(@"0.5.0", NO);
    CHECK2(@"0.9.0", NO);
    CHECK2(@"1", YES);
    CHECK2(@"1.0", YES);
    CHECK2(@"1.0.0", YES);
    CHECK2(@"1.0.1", YES);
    CHECK2(@"1.5", YES);
    CHECK2(@"2", YES);
    CHECK2(@"2.0", YES);
    CHECK2(@"2.0.0", YES);
    CHECK2(@"2.0.1", NO);
    CHECK2(@"3", NO);

    testSku.startVersion = @"1.5.5";
    testSku.endVersion = @"2.5.5";
    CHECK2(@"1.5", NO);
    CHECK2(@"1.5.4", NO);
    CHECK2(@"1.5.5", YES);
    CHECK2(@"1.6", YES);
    CHECK2(@"2.4.9", YES);
    CHECK2(@"2.5", YES);
    CHECK2(@"2.5.4", YES);
    CHECK2(@"2.5.5", YES);
    CHECK2(@"2.5.6", NO);
    
    [testSku release];
}


+(void)addFixtures {
    NSMutableDictionary* totalCurrencies = [NSMutableDictionary dictionaryWithDictionary:sharedCatalog.currencies];
    [totalCurrencies addEntriesFromDictionary:convertArrayToDictionary([OFCurrency testFixtures])];
    sharedCatalog.currencies = [NSDictionary dictionaryWithDictionary:totalCurrencies];
    
    NSMutableDictionary* totalDeliverables = [NSMutableDictionary dictionaryWithDictionary:sharedCatalog.deliverablesInternal];
    [totalDeliverables addEntriesFromDictionary:convertArrayToDictionary([OFDeliverable testFixtures])];
     sharedCatalog.deliverablesInternal = [NSDictionary dictionaryWithDictionary:totalDeliverables];
    
    sharedCatalog.skus = [sharedCatalog.skus arrayByAddingObjectsFromArray:[OFInAppPurchase testFixtures]];
    
    sharedCatalog.categoriesInternal = [sharedCatalog.categoriesInternal arrayByAddingObjectsFromArray:[OFSkuCategory testFixtures]];
    //write them to disc?
}



@end
