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
#import <UIKit/UIKit.h>
#import "GreePluginOpenFeintEnumTypes.h"
#import "GreePluginOpenFeintDelegate.h"


#import <OpenFeint.h>
#import <OFHighScore.h>
#import <OFLeaderboard.h>
#import <OFAchievement.h>
#import <OFChallenge.h>
#import <OFChallengeToUser.h>
#import <OFChallengeDefinition.h>
#import <OFCloudStorage.h>

#import <OpenFeint/OpenFeint+Dashboard.h>
#import <OpenFeint/OpenFeint+UserOptions.h>
#import <OpenFeint/OFHighScoreService.h>
#import <OpenFeint/OFAchievementService.h>
#import <OpenFeint/OFFriendsService.h>
#import <OpenFeint/OFChallengeService.h>
#import <OpenFeint/OFChallenge.h>
#import <OpenFeint/OFChallengeDefinition.h>
#import <OpenFeint/OFChallengeDefinitionService.h>
#import <OpenFeint/OFChallengeToUser.h>
#import <OpenFeint/OFCloudStorageService.h>

// new stuff for scores (Seon)
#import <OpenFeint/OFLeaderboardService.h>
#import <OpenFeint/OFLeaderboard.h>
#import <OpenFeint/OFAchievement.h>
//#import <OpenFeint/OFHighScore.h>
#import <OpenFeint/OFTableSectionDescription.h>
#import <OpenFeint/OFNotification.h>

// Gree Categories
#import "NSDictionary+GreeExtensions.h"
//#import "OFAchievement+Gree.h"
#import "NSFileManager+OverwriteFile.h" 

// Constants.
#define ofChallengeToUserFile @"OFChallengeToUser.dat"
#define ofChallengeDataFile  @"OFChallengeData.dat"

/**
 Wraps the OpenFeint SDK into more easily used methods to accomplish most commonly used features.
 
 This class isn't usable unless an NSDictionary with a minimum set of keys is passed to initWithOptions:
 - OF_PRODUCT_KEY
 - OF_SECRET_KEY
 - OF_APP_DISPLAY_NAME
 - OF_ALLOW_NOTIFICATIONS
 - OF_ALLOW_STRONGER_ATTEMPT_FOR_CHALLENGES
 
 Other keys can be used to configure OpenFeint and have corresponding OpenFeintSetting keys:
 - OF_INITIAL_DASHBOARD_ORIENTATION corresponds to OpenFeintSettingDashboardOrientation
 - OF_APP_SHORT_DISPLAY_NAME corresponds to OpenFeintSettingShortDisplayName
 - OF_NOTIFICATION_POSITION corresponds to OpenFeintSettingNotificationPosition
 - OF_ENABLE_PUSH_NOTIFICATIONS corresponds to OpenFeintSettingEnablePushNotifications
 - OF_DISABLE_USER_GENERATED_CONTENT corresponds to OpenFeintSettingDisableUserGeneratedContent
 - OF_ALWAYS_ASK_FOR_APPROVAL_IN_DEBUG corresponds to OpenFeintSettingAlwaysAskForApprovalInDebug
 - OF_DISABLE_LOCATION_SERVICES corresponds to OpenFeintSettingDisableLocationServices
 - OF_PROMPT_USER_FOR_LOGIN corresponds to OpenFeintSettingPromptUserForLogin,
	- By default, approval screen will pop up when calling initialization method.  If this behavior is uncomfortable for your application, you can defer the approval by specifying NO for OpenFeintSettingPromptUserForLogin.  The approval process can be started by calling [OpenFeint presentUserFeintApprovalModalInvocation:deniedInvocation:] any time. 
 - OF_DEVELOPMENT_MODE corresponds to OpenFeintSettingDevelopmentMode,
 */
@interface GreePluginOpenFeint : NSObject <OpenFeintDelegate, OFNotificationDelegate, OFHighScoreDelegate, OFAchievementDelegate, OFChallengeDelegate, OFChallengeToUserDelegate, OFChallengeDefinitionDelegate, OFCloudStorageDelegate, OFLeaderboardDelegate>

