//  Copyright 2009-2010 Aurora Feint, Inc.
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

#import "OpenFeintX.h"
#import "OFXStore+Internal.h"
#import "OFXStore+Private.h"
#import "OFXStoreSettings.h"
#import "OFXDebug.h"

#import "OpenFeint/OpenFeint+AddOns.h"
#import "OpenFeint/OpenFeint+Private.h"
#import "OpenFeint/OpenFeint+UserOptions.h"
#import "OpenFeint/OpenFeint+UserStats.h"
#import "OpenFeint/OFSettings.h"
#import "OpenFeint/OFReachability.h"

// [adill] @hack -- imports for horrible hack
#import "OpenFeint/OFRootController.h"
#import "OpenFeint/OFEnableOpenFeintInDashboardController.h"
#import "OpenFeint/OFTabBarController.h"
#import "OpenFeint/OFIntroNavigationController.h"

#import "OFStoreKit.h"
#import "OFContentDownloader.h"
#import "OpenFeint/OFSession.h"
#import "OFInventory.h"
#import "OFInAppPurchaseCatalog.h"
#import "OpenFeint/OFControllerLoaderObjC.h"

static OFXStore* sharedInstance = nil;
static BOOL hasSeenPostIntroCallbacks = NO;
static BOOL shouldInitializeInventoryInPostIntroCallbacks = NO;
static NSString* hasSeenStoreNag = @"OFXStoreManager_UserSettings_HasEnteredStore";


@interface OFXStore ()
@property (nonatomic, retain) OFStoreKit* storeKit;
@property (nonatomic, retain) OFContentDownloader* downloader;
@property (nonatomic, assign) id<OFXStoreDelegate> delegate;
@property (nonatomic, retain) NSString* rootPathForPayloads;
@end

@implementation OFXStore

@synthesize storeKit;
@synthesize downloader;
@synthesize delegate;
@synthesize rootPathForPayloads;

#pragma mark -
#pragma mark Life-Cycle
#pragma mark -

- (id) initUsingCachesDirectory:(BOOL)usingCachesDirectory
{
	self = [super init];
	if (self != nil)
	{
		storeKit = [OFStoreKit new];
        if(usingCachesDirectory)
        {
            //move Legacy data
            NSString* oldRoot = [OFSettings documentsPathForFile:@"ofx_offline_cache"];
            NSFileManager* manager = [NSFileManager defaultManager];
            if([manager fileExistsAtPath:oldRoot])
            {
                NSString* newCacheLocation = [OFSettings savePathForFile:@"dlc/ofx_offline_cache"];
                [manager createDirectoryAtPath:[OFSettings savePathForFile:@"dlc"] withIntermediateDirectories:YES attributes:nil error:nil];
                [manager moveItemAtPath:oldRoot toPath:newCacheLocation error:nil];
            }
            rootPathForPayloads = [[[OFSettings cachingPathRoot] stringByAppendingPathComponent:@"dlc"] retain];
        }
        else
        {
            rootPathForPayloads = [[OFSettings documentsPathForFile:nil] retain];
        }
        downloader = [[OFContentDownloader alloc] initWithFile:rootPathForPayloads];
        [[NSNotificationCenter defaultCenter] addObserver:self selector:@selector(storeKitRestoreFinished:) name:OFStoreKitRestoreFinished object:storeKit];
	}
	
	return self;
}


- (void)dealloc
{
    [rootPathForPayloads release];
    [[NSNotificationCenter defaultCenter] removeObserver:self];
	OFSafeRelease(storeKit);
	OFSafeRelease(downloader);
	[super dealloc];
}

#pragma mark -
#pragma mark Public Class Methods
#pragma mark -

+ (OFStoreKit*)storeKit
{
	return sharedInstance.storeKit;
}

+ (OFContentDownloader*)downloader
{
	return sharedInstance.downloader;
}

+ (NSString*)version
{
	return @"2.0.6";
}

+ (id<OFXStoreDelegate>)delegate
{
    return sharedInstance.delegate;
}


#pragma mark -
#pragma mark Notifications
#pragma mark -

- (void)storeKitRestoreFinished:(NSNotification*)notification
{
	if ([[OFXStore delegate] respondsToSelector:@selector(storeKitNonconsumableRestoreFinished)])
		[[OFXStore delegate] storeKitNonconsumableRestoreFinished];
}

#pragma mark -
#pragma mark Notification forwards
#pragma mark -

+ (void)applicationWillEnterForegroundNotification
{
    [OFInventory synchronizeInventory];
}

+ (void)applicationWillResignActive
{
    [OFInventory storeInventory];
}

+ (void)applicationWillTerminate
{
    [OFInventory storeInventory];
}


#pragma mark -
#pragma mark OFSessionObserver
#pragma mark -

