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
#import "OFXStore.h"
#import "OFXStore+Private.h"
#import "OpenFeint/OpenFeint+UserOptions.h"
#import "OpenFeint/OpenFeint+Private.h"
#import "OpenFeint/OFFramedNavigationController.h"
#import "OpenFeint/OFIntroNavigationController.h"
#import "OpenFeint/OFControllerLoaderObjC.h"
#import "OFXHttpCredentialsFromStoreController.h"
#import "OpenFeint/OFReachability.h"

#ifdef _DEBUG
//#define ALWAYS_SHOW_OFX_FORMS
#endif

@implementation OFXStore (Public)

+(void)userWillEnterStore 
{
    BOOL showNagScreens = [OFReachability isConnectedToInternet];
#ifndef ALWAYS_SHOW_OFX_FORMS 
    showNagScreens = showNagScreens && ![OFXStore userHasSeenStoreNag];
#endif

    if (showNagScreens)
    {
        //first check if OpenFeint is not enabled, show special EnableOF screen if not
        if(![OpenFeint hasUserApprovedFeint])
        {
            //note that presentUserFeintApprovalMode will call the showCustomOpenFeintApprovalScreen delegate method
            //this means that if the developer says to override that, it will also override the store call
            //this needs to be communicated somehow, we don't want docs for OFX specific info inside the regular OF docs

            //swap in custom acceptance form
            [OFControllerLoaderObjC setOverrideAssetFileSuffix:@"OfxStore"];
            [OpenFeint presentUserFeintApprovalModalInvocation:nil deniedInvocation:nil];
            [OFControllerLoaderObjC setOverrideAssetFileSuffix:@"Ofx"];
            //TODO: need to tag the secure screen if the user chooses an unsecured account
        }
        //otherwise, a gentle reminder to secure your account
        else if(![OpenFeint loggedInUserHasHttpBasicCredential])
        {
            OFHttpCredentialsCreateController* httpController = (OFHttpCredentialsCreateController*)[[OFControllerLoaderObjC loader] load:@"HttpCredentialsCreateFromStore"];
            httpController.addingAdditionalCredential = YES;
            OFFramedNavigationController* navController = (OFFramedNavigationController*)[[OFNavigationController alloc] initWithRootViewController:httpController];
            [navController setNavigationBarHidden:YES];
            OFIntroNavigationController* introController = [[OFIntroNavigationController alloc] initWithNavigationController:navController];
            [OpenFeint presentRootControllerWithModal:introController];
            [navController release];
            [introController release];
        }

        [OFXStore setUserHasSeenStoreNag];
    }
}

+ (void)userWillLeaveStore
{
	[OFInventory storeInventory];
	[OFInventory synchronizeInventory];
}

+ (void)restoreStoreKitNonconsumablePurchases
{
	[[OFXStore storeKit] restorePurchases];
}

+ (NSString*)rootPathForPayloads
{
    return [OFXStore _rootPathForPayloads];
}

@end
