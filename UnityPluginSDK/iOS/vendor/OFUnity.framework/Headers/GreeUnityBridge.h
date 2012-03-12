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

#import <Foundation/Foundation.h>
#import "GreeBridgeFromUnity.h"
#import "GreeBridgeToUnity.h"
#import "GreeSelectorInvocationMap.h"

@interface GreeUnityBridge : NSObject {
    GreeBridgeFromUnity *_bridgeFromUnity;
    GreeBridgeToUnity *_bridgeToUnity;
    GreeSelectorInvocationMap *_selectorInvocationMap;
}

@property (nonatomic, retain) GreeBridgeFromUnity *bridgeFromUnity;
@property (nonatomic, retain) GreeBridgeToUnity *bridgeToUnity;
@property (nonatomic, retain) GreeSelectorInvocationMap *selectorInvocationMap;

@property (nonatomic, assign) NSString *unityMessageClass;
@property (nonatomic, assign) NSString *unityMessageMethod;

+ (GreeUnityBridge *)sharedGreeUnityBridge:(GreeUnityBridge *(^)(Class allocClass))initBlock;
- (id)sendMessage:(NSString*)methodname args:(id)args;
- (void)_superClassesForClass:(Class)parentClass withArray:(NSMutableArray *)resultArray;
- (NSArray *)superClassesForClass:(Class)parentClass;
- (NSMutableArray *)methodStringsForClass:(Class)_class;
- (NSMutableArray *)bridgeMethodsForClass:(Class)_class;
- (void)addBridgeMethodsOfInstance:(id)instance;
@end

extern void UnityPause(bool);
