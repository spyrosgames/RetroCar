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

#import "OFSkuCategory.h"
#import "OFDeliverable.h"

#import "OFXStore.h"
#import "OFXStore+Private.h"
#import "OFStoreKit.h"

#import "OFXDebug.h"
#import "OpenFeint/OFReachability.h"
#import "OpenFeint/OFASIHTTPRequest.h"

#if TARGET_IPHONE_SIMULATOR
static NSString* kUnknownPriceString = @"N/A in simulator";
#else
static NSString* kUnknownPriceString = @"...";
#endif

static id sharedDelegate = nil;

@interface OFInAppPurchase (Tests)
+(NSArray*)testFixtures;
@end

@interface OFInAppPurchase ()
@property (nonatomic, retain) NSString* storeItemTitle;
@property (nonatomic, retain) NSString* storeItemDescription;
@property (nonatomic, assign) NSInteger quantity;
@property (nonatomic, retain) NSString* categoryIdentifier;
@property (nonatomic, assign) NSUInteger position;

@property (nonatomic, retain) NSString* storeKitProductIdentifier;
@property (nonatomic, retain) SKProduct* storeKitProduct;
@property (nonatomic, assign) BOOL storeKitNonConsumable;
@property (nonatomic, retain) NSString* cachedStoreKitPrice;

@property (nonatomic, assign) NSInteger purchaseCurrencyAmount;
@property (nonatomic, retain) NSString* purchaseCurrencyIdentifier;
@property (nonatomic, retain) OFCurrency* purchaseCurrency;

@property (nonatomic, retain) OFDeliverable* deliverable;
@property (nonatomic, retain) OFSkuCategory* category;


@property (nonatomic, retain) NSString* deliverableIdentifier;

@property (nonatomic, retain) NSURL* iconUrl;
@property (nonatomic, retain) NSURL* screenshotUrl;
@property (nonatomic, retain) NSURL* storePayloadUrl;

@property (nonatomic, retain) NSString* startVersion;
@property (nonatomic, retain) NSString* endVersion;

- (void)purchaseSucceeded;
- (void)purchaseFailed:(OFInAppPurchaseStatus)status;
- (void)invokeDelegate:(OFInAppPurchaseStatus)status;
- (NSString*)descriptionForPurchaseStatus:(OFInAppPurchaseStatus)status;
- (OFInAppPurchaseStatus)inAppPurchaseStatusFromStoreKitPurchaseStatus:(OFStoreKitPurchaseStatus)status;
@end

@implementation OFInAppPurchase

@synthesize storeItemTitle;
@synthesize storeItemDescription;
@synthesize quantity;
@synthesize categoryIdentifier;
@synthesize category;
@synthesize position;

@synthesize storeKitProductIdentifier;
@synthesize storeKitProduct;
@synthesize storeKitNonConsumable;
@synthesize cachedStoreKitPrice;

@synthesize purchaseCurrencyAmount;
@synthesize purchaseCurrencyIdentifier;
@synthesize purchaseCurrency;

@synthesize deliverableIdentifier;
@synthesize deliverable;

@synthesize iconUrl;
@synthesize screenshotUrl;
@synthesize storePayloadUrl;

@synthesize startVersion;
@synthesize endVersion;

#pragma mark -
#pragma mark Class Methods
#pragma mark -

+ (void)setDelegate:(id<OFInAppPurchaseDelegate>)delegate
{
	sharedDelegate = delegate;
}

#pragma mark -
#pragma mark Life-Cycle
#pragma mark -

- (id)init
{
	self = [super init];
	if (self != nil)
	{
        self.categoryIdentifier = [OFSkuCategory uncategorized].name;
        startVersion = @"0.0.0";
        endVersion = @"9999.99.99";  //in case of loading from older offline configuration
	}
	
	return self;
}

