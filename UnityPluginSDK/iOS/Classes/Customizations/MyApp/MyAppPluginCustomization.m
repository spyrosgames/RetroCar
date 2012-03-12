//
// Copyright 2011 GREE International, Inc.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//    http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//

#import "MyAppPluginCustomization.h"
#import "GreeSynthesizeSingleton.h"
#import "GreeUnityBridge.h"

// OpenFeint
#import <OpenFeint/OpenFeint.h>

// Plugins
#import "GreePluginOpenFeintX+FromUnity.h"
#import "GreePluginOpenFeint+FromUnity.h"


@implementation MyAppPluginCustomization

@synthesize pluginOpenFeint             = _pluginOpenFeint;
@synthesize pluginOpenFeintX            = _pluginOpenFeintX;

@synthesize pluginOpenFeintToUnity      = _pluginOpenFeintToUnity;
@synthesize pluginOpenFeintXToUnity     = _pluginOpenFeintXToUnity;

GREE_SYNTHESIZE_SINGLETON_FOR_CLASS(MyAppPluginCustomization)


-(void)dealloc {
    
    self.pluginOpenFeintToUnity = nil;
    self.pluginOpenFeintXToUnity = nil;
    
    self.pluginOpenFeint = nil;
    self.pluginOpenFeintX = nil;
    
    [super dealloc];
}

- (void)configurationDidLoad {
    
    _pluginOpenFeintX = [[GreePluginOpenFeintX alloc] initWithOptions:_configuration];
    _pluginOpenFeintXToUnity = [[GreePluginOpenFeintXToUnity alloc] init];
    _pluginOpenFeintX.pluginOFXDelegate = _pluginOpenFeintXToUnity;  
    _pluginOpenFeintToUnity = [[GreePluginOpenFeintToUnity alloc] init];
    _pluginOpenFeintX.pluginOFDelegate = _pluginOpenFeintToUnity;
    
    [[GreeUnityBridge sharedGreeUnityBridge:nil] addBridgeMethodsOfInstance:_pluginOpenFeintX];
        
    [OpenFeint respondToApplicationLaunchOptions:[self launchOptions]];
}

- (void)willResignActive {
    
}

- (void)didEnterBackground {
    
}

- (void)willEnterForground {
    
}

- (void)didBecomeActive {
    
}

- (void)didRegisterForRemoteNotificationsWithDeviceToken:(NSData*)deviceToken
{
    [OpenFeint applicationDidRegisterForRemoteNotificationsWithDeviceToken:deviceToken];
    [[NSNotificationCenter defaultCenter] postNotificationName:@"UIApplicationDidRegisterForRemoteNotificationsNotification" object:deviceToken];
}

- (void)didFailToRegisterForRemoteNotificationsWithError:(NSError*)error
{
    OFLog(@"Failed to register for remote notifications with error: %@", [error localizedDescription]);
    [OpenFeint applicationDidFailToRegisterForRemoteNotifications];
    [[NSNotificationCenter defaultCenter] postNotificationName:@"UIApplicationDidFailedToRegisterForRemoteNotificationsNotification" object:error];
}

- (void)didReceiveRemoteNotification:(NSDictionary *)userInfo {
    [OpenFeint applicationDidReceiveRemoteNotification:userInfo];
}

- (void)didReceiveLocalNotification:(UILocalNotification *)notification {
    
}

- (void)willTerminate {
    [_pluginOpenFeintX shutDownOpenFeint];
}

@end