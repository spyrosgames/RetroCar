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

#import "OFStoreKit.h"
#import "OpenFeintX.h"
#import "OFXDebug.h"
#import "OFXStore.h"
#import "OpenFeint/OpenFeint+UserOptions.h"
#import "OpenFeint/OFResourceRequest.h"
#import "OpenFeint/OpenFeint+Private.h"
#import "OpenFeint/OpenFeint+UserOptions.h"
#import "OpenFeint/IPhoneOSIntrospection.h"
#import "OpenFeint/OFSettings.h"
#import "OpenFeint/OFEncryptedFile.h"
#import "OFServerException+OFXStore.h"

NSString* const OFStoreKitPurchaseCompleted = @"OFStoreKitPurchaseCompleted";
NSString* const OFStoreKitPurchaseInvalidated = @"OFStoreKitPurchaseInvalidated";
NSString* const OFStoreKitUserInfo_ProductIdentifierKey = @"OFStoreKitUserInfo_ProductIdentifierKey";
NSString* const OFStoreKitUserInfo_StatusKey = @"OFStoreKitUserInfo_StatusKey";

NSString* const OFStoreKitRestoreFinished = @"OFStoreKitRestoreFinished";
NSString* const OFStoreKitUserInfo_RestoreError = @"OFStoreKitUserInfo_RestoreError";

@interface OFStoreKit ()
- (NSDictionary*)_requestDictionaryForRequest:(SKProductsRequest*)request;
- (void)_invokeRequestDelegate:(NSDictionary*)requestDictionary;
- (NSString*)_stringForState:(SKPaymentTransactionState)state;
- (void)finishPurchase:(id)purchase;
- (void)invokePurchaseCompleteNotification:(SKPaymentTransaction*)purchase;

- (void)writeValidationQueue;
- (void)kickValidationQueue;
- (void)validatePurchase:(SKPaymentTransaction*)purchase;
- (void)_validateNextPurchase;
- (void)_validationResponse:(OFResourceRequest*)request;
@end

@implementation OFStoreKit

#pragma mark -
#pragma mark Life-Cycle
#pragma mark -

- (id)init
{
	self = [super init];
	if (self != nil)
	{
		OFEncryptedFile* validationFile = [OFEncryptedFile encryptedFileWithFilename:@"ofxskvalidate"];
		if (validationFile.plaintext)
		{
			validationQueue = [[NSKeyedUnarchiver unarchiveObjectWithData:validationFile.plaintext] retain];
			[self kickValidationQueue];
		}
		else
		{
			validationQueue = [[NSMutableArray alloc] initWithCapacity:8];
		}
		
		productCatalog = [[NSMutableDictionary alloc] initWithCapacity:8];
	
		outstandingRequests = [[NSMutableArray alloc] initWithCapacity:1];
		
		NSData* logData = [NSData dataWithContentsOfFile:[OFSettings savePathForFile:@"ofxtransactionlog"]];
		if (logData)
			completedTransactions = [[NSKeyedUnarchiver unarchiveObjectWithData:logData] retain];

		if (!completedTransactions)
			completedTransactions = [[NSMutableSet alloc] initWithCapacity:2];

		[[SKPaymentQueue defaultQueue] addTransactionObserver:(id<SKPaymentTransactionObserver>)self];
		
		[OFReachability addObserver:self];
	}
	
	return self;
}

- (void)dealloc
{
	[[SKPaymentQueue defaultQueue] removeTransactionObserver:(id<SKPaymentTransactionObserver>)self];
	
	[OFReachability removeObserver:self];

	OFSafeRelease(completedTransactions);
	OFSafeRelease(outstandingRequests);
	OFSafeRelease(validationQueue);
	OFSafeRelease(productCatalog);
	[super dealloc];
}

#pragma mark -
#pragma mark Instance Methods
#pragma mark -

- (void)downloadProductInformation:(NSArray*)purchasables delegate:(id)delegate
{
	NSMutableDictionary* products = [NSMutableDictionary dictionaryWithCapacity:[purchasables count]];    
	NSMutableDictionary* request = [NSMutableDictionary dictionaryWithObjectsAndKeys:
                             [NSValue valueWithNonretainedObject:delegate], @"delegate",
                             products, @"purchasables",
                             nil];

#if TARGET_IPHONE_SIMULATOR
	[self _invokeRequestDelegate:request];
#else
	NSMutableSet* appleProductIds = [NSMutableSet setWithCapacity:[purchasables count]];

	for (id<OFStoreKitPurchasable> purchasable in purchasables)
	{
		NSString* productId = [purchasable storeKitProductIdentifier];
		if ([productId length] == 0)
			continue;

		[products setObject:purchasable forKey:productId];
		[appleProductIds addObject:productId];
	}

	SKProductsRequest* productsRequest = [[SKProductsRequest alloc] initWithProductIdentifiers:appleProductIds];
	productsRequest.delegate = self;

    [request setObject:productsRequest forKey:@"original_request"];
	[outstandingRequests addObject:request];

	OFXLog(@"StoreKit", @"Downloading product information for %d product(s).", [products count]);
	[productsRequest start];
#endif
}

- (BOOL)canMakePurchases
{
    return [SKPaymentQueue canMakePayments];
}

- (void)purchase:(id<OFStoreKitPurchasable>)purchasable
{
	NSString* productIdentifier = [purchasable storeKitProductIdentifier];

#if TARGET_IPHONE_SIMULATOR
	[self invokePurchaseCompleteNotification:(SKPaymentTransaction*)productIdentifier];
#else
	if (![self canMakePurchases])
	{
		OFXLog(@"StoreKit", @"Payments are not currently allowed on this device.");
		[[NSNotificationCenter defaultCenter] 
			postNotificationName:OFStoreKitPurchaseCompleted 
			object:nil 
			userInfo:[NSDictionary dictionaryWithObjectsAndKeys:
				productIdentifier, OFStoreKitUserInfo_ProductIdentifierKey,
				[NSNumber numberWithInt:OFStoreKitPurchaseStatus_Disabled], OFStoreKitUserInfo_StatusKey,
				nil]];
		return;
	}
	
	SKProduct* product = [purchasable storeKitProduct];
	if (!product)
	{
		OFXLog(@"StoreKit", @"Attempting to purchase product identifier (%@) without an SKProduct*, failing.", productIdentifier);
		return;
	}

    OFXLog(@"StoreKit", @"Purchasing product %@", productIdentifier);
    SKPayment* payment = [SKPayment paymentWithProduct:product];
    [[SKPaymentQueue defaultQueue] addPayment:payment];
#endif
}

- (void)restorePurchases
{
	if (restoreInProgress)
		return;
		
	restoreInProgress = YES;
	
	[[SKPaymentQueue defaultQueue] restoreCompletedTransactions];
}

#pragma mark -
#pragma mark Internal Methods
#pragma mark -

- (NSDictionary*)_requestDictionaryForRequest:(SKProductsRequest*)request
{
	for (NSMutableDictionary* dict in outstandingRequests)
	{
		if ([dict objectForKey:@"original_request"] == request)
		{
			return dict;
		}			
	}
	
	return nil;
}

- (void)_invokeRequestDelegate:(NSDictionary*)requestDictionary
{
	NSValue* delegateVal = [requestDictionary objectForKey:@"delegate"];
	id delegate = [delegateVal nonretainedObjectValue];
	
	if ([delegate respondsToSelector:@selector(storeKitProductInfoRequestFinished:)])
	{
		NSMutableDictionary* purchasables = [requestDictionary objectForKey:@"purchasables"];
		[delegate performSelector:@selector(storeKitProductInfoRequestFinished:) withObject:purchasables];
	}
}

- (NSString*)_stringForState:(SKPaymentTransactionState)state
{
	NSString* stateString = @"Unknown";
	
	switch (state)
	{
		case SKPaymentTransactionStatePurchasing:	stateString = @"Purchasing"; break;
		case SKPaymentTransactionStatePurchased:	stateString = @"Purchased"; break;
		case SKPaymentTransactionStateFailed:		stateString = @"Failed"; break;
		case SKPaymentTransactionStateRestored:		stateString = @"Restored"; break;
	}
	
	return stateString;
}

- (void)finishPurchase:(id)purchase
{
	if ([purchase isKindOfClass:[SKPaymentTransaction class]])
	{
		SKPaymentTransaction* transaction = (SKPaymentTransaction*)purchase;
		if (transaction.transactionIdentifier != nil)
		{
            OFXLog(@"StoreKit", @"Finishing transaction ID %@ for product %@", transaction.transactionIdentifier, transaction.payment.productIdentifier);
			[completedTransactions addObject:transaction.transactionIdentifier];
			[NSKeyedArchiver archiveRootObject:completedTransactions toFile:[OFSettings savePathForFile:@"ofxtransactionlog"]];
		}
		
		[[SKPaymentQueue defaultQueue] finishTransaction:transaction];
	}
}

- (void)invokePurchaseCompleteNotification:(SKPaymentTransaction*)purchase
{
	OFStoreKitPurchaseStatus status;
	
#if TARGET_IPHONE_SIMULATOR
	NSString* productId = (NSString*)purchase;
	status = OFStoreKitPurchaseStatus_Completed;
#else
	SKPaymentTransaction* transaction = (SKPaymentTransaction*)purchase;
	NSString* productId = [[transaction payment] productIdentifier];
    OFXLog(@"StoreKit", @"Purchase completed (transaction ID %@, product %@)", [transaction transactionIdentifier], productId);

	if (purchase.transactionState == SKPaymentTransactionStateRestored ||
		purchase.transactionState == SKPaymentTransactionStatePurchased)
	{
		status = OFStoreKitPurchaseStatus_Completed;
	}
	else if (purchase.error.code == SKErrorPaymentCancelled)
	{
		status = OFStoreKitPurchaseStatus_Cancelled;
	}
	else
	{
		status = OFStoreKitPurchaseStatus_Failed;
	}
#endif

	[[NSNotificationCenter defaultCenter] 
		postNotificationName:OFStoreKitPurchaseCompleted 
		object:nil 
		userInfo:[NSDictionary dictionaryWithObjectsAndKeys:
			productId, OFStoreKitUserInfo_ProductIdentifierKey,
			[NSNumber numberWithInt:status], OFStoreKitUserInfo_StatusKey,
			nil]];
}

#pragma mark -
#pragma mark OFXReachabilityObserver
#pragma mark -

- (void)reachabilityChangedFrom:(OFReachabilityStatus)oldStatus to:(OFReachabilityStatus)newStatus
{
	if (oldStatus == OFReachability_Not_Connected && newStatus != OFReachability_Not_Connected)
	{
		[self kickValidationQueue];		
	}
}

#pragma mark -
#pragma mark Receipt Validation
#pragma mark -

- (void)writeValidationQueue
{
	OFEncryptedFile* validationFile = [OFEncryptedFile encryptedFileWithFilename:@"ofxskvalidate"];
	validationFile.plaintext = [NSKeyedArchiver archivedDataWithRootObject:validationQueue];
}

- (void)kickValidationQueue
{
	if ([validationQueue count] > 0 && [OFReachability isConnectedToInternet])
	{
		validationInProgress = YES;
		[self performSelector:@selector(_validateNextPurchase) withObject:nil afterDelay:0.f];
	}
}

- (void)validatePurchase:(SKPaymentTransaction*)purchase
{
    // XXX [adill] i feel dirty about this line. we don't yet support multipart signed uploads in XP. feck.
	NSString* receiptData = [[[NSString alloc] initWithData:[purchase transactionReceipt] encoding:NSUTF8StringEncoding] autorelease];

    SKProduct* product = [productCatalog objectForKey:purchase.payment.productIdentifier];
    
	NSDictionary* purchaseDictionary = [NSDictionary dictionaryWithObjectsAndKeys:
		receiptData, @"receipt",
		[purchase transactionIdentifier], @"transaction_identifier",
		[purchase transactionDate], @"date",
		[[purchase payment] productIdentifier], @"product_identifier",
		[product.price stringValue], @"cost",
        [product.priceLocale objectForKey:NSLocaleCountryCode], @"country_code",
		nil];

	[validationQueue insertObject:purchaseDictionary atIndex:0];
	OFXLog(@"StoreKit", @"Purchase (%@) queued for validation", [purchaseDictionary objectForKey:@"transaction_identifier"]);
	
	[self writeValidationQueue];
	[self kickValidationQueue];
}

- (void)_validateNextPurchase
{
	NSDictionary* purchase = [validationQueue lastObject];

	OFAssert([[OpenFeint clientApplicationId] length] > 0, 
			 @"No Client Application ID set.  This is most likely because you don't have an offline config.  " 
			 "Offline configs are required for OFXStore");

	OFXLog(@"StoreKit", @"Purchase (%@) began validation", [purchase objectForKey:@"transaction_identifier"]);

	NSDictionary* params = [NSDictionary dictionaryWithObjectsAndKeys:
                            [OpenFeint clientApplicationId], @"client_application_id",
                            [purchase objectForKey:@"transaction_identifier"], @"transaction_id",
                            [OpenFeint lastLoggedInUserId], @"user_id",
                            [purchase objectForKey:@"country_code"], @"country",
                            [purchase objectForKey:@"receipt"], @"data",
                            [purchase objectForKey:@"cost"], @"cost",
                            nil];

    NSString* requestPath = [NSString stringWithFormat:@"/xp/games/%@/store_kit_receipts", [OpenFeint clientApplicationId]];
    OFResourceRequest* r = [OFResourceRequest postRequestWithPath:requestPath andBody:params];
	r.requiresUserSession = NO;
    [[r onRespondTarget:self selector:@selector(_validationResponse:)] execute];
}

- (void)_validationResponse:(OFResourceRequest*)request
{
	NSDictionary* purchase = [[validationQueue lastObject] retain];
	BOOL logEvent = YES;

    if (request.httpResponseCode == 200 ||		// restored (:ok)
		request.httpResponseCode == 201)		// unique (:created)
    {
		[validationQueue removeLastObject];
		OFXLog(@"StoreKit", @"Purchase (%@) validation succeeded.", [purchase objectForKey:@"transaction_identifier"]);
	}
    else if (request.httpResponseCode == 402)	// trial mode
    {
		[validationQueue removeLastObject];
		OFXLog(@"StoreKit", @"Purchase (%@) validation failed; trial mode.", [purchase objectForKey:@"transaction_identifier"]);

		[[NSNotificationCenter defaultCenter] 
			postNotificationName:OFStoreKitPurchaseInvalidated 
			object:nil 
			userInfo:[NSDictionary dictionaryWithObjectsAndKeys:
				[purchase objectForKey:@"product_identifier"], OFStoreKitUserInfo_ProductIdentifierKey,
				[NSNumber numberWithInt:OFStoreKitPurchaseStatus_Invalidated], OFStoreKitUserInfo_StatusKey,
				nil]];

        [[[[UIAlertView alloc] 
           initWithTitle:@"Trial Mode" 
           message:@"OFX is working in trial mode. Upgrade to the full version at api.openfeint.com."
           delegate:nil 
           cancelButtonTitle:@"Ok" 
           otherButtonTitles:nil] autorelease] show];
    }
    else
    {
		if ([request.resources isKindOfClass:[OFServerException class]] &&
			[request.resources isStoreKitReceiptInvalidException])
		{
			[validationQueue removeLastObject];
			OFXLog(@"StoreKit", @"Purchase (%@) validation failed; invalid receipt.", [purchase objectForKey:@"transaction_identifier"]);

			[[NSNotificationCenter defaultCenter] 
				postNotificationName:OFStoreKitPurchaseInvalidated 
				object:nil 
				userInfo:[NSDictionary dictionaryWithObjectsAndKeys:
					[purchase objectForKey:@"product_identifier"], OFStoreKitUserInfo_ProductIdentifierKey,
					[NSNumber numberWithInt:OFStoreKitPurchaseStatus_Invalidated], OFStoreKitUserInfo_StatusKey,
					nil]];
		}
		else
		{
			logEvent = NO;
			OFXLog(@"StoreKit", @"Purchase (%@) validation failed; network error.", [purchase objectForKey:@"transaction_identifier"]);
		}
    }
    
	if (logEvent)
	{
		NSDictionary* params = [NSDictionary dictionaryWithObjectsAndKeys:
			[purchase objectForKey:@"transaction_identifier"], @"transaction_id",
			[purchase objectForKey:@"product_identifier"], @"apple_product_identifier",
			[[UIDevice currentDevice] uniqueIdentifier], @"udid",
            [purchase objectForKey:@"country_code"], @"country",
			[purchase objectForKey:@"cost"], @"cost",
			nil];
		[[OpenFeintX eventLog] logEventNamed:@"store_kit_purchase" parameters:params];
	}
	
    [purchase release];
	[self writeValidationQueue];
	validationInProgress = NO;
	[self kickValidationQueue];
}

#pragma mark -
#pragma mark SKPaymentTransactionObserver
#pragma mark -

- (void)paymentQueue:(SKPaymentQueue *)queue updatedTransactions:(NSArray *)transactions
{
	for (SKPaymentTransaction* transaction in transactions)
	{
		if ([completedTransactions containsObject:transaction.transactionIdentifier])
		{
			OFXLog(@"StoreKit", @"Transaction (%@) has already been finished. Ignoring.", transaction.transactionIdentifier);
			[queue finishTransaction:transaction];
			continue;
		}

		OFXLog(@"StoreKit", @"Transaction (%@) updated state to %@", 
			transaction.transactionIdentifier,
			[self _stringForState:transaction.transactionState]);

		switch (transaction.transactionState)
		{
			case SKPaymentTransactionStatePurchased:
				[self invokePurchaseCompleteNotification:transaction];
				[self finishPurchase:transaction];
				[self validatePurchase:transaction];
				break;

			case SKPaymentTransactionStateFailed:
				[self invokePurchaseCompleteNotification:transaction];
				[self finishPurchase:transaction];
				break;

			case SKPaymentTransactionStateRestored:
				[self invokePurchaseCompleteNotification:transaction];
				[self finishPurchase:transaction];
				[self validatePurchase:transaction];
				break;

			case SKPaymentTransactionStatePurchasing:
			default:
				break;
		}
	}
}

- (void)paymentQueue:(SKPaymentQueue *)queue removedTransactions:(NSArray *)transactions
{
}

- (void)paymentQueue:(SKPaymentQueue *)queue restoreCompletedTransactionsFailedWithError:(NSError *)error
{
	OFXLog(@"StoreKit", @"Failed restoring completed transactions with error: %@", [error localizedDescription]);

	restoreInProgress = NO;

	[[NSNotificationCenter defaultCenter] 
		postNotificationName:OFStoreKitRestoreFinished 
		object:self 
		userInfo:[NSDictionary dictionaryWithObjectsAndKeys:
			error, OFStoreKitUserInfo_RestoreError,
			nil]];
}

- (void)paymentQueueRestoreCompletedTransactionsFinished:(SKPaymentQueue *)queue
{
	OFXLog(@"StoreKit", @"Finished restoring completed transactions!");

	restoreInProgress = NO;

	[[NSNotificationCenter defaultCenter] 
		postNotificationName:OFStoreKitRestoreFinished 
		object:self 
		userInfo:nil];
}

#pragma mark -
#pragma mark SKProductsRequestDelegate
#pragma mark -

- (void)productsRequest:(SKProductsRequest *)request didReceiveResponse:(SKProductsResponse *)response
{
	NSDictionary* requestDictionary = [self _requestDictionaryForRequest:request];	
	if (!requestDictionary)
		return;
	
	NSMutableDictionary* purchasables = [requestDictionary objectForKey:@"purchasables"];
	
	for (SKProduct* product in response.products)
	{
		id<OFStoreKitPurchasable> p = [purchasables objectForKey:product.productIdentifier];
		[p receivedStoreKitProductInformation:product];

		[productCatalog setObject:product forKey:product.productIdentifier];
	}
	
	for (NSString* invalidIdentifier in response.invalidProductIdentifiers)
	{
		OFXLog(@"StoreKit", @"Product has invalid Apple product identifier: %@", invalidIdentifier);
		[purchasables removeObjectForKey:invalidIdentifier];
	}
	
	[request release];
	
	OFXLog(@"StoreKit", @"Product information downloaded for %d product(s).", [response.products count]);

	[self _invokeRequestDelegate:requestDictionary];
}

- (void)request:(SKRequest *)request didFailWithError:(NSError *)error
{
	NSDictionary* requestDictionary = [self _requestDictionaryForRequest:(SKProductsRequest*)request];	
	if (!requestDictionary)
		return;

	NSMutableDictionary* purchasables = [requestDictionary objectForKey:@"purchasables"];
    [purchasables removeAllObjects];

	OFXLog(@"StoreKit", @"Failed to download product information. Reason: %@", [error localizedDescription]);

	[self _invokeRequestDelegate:requestDictionary];
}

@end
