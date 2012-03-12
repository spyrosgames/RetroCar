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

#import "OpenFeint/OFResource.h"
#import "OFContentDownloader.h"

@protocol OFCurrencyDelegate;

//////////////////////////////////////////////////////////////////////////////////////////
/// OFCurrency represents one single configured virtual currency in your game.
//////////////////////////////////////////////////////////////////////////////////////////
@interface OFCurrency : OFResource< OFContentDownloadDelegate >
{
	NSString* identifier;
	NSString* name;
	NSURL* iconUrl;
    BOOL usedForOffer;
}

//////////////////////////////////////////////////////////////////////////////////////////
/// Set a delegate for OFCurrency. Must adopt the OFCurrencyDelegate protocol.
///
/// @note Defaults to nil. Weak reference
//////////////////////////////////////////////////////////////////////////////////////////
+ (void)setDelegate:(id<OFCurrencyDelegate>)delegate;

@property (nonatomic, readonly, retain) NSString* identifier;
@property (nonatomic, readonly, retain) NSString* name;
@property (nonatomic, readonly, retain) NSURL* iconUrl;
@property (nonatomic, readonly, assign) BOOL usedForOffer;

//////////////////////////////////////////////////////////////////////////////////////////
/// Request to load the icon image for this currency.
///
/// @note This method may incur a network request to download the data.
//////////////////////////////////////////////////////////////////////////////////////////
- (void)loadIcon;

@end

@protocol OFCurrencyDelegate
//////////////////////////////////////////////////////////////////////////////////////////
/// Invoked when the icon for a currency is loaded
///
/// @param currency     The currency whose icon was loaded
/// @param iconImage    A UIImage* containing the icon image data
//////////////////////////////////////////////////////////////////////////////////////////
- (void)currency:(OFCurrency*)currency iconLoaded:(UIImage*)iconImage;
@end
