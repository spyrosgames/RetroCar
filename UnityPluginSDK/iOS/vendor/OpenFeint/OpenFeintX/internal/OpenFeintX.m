//  Copyright 2011 Aurora Feint, Inc.
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
#import "OpenFeint/OpenFeint+AddOns.h"
#import "OpenFeint/OpenFeint+Private.h"
#import "OpenFeint/OpenFeint+UserOptions.h"
#import "OpenFeint/OpenFeint+UserStats.h"
#import <Foundation/NSObjCRuntime.h>
#import "OpenFeint/OFSettings.h"
#import "OpenFeint/OFEventLog.h"
#import "OpenFeint/OFSessionInfo.h"

#define FORWARD_TO_CLASS(klass) \
    if (klass != nil && [(id)klass respondsToSelector:_cmd]) \
    { \
        [klass performSelector:_cmd]; \
    }

#define FORWARD_TO_CLASS_WITH_PARAM(klass, param) \
    if (klass != nil && [(id)klass respondsToSelector:_cmd]) \
    { \
        [klass performSelector:_cmd withObject:param]; \
    }

static OpenFeintX* sharedInstance;

@interface OpenFeintX()
@property (nonatomic, assign) Class storeClass;
@property (nonatomic, retain) OFEventLog* eventLog;
@end

@implementation OpenFeintX

@synthesize storeClass;
@synthesize eventLog;

OPENFEINT_AUTOREGISTER_ADDON

- (id)init
{
    if((self = [super init]))
    {
        storeClass = NSClassFromString(@"OFXStore");
        eventLog = [[OFEventLog eventLog:@"ofx"] retain];
        [eventLog addObserver:self];

		[OFReachability addObserver:self];

        [[NSNotificationCenter defaultCenter] addObserver:self selector:@selector(applicationWillResignActive) name:UIApplicationWillResignActiveNotification object:nil];
        [[NSNotificationCenter defaultCenter] addObserver:self selector:@selector(applicationWillTerminate) name:UIApplicationWillTerminateNotification object:nil];
		BOOL isForegroundNotificationPresent = (&UIApplicationWillEnterForegroundNotification != NULL);
		if (isForegroundNotificationPresent)
		{
			[[NSNotificationCenter defaultCenter]	addObserver:self
                                                     selector:@selector(applicationWillEnterForegroundNotification)
                                                         name:UIApplicationWillEnterForegroundNotification
                                                       object:nil];
		}
    
    
    
    }
    return self;
}

- (void)dealloc
{
    [[NSNotificationCenter defaultCenter] removeObserver:self];
    [eventLog release];
	[OFReachability removeObserver:self];
    [super dealloc];
}

#pragma mark -
#pragma mark Notifications
#pragma mark -

- (void)eventLog:(OFEventLog*)log willLogEvent:(NSString*)eventName parameters:(NSMutableDictionary*)parameters
{
	[parameters setObject:[[OFSessionInfo sharedInstance] sessionId] forKey:@"session_id"];
	[parameters setObject:[OpenFeint lastLoggedInUserId] forKey:@"user_id"];
	[parameters setObject:[NSNumber numberWithInteger:[OpenFeint totalGameSessionsDuration]] forKey:@"total_play_time"];
	[parameters setObject:[NSNumber numberWithInteger:(NSInteger)[[NSDate date] timeIntervalSinceDate:[[OFSessionInfo sharedInstance] sessionStartDate]]] forKey:@"session_length"];
	
	if ([[NSUserDefaults standardUserDefaults] boolForKey:@"___OFX__SAMPLE__APP___"])
	{
		[parameters setObject:[NSNumber numberWithBool:YES] forKey:@"from_sample_app"];
	}
}

- (void)reachabilityChangedFrom:(OFReachabilityStatus)oldStatus to:(OFReachabilityStatus)newStatus
{
	if (newStatus != OFReachability_Not_Connected && oldStatus == OFReachability_Not_Connected)
		[sharedInstance.eventLog upload];
}

- (void)applicationWillEnterForegroundNotification
{
    FORWARD_TO_CLASS(storeClass)

	if ([OFReachability isConnectedToInternet])
		[[OpenFeintX eventLog] upload];
}

- (void)applicationWillResignActive
{
    FORWARD_TO_CLASS(storeClass)

    [self.eventLog serializeToDisk];
}

- (void)applicationWillTerminate
{
    FORWARD_TO_CLASS(storeClass)

    [self.eventLog serializeToDisk];
}

#pragma mark -
#pragma mark OpenFeintAddOns
#pragma mark -

+ (void)initializeAddOn:(NSDictionary*)settings
{
    sharedInstance = [OpenFeintX new];
    
    FORWARD_TO_CLASS_WITH_PARAM(sharedInstance.storeClass, settings)
    
    NSDictionary* eventLogParameters = [NSDictionary dictionaryWithObjectsAndKeys:
                                        [NSNumber numberWithBool:(sharedInstance.storeClass != nil)], @"usingStore",
                                        nil];                                        
    [sharedInstance.eventLog logEventNamed:@"init" parameters:eventLogParameters];
    
}

+ (void)shutdownAddOn
{
    FORWARD_TO_CLASS(sharedInstance.storeClass)
	[sharedInstance.eventLog serializeToDisk];
    [sharedInstance release];
}

+ (OFEventLog*) eventLog
{
    return sharedInstance.eventLog;
}


+ (void)loadSettings:(OFSettings*)settings fromParser:(OFSettingsParser*) parser
{
}

+ (void)userLoggedIn
{
}

+ (void)userLoggedOut
{
}
    
+ (void)userLoggedInPostIntro
{
    FORWARD_TO_CLASS(sharedInstance.storeClass)
}

+ (void)offlineUserLoggedInPostIntro
{
    FORWARD_TO_CLASS(sharedInstance.storeClass)
}

+ (BOOL)respondToPushNotification:(NSDictionary*)notificationInfo duringApplicationLaunch:(BOOL)duringApplicationLaunch
{
	return NO;
}

+ (void)defaultSettings:(OFSettings *)settings
{
    
}

@end
