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

typedef enum
{
    OFItemPayloadStatus_None,            ///This purchase does not have a downloadable payload
    OFItemPayloadStatus_NotLoaded,       ///This purchase has a payload, but it is not downloaded
    OFItemPayloadStatus_Loading,         ///This purchase payload is in progress
    OFItemPayloadStatus_Loaded,          ///This purchase payload is already on the device    
} OFItemPayloadStatus;

typedef enum
{
	OFInventorySynchronization_Updated,			/// Inventory was successfully synchronized
	OFInventorySynchronization_NetworkError,	/// Generic network error occurred
	OFInventorySynchronization_Stale,			/// Inventory is stale; internally used
} OFInventorySynchronizationStatus;
