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

#pragma once
#import "OFInAppPurchase.h"

@class OFCurrency;
@class OFDeliverable;

//////////////////////////////////////////////////////////////////////////////////////////
/// OFInAppPurchaseCatalog is your main interface for retrieving information about
/// in app purchases available in your application.
//////////////////////////////////////////////////////////////////////////////////////////
@interface OFInAppPurchaseCatalog : NSObject
{
@private
	NSArray* skus;
	NSDictionary* currencies;
	NSDictionary* deliverablesInternal;
    NSArray* categoriesInternal;
	
	NSArray* skusAddedInLastUpdate;
	
	struct
	{
		BOOL hasExtractedOfflinePackage;
		BOOL shouldUpdateFromServer;
		BOOL shouldMigrateDLC;
	} initializationFlags;
    
    BOOL serverLoaded;
}

//////////////////////////////////////////////////////////////////////////////////////////
/// Downloads any updated catalog information from the OpenFeint servers.
/// This is automatically called during application initialization
//////////////////////////////////////////////////////////////////////////////////////////
+ (void)updateCatalogFromServer;

//////////////////////////////////////////////////////////////////////////////////////////
/// Retrieve an unordered list of in app purchases which have been added to the store
/// between the previous server catalog update and the latest server catalog update.
///
/// @return An unordered NSArray* of OFInAppPurchase* objects
//////////////////////////////////////////////////////////////////////////////////////////
+ (NSArray*)inAppPurchasesAddedInLastUpdate;

//////////////////////////////////////////////////////////////////////////////////////////
/// Retrieve a list of in app purchases by category
///
/// @param category The category to filter by
///
/// @return A sorted NSArray* of OFInAppPurchase* objects within the given category
//////////////////////////////////////////////////////////////////////////////////////////
+ (NSArray*)inAppPurchasesForCategory:(NSString*)category;

//////////////////////////////////////////////////////////////////////////////////////////
/// Retrieve a dictionary of all virtual currencies indexed by their identifier
///
/// @return NSDictionary* of OFCurrency* objects keyed by NSString* identifiers
//////////////////////////////////////////////////////////////////////////////////////////
+ (NSDictionary*)currencies;

//////////////////////////////////////////////////////////////////////////////////////////
/// Retrive a list of all in app purchase categories
///
/// @return A sorted NSArray* of NSString* category identifiers.
//////////////////////////////////////////////////////////////////////////////////////////
+ (NSArray*)categories;

//////////////////////////////////////////////////////////////////////////////////////////
/// Retrieve a list of all in app purchases available, unordered.
///
/// @return Unordered NSArray* of OFInAppPurchase* objects
//////////////////////////////////////////////////////////////////////////////////////////
+ (NSArray*)inAppPurchases;

//////////////////////////////////////////////////////////////////////////////////////////
/// Returns all deliverable payload identifiers
///
/// @return Unordered NSArray* of NSString* payload identifiers
//////////////////////////////////////////////////////////////////////////////////////////
+ (NSArray*)deliverables;

@end