/*
	NSDictionary						*_configuration;
	NSString							*_cloudBlobDownloadLocation;
	NSArray								*_cloudBlobPlayerPrefKeys;
	NSDictionary						*_playerPrefs;
	OFChallengeDefinition				*_currentChallengeDefinition;	
	
	id<GreePluginOpenFeintDelegate>		_pluginOFDelegate;
	BOOL								_gamePaused;
	BOOL								_allowNotifications;
	BOOL								_allowStrongerChallenges;
	NSUInteger							_numberOfUnviewedChallenges;
*/	 

@property (nonatomic, assign) id<GreePluginOpenFeintDelegate>	pluginOFDelegate;


- (id)initWithOptions:(NSDictionary *)opts;
- (void)shutDownOpenFeint;
- (NSDictionary*)getOpenFeintSettings;

//////////////////////////////////////////////////////////////////////////////////////////
/// Uses `[OpenFeint releaseVersionString]` to return the version of the linked OpenFeint SDK.
/// @return	release version
//////////////////////////////////////////////////////////////////////////////////////////
- (NSString *)openFeintReleaseVersionString;

//////////////////////////////////////////////////////////////////////////////////////////
/// 
/// Sets what orientation the dashboard and notifications will show in.
/// 
//////////////////////////////////////////////////////////////////////////////////////////
- (void)setDashboardOrientation:(UIInterfaceOrientation)orientation;


//////////////////////////////////////////////////////////////////////////////////////////
/// Launches the OpenFeint Dashboard view at the top of your application's keyed window.
///
/// @note	If the player has not yet authorized your app, they will be prompted to setup an 
/// account or authorize your application before accessing the OpenFeint dashboard
//////////////////////////////////////////////////////////////////////////////////////////
- (void)launchDashboard:(NSString *)pageId;
- (void)launchDashboardWithLeaderboardId:(NSString *)leaderboardId;


////////////////////////////////////////////////////////////
///
/// The OpenFeint Approval flow is launched automatically on initialization (and dashboard launch if user hasn't approved OpenFeint). 
/// @note	Only call this when the user requests to use OpenFeint features and `[OpenFeint hasUserApprovedFeint]` returns NO.
/// An example is when a user wants to create a challenge.
///
////////////////////////////////////////////////////////////
- (void)presentUserFeintApprovalModal;

////////////////////////////////////////////////////////////
///
/// Returns whether or not the user has enabled OpenFeint for this game.
///
////////////////////////////////////////////////////////////
- (id)hasUserApprovedFeint;

////////////////////////////////////////////////////////////
///
/// Returns whether or not the game is connected to the OpenFeint server
///
////////////////////////////////////////////////////////////
- (id)isOnline;

//////////////////////////////////////////////////////////////////////////////////////////
/// 
/// Returns the user id of the local user.
/// 
//////////////////////////////////////////////////////////////////////////////////////////
- (id)localUserID;

//////////////////////////////////////////////////////////////////////////////////////////
/// 
/// Returns the user name of the local user.
/// 
//////////////////////////////////////////////////////////////////////////////////////////
- (id)localUserName;


////////////////////////////////////////////////////////////
///
/// isLocalUserFollowingAnyone
/// 
/// @note	calls delegate method with the result `- (void)triggerFollowingResultCallback:(BOOL)success commandResult:(BOOL)commandResult`
////////////////////////////////////////////////////////////
- (void)isLocalUserFollowingAnyone;

//////////////////////////////////////////////////////////////////////////////////////////
/// 
/// returns NSNumber of true bool if `-setUserAllowsNotifications` has been called to allow notifications
/// 
//////////////////////////////////////////////////////////////////////////////////////////                                                                                            
- (id)userAllowsNotifications;

