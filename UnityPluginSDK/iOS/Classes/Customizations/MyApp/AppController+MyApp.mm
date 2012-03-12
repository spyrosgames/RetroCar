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

#import "AppController+MyApp.h"
#import "MyAppPluginCustomization.h"

@implementation AppController (MyApp)
- (BOOL)application:(UIApplication*)application didFinishLaunchingWithOptions:(NSDictionary*)launchOptions
{
    
    [MyAppPluginCustomization sharedMyAppPluginCustomization:^(Class allocClass) {
        return [[allocClass alloc] initWithLaunchOptions:launchOptions];
    }];
    
    [self startUnity:application];
    
    [[MyAppPluginCustomization sharedMyAppPluginCustomization:nil] loadProperties];
    
    return YES;
}

// For iOS 4
// Callback order:
//   applicationDidResignActive()
//   applicationDidEnterBackground()
- (void)applicationDidEnterBackground:(UIApplication *)application
{
    printf_console("-> applicationDidEnterBackground()\n");
    
    [[MyAppPluginCustomization sharedMyAppPluginCustomization:nil] didEnterBackground];
}

// For iOS 4
// Callback order:
//   applicationWillEnterForeground()
//   applicationDidBecomeActive()
- (void)applicationWillEnterForeground:(UIApplication *)application
{
	printf_console("-> applicationWillEnterForeground()\n");
    
    [[MyAppPluginCustomization sharedMyAppPluginCustomization:nil] willEnterForground];
}

- (void) applicationDidBecomeActive:(UIApplication*)application
{
	printf_console("-> applicationDidBecomeActive()\n");
	if (_didResignActive)
	{
		UnityPause(false);
	}
    
	_didResignActive = NO;
    
    [[MyAppPluginCustomization sharedMyAppPluginCustomization:nil] didBecomeActive];
}

- (void) applicationWillResignActive:(UIApplication*)application
{
	printf_console("-> applicationDidResignActive()\n");
	UnityPause(true);
    
	_didResignActive = YES;
    
    [[MyAppPluginCustomization sharedMyAppPluginCustomization:nil] willResignActive];
}

- (void) applicationDidReceiveMemoryWarning:(UIApplication*)application
{
	printf_console("WARNING -> applicationDidReceiveMemoryWarning()\n");
}
/*
- (void) applicationWillTerminate:(UIApplication *)application
{
    printf_console("WARNING -> applicationWillTerminate()\n");
    [[MyAppPluginCustomization sharedMyAppPluginCustomization:nil]willTerminate];
    Profiler_UninitProfiler();
    UnityCleanup();
}
 */
- (void)application:(UIApplication*)application didRegisterForRemoteNotificationsWithDeviceToken:(NSData*)deviceToken
{
    [[MyAppPluginCustomization sharedMyAppPluginCustomization:nil] didRegisterForRemoteNotificationsWithDeviceToken:deviceToken];
}

- (void)application:(UIApplication*)application didFailToRegisterForRemoteNotificationsWithError:(NSError*)error
{
    [[MyAppPluginCustomization sharedMyAppPluginCustomization:nil] didFailToRegisterForRemoteNotificationsWithError:error];
}

- (void)application:(UIApplication*)application didReceiveRemoteNotification:(NSDictionary*)userInfo
{
    [[MyAppPluginCustomization sharedMyAppPluginCustomization:nil] didReceiveRemoteNotification:userInfo];
}

- (void)application:(UIApplication *)application didReceiveLocalNotification:(UILocalNotification *)notification {
    
    [[MyAppPluginCustomization sharedMyAppPluginCustomization:nil] didReceiveLocalNotification:notification];
}

@end
