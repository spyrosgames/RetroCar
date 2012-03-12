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

#import "OFXHttpCredentialsFromIntroController.h"
#import "OpenFeint/OpenFeint+Private.h"
#import "OpenFeint/OFControllerLoaderObjC.h"
#import "OpenFeint/OpenFeint+GameCenter.h"
#import "OpenFeint/OpenFeint+UserOptions.h"

//this is a private method in the parent class
@interface OFHttpCredentialsCreateController ()
- (void)onFormSubmitted;
- (void)onFormSubmitted:(id)resources;
@end


@implementation OFXHttpCredentialsFromIntroController


-(void)viewWillAppear:(BOOL)animated {
    self.addingAdditionalCredential = YES;
    self.navigationItem.hidesBackButton = YES;
    [super viewWillAppear:animated];
}

-(void)viewDidAppear:(BOOL)animated {
    [[self navigationController] setNavigationBarHidden:YES animated:NO];
    [super viewDidAppear:animated];
}

-(void)_showGameCenterOrQuit {
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


-(IBAction) cancel {
	[self _showGameCenterOrQuit];
}

-(void)continueFlow {
	[self _showGameCenterOrQuit];	
}

- (void)onFormSubmitted
{
    [self onFormSubmitted:nil];
}

- (void)onFormSubmitted:(id)resources
{
	//can't use the standard flow control logic, it pops the control or pushes a "popping" controller
	[OpenFeint setLoggedInUserHasHttpBasicCredential:YES];
	[self _showGameCenterOrQuit];	
}


-(void) popOutOfAccountFlow {
    [OpenFeint dismissRootControllerOrItsModal];
}

@end
