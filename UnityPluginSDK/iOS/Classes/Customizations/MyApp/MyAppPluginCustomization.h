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

#import "GreeUnityBridgeConfiguration.h"

#import "GreePluginConfigurationDelegate.h"

#import "GreePluginOpenFeint.h"
#import "GreePluginOpenFeintToUnity.h"
#import "GreePluginOpenFeintX.h"
#import "GreePluginOpenFeintXToUnity.h"

@interface MyAppPluginCustomization : GreeUnityBridgeConfiguration {
    
    GreePluginOpenFeint                     *_pluginOpenFeint;
    GreePluginOpenFeintX                    *_pluginOpenFeintX;
    
    GreePluginOpenFeintToUnity              *_pluginOpenFeintToUnity;
    GreePluginOpenFeintXToUnity             *_pluginOpenFeintXToUnity;
}

@property (nonatomic, retain) GreePluginOpenFeint                   *pluginOpenFeint;
@property (nonatomic, retain) GreePluginOpenFeintX                  *pluginOpenFeintX;

@property (nonatomic, retain) GreePluginOpenFeintToUnity            *pluginOpenFeintToUnity;
@property (nonatomic, retain) GreePluginOpenFeintXToUnity           *pluginOpenFeintXToUnity;

+ (MyAppPluginCustomization *)sharedMyAppPluginCustomization:(MyAppPluginCustomization *(^)(Class allocClass))initBlock;

- (void)willResignActive;
- (void)didEnterBackground;
- (void)willEnterForground;
- (void)didBecomeActive;
- (void)didRegisterForRemoteNotificationsWithDeviceToken:(NSData*)deviceToken;
- (void)didFailToRegisterForRemoteNotificationsWithError:(NSError*)error;
- (void)didReceiveRemoteNotification:(NSDictionary *)userInfo;
- (void)didReceiveLocalNotification:(UILocalNotification *)notification;
- (void)willTerminate;

@end