//////////////////////////////////////////////////////////////////////////////////////////
/// 
///	Allows notifications.
/// 
/// @param	notificationToggle	value turn on or off notifications.
/// @note:	This overrides the configuration setting: OF_ALLOW_NOTIFICATIONS
/// 
//////////////////////////////////////////////////////////////////////////////////////////
- (void)setUserAllowsNotifications:(BOOL)notificationToggle;


//////////////////////////////////////////////////////////////////////////////////////////
/// Sends a social notification to Twitter and Facebook.
/// 
/// @param	notificationText		text to send
/// @param	imageIdentifierName	the identifier name of the image to send with the text.
//////////////////////////////////////////////////////////////////////////////////////////
- (void)sendSocialNotification:(NSString *)notificationText imageIdentifierName:(NSString *)imageIdentifierName;


//////////////////////////////////////////////////////////////////////////////////////////
/// Uses FBConnect to send a text.
/// 
/// @param	displayedText	Displayed Text.
/// @param	originalMessage	Original message.
/// @param	imageName		Image name for displaying.
/// @param	linkedUrl		Custom url for social api.
//////////////////////////////////////////////////////////////////////////////////////////
- (void)sendWithPrepopulatedText:(NSString *)displayedText originalMessage:(NSString *)originalMessage imageName:(NSString *)imageName linkedUrl:(NSString *)linkedUrl; 

//////////////////////////////////////////////////////////////////////////////////////////
/// Send In Game notification
/// 
/// @param	notificationText	The text for content of the notification
/// @param	imageName			The image name for notification icon, @"" for no image
//////////////////////////////////////////////////////////////////////////////////////////
- (void)inGameNotification:(NSString *)notificationText imageName:(NSString *)imageName;


//////////////////////////////////////////////////////////////////////////////////////////
/// Submit score to a leaderboard.
///
/// @param	score			The score to submit.
/// @param	displayText		What is displayed in the leaderboard.  If displayText is nil, the score is shown instead.
/// @param	leaderboardId	The leaderboard id to submit to.
/// @param	silently		The leaderboard id string to submit to.
/// 
/// @note	calls delegate method `[self.pluginOFDelegate triggerDeferredResultEvent:eDeferredResultEventOnHighScoreSubmitted result:YES]` on success or
/// delegate method `[self.pluginOFDelegate triggerDeferredResultEvent:eDeferredResultEventOnHighScoreSubmitted result:NO]` on failure.
/// @note	silently uses the following logic internally to show notifications	
///			if (!silently)
///			{
///				OFNotificationData* notice = [OFNotificationData dataWithText:notificationText andCategory:kNotificationCategoryHighScore andType:kNotificationTypeSuccess];
///				notice.imageName = @"HighScoreNotificationIcon.png";
///				if([OpenFeint isOnline])
///				{
///					[[OFNotification sharedInstance] setDefaultBackgroundNotice:notice andStatus:OFNotificationStatusSuccess];
///				}
///				else 
///				{
///					[[OFNotification sharedInstance] showBackgroundNotice:notice andStatus:OFNotificationStatusSuccess];
///				}
///			}
//////////////////////////////////////////////////////////////////////////////////////////
- (void)submitHighScore:(long long)score displayText:(NSString *)displayText leaderboardId:(NSString *)leaderboardId silently:(BOOL)silently;



//////////////////////////////////////////////////////////////////////////////////////////
/// Get a high score of a specific leaderboard.
///
/// @return	The leaderboard high score.
/// @param	leaderboardId	String containing the leaderboard ID where the score is to be retrived.
/// @note	calls `[self.pluginOFDelegate triggerDeferredLeaderboardHighscoreCallback:highScore]` with the OFHighScore object.
//////////////////////////////////////////////////////////////////////////////////////////
- (void)getLeaderboardHighScore:(NSString *)leaderboardId;

