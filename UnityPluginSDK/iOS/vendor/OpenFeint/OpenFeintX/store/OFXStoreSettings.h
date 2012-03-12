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

#pragma once

//////////////////////////////////////////////////////////////////////////////////////////
/// OFXStoreSettings describes additional settings you can include in the OpenFeint
/// settings dictionary that you provide when initializing OpenFeint!
//////////////////////////////////////////////////////////////////////////////////////////

//////////////////////////////////////////////////////////////////////////////////////////
/// @type		id<OFXStoreDelegate>
/// @default	nil
/// @behavior	Specify the OFXStoreDelegate object used for communicating global
///             OFXStore information to your application such as updates to the
///             in app purchase catalog, inventory contents, required payload downloads,
///             etc.
//////////////////////////////////////////////////////////////////////////////////////////
extern const NSString* OFXStore_Setting_Delegate;

//////////////////////////////////////////////////////////////////////////////////////////
/// @type		NSNumber BOOL
/// @default	YES
/// @behavior	If YES the in app purchase catalog will automatically update upon 
///             OpenFeint initialization. If NO index must be manually updated by 
///             invoking [OFInAppPurchaseCatalog updateCatalog].
//////////////////////////////////////////////////////////////////////////////////////////
extern const NSString* OFXStore_Setting_AutomaticallyUpdateIAPCatalog;

//////////////////////////////////////////////////////////////////////////////////////////
/// @type		NSNumber BOOL
/// @default	none, this is a required setting
/// @behavior	Determines whether OFX data (including DLC) will be stored under Library/Cache
///             and thus, not included in iCloud backups.  If YES is selected, it will be
///             up to the developer to move any existing DLC files to the new directory.
///             This directory can be accessed with [OFXStore rootPathForPayloads]
//////////////////////////////////////////////////////////////////////////////////////////
extern const NSString* OFXStore_Setting_StoreOFXDataInCacheDirectory;