- (void)dealloc
{
	self.storeItemTitle = nil;
	self.storeItemDescription = nil;
    // we disallow a nil category identifier in the setter
    // so we must explicitly release here
    OFSafeRelease(categoryIdentifier);
    self.category = nil;
	
	self.storeKitProductIdentifier = nil;
	self.storeKitProduct = nil;
	
	self.purchaseCurrencyIdentifier = nil;
    self.purchaseCurrency = nil;
	
	self.deliverableIdentifier = nil;
    self.deliverable = nil;
	
	self.iconUrl = nil;
	self.screenshotUrl = nil;
	self.storePayloadUrl = nil;
    
    OFSafeRelease(startVersion);
    OFSafeRelease(endVersion);

	[super dealloc];
}

#pragma mark -
#pragma mark NSObject
#pragma mark -

- (BOOL)isEqual:(id)object
{
	if ([object isKindOfClass:[OFInAppPurchase class]])
		return [self.resourceId isEqualToString:[object resourceId]];
	
	return NO;
}

- (NSUInteger)hash
{
	return [self.resourceId hash];
}

#pragma mark -
#pragma mark Property Methods
#pragma mark -

- (NSString*)formattedPrice
{
    NSString* price = kUnknownPriceString;
    
    if (storeKitProduct != nil)
    {
        NSNumberFormatter* numberFormatter = [[[NSNumberFormatter alloc] init] autorelease];
        [numberFormatter setFormatterBehavior:NSNumberFormatterBehavior10_4];
        [numberFormatter setNumberStyle:NSNumberFormatterCurrencyStyle];
        [numberFormatter setLocale:storeKitProduct.priceLocale];
        price = [numberFormatter stringFromNumber:storeKitProduct.price];
    }
    else if ([storeKitProductIdentifier length] > 0)
    {
		if ([cachedStoreKitPrice length] > 0)
		{
			price = [NSString stringWithString:cachedStoreKitPrice];
		}
		else
		{
			price = kUnknownPriceString;
		}
    }
    else if (self.purchaseCurrency != nil && purchaseCurrencyAmount > 0)
    {
        price = [NSString stringWithFormat:@"%d %@", purchaseCurrencyAmount, self.purchaseCurrency.name];
    }
    else
    {
        price = @"Free";
    }
    
    return price;
}

- (BOOL)isFree
{
	if (storeKitProduct == nil && 
		[storeKitProductIdentifier length] == 0 &&
		(self.purchaseCurrency == nil ||
		purchaseCurrencyAmount == 0))
	{
		return YES;
	}
	else 
	{
		return NO;
	}
}

- (NSString*)itemIdentifier
{
    if (self.deliverable.currencyIdentifier != nil)
        return self.deliverable.currencyIdentifier;
    else
        return self.deliverable.identifier;
}

- (void)setCategoryIdentifier:(NSString *)_categoryIdentifier
{
    if (!_categoryIdentifier)
        _categoryIdentifier = [[OFSkuCategory uncategorized] name];
        
    OFSafeRelease(categoryIdentifier);
    categoryIdentifier = [_categoryIdentifier retain];
}

- (OFCurrency*)purchaseCurrency
{
	if (!purchaseCurrency)
	{
		purchaseCurrency = [[[OFInAppPurchaseCatalog currencies] objectForKey:self.purchaseCurrencyIdentifier] retain];
	}
	
	return purchaseCurrency;
}

- (OFDeliverable*)deliverable
{
	if (!deliverable)
	{
		deliverable = [[[OFInAppPurchaseCatalog deliverablesInternal] objectForKey:self.deliverableIdentifier] retain];
	}
	
	return deliverable;
}

-(OFSkuCategory*)category 
{
    if(!category) 
    {
        for(OFSkuCategory* cat in [OFInAppPurchaseCatalog categoriesInternal]) {
            if([cat.name isEqualToString:self.categoryIdentifier]) {
                category = [cat retain];
            }
        }
        if(!category) category = [[OFSkuCategory uncategorized] retain];
    }
    return category;
}

-(NSInteger) sortByPosition:(OFInAppPurchase*) rhs {
    if(self.position < rhs.position) return NSOrderedAscending;
    if(self.position > rhs.position) return NSOrderedDescending;
    return NSOrderedSame; //which shouldn't happen
}


