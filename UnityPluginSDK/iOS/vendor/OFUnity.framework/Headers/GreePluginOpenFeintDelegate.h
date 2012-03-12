//
// Copyright 2011 GREE International, Inc.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//    http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//

#import <Foundation/Foundation.h>
#import "GreePluginOpenFeintEnumTypes.h"

#import <OFHighScore.h>
#import <OFAchievement.h>
#import <OFChallengeToUser.h>
#import <OFChallengeDefinition.h>
#import	<OFCloudStorage.h>

@class OFAchievement;
@class OFNotificationData;
@class OFChallengeDefinition;


@protocol GreePluginOpenFeintDelegate <NSObject>

- (void)triggerGeneralEvent:(eGeneralEvent)eventIndex;
- (void)triggerDeferredResultEvent:(eDeferredResultEvent)eventIndex result:(BOOL)result;
- (void)triggerUserLogInEvent:(eUserLogInEvent)eventIndex userId:(NSString*)userId;
- (void)triggerNotificationEvent:(OFNotificationData*)notificationData;
- (void)triggerStartedChallengeEvent:(OFChallengeDefinition *)challengeDefinition;

- (void)triggerAchievementIconLoad:(OFAchievement *)achievement withFilePath:(NSString *)filePath;

- (void)triggerDeferredAchievementElementCallback:(NSString *)elementValue result:(BOOL)result;
- (void)triggerDeferredAchievementListCallback:(NSArray *)achievments;
- (void)triggerDeferredAchievementPropertiesCallback:(NSDictionary *)achievementProperties;

- (void)triggerChallengeDefinitionValuesCallback:(NSDictionary *)definitionValues success:(BOOL)success;
- (void)triggerDownloadHighScorePayloadCallback:(NSString*)filePath;

//////////////////////////////////////////////////////////////////////////////////////////
/// 
/// Called on result of `-isLocalUserFollowingAnyone`
/// 
//////////////////////////////////////////////////////////////////////////////////////////
- (void)triggerFollowingResultCallback:(BOOL)success commandResult:(BOOL)commandResult;
- (void)triggerDownloadCloudBlobResultCallback:(BOOL)result;

- (void)triggerDeferredLeaderboardHighscoreCallback:(OFHighScore *)highScore;

- (void)triggerDeferredHighscoreResultEvent:(NSString *)tableString leaderboardString:(NSString *)leaderboardString;
- (void)triggerHighScoreDownloadFailed;
- (void)pauseAudio:(BOOL)pauseAudioToggle;

@end
