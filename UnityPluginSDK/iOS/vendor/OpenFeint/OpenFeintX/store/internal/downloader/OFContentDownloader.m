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

#import "OFContentDownloader.h"
//#import "OFXStore+Internal.h"
#import "OpenFeint/OFASINetworkQueue.h"
#import "OpenFeint/OFASIHTTPRequest.h"
#import "OpenFeint/OFZipArchive.h"
#import "OpenFeint/OFSettings.h"
#import "OFXDebug.h"


@interface OFContentDownloaderCache : NSObject {    
    NSMutableDictionary* cacheETags;
    NSString* cacheFilename;
}
-(void) setETag:(NSString*)ETag forPath:(NSString*)urlPath;
-(NSString*) eTagForPath:(NSString*)urlPath;
-(void) clearETagForPath:(NSString*)urlPath;
-(void) batchSetETags:(NSDictionary*)tags;
-(void) batchUpdateETags:(NSDictionary*)tags;
@property (nonatomic, retain) NSMutableDictionary* cacheETags;
@property (nonatomic, retain) NSString* cacheFilename;
@end


NSString* const OFContentDownloadKey_OriginalURL = @"OFContentDownloadKey_OriginalURL";
NSString* const OFContentDownloadKey_Delegate = @"OFContentDownloadKey_Delegate";
NSString* const OFContentDownloadKey_InstallRoot = @"OFContentDownloadKey_InstallRoot";
NSString* const OFContentDownloadKey_ContentDownloadSize = @"OFContentDownloadKey_ContentDownloadSize";
NSString* const OFContentDownloadKey_ContentType = @"OFContentDownloadKey_ContentType";
NSString* const OFContentDownloadKey_DownloadDestinationPath = @"OFContentDownloadKey_DownloadDestinationPath";


@implementation OFContentDownloaderCache
@synthesize cacheETags;
@synthesize cacheFilename;
#pragma mark boilerplate
-(id) initWithFile:(NSString*)filename {
    if((self = [super init])) {
        cacheFilename = [filename retain];
        [[NSFileManager defaultManager] createDirectoryAtPath:[filename stringByDeletingLastPathComponent]
                                  withIntermediateDirectories:YES attributes:nil error:nil];                
        self.cacheETags = [NSMutableDictionary dictionaryWithContentsOfFile:filename];
        if(!self.cacheETags) self.cacheETags = [NSMutableDictionary dictionaryWithCapacity:10];    
    }
    return self;
}

-(void) dealloc {
    self.cacheETags = nil;
    [cacheFilename release];
    [super dealloc];
}


-(void) setETag:(NSString*)ETag forPath:(NSString*)urlPath {
    [self.cacheETags setObject:ETag forKey:urlPath];
    [self.cacheETags writeToFile:self.cacheFilename atomically:YES];
}

-(NSString*) eTagForPath:(NSString*)urlPath {
    return [self.cacheETags objectForKey:urlPath];
}

-(void) clearETagForPath:(NSString *)urlPath {
    [self.cacheETags removeObjectForKey:urlPath];
    [self.cacheETags writeToFile:self.cacheFilename atomically:YES];
}


-(void) batchSetETags:(NSDictionary*) tags {
    for(NSString*key in tags) {
        [self.cacheETags setObject:[tags objectForKey:key] forKey:key];
    }
    [self.cacheETags writeToFile:self.cacheFilename atomically:YES];
}

-(void) batchUpdateETags:(NSDictionary*) tags {
    for(NSString*key in tags) {
        NSString*oldValue = [self eTagForPath:key];
        if(oldValue) {
            //if cache holds an old value, forget about it
            if(![oldValue isEqualToString:[tags objectForKey:key]]) 
                [self.cacheETags removeObjectForKey:key];
        }
    }
    [self.cacheETags writeToFile:self.cacheFilename atomically:YES];
}

@end

#pragma mark Active Download

@interface OFContentDownloaderActiveDownload : NSObject< OFASIProgressDelegate >
{
    @private
    float progress;
    OFASIHTTPRequest* request;
    OFContentDownloader* owner; // weak references
}
@property (nonatomic, assign, readonly) float progress;
@property (nonatomic, retain, readonly) OFASIHTTPRequest* request;
- (id)initWithOwner:(OFContentDownloader*)_owner request:(OFASIHTTPRequest*)_request;
@end

@implementation OFContentDownloaderActiveDownload

@synthesize progress;
@synthesize request;

- (id)initWithOwner:(OFContentDownloader*)_owner request:(OFASIHTTPRequest*)_request;
{
    self = [super init];
    if (self != nil)
    {
        owner = _owner;
        request = [_request retain];
    }
    
    return self;
}

- (void)dealloc
{
    request.downloadProgressDelegate = nil;
    request.delegate = nil;
    request.userInfo = nil;
    [request release];
    [super dealloc];
}

- (void)setProgress:(float)newProgress
{
    progress = newProgress;

	id delegate = [[[request userInfo] objectForKey:OFContentDownloadKey_Delegate] nonretainedObjectValue];
	if (delegate && [delegate respondsToSelector:@selector(contentDownloader:updatedProgress:)])
	{
		[delegate contentDownloader:owner updatedProgress:progress];
	}
}

@end

#pragma mark Content Downloader

@interface OFContentDownloader ()
- (void)_backgroundInflate:(NSMutableDictionary*)params;
- (void)_inflationFinished:(NSMutableDictionary*)params;
- (void)_completeRequest:(OFASIHTTPRequest*)request;
@property (nonatomic,retain) NSString* cacheFileRoot;
@property (nonatomic,retain) OFContentDownloaderCache* cache;
@end

@implementation OFContentDownloader
@synthesize cacheFileRoot;
@synthesize cache;

#pragma mark -
#pragma mark Life-Cycle
#pragma mark -

- (id)initWithFile:(NSString *)_filename
{
	self = [super init];
	if (self != nil)
	{
        cacheFileRoot = [_filename retain];
        cache = [[OFContentDownloaderCache alloc] initWithFile:[_filename stringByAppendingPathComponent:@"ofx_offline_cache/cache.plist"]];
		queue = [[OFASINetworkQueue alloc] init];
		queue.delegate = self;
		queue.requestDidStartSelector = @selector(_requestStarted:);
		queue.requestDidFinishSelector = @selector(_requestFinished:);
		queue.requestDidFailSelector = @selector(_requestFinished:);
		queue.queueDidFinishSelector = @selector(_queueFinished:);
		queue.shouldCancelAllRequestsOnFailure = NO;
		queue.maxConcurrentOperationCount = 1;
		[queue setShowAccurateProgress:YES];
		[queue go];
        
        activeDownloads = [[NSMutableDictionary alloc] initWithCapacity:4];
	}
	
	return self;
}

- (void)dealloc
{
    [cacheFileRoot release];
	[queue release];
    [activeDownloads release];
	
	[super dealloc];
}

#pragma mark -
#pragma mark InstanceMethods
#pragma mark -

- (BOOL)urlIsCached:(NSURL*)url
{
    return [self.cache eTagForPath:url.path] != nil;
}

- (BOOL)urlInProgress:(NSURL*)url
{
    return [activeDownloads objectForKey:url] != nil;
}

- (float)progressOfUrl:(NSURL*)url
{
    if ([self urlInProgress:url])
	{
        return [[activeDownloads objectForKey:url] progress];
	}
	
	return 0.f;
}

- (void)batchSetFingerprints:(NSDictionary*)fingerprints
{
    [self.cache batchSetETags:fingerprints];
}

- (void)batchUpdateFingerprints:(NSDictionary*)fingerprints
{
    [self.cache batchUpdateETags:fingerprints];
}

- (void)setCacheFingerprint:(NSString*) fingerprint forUrl:(NSURL*)url
{
    NSString* path = url.path;
    if(path.length) {
        NSString*oldFingerprint = [self.cache eTagForPath:path];
        if(oldFingerprint.length && ![oldFingerprint isEqualToString:fingerprint]) 
            [self.cache clearETagForPath:path];
        else
            [self.cache setETag:fingerprint forPath:path];
    }
}

- (void)cancelAllDownloads
{
    NSArray* downloadsToCancel = [activeDownloads allValues];
    
    for (OFContentDownloaderActiveDownload* tracker in downloadsToCancel)
    {
        tracker.request.downloadProgressDelegate = nil;
        tracker.request.delegate = nil;
        tracker.request.userInfo = nil;
        // Not actually cancelling requests here due to what I think is a bug in ASIHTTPRequest.
        //[tracker.request cancel];
    }
    
    [activeDownloads removeAllObjects];

    queue.delegate = self;
}

