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

#import "OpenFeint/OFExistingAccountController.h"
#import "OpenFeint/OpenFeint+Private.h"
#import "OpenFeint/OpenFeint+UserOptions.h"
#import "OpenFeint/OpenFeint+GameCenter.h"
#import "OpenFeint/OFControllerLoaderObjC.h"
#import "OpenFeint/OFAccountSetupBaseController.h"

@implementation OFExistingAccountController (OFXFlow)
- (void)continueFlow
{
	[OpenFeint setDoneWithGetTheMost:YES];
    if([OpenFeint loggedInUserHasHttpBasicCredential]) {
		//Game Center or dismiss
		if([OpenFeint isUsingGameCenter]) {
			OFViewController* controller = (OFViewController*)[[OFControllerLoaderObjC loader] load:@"GameCenterIntegration"];
			[[self navigationController] pushViewController:controller animated:YES];
		}
		else {
			[OpenFeint startLocationManagerIfAllowed];
			[OpenFeint allowErrorScreens:YES];
			[OpenFeint dismissRootControllerOrItsModal];
		}

        [onCompletionInvocation invoke];
    }
    else {
        OFAccountSetupBaseController* controller = (OFAccountSetupBaseController*)[[OFControllerLoaderObjC loader] load:@"HttpCredentialsCreateFromIntro"];
        [controller setOnCompletionInvocation:onCompletionInvocation];
        [[self navigationController] pushViewController:controller animated:YES];
    }
}

@end