#pragma mark -
#pragma mark OFStoreKitPurchasable
#pragma mark -

- (void)receivedStoreKitProductInformation:(SKProduct*)product
{
	self.storeKitProduct = product;

	if ([storeItemTitle length] == 0)
		self.storeItemTitle = product.localizedTitle;
		
	if ([storeItemDescription length] == 0)
		self.storeItemDescription = product.localizedDescription;
	
	self.cachedStoreKitPrice = [self formattedPrice];
}

#pragma mark -
#pragma mark Purchasing Logic
#pragma mark -

- (void)purchase
{
    OFInAppPurchaseStatus status;
    BOOL isPurchasable = [self isPurchasable:&status];

    if (isPurchasable)
    {
        if ([storeKitProductIdentifier length] > 0)
        {
            [[OFXStore storeKit] purchase:self];
        }
        else
        {
            [self purchaseSucceeded];
        }
    }
    else
    {
        [self purchaseFailed:status];
    }
}

- (BOOL)isPurchasable:(OFInAppPurchaseStatus*)reason
{
    OFInAppPurchaseStatus status = OFInAppPurchase_PurchaseSucceeded;

	if (![OFInventory canHaveMore:self.deliverable.identifier])
    {
        status = OFInAppPurchase_AlreadyOwned;
    }
    else if ([storeKitProductIdentifier length] > 0)
	{
        if ([[OFXStore storeKit] canMakePurchases])
        {
            if (storeKitProduct != nil)
            {
                status = OFInAppPurchase_PurchaseSucceeded;
            }
            else
            {
                status = OFInAppPurchase_ProductUnavailable;
            }
        }
        else
        {
            status = OFInAppPurchase_PurchasingDisabled;
        }
	}
	else
	{
		BOOL hasCurrencyCost = purchaseCurrencyIdentifier && purchaseCurrencyAmount > 0;
		BOOL canAfford = purchaseCurrencyAmount <= [OFInventory amountOfCurrency:purchaseCurrencyIdentifier];
		
		if (!hasCurrencyCost || (hasCurrencyCost && canAfford))
        {
            status = OFInAppPurchase_PurchaseSucceeded;
        }
		else
        {
            status = OFInAppPurchase_InsufficientFunds;
        }
	}
	
	if (status == OFInAppPurchase_PurchaseSucceeded &&
		self.deliverable.payloadDownloadRequired &&
		![OFReachability isConnectedToInternet])
	{
		status = OFInAppPurchase_MustBeOnline;
	}
    
    if (reason != nil)
        *reason = status;
    
    return (status == OFInAppPurchase_PurchaseSucceeded);
}

- (void)purchaseSucceeded
{
	[OFInventory applyInAppPurchase:self];
    [self invokeDelegate:OFInAppPurchase_PurchaseSucceeded];
}

- (void)purchaseFailed:(OFInAppPurchaseStatus)status
{
    [self invokeDelegate:status];
}

- (void)storeKitPurchaseCompleted:(OFStoreKitPurchaseStatus)status
{
    [self invokeDelegate:[self inAppPurchaseStatusFromStoreKitPurchaseStatus:status]];
}

+(NSString*)pathForUrl:(NSURL*) url {
    return [@"ofx_offline_cache" stringByAppendingPathComponent:url.path];
}

- (void)loadIcon {
    if(self.iconUrl)
        [[OFXStore downloader] downloadUrl:self.iconUrl toPath:[OFInAppPurchase pathForUrl:self.iconUrl] delegate:self userInfo:nil];
    else {
        if([sharedDelegate respondsToSelector:@selector(inAppPurchase:iconLoaded:)]) 
            [sharedDelegate inAppPurchase:self iconLoaded:nil];
    }
}

