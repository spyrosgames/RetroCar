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

#import <UIKit/UIKit.h>
#import "GreePluginConfigurationDelegate.h"

@interface GreeConfigurationBase : NSObject {
    id<GreePluginConfigurationDelegate> _pluginConfigurationDelegate;
    BOOL                                _initializationComplete;
    NSDictionary                        *_launchOptions;
    NSMutableDictionary                 *_configuration;
}

@property (nonatomic, assign) id<GreePluginConfigurationDelegate> pluginConfigurationDelegate;
@property (nonatomic, assign) BOOL initializationComplete;
@property (nonatomic, retain) NSDictionary *launchOptions;
@property (nonatomic, retain) NSMutableDictionary *configuration;


- (id)initWithLaunchOptions:(NSDictionary *)launchOptions;
- (id)buildSettings;
- (void)loadConfiguration;
- (void)loadConfigurationCallback:(NSMutableDictionary *)opts;
- (void)loadedConfiguration:(NSMutableDictionary *)opts;

- (void)loadProperties;
- (void)propertiesLoaded:(BOOL)loadSuccess;
    
- (void)configurationWillLoad;
- (void)configurationDidLoad;
@end