//////////////////////////////////////////////////////////////////////////////////////////
/// Gets a paginated table of highscores for a specific leaderboard
/// 
/// @param	leaderboardId	String containing the leaderboard ID where the score is to be retrived.
/// @param	pageIndex		The page number to retrieve
/// @param	silently		BOOL to show related notifications
/// @note	on successful completetion, the delegate method `[self.pluginOFDelegate triggerDeferredHighscoreResultEvent:tableString leaderboardString:leaderboardString]` is called
/// with a table of highscores represented by rows delimited by `:` and fields of userName and scores delimited by `,`. 
/// On a failed completion `[self.pluginOFDelegate triggerHighScoreDownloadFailed]` is called.
//////////////////////////////////////////////////////////////////////////////////////////
- (void)getHighScoreForLeaderboardIDAndPageNumber:(NSString *)leaderboardId page:(NSInteger)pageIndex silently:(BOOL)silently;

//////////////////////////////////////////////////////////////////////////////////////////
/// Download the blob asociated with a high score of a specific leaderboard.
///
/// @return	The leaderboard high score.
/// @param	leaderboardId	String containing the leaderboard ID where the score is to be retrived.
/// @param  scoreId String containing the score ID to look for
/// @note	calls `[self.pluginOFDelegate triggerDownloadHighScorePayloadCallback:].
//////////////////////////////////////////////////////////////////////////////////////////
- (void)downloadHighScorePayload:(NSString*)leaderboardId scoreId:(NSString*)scoreId;

//////////////////////////////////////////////////////////////////////////////////////////
/// Get element for special achivement.
///
/// @return The achivement element by identifier and field.
/// @param	fieldName		The property name of the Achivement object
/// @param	achievementId	String containing the achivement ID.
/// @note	calls delegate method `[self.pluginOFDelegate triggerDeferredAchievementElementCallback:elementValue result:YES]` with the value
/// of the property.
//////////////////////////////////////////////////////////////////////////////////////////
- (void)getAchievementField:(NSString *)fieldName forAchievementId:(NSString *)achievementId;


//////////////////////////////////////////////////////////////////////////////////////////
/// Unlocks the achievement.
/// 
/// @param	achievementId	Achievement identifier.
/// @note	calls delegate method `[self.pluginOFDelegate triggerDeferredResultEvent:eDeferredResultEventOnAchievementUnlocked result:YES]` on success.
//////////////////////////////////////////////////////////////////////////////////////////
- (void)unlockAchievement:(NSString *)achievementId;

//////////////////////////////////////////////////////////////////////////////////////////
/// Unlocks the achievement deferred.
/// 
/// @param	achievementId	Achievement identifier.
/// @note	calls delegate method `[self.pluginOFDelegate triggerDeferredResultEvent:eDeferredResultEventOnAchievementUnlocked result:YES]` on success.
//////////////////////////////////////////////////////////////////////////////////////////
- (void)unlockAchievementDeferred:(NSString *)achievementId;
           

//////////////////////////////////////////////////////////////////////////////////////////
/// Updates an achievement.
/// 
/// @param	achievementId	Achievement identifier.
/// @param	percentageComplete	Percentage of the completion.
/// @param	showNotification	Whether to show notification.
/// @note	calls delegate method `[self.pluginOFDelegate triggerDeferredResultEvent:eDeferredResultEventOnAchievementUnlocked result:YES]` on success.
//////////////////////////////////////////////////////////////////////////////////////////
- (void)updateAchievement:(NSString *)achievementId percentageComplete:(double)percentageComplete showNotification:(BOOL)showNotification;

//////////////////////////////////////////////////////////////////////////////////////////
/// Updates an achievement deferred.
/// 
/// @param	achievementId	Achievement identifier.
/// @param	percentageComplete	Percentage of the completion.
/// @param	showNotification	Whether to show notification.
/// @note	calls delegate method `[self.pluginOFDelegate triggerDeferredResultEvent:eDeferredResultEventOnAchievementUnlocked result:YES]` on success.
//////////////////////////////////////////////////////////////////////////////////////////
- (void)updateAchievementDeferred:(NSString *)achievementId percentageComplete:(double)percentageComplete showNotification:(BOOL)showNotification;
        