- (void)loadScreenshot {
    if(self.screenshotUrl) {
        [[OFXStore downloader] downloadUrl:self.screenshotUrl toPath:[OFInAppPurchase pathForUrl:self.screenshotUrl] delegate:self userInfo:nil];
    }
    else {
        if([sharedDelegate respondsToSelector:@selector(inAppPurchase:screenshotLoaded:)]) 
            [sharedDelegate inAppPurchase:self screenshotLoaded:nil];
    }
}

- (void)loadStorePayload {
    if(self.storePayloadUrl) {
        [[OFXStore downloader] downloadUrl:self.storePayloadUrl toPath:nil delegate:self 
                                    userInfo:[NSDictionary dictionaryWithObject:@"ZIP" forKey:OFContentDownloadKey_ContentType]];
    }
    else {
        if([sharedDelegate respondsToSelector:@selector(inAppPurchase:storePayloadFinishedLoading:)])
            [sharedDelegate inAppPurchase:self storePayloadFinishedLoading:YES];
    }
}

- (OFItemPayloadStatus)itemPayloadStatus
{
    return self.deliverable.payloadStatus;
}

#pragma mark -
#pragma mark Private Methods
#pragma mark -

- (void)contentDownloader:(OFContentDownloader*)downloader didStartDownload:(NSDictionary*)userInfo {
    NSURL*loadUrl = [userInfo objectForKey:OFContentDownloadKey_OriginalURL];
    if([loadUrl isEqual:self.storePayloadUrl]) {
        if([sharedDelegate respondsToSelector:@selector(inAppPurchaseStorePayloadStartedLoading:)]) {
            [sharedDelegate inAppPurchaseStorePayloadStartedLoading:self];
        }
    }
}

- (void)contentDownloader:(OFContentDownloader*)downloader updatedProgress:(CGFloat)progress {
    if ([downloader urlInProgress:storePayloadUrl])
    {
        float storePayloadProgress = [downloader progressOfUrl:storePayloadUrl];
        if([sharedDelegate respondsToSelector:@selector(inAppPurchase:storePayloadUpdatedProgress:)]) {
            [sharedDelegate inAppPurchase:self storePayloadUpdatedProgress:storePayloadProgress];
        }
    }
}

- (void)contentDownloader:(OFContentDownloader*)downloader didFinishDownload:(NSDictionary*)userInfo {
    NSURL*loadUrl = [userInfo objectForKey:OFContentDownloadKey_OriginalURL];
    if([loadUrl isEqual:self.iconUrl]) {
        if([sharedDelegate respondsToSelector:@selector(inAppPurchase:iconLoaded:)]) 
            [sharedDelegate inAppPurchase:self iconLoaded:[UIImage imageWithContentsOfFile:[userInfo objectForKey:OFContentDownloadKey_InstallRoot]]];
    }
    else if([loadUrl isEqual:self.screenshotUrl]) {
        if([sharedDelegate respondsToSelector:@selector(inAppPurchase:screenshotLoaded:)]) 
            [sharedDelegate inAppPurchase:self screenshotLoaded:[UIImage imageWithContentsOfFile:[userInfo objectForKey:OFContentDownloadKey_InstallRoot]]];
    }
    else if([loadUrl isEqual:self.storePayloadUrl]) {
        if([sharedDelegate respondsToSelector:@selector(inAppPurchase:storePayloadFinishedLoading:)]) 
            [sharedDelegate inAppPurchase:self storePayloadFinishedLoading:[[OFXStore downloader] urlIsCached:loadUrl]];
    }
}

- (NSString*)descriptionForPurchaseStatus:(OFInAppPurchaseStatus)status
{
	switch (status)
	{
		case OFInAppPurchase_PurchaseSucceeded:		return @"Success"; break;
		case OFInAppPurchase_InsufficientFunds:		return @"Insufficient Funds"; break;
		case OFInAppPurchase_PurchaseFailed:		return @"Failed"; break;
		case OFInAppPurchase_PurchaseCancelled:		return @"Cancelled"; break;
		case OFInAppPurchase_PurchasingDisabled:	return @"Disabled"; break;
		case OFInAppPurchase_ProductUnavailable:	return @"Unavailable"; break;
        case OFInAppPurchase_AlreadyOwned:          return @"Already Owned"; break;
        case OFInAppPurchase_MustBeOnline:          return @"Must Be Online"; break;
	}
	
	return @"None";
}

