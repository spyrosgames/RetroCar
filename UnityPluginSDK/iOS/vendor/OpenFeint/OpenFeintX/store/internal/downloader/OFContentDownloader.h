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

extern NSString* const OFContentDownloadKey_OriginalURL;
extern NSString* const OFContentDownloadKey_Delegate;
extern NSString* const OFContentDownloadKey_InstallRoot;
extern NSString* const OFContentDownloadKey_ContentDownloadSize;
extern NSString* const OFContentDownloadKey_ContentType;   //if missing, uses ZIP.

@class OFASINetworkQueue;
@protocol OFContentDownloadDelegate;

@interface OFContentDownloader : NSObject
{
	OFASINetworkQueue* queue;
    NSMutableDictionary* activeDownloads;
}
-(id) initWithFile:(NSString*)filename;

- (void)downloadUrl:(NSURL*)url toPath:(NSString*)path delegate:(id)delegate userInfo:(NSDictionary*)userInfo;

- (BOOL)urlIsCached:(NSURL*)url;
- (BOOL)urlInProgress:(NSURL*)url;
- (float)progressOfUrl:(NSURL*)url;

- (void)batchSetFingerprints:(NSDictionary*)fingerprints;
- (void)batchUpdateFingerprints:(NSDictionary*)fingerprints;

- (void)cancelAllDownloads;

@end

@protocol OFContentDownloadDelegate
@optional
- (void)contentDownloader:(OFContentDownloader*)downloader didStartDownload:(NSDictionary*)userInfo;
- (void)contentDownloader:(OFContentDownloader*)downloader updatedProgress:(CGFloat)progress;
- (void)contentDownloader:(OFContentDownloader*)downloader didFinishDownload:(NSDictionary*)userInfo;
@end
