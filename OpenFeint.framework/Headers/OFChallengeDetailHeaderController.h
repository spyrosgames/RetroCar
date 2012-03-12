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

#import "OFTableControllerHeader.h"
#import "OFChallengeToUser.h"
#import "OFImageView.h"

@class OFChallengeDetailController;
@class OFChallengeDetailFrame;

@interface OFChallengeDetailHeaderController : UIViewController<OFTableControllerHeader>
{
@private
	OFChallengeDetailController* challengeDetailController;
	UIButton*					startChallengeButton;
	OFChallengeDetailFrame*		challengeDescriptionContainer;
	
}

@property (nonatomic, readwrite, assign) IBOutlet OFChallengeDetailController* challengeDetailController;
@property (nonatomic, retain) IBOutlet UIButton*				startChallengeButton;
@property (nonatomic, retain) IBOutlet OFChallengeDetailFrame*	challengeDescriptionContainer;

- (void)setChallengeToUser:(OFChallengeToUser*)newChallenge;

@end
