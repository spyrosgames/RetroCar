//  Copyright 2009-2010 Aurora Feint, Inc.
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

#import "NSDictionary+NumericValues.h"

@implementation NSMutableDictionary (NumericValues)
-(void) addInteger:(NSInteger)value toKey:(NSString*) key {
    NSInteger oldValue = [[self objectForKey:key] intValue];
    [self setObject:[NSNumber numberWithInt:oldValue + value] forKey:key]; 
}

-(void) mergeIntegerDictionary:(NSDictionary*) rhs {
    for(NSString* key in rhs) {
        [self addInteger:[rhs getIntegerForKey:key] toKey:key];
    }
}


@end

@implementation NSDictionary (NumericValues)
-(NSInteger) getIntegerForKey:(NSString*) key {
    return [[self objectForKey:key] intValue];
}
-(NSDictionary*) dictionaryByMergingWithIntegerDictionary:(NSDictionary*) rhs {
    NSMutableDictionary *combined = [NSMutableDictionary dictionaryWithCapacity:rhs.count + self.count];
    [combined setDictionary:self];
    [combined mergeIntegerDictionary:rhs];
    return [NSDictionary dictionaryWithDictionary:combined];
}

+(void)testNumericValues {
#if defined(_DEBUG)
    NSDictionary* test1 = [NSDictionary dictionaryWithObjectsAndKeys:
                           [NSNumber numberWithInt:3], @"a",
                           [NSNumber numberWithInt:4], @"b",
                           nil];
    NSDictionary* test2 = [NSDictionary dictionaryWithObjectsAndKeys:
                           [NSNumber numberWithInt:5], @"b",
                           [NSNumber numberWithInt:6], @"c",
                           nil];
    OFLog(@"NSDictionary Numeric combined %@", [test1 dictionaryByMergingWithIntegerDictionary:test2]);
#endif
}

@end