- (OFInAppPurchaseStatus)inAppPurchaseStatusFromStoreKitPurchaseStatus:(OFStoreKitPurchaseStatus)status
{
	switch (status)
	{
		case OFStoreKitPurchaseStatus_Completed:				return OFInAppPurchase_PurchaseSucceeded; break;
		case OFStoreKitPurchaseStatus_Failed:					return OFInAppPurchase_PurchaseFailed; break;
		case OFStoreKitPurchaseStatus_Cancelled:				return OFInAppPurchase_PurchaseCancelled; break;
		case OFStoreKitPurchaseStatus_Disabled:					return OFInAppPurchase_PurchasingDisabled; break;
		case OFStoreKitPurchaseStatus_Invalidated:				return OFInAppPurchase_PurchaseFailed; break;
	}
	
	return OFInAppPurchase_PurchaseFailed;
}

- (void)invokeDelegate:(OFInAppPurchaseStatus)status
{
    if (status == OFInAppPurchase_PurchaseSucceeded)
    {
        if ([sharedDelegate respondsToSelector:@selector(inAppPurchaseSucceeded:)])
        {
            [sharedDelegate inAppPurchaseSucceeded:self];
        }
    }
    else
    {
        if ([sharedDelegate respondsToSelector:@selector(inAppPurchase:purchaseFailedWithStatus:)])
        {
            [sharedDelegate inAppPurchase:self purchaseFailedWithStatus:status];
        }
    }
}

#pragma mark -
#pragma mark OFResource
#pragma mark -

AUTOREGISTER_CLASS_WITH_OFJSONCODER

+ (NSString*)classNameForJsonCoding
{
	return @"sku";
}

+ (void)registerJsonValueTypesForDecoding:(NSMutableDictionary*)valueMap
{
	[super registerJsonValueTypesForDecoding:valueMap];

	[valueMap setObject:[OFJsonObjectValue valueWithSelector:@selector(setStoreItemTitle:)] forKey:@"title"];
	[valueMap setObject:[OFJsonObjectValue valueWithSelector:@selector(setStoreItemDescription:)] forKey:@"description"];
	[valueMap setObject:[OFJsonIntegerValue valueWithSelector:@selector(setQuantity:)] forKey:@"quantity"];
	[valueMap setObject:[OFJsonObjectValue valueWithSelector:@selector(setCategoryIdentifier:)] forKey:@"category"];
    [valueMap setObject:[OFJsonIntegerValue valueWithSelector:@selector(setPosition:)] forKey:@"position"];
    
	[valueMap setObject:[OFJsonObjectValue valueWithSelector:@selector(setStoreKitProductIdentifier:)] forKey:@"apple_product_identifier"];
    [valueMap setObject:[OFJsonBoolValue valueWithSelector:@selector(setStoreKitNonConsumable:)] forKey:@"nonconsumable"];
	[valueMap setObject:[OFJsonObjectValue valueWithSelector:@selector(setCachedStoreKitPrice:)] forKey:@"__cached_store_kit_price__"];

	[valueMap setObject:[OFJsonIntegerValue valueWithSelector:@selector(setPurchaseCurrencyAmount:)] forKey:@"cost"];
	[valueMap setObject:[OFJsonObjectValue valueWithSelector:@selector(setPurchaseCurrencyIdentifier:)] forKey:@"currency_identifier"];

	[valueMap setObject:[OFJsonObjectValue valueWithSelector:@selector(setDeliverableIdentifier:)] forKey:@"deliverable_identifier"];

	[valueMap setObject:[OFJsonUrlValue valueWithSelector:@selector(setIconUrl:)] forKey:@"icon_url"];
	[valueMap setObject:[OFJsonUrlValue valueWithSelector:@selector(setScreenshotUrl:)] forKey:@"screenshot_url"];
	[valueMap setObject:[OFJsonUrlValue valueWithSelector:@selector(setStorePayloadUrl:)] forKey:@"catalog_data_url"];
    
    [valueMap setObject:[OFJsonObjectValue valueWithSelector:@selector(setStartVersion:)] forKey:@"start_version"];
    [valueMap setObject:[OFJsonObjectValue valueWithSelector:@selector(setEndVersion:)] forKey:@"end_version"];
}

