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

#import "OFSkuCategory.h"

@interface OFSkuCategory ()
@property (nonatomic, readwrite, retain) NSString* name;
@property (nonatomic, readwrite) NSUInteger position;
@end

@interface OFSkuCategory (Tests)
+(NSArray*)testFixtures;
@end


@implementation OFSkuCategory
@synthesize name, position;

-(void)dealloc {
    self.name = nil;
    [super dealloc];
}

-(NSInteger)positionSort:(OFSkuCategory*) rhs {
    if(self.position < rhs.position) return NSOrderedAscending;
    if(self.position > rhs.position) return NSOrderedDescending;
    return NSOrderedSame; //which shouldn't happen
}

+(OFSkuCategory*) uncategorized {
    static OFSkuCategory* nullCategory = nil;
    if(!nullCategory) {
        nullCategory = [OFSkuCategory new];
        nullCategory.name = @"Uncategorized";
        nullCategory.position = 1000000;
    }
    return nullCategory;
}

#pragma mark OFResource

AUTOREGISTER_CLASS_WITH_OFJSONCODER

+(NSString*) classNameForJsonCoding
{
    return @"sku_category";
}

+ (void)registerJsonValueTypesForDecoding:(NSMutableDictionary*)valueMap
{
	[super registerJsonValueTypesForDecoding:valueMap];	
	[valueMap setObject:[OFJsonIntegerValue valueWithSelector:@selector(setPosition:)] forKey:@"position"];
	[valueMap setObject:[OFJsonObjectValue valueWithSelector:@selector(setName:)] forKey:@"name"];
}

- (void)encodeWithJsonCoder:(OFJsonCoder*)coder
{
	[super encodeWithJsonCoder:coder];
	[coder encodeInteger:position withKey:@"position"];
	[coder encodeObject:name withKey:@"name"];
}

#pragma mark Testing
+(NSArray*)testFixtures {
    OFSkuCategory* cat = [[OFSkuCategory new] autorelease];
    cat.name = @"FIXTURES";
    cat.position = 100;
    return [NSArray arrayWithObject:cat];
}

@end