- (void)downloadUrl:(NSURL*)url toPath:(NSString*)path delegate:(id)delegate userInfo:(NSDictionary*)userInfo
{
    if (!url)
    {
        OFXLog(@"Downloader", @"Aborting download because it lacks a url.");
        
        if ([delegate respondsToSelector:@selector(contentDownloader:didFinishDownload:)])
            [delegate contentDownloader:self didFinishDownload:nil];
        
        return;
    }
    
    // calculate install root path: (Documents | Library/Cache/openfeint_[id])/path/
	NSString* installRoot = self.cacheFileRoot;
	if ([path length] > 0)
		installRoot = [installRoot stringByAppendingPathComponent:path];
	else
		installRoot = [installRoot stringByAppendingString:@"/"];
    
    
    if([self.cache eTagForPath:url.path]) {
        //valid cache
        if ([delegate respondsToSelector:@selector(contentDownloader:didFinishDownload:)]) {
            NSMutableDictionary* infoDict = [NSMutableDictionary dictionaryWithCapacity:10];
            [infoDict setObject:url forKey:OFContentDownloadKey_OriginalURL];
            [infoDict setObject:installRoot forKey:OFContentDownloadKey_InstallRoot];
            [infoDict addEntriesFromDictionary:userInfo];
            
            [delegate contentDownloader:self didFinishDownload:infoDict];        
        }
        return;
    }
    
    if ([activeDownloads objectForKey:url] != nil)
    {
        //what delegates to call, and what to tell them?
        //mostly of importance for ZIPs, where ignoring the extra request is a valid response
        return;
    }
    
    NSString* downloadPath = installRoot;
    NSMutableDictionary* infoDict = [NSMutableDictionary dictionaryWithCapacity:10];
    [infoDict setObject:url forKey:OFContentDownloadKey_OriginalURL];
    [infoDict setObject:[NSValue valueWithNonretainedObject:delegate] forKey:OFContentDownloadKey_Delegate];
    if([[userInfo objectForKey:OFContentDownloadKey_ContentType] isEqualToString:@"ZIP"]) {
        // compute temporary zip filename: [install root]/[generated UUID].zip
        CFUUIDRef theUUID = CFUUIDCreate(NULL);
        NSString* uuidString = [(NSString*)CFUUIDCreateString(NULL, theUUID) autorelease];
        NSString* filename = [NSString stringWithFormat:@"%@.zip", uuidString];
        downloadPath = [installRoot stringByAppendingPathComponent:filename];
        CFRelease(theUUID);
        
        [infoDict setObject:installRoot forKey:OFContentDownloadKey_InstallRoot];

    }
    else {
        [infoDict setObject:downloadPath forKey:OFContentDownloadKey_InstallRoot];
    }
    [infoDict setObject:downloadPath forKey:OFContentDownloadKey_DownloadDestinationPath];
    [infoDict addEntriesFromDictionary:userInfo];
    
    [[NSFileManager defaultManager] createDirectoryAtPath:[downloadPath stringByDeletingLastPathComponent]
                              withIntermediateDirectories:YES attributes:nil error:nil];                

    OFXLog(@"Downloader", @"Commencing download of URL %@", url);
    OFASIHTTPRequest* request = [OFASIHTTPRequest requestWithURL:url];
    [request setUserInfo:infoDict];
    [request setDownloadDestinationPath:downloadPath];
    
    OFContentDownloaderActiveDownload* tracker = [[OFContentDownloaderActiveDownload alloc] initWithOwner:self request:request];
    [request setDownloadProgressDelegate:tracker];
    [activeDownloads setObject:tracker forKey:url];
    [tracker release];
    
    [queue addOperation:request];	
}

#pragma mark -
#pragma mark ASINetworkQueue Delegate Methods
#pragma mark -