- (void)encodeWithJsonCoder:(OFJsonCoder*)coder
{
	[super encodeWithJsonCoder:coder];
	
	[coder encodeObject:storeItemTitle withKey:@"title"];
	[coder encodeObject:storeItemDescription withKey:@"description"];	
	[coder encodeInteger:quantity withKey:@"quantity"];
	[coder encodeObject:categoryIdentifier withKey:@"category"];
    [coder encodeInteger:position withKey:@"position"];
	
	[coder encodeObject:storeKitProductIdentifier withKey:@"apple_product_identifier"];
    [coder encodeBool:storeKitNonConsumable withKey:@"nonconsumable"];
	[coder encodeObject:cachedStoreKitPrice withKey:@"__cached_store_kit_price__"];

	[coder encodeInteger:purchaseCurrencyAmount withKey:@"cost"];
	[coder encodeObject:purchaseCurrencyIdentifier withKey:@"currency_identifier"];

	[coder encodeObject:deliverableIdentifier withKey:@"deliverable_identifier"];

	[coder encodeUrl:iconUrl withKey:@"icon_url"];
	[coder encodeUrl:screenshotUrl withKey:@"screenshot_url"];
	[coder encodeUrl:storePayloadUrl withKey:@"catalog_data_url"];
    
    [coder encodeObject:startVersion withKey:@"start_version"];
    [coder encodeObject:endVersion withKey:@"end_version"];
}

#pragma mark Testing

//type 0=1 SWAG, 1=Apple Id, 2=Apple NonConsumable, 3=Free, 4=Fake NC
+(OFInAppPurchase*) testFixtureWithUnique:(BOOL) unique type:(NSInteger) type {
    static NSInteger sPosition = 100;
    NSString* typeStrings[] = { @"SWAG", @"Apple Consumable", @"Apple NonConsume", @"Freebie", @"Fake NC" };
    OFInAppPurchase* sku = [[OFInAppPurchase new] autorelease];
    sku.storeItemTitle = [NSString stringWithFormat:@"Fixture: %@ %@", typeStrings[type], unique ? @"UNIQUE" : @""];
    sku.storeItemDescription = [NSString stringWithFormat:@"Desc: %@", sku.storeItemTitle];
    sku.quantity = 1;
    sku.categoryIdentifier = @"FIXTURES";

    switch(type) {
        case 0:
            sku.purchaseCurrencyIdentifier = @"FIXTURE";
            sku.purchaseCurrencyAmount = 1;
            break;
        case 1:
            sku.storeKitProductIdentifier = @"com.aurorafeint.ofxdev.testitem7";
            sku.storeKitNonConsumable = NO;
            break;
        case 2:
            sku.storeKitProductIdentifier = @"com.aurorafeint.ofxdev.testitem6";
            sku.storeKitNonConsumable = YES;
            break;
        case 4:    
            sku.storeKitNonConsumable = YES;
    }
    
    sku.deliverableIdentifier = unique ? @"FIXTURE_UNIQUE" : @"FIXTURE_NONUNIQUE";
    sku.position = sPosition++;
    return sku;
}

+(NSArray*) testFixtures {
    return [NSArray arrayWithObjects:
            [self testFixtureWithUnique:YES type:0],
            [self testFixtureWithUnique:YES type:1],
            [self testFixtureWithUnique:YES type:2],
            [self testFixtureWithUnique:YES type:3],
            [self testFixtureWithUnique:YES type:4],
            [self testFixtureWithUnique:NO type:0],
            [self testFixtureWithUnique:NO type:1],
            [self testFixtureWithUnique:NO type:3], nil];
}

@end
