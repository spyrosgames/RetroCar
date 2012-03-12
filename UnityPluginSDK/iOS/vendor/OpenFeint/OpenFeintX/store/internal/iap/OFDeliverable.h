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
#import "OFXStore.h"
#import "OFContentDownloader.h"
#import "OpenFeint/OFResource.h"

@interface OFDeliverable : OFResource<OFContentDownloadDelegate> {
    NSString* identifier;
    NSString* currencyIdentifier;
    NSURL* payloadUrl;
    BOOL unique;    
}

- (void) loadPayload;
- (float)payloadProgress;

@property (nonatomic, readonly, retain) NSString* identifier;
@property (nonatomic, readonly, retain) NSString* currencyIdentifier;
@property (nonatomic, readonly, retain) NSURL* payloadUrl;
@property (nonatomic, readonly) BOOL unique;    
@property (nonatomic, readonly) BOOL payloadDownloadRequired;
@property (nonatomic, readonly) OFItemPayloadStatus payloadStatus;

@end