//////////////////////////////////////////////////////////////////////////////////////////
/// 
/// Triggers the submission of defered achievements
/// 
//////////////////////////////////////////////////////////////////////////////////////////
- (void)submitDeferredAchievements; 


//////////////////////////////////////////////////////////////////////////////////////////
/// Gets the achievement icon.
/// 
/// @param	achievementId	Achievement identifier.
/// @note	On success calls:  `[self.pluginOFDelegate triggerAchievementIconLoad:withFilePath:]` with the achievement id and filePath string.
/// filepath is nil if unsucessfull. 
//////////////////////////////////////////////////////////////////////////////////////////
- (void)getAchievementIcon:(NSString *)achievementId;

//////////////////////////////////////////////////////////////////////////////////////////
/// Set a url to goto when someone clicks the social post.
///
/// @param	url	The url to goto.
//////////////////////////////////////////////////////////////////////////////////////////
- (void)setAchievementSocialUrl:(NSString *)url;

//////////////////////////////////////////////////////////////////////////////////////////
/// Retrieve a list of achievement ids.
///
/// @note	Calls delegate method `[self.pluginOFDelegate triggerDeferredAchievementListCallback:achievements]` with an NSArray of achievements.
//////////////////////////////////////////////////////////////////////////////////////////
- (void)achievementList;

//////////////////////////////////////////////////////////////////////////////////////////
/// Retrieve a list of achievement properties.
/// 
/// @param	achievementId	Achievement identifier.
/// @note	Calls delegate method `[self.pluginOFDelegate triggerDeferredAchievementPropertiesCallback:achievementProperties]` with an NSDictionary of properties of the
/// Achievement object represented my properties as keys.
//////////////////////////////////////////////////////////////////////////////////////////
- (void)achievementProperties:(NSString *)achievementId;


//////////////////////////////////////////////////////////////////////////////////////////
/// The number of unviewed challenges when the user logged in.
/// 
/// @return	The number of unviewed challenges at login.
//////////////////////////////////////////////////////////////////////////////////////////
- (NSUInteger)getNumberOfUnviewedChallenges;

//////////////////////////////////////////////////////////////////////////////////////////
/// Get the challenge data that was sent with a challenge.
/// 
/// @param	args	An array of PlayerPrefs keys where the challenge data for this challenge will be stored to.
//////////////////////////////////////////////////////////////////////////////////////////
- (void)getChallengeDataToPlayerPrefs:(NSMutableArray *)args;

//////////////////////////////////////////////////////////////////////////////////////////
/// Get the challenge data that was sent with a challenge.
/// 
/// @param	filePath	The path to a file in the app's Documents directory that will hold the challenge data.
//////////////////////////////////////////////////////////////////////////////////////////
- (void)getChallengeDataToFile:(NSString *)filePath;


//////////////////////////////////////////////////////////////////////////////////////////
/// Displays the modal for sending a challenge to another player.
/// 
/// @param	challengeId		The ID of the challenge.
/// @param	challengeText	Displays what needs to be fulfilled to complete the challenge.
/// @param	playerPrefsKeys	The path to a file located in the app's Caches directory that contains the challenge data. 
/// This is the preferred method for serialized XML data (Unity iPhone Advanced only).
//////////////////////////////////////////////////////////////////////////////////////////
- (void)displaySendChallengeModal:(NSString *)challengeId challengeText:(NSString *)challengeText withPlayerPrefsKeys:(NSMutableArray *)playerPrefsKeys;

//////////////////////////////////////////////////////////////////////////////////////////
/// Displays the modal for sending a challenge to another player.
/// 
/// @param	challengeId		The ID of the challenge.
/// @param	challengeText	Displays what needs to be fulfilled to complete the challenge.
/// @param	playerPrefsKeys	An array of PlayerPrefs keys where the data for this challenge is stored. This will be packaged 
/// up an sent as the challenge data from this player.
//////////////////////////////////////////////////////////////////////////////////////////
- (void)displaySendChallengeModal:(NSString *)challengeId challengeText:(NSString *)challengeText withFile:(NSString *)dataFilePath;

