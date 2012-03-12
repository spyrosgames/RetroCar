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

#import "GreePluginOpenFeint.h"

@interface GreePluginOpenFeint (FromUnity)
// Unity To Native Bridge Methods
- (void)bridge_launchDashboard:(NSMutableArray *)args;
- (void)bridge_submitHighScore:(NSMutableArray *)args;
- (void)bridge_launchDashboardWithLeaderboardId:(NSMutableArray *)args;
- (void)bridge_sendSocialNotification:(NSMutableArray *)args;
- (void)bridge_sendWithPrepopulatedText:(NSMutableArray *)args;
- (void)bridge_inGameNotification:(NSMutableArray *)args;
- (void)bridge_getLeaderboardHighScore:(NSMutableArray *)args;
- (void)bridge_downloadHighScorePayload:(NSMutableArray *)args;
- (void)bridge_getAchievementByElementIdAndField:(NSMutableArray *)args;
- (void)bridge_unlockAchievement:(NSMutableArray *)args;
- (void)bridge_unlockAchievementDeferred:(NSMutableArray *)args;
- (void)bridge_updateAchievement:(NSMutableArray *)args;
- (void)bridge_updateAchievementDeferred:(NSMutableArray *)args;
- (void)bridge_submitDeferredAchievements:(NSMutableArray *)args;
- (void)bridge_getAchievementIcon:(NSMutableArray *)args;
- (void)bridge_achievementList:(NSMutableArray *)args;
- (void)bridge_setAchievementSocialUrl:(NSMutableArray *)args;
- (void)bridge_achievementProperties:(NSMutableArray *)args;
- (void)bridge_getChallengeDataToPlayerPrefs:(NSMutableArray *)args;
- (void)bridge_getChallengeDataToFile:(NSMutableArray *)args;
- (void)bridge_displaySendChallengeModalWithPlayerPrefs:(NSMutableArray *)args;
- (void)bridge_displaySendChallengeModalWithFile:(NSMutableArray *)args;
- (void)bridge_submitChallengeResult:(NSMutableArray *)args;
- (void)bridge_displayChallengeCompletedModalWithPlayerPrefs:(NSMutableArray *)args;
- (void)bridge_displayChallengeCompletedModalWithFile:(NSMutableArray *)args;
- (void)bridge_getChallengeDefinition:(NSMutableArray *)args;
- (void)bridge_uploadBlobFromPlayerPrefs:(NSMutableArray *)args;
- (void)bridge_uploadBlobFromFile:(NSMutableArray *)args;
- (void)bridge_downloadBlobToPlayerPrefs:(NSMutableArray *)args;
- (void)bridge_downloadBlobToFile:(NSMutableArray *)args;
- (void)bridge_setDashboardOrientation:(NSMutableArray *)args;
- (void)bridge_presentUserFeintApprovalModal:(NSMutableArray *)args;
- (void)bridge_isLocalUserFollowingAnyone:(NSMutableArray *)args;
- (id)bridge_hasUserApprovedFeint:(NSMutableArray *)args;
- (id)bridge_isOnline:(NSMutableArray *)args;
- (id)bridge_localUserID:(NSMutableArray *)args;
- (id)bridge_localUserName:(NSMutableArray *)args;
- (id)bridge_userAllowsNotifications:(NSMutableArray *)args;
- (void)bridge_setUserAllowsNotifications:(NSMutableArray *)args;
- (id)bridge_getNumberOfUnviewedChallenges:(NSMutableArray *)args;
- (id)bridge_openFeintReleaseVersionString:(NSMutableArray *)args;


@end