- (void)_requestStarted:(OFASIHTTPRequest*)request
{
    OFContentDownloaderActiveDownload* download = [activeDownloads objectForKey:request.originalURL];
    if (download.request != request)
{
        return;
    }
	
	id delegate = [(NSValue*)[request.userInfo objectForKey:OFContentDownloadKey_Delegate] nonretainedObjectValue];
	if (delegate && [delegate respondsToSelector:@selector(contentDownloader:didStartDownload:)])
	{
		[delegate contentDownloader:self didStartDownload:request.userInfo];
	}
}

- (void)_requestFinished:(OFASIHTTPRequest*)request
{
    OFContentDownloaderActiveDownload* download = [activeDownloads objectForKey:request.originalURL];
    if (download.request != request)
{
        return;
    }
    
	[(NSMutableDictionary*)[request userInfo] 
		setObject:[NSNumber numberWithUnsignedLongLong:request.totalBytesRead] 
		forKey:OFContentDownloadKey_ContentDownloadSize];
	
    if([[[request userInfo] objectForKey:OFContentDownloadKey_ContentType] isEqualToString:@"ZIP"])
	{
        NSMutableDictionary* params = [[request userInfo] mutableCopy];
        [self performSelectorInBackground:@selector(_backgroundInflate:) withObject:params];
        [params release];
	}
    else
    {
        [self _completeRequest:request];
}

}

- (void)_queueFinished:(OFASINetworkQueue*)queue
{
}

// executed on a background thread
- (void)_backgroundInflate:(NSMutableDictionary*)params
{
    NSAutoreleasePool*pool = [NSAutoreleasePool new];
    
    NSString* installRoot = [params objectForKey:OFContentDownloadKey_InstallRoot];
	NSString* downloadedFile = [params objectForKey:OFContentDownloadKey_DownloadDestinationPath];
	
	BOOL openedZipFile = NO;
	BOOL unzippedZipFile = NO;
	
	OFZipArchive* zip = [[OFZipArchive alloc] init];
	if ([zip UnzipOpenFile:downloadedFile])
	{
		openedZipFile = YES;
		unzippedZipFile = [zip UnzipFileTo:installRoot overWrite:YES];
		[zip UnzipCloseFile];
	}

	[[NSFileManager defaultManager] removeItemAtPath:downloadedFile error:nil];

	if (openedZipFile && !unzippedZipFile)
	{
		OFXLog(@"Downloader", @"Content download succeeded but the content was not able to be decompressed!");
		[params removeObjectForKey:OFContentDownloadKey_InstallRoot];
	}
    [self performSelectorOnMainThread:@selector(_inflationFinished:) withObject:params waitUntilDone:NO];
    [pool release];
}

// back on the main thread here
- (void)_inflationFinished:(NSMutableDictionary*)params
{
    OFContentDownloaderActiveDownload* download = [activeDownloads objectForKey:[params objectForKey:OFContentDownloadKey_OriginalURL]];

    if ([params objectForKey:OFContentDownloadKey_InstallRoot] == nil)
    {
        [(NSMutableDictionary*)[download.request userInfo] removeObjectForKey:OFContentDownloadKey_InstallRoot];
    }
    
    [self _completeRequest:download.request];
}

-(void)_completeRequest:(OFASIHTTPRequest*)request
{
    BOOL setEtag = ([[request userInfo] objectForKey:OFContentDownloadKey_InstallRoot] != nil);

    //set the eTag
    NSURL*keyUrl = [request.userInfo objectForKey:OFContentDownloadKey_OriginalURL];
    if(setEtag)
    {
		NSString*keyPath = keyUrl.path;
		if(keyPath) {
			//store eTag
			NSString* eTag = [[request.responseHeaders objectForKey:@"Etag"] stringByReplacingOccurrencesOfString:@"\"" withString:@""];
			if(eTag)
            {
				[self.cache setETag:eTag forPath:keyPath];
            }
			else
            {
				[self.cache clearETagForPath:keyPath];
			}
		}    
    }
	
	id delegate = [(NSValue*)[request.userInfo objectForKey:OFContentDownloadKey_Delegate] nonretainedObjectValue];
	if (delegate && [delegate respondsToSelector:@selector(contentDownloader:didFinishDownload:)])
	{
		[delegate contentDownloader:self didFinishDownload:request.userInfo];
	}

    if (keyUrl) 
{
        [activeDownloads removeObjectForKey:keyUrl];
    }
}

@end