//////////////////////////////////////////////////////////////////////////////////////////
/// Submits the result of a played challenge to the server. This should always be called
/// after the challenge game play session is over, regardless of the result or the type
/// of challenge. It should be called before DisplayChallengeCompletedModal.
/// 
/// @param	challengeResult	The result of the challenge.
/// @param	description		A string that will be prefixed by either the recipient's name or "You", depending on 
/// the player that is viewing the result. It should not contain whether the recipient won 
/// or lost, but should contain the statistics of his attempt. (Ex. "beat 30 monsters" will 
/// turn into "You beat 30 monsters")
/// @note	On success calls delegate method `[self.pluginOFDelegate triggerDeferredResultEvent:eDeferredResultEventOnChallengeResultSubmitted result:YES]`.
/// On failure calls delegate method `[self.pluginOFDelegate triggerDeferredResultEvent:eDeferredResultEventOnChallengeResultSubmitted result:NO]`.
//////////////////////////////////////////////////////////////////////////////////////////
- (void)submitChallengeResult:(int)challengeResult description:(NSString *)description;


//////////////////////////////////////////////////////////////////////////////////////////
/// Displays the modal for showing the result of the challenge and allows for the user
/// to re-challenge (if it was a multi-attempt challenge). Call SubmitChallengeResult BEFORE
/// calling this.
/// 
/// @param	challengeResult		The result of the challenge.
/// @param	resultDescription	A string that will be prefixed by either the recipient's name or "You", depending on the player 
/// that is viewing the result. It should not contain whether the recipient won or lost, but 
/// should contain the statistics of his attempt. (Ex. "beat 30 monsters" will turn into 
/// "You beat 30 monsters")
/// @param	description			If the user decides to send out the result as a re-challenge, this will be the description 
/// for the new challenge. It should display what needs to be fulfilled to complete the challenge. 
/// Only used by multi-attempt challenges.
/// @param	challengeDataKeys	Tells the native code how/where to access the challenge data if the user decides to send out a 
/// "re-challenge". Only used for multi-attempt challenges.
//////////////////////////////////////////////////////////////////////////////////////////
- (void)displayChallengeCompletedModal:(int)challengeResult resultDescription:(NSString *)resultDescription description:(NSString *)description withPlayerPrefsKeys:(NSArray *)challengeDataKeys;

//////////////////////////////////////////////////////////////////////////////////////////
/// Displays the modal for showing the result of the challenge and allows for the user
/// to re-challenge (if it was a multi-attempt challenge). Call SubmitChallengeResult BEFORE
/// calling this.
/// 
/// @param	challengeResult		The result of the challenge.
/// @param	resultDescription	A string that will be prefixed by either the recipient's name or "You", depending on the player 
/// that is viewing the result. It should not contain whether the recipient won or lost, but 
/// should contain the statistics of his attempt. (Ex. "beat 30 monsters" will turn into 
/// "You beat 30 monsters")
/// @param	description			If the user decides to send out the result as a re-challenge, this will be the description 
/// for the new challenge. It should display what needs to be fulfilled to complete the challenge. 
/// Only used by multi-attempt challenges.
/// @param	dataFilePath	Tells the native code how/where to access the challenge data if the user decides to send out a 
/// "re-challenge". Only used for multi-attempt challenges.
//////////////////////////////////////////////////////////////////////////////////////////
- (void)displayChallengeCompletedModal:(int)challengeResult resultDescription:(NSString *)resultDescription description:(NSString *)description withFile:(NSString *)dataFilePath;

