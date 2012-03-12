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

#import "OFCurrency.h"

#import "OpenFeint/OFResourceRequest.h"
#import "OpenFeint/OpenFeint+UserOptions.h"
#import "OFXStore.h"

static id sharedDelegate_OFCurrency = nil;

@interface OFCurrency ()
@property (nonatomic, retain) NSString* identifier;
@property (nonatomic, retain) NSString* name;
@property (nonatomic, retain) NSURL* iconUrl;
@property (nonatomic, assign) BOOL usedForOffer;
@end

@interface OFCurrency (Tests)
+(NSArray*) testFixtures;
@end

@implementation OFCurrency

@synthesize identifier;
@synthesize name;
@synthesize iconUrl;
@synthesize usedForOffer;

#pragma mark -
#pragma mark Class Methods
#pragma mark -

+ (void)setDelegate:(id<OFCurrencyDelegate>)delegate
{
	sharedDelegate_OFCurrency = delegate;
}

#pragma mark -
#pragma mark Life-cycle
#pragma mark -

- (id)init
{
	self = [super init];
	if (self != nil)
	{
	}
	
	return self;
}

- (void)dealloc
{
	self.identifier = nil;
	self.name = nil;
	self.iconUrl = nil;
	[super dealloc];
}

#pragma mark -
#pragma mark Public API
#pragma mark -

- (void)loadIcon
{
	if (iconUrl)
	{
		NSString* iconPath = [@"ofx_offline_cache" stringByAppendingPathComponent:iconUrl.path];
		[[OFXStore downloader] downloadUrl:iconUrl toPath:iconPath delegate:self userInfo:nil];
	}
	else
	{
		[self contentDownloader:nil didFinishDownload:nil];
	}
}

#pragma mark -
#pragma mark OFContentDownloadDelegate
#pragma mark -

- (void)contentDownloader:(OFContentDownloader*)downloader didFinishDownload:(NSDictionary*)userInfo
{
	NSString* imagePath = [userInfo objectForKey:OFContentDownloadKey_InstallRoot];

	if ([sharedDelegate_OFCurrency respondsToSelector:@selector(currency:iconLoaded:)])
	{
		UIImage* image = ([imagePath length] > 0) ? [UIImage imageWithContentsOfFile:imagePath] : nil;
		[sharedDelegate_OFCurrency currency:self iconLoaded:image];
	}
}

#pragma mark -
#pragma mark OFResource
#pragma mark -

AUTOREGISTER_CLASS_WITH_OFJSONCODER

+ (NSString*)classNameForJsonCoding
{
	return @"currency";
}

+ (void)registerJsonValueTypesForDecoding:(NSMutableDictionary*)valueMap
{
	[super registerJsonValueTypesForDecoding:valueMap];	
	[valueMap setObject:[OFJsonObjectValue valueWithSelector:@selector(setIdentifier:)] forKey:@"identifier"];
	[valueMap setObject:[OFJsonObjectValue valueWithSelector:@selector(setName:)] forKey:@"name"];
	[valueMap setObject:[OFJsonUrlValue valueWithSelector:@selector(setIconUrl:)] forKey:@"icon_url"];
    [valueMap setObject:[OFJsonBoolValue valueWithSelector:@selector(setUsedForOffer:)] forKey:@"used_for_offer"];
}

- (void)encodeWithJsonCoder:(OFJsonCoder*)coder
{
	[super encodeWithJsonCoder:coder];
	[coder encodeObject:identifier withKey:@"identifier"];
	[coder encodeObject:name withKey:@"name"];
	[coder encodeUrl:iconUrl withKey:@"icon_url"];
    [coder encodeBool:usedForOffer withKey:@"used_for_offer"];
}

#pragma mark Testing

+(id)currencyWithTestData {
    OFCurrency *currency = [[OFCurrency new] autorelease];
    currency.identifier = @"Testing";
    const static char utfCheck[] = { 't', 'i', 't', 'l', 'e', ' ', 'c', 'l', 'e', 'f', 0xf0, 0x9d, 0x84, 0x9e, 0};
    currency.name = [NSString stringWithCString:utfCheck encoding:NSUTF8StringEncoding];
    currency.iconUrl = [NSURL URLWithString:@"http://www.example.com/testcurrency"];
    return currency;
}

+(NSArray*) testFixtures {
    OFCurrency* newCurrency = [[OFCurrency new] autorelease];
    newCurrency.identifier = @"FIXTURE";
    newCurrency.name = @"Testing Swag";
    newCurrency.iconUrl = nil;
    return [NSArray arrayWithObject:newCurrency];
}

@end