- (void)session:(OFSession*)session didLoginUser:(OFUser*)user previousUser:(OFUser*)previousUser
{
	// [adill] @hack -- if OF root controller is up AND it has a tab bar controller
	// AND that tab controller has the OFEnableOpenFeintInDashboardController AND
	// that guy still supports _switchToOnline mode then call it. We know we're logged in, damnit.
	// ...please forgive me... orz
	OFRootController* rootController = (OFRootController*)[OpenFeint getRootController];
	if ([rootController isKindOfClass:[OFRootController class]])
	{
		UIViewController* topController = rootController.contentController;
		if([topController isKindOfClass:[OFTabBarController class]])
		{
			OFTabBarController* tabController = (OFTabBarController*)topController;
			id selected = [[tabController tabBar] selectedViewController];
			if ([selected isKindOfClass:[UINavigationController class]])
			{
				UIViewController* top = [selected topViewController];
				if ([top isKindOfClass:[OFEnableOpenFeintInDashboardController class]])
				{
					if ([top respondsToSelector:@selector(_switchToOnlineMode)])
					{
						[top performSelector:@selector(_switchToOnlineMode)];
					}
				}
			}
		}
        
        // [adill] @hack -- again, if we're in the intro flow when the session changes we **do not** want
        // to initialize the inventory. that would cause us to award inv0 to a potentially temporary
        // account. we'll instead initialize in our postIntro callbacks. this hack should absolutely
        // be eliminated after we refactor the hell out of the intro flow such that it doesn't LOGIN
        // multiple times!!
        if ([rootController.contentController isKindOfClass:[OFIntroNavigationController class]] ||
            [rootController.nonFullscreenModalController isKindOfClass:[OFIntroNavigationController class]] ||
            [rootController.modalViewController isKindOfClass:[OFIntroNavigationController class]])
        {
            shouldInitializeInventoryInPostIntroCallbacks = !hasSeenPostIntroCallbacks;
        }
	}

    if (!shouldInitializeInventoryInPostIntroCallbacks)
    {
	[OFInventory initializeInventory];
        hasSeenPostIntroCallbacks = NO;
    }
}

- (void)session:(OFSession*)session didLogoutUser:(OFUser*)user
{
	[OFInventory initializeInventory];
}

#pragma mark -
#pragma mark OpenFeintAddOn forwards
#pragma mark -

+ (void)initializeAddOn:(NSDictionary*)settings
{

    NSNumber* shouldUseCachesDirectory = [settings objectForKey:OFXStore_Setting_StoreOFXDataInCacheDirectory];
    if(!shouldUseCachesDirectory)
    {
        [[NSException exceptionWithName:@"OFXMissingRequiredSetting" 
                                 reason:@"You must set the setting 'OFXStore_Setting_StoreOFXDataInCacheDirectory'" 
          "to inform OpenFeintX whether it should put item payloads in ~/Library/Caches/ (iCloud preferred) "
          "or ~/Documents/ (previous default). When loading item payloads please use [OFXStore rootPathForPayloads] "
          "rather than a specific, hardcoded root path." userInfo:nil] raise];
    }
    sharedInstance = [[OFXStore alloc] initUsingCachesDirectory:[shouldUseCachesDirectory boolValue]];    
    sharedInstance.delegate = [settings objectForKey:OFXStore_Setting_Delegate];
    NSLog(@"Initialized OFXStore version %@.", [OFXStore version]);
    
    [OFInventory initializeInventory];
    
    if ([[self delegate] respondsToSelector:@selector(migrateLegacyInventoryData)])
    {
        [[self delegate] migrateLegacyInventoryData];
        [OFInventory storeInventory];
    }

	[OFInAppPurchaseCatalog initializeCatalog];

	NSNumber* autoUpdateCatalog = [settings objectForKey:OFXStore_Setting_AutomaticallyUpdateIAPCatalog];
	if (!autoUpdateCatalog || [autoUpdateCatalog boolValue])
    {
        [OFInAppPurchaseCatalog updateCatalogFromServer];
    }
    
    [OFControllerLoaderObjC setOverrideAssetFileSuffix:@"Ofx"];
    [OFControllerLoaderObjC setOverrideClassNamePrefix:@"OFX"];
	
	[[OpenFeint session] addObserver:sharedInstance];
}

+ (void)shutdownAddOn
{
	[[OpenFeint session] removeObserver:sharedInstance];
    [OFInventory storeInventory];
    [OFInAppPurchaseCatalog shutdownCatalog];
	OFSafeRelease(sharedInstance);
}

// [adill] @hack -- this goes along with the hack in session:didLoginUser:previousUser:
+ (void)userLoggedInPostIntro
{
    if (shouldInitializeInventoryInPostIntroCallbacks)
    {
        [OFInventory initializeInventory];
        shouldInitializeInventoryInPostIntroCallbacks = NO;
        hasSeenPostIntroCallbacks = NO;
    }
    else if (!hasSeenPostIntroCallbacks)
    {
        hasSeenPostIntroCallbacks = YES;        
    }
}

// [adill] @hack -- this goes along with the hack in session:didLoginUser:previousUser:
+ (void)offlineUserLoggedInPostIntro
{
    if (shouldInitializeInventoryInPostIntroCallbacks)
    {
        [OFInventory initializeInventory];
        shouldInitializeInventoryInPostIntroCallbacks = NO;
        hasSeenPostIntroCallbacks = NO;
    }
    else if (!hasSeenPostIntroCallbacks)
    {
        hasSeenPostIntroCallbacks = YES;
    }
}

+(void)setUserHasSeenStoreNag {
    [[NSUserDefaults standardUserDefaults] setObject:@"YES" forKey:hasSeenStoreNag];
}
+(BOOL)userHasSeenStoreNag {
    return [[NSUserDefaults standardUserDefaults] stringForKey:hasSeenStoreNag].length > 0;
}

+ (NSString*)_rootPathForPayloads
{
    return sharedInstance.rootPathForPayloads;
}

@end