//////////////////////////////////////////////////////////////////////////////////////////
/// Get a challenge defintion structure for a particular challenge ID.
/// 
/// @param	challengeId	The ID of the challenge.
/// @note	On success calls the delegate method `[self.pluginOFDelegate triggerChallengeDefinitionValuesCallback:definitionValues success:YES]` with an NSDictionary with the keys
/// @"title", @"challengeId", @"iconUrl", and @"multiAttempt" to represent the values.
/// On failure calls the delegate method `[self.pluginOFDelegate triggerChallengeDefinitionValuesCallback:nil success:NO]`
//////////////////////////////////////////////////////////////////////////////////////////
- (void)getChallengeDefinition:(NSString *)challengeId;
     

//////////////////////////////////////////////////////////////////////////////////////////
/// Upload a data blob to the "cloud", which can be later retrieved on other devices.
/// 
/// @param	blobKey			The key of the blob you wish to upload to the cloud.
/// @param	playerPrefsKeys	An array of PlayerPrefs keys you wish to send to the cloud.
/// @note	On success calls delegate method `[self.pluginOFDelegate triggerDeferredResultEvent:eDeferredResultEventOnUploadedCloudBlob result:YES]`
/// On failure calls delegate method `[self.pluginOFDelegate triggerDeferredResultEvent:eDeferredResultEventOnUploadedCloudBlob result:NO]`
//////////////////////////////////////////////////////////////////////////////////////////
- (void)uploadBlob:(NSString *)blobKey fromPlayerPrefsKeys:(NSArray *)playerPrefsKeys;

//////////////////////////////////////////////////////////////////////////////////////////
/// Upload a data blob to the "cloud", which can be later retrieved on other devices.
/// 
/// @param	blobKey			The key of the blob you wish to upload to the cloud.
/// @param	filePath	A path to a file in the app's Documents directory that you wish to upload to the cloud. 
/// This is the preferred method if using serialized XML data (Unity iPhone Advanced only).
/// @note	On success calls delegate method `[self.pluginOFDelegate triggerDeferredResultEvent:eDeferredResultEventOnUploadedCloudBlob result:YES]`
/// On failure calls delegate method `[self.pluginOFDelegate triggerDeferredResultEvent:eDeferredResultEventOnUploadedCloudBlob result:NO]`
//////////////////////////////////////////////////////////////////////////////////////////
- (void)uploadBlob:(NSString *)blobKey fromFile:(NSString *)filePath;


//////////////////////////////////////////////////////////////////////////////////////////
/// Download a previously uploaded data blob from the "cloud".
/// 
/// @param	blobKey			The key of the blob you wish to download from the cloud.
/// @param	playerPrefsKeys	An array of PlayerPrefs keys that will be populated with the downloaded blob data.
/// @note	On success calls delegate method `[self.pluginOFDelegate triggerDeferredResultEvent:eDeferredResultEventOnUploadedCloudBlob result:YES]`
/// On failure calls delegate method `[self.pluginOFDelegate triggerDownloadCloudBlobResultCallback:NO]`
//////////////////////////////////////////////////////////////////////////////////////////
- (void)downloadBlob:(NSString *)blobKey toPlayerPrefsKeys:(NSArray *)playerPrefsKeys; 

//////////////////////////////////////////////////////////////////////////////////////////
/// Download a previously uploaded data blob from the "cloud".
/// 
/// @param	blobKey		The key of the blob you wish to download from the cloud.
/// @param	filePath	A path to a file in the app's Documents directory that you wish to be populated 
/// with the downloaded blob contents. This is the preferred method if using serialized 
/// XML data (Unity iPhone Advanced only).
/// @note	On success calls delegate method `[self.pluginOFDelegate triggerDownloadCloudBlobResultCallback:YES];`
/// On failure calls delegate method `[self.pluginOFDelegate triggerDownloadCloudBlobResultCallback:NO];`
//////////////////////////////////////////////////////////////////////////////////////////
- (void)downloadBlob:(NSString *)blobKey toFile:(NSString *)filePath;
 

//////////////////////////////////////////////////////////////////////////////////////////
///
/// Present the OpenFeint user approval modal.
/// 
//////////////////////////////////////////////////////////////////////////////////////////
- (void)presentUserFeintApprovalModal; 

@end

