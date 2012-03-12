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

#import "OFDeliverable.h"
#import "OFXStore.h"

@interface OFDeliverable ()
@property (nonatomic, readwrite, retain) NSString* identifier;
@property (nonatomic, readwrite, retain) NSString* currencyIdentifier;
@property (nonatomic, readwrite, retain) NSURL* payloadUrl;
@property (nonatomic, readwrite) BOOL unique;    
@end

@interface OFDeliverable (Tests)
+(NSArray*)testFixtures;

@end


@implementation OFDeliverable

@synthesize identifier;
@synthesize currencyIdentifier;
@synthesize payloadUrl;
@synthesize unique;

- (void)dealloc
{
	self.identifier = nil;
	self.currencyIdentifier = nil;
	self.payloadUrl = nil;
	[super dealloc];
}

- (void) loadPayload {
    if(self.payloadUrl) {
        [[OFXStore downloader] downloadUrl:self.payloadUrl toPath:nil delegate:self 
                                    userInfo:[NSDictionary dictionaryWithObject:@"ZIP" forKey:OFContentDownloadKey_ContentType]];
    }
    else {
        if ([[OFXStore delegate] respondsToSelector:@selector(payloadForItem:finishedLoading:)])
            [[OFXStore delegate] payloadForItem:identifier finishedLoading:YES];
    }
}

- (BOOL) payloadDownloadRequired {
    if(self.payloadUrl == nil) return NO;
    return ![[OFXStore downloader] urlIsCached:self.payloadUrl];
}

- (OFItemPayloadStatus) payloadStatus {
    if(self.payloadUrl) {
        if([[OFXStore downloader] urlIsCached:self.payloadUrl]) return OFItemPayloadStatus_Loaded;
        return [[OFXStore downloader] urlInProgress:self.payloadUrl] ? OFItemPayloadStatus_Loading : OFItemPayloadStatus_NotLoaded;
    }
    return OFItemPayloadStatus_None;
}

- (float)payloadProgress
{
	return [[OFXStore downloader] progressOfUrl:self.payloadUrl];
}

#pragma mark -
#pragma mark OFContentDownloader
#pragma mark -

- (void)contentDownloader:(OFContentDownloader*)downloader didStartDownload:(NSDictionary*)userInfo {
    if ([[OFXStore delegate] respondsToSelector:@selector(payloadForItemStartedLoading:)])
        [[OFXStore delegate] payloadForItemStartedLoading:identifier];
}

- (void)contentDownloader:(OFContentDownloader*)downloader updatedProgress:(CGFloat)progress {
    if ([[OFXStore delegate] respondsToSelector:@selector(payloadForItem:updatedProgress:)])
        [[OFXStore delegate] payloadForItem:identifier updatedProgress:progress];
}

- (void)contentDownloader:(OFContentDownloader*)downloader didFinishDownload:(NSDictionary*)userInfo {
    if ([[OFXStore delegate] respondsToSelector:@selector(payloadForItem:finishedLoading:)])
        [[OFXStore delegate] payloadForItem:identifier finishedLoading:![self payloadDownloadRequired]];
}


#pragma mark -
#pragma mark OFResource
#pragma mark -

AUTOREGISTER_CLASS_WITH_OFJSONCODER

+(NSString*)classNameForJsonCoding
{
    return @"deliverable";
}

+ (void)registerJsonValueTypesForDecoding:(NSMutableDictionary*)valueMap
{
	[super registerJsonValueTypesForDecoding:valueMap];	
    
	[valueMap setObject:[OFJsonObjectValue valueWithSelector:@selector(setIdentifier:)] forKey:@"identifier"];
	[valueMap setObject:[OFJsonObjectValue valueWithSelector:@selector(setCurrencyIdentifier:)] forKey:@"currency_identifier"];
	[valueMap setObject:[OFJsonUrlValue valueWithSelector:@selector(setPayloadUrl:)] forKey:@"payload_url"];
    [valueMap setObject:[OFJsonBoolValue valueWithSelector:@selector(setUnique:)] forKey:@"unique"];
}

- (void)encodeWithJsonCoder:(OFJsonCoder*)coder
{
	[super encodeWithJsonCoder:coder];
	[coder encodeObject:identifier withKey:@"identifier"];
	[coder encodeObject:currencyIdentifier withKey:@"currency_identifier"];
	[coder encodeUrl:payloadUrl withKey:@"payload_url"];
    [coder encodeBool:unique withKey:@"unique"];
}

#pragma mark Testing
+(NSArray*) testFixtures {
    OFDeliverable* fixture1 = [[OFDeliverable new] autorelease];
    fixture1.identifier = @"FIXTURE_UNIQUE";
    fixture1.unique = YES;
    
    OFDeliverable* fixture2 = [[OFDeliverable new] autorelease];
    fixture2.identifier = @"FIXTURE_NONUNIQUE";
    fixture2.unique = NO;
    return [NSArray arrayWithObjects:fixture1, fixture2, nil];
}

@end
