//
//  NSArray+GreeExtensions.h
//  Unity-iPhone
//
//  Created by Michael Swearingen on 12/28/11.
//  Copyright (c) 2011 __MyCompanyName__. All rights reserved.
//

#import <Foundation/Foundation.h>

@interface NSArray (GreeExtensions)

+ (NSArray *)gree_nilSafeArrayOfCount:(NSUInteger)arrayCount withObjects:(id)firstObject, ...;

@end
