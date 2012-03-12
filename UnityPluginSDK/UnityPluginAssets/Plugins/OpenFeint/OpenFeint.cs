using UnityEngine;
using System.Collections;
using System.Text;
using System;

/// <summary>
/// OpenFeint bridge. The methods can be called directly from your scripts.
/// Example: OpenFeint.LaunchDashboard(OpenFeint.eDashboardPage.Leaderboards);
/// </summary>
public class OpenFeint : Gree.Unity.UnityToNative
{	
	
	public enum eDashboardPage
	{
		Main,
		Leaderboards,
		Achievements,
		Challenges,
		FindFriends,
		WhosPlaying,
		GlobalChatRooms,
		GameChatRooms,
		SwitchUser
	}
	
	/// <summary>
	/// Launches the OpenFeint Forums.
	/// </summary>
	static public void GoToForum()
	{
		#if UNITY_ANDROID
			throwAndroidUnsupported("OpenFeint.cs", "GoToForum");
		#endif
		
		#if UNITY_IPHONE
			throwIOSUnsupported("OpenFeint.cs", "GoToForum");
		#endif
	}

	/// <summary>
	/// Inits OpenFeint.
	/// </summary>
	/// <param name='appName'>
	/// App name.
	/// </param>
	/// <param name='key'>
	/// Key.
	/// </param>
	/// <param name='secret'>
	/// Secret.
	/// </param>
	/// <param name='id'>
	/// Identifier.
	/// </param>
	/// <param name='orientation'>
	/// Orientation.
	/// </param>
	public static void Init(String appName,String key,String secret,String id, int orientation)
	{	
				ArrayList args = new ArrayList{
					appName,key,secret,id,orientation
				};
				SendMessage("initOpenFeint", args);
						
	}
	
	/// <summary>
	/// Launches the OpenFeint Dashboard.
	/// </summary>
	/// <param name='dashboardPage'>
	/// The page to open the dashboard to.
	/// </param>
	public static void LaunchDashboard(eDashboardPage dashboardPage)
	{
		ArrayList args = new ArrayList{
			System.Convert.ToString((int)dashboardPage)
		};
		SendMessage("launchDashboard", args);
	}

	/// <summary>
	/// Launches the OpenFeint Dashboard at the Main dashboard page.
	/// </summary>
	static public void LaunchDashboard()
	{
		LaunchDashboard(eDashboardPage.Main);
	}

	/// <summary>
	/// Launches the dashboard with leaderboard I.
	/// </summary>
	/// <param name='leaderboardId'>
	/// Leaderboard string identifier.
	/// </param>	
	static public void LaunchDashboardWithLeaderboardID(string leaderboardId)
	{
		ArrayList args = new ArrayList{
			leaderboardId
		};
		SendMessage("launchDashboardWithLeaderboardId", args);
	}

	/// <summary>
	/// Launches the dashboard with leaderboard I.
	/// </summary>
	/// <param name='leaderboardId'>
	/// Leaderboard long identifier.
	/// </param>
	static public void LaunchDashboardWithLeaderboardID(long leaderboardId)
	{
		LaunchDashboardWithLeaderboardID(leaderboardId.ToString());
	}
	
	/// <summary>
	/// Submit a high score to a specific leaderboard.
	/// </summary>
	/// <param name='score'>
	/// The integer score.
	/// </param>
	/// <param name='displayText'>
	/// The text to display for this score instead of the integer score value.
	/// </param>
	/// <param name='leaderboardId'>
	/// String containing the leaderboard ID where the score is to be submitted.
	/// </param>
	/// <param name='silently'>
	/// Whether or not to display a notification upon successful high score submission.
	/// </param>
	static public void SubmitHighScore(long score, string displayText, string leaderboardId, bool silently)
	{
		#if UNITY_ANDROID
			throwAndroidUnsupported("OpenFeint.cs", "SubmitHighScore");
		#else
			ArrayList args = new ArrayList{
				score.ToString(),
				displayText,
				leaderboardId,
				silently.ToString()
			};
			SendMessage("submitHighScore", args);
		#endif
	}

	static public void SubmitHighScore(long score, string displayText, long leaderboardId, bool silently)
	{
		SubmitHighScore(score, displayText, leaderboardId.ToString(), silently);
	}

	static public void SubmitHighScore(long score, string displayText, string leaderboardId)
	{
		SubmitHighScore(score, displayText, leaderboardId, false);
	}

	static public void SubmitHighScore(long score, string displayText, long leaderboardId)
	{
		SubmitHighScore(score, displayText, leaderboardId.ToString(), false);
	}

	static public void SubmitHighScore(long score, string leaderboardId)
	{
		SubmitHighScore(score, score.ToString(), leaderboardId, false);
	}

	static public void SubmitHighScore(long score, long leaderboardId)
	{
		SubmitHighScore(score, score.ToString(), leaderboardId.ToString(), false);
	}
	
	/// <summary>
	/// It downloads the high score blob for given score key.
	/// </summary>
	/// <param name='leaderboardId'>
	/// Leaderboard identifier.
	/// </param>
	/// <param name='scoreKey'>
	/// Score built key. It is : string(score.leaderboardId) + '-' + string(score.rank)
	/// </param>
	static public void DownloadHighScorePayload(string leaderboardId, string scoreKey) {

		ArrayList args = new ArrayList {
			leaderboardId,
			scoreKey
		};

		SendMessage("downloadHighScorePayload", args);

	}	
		
	/// <summary>
	/// Sends a social notification to Twitter and Facebook.
	/// </summary>
	/// <param name='text'>
	/// Text to send.
	/// </param>
	/// <param name='imageIdentifierName'>
	/// The identifier name of the image to send with the text.
	/// </param>
	static public void SendSocialNotification(string text, string imageIdentifierName)
	{
		#if UNITY_ANDROID
			throwAndroidUnsupported("OpenFeint.cs", "SendSocialNotification");
		#else
			ArrayList args = new ArrayList{
				text,
				imageIdentifierName,
			};
			SendMessage("sendSocialNotification", args);
		#endif
	}

	static public void SendSocialNotification(string text)
	{
		SendSocialNotification(text, "");
	}
	
	/// <summary>
	/// Uses FBConnect to send a text.
	/// </summary>
	/// <param name='textStr'>
	/// Displayed Text.
	/// </param>
	/// <param name='message'>
	/// Original message.
	/// </param>
	/// <param name='imageName'>
	/// Image name for displaying.
	/// </param>
	/// <param name='customUrl'>
	/// Custom url for social api.
	/// </param>
	static public void SendWithPrepopulatedText(string textStr, string message, string imageName, string customUrl)
	{
		#if UNITY_ANDROID
			throwAndroidUnsupported("OpenFeint.cs", "SendWithPrepopulatedText");
		#else
			ArrayList args = new ArrayList{
				textStr,
				message,
				imageName,
				customUrl
			};
			
			SendMessage("sendWithPrepopulatedText", args);
		#endif
	}
	
	/// </summary>
	/// <param name='message'>
	/// The text for content of the notification
	/// </param>
	/// <param name='imageName'>
	/// The image name for notification, "" for no image/
	/// </param>
	static public void InGameNotification(string message, string imageName)
	{
		ArrayList args = new ArrayList{
			message,
			imageName
		};
		
		SendMessage("inGameNotification", args);
	}
	
	static public void InGameNotification(string message)
	{
		InGameNotification(message, "");
	}

	/// <summary>
	/// Get a high score of a specific leaderboard.
	/// </summary>
	/// <param name='leaderboardId'>
	/// String containing the leaderboard ID where the score is to be retrieved.
	/// </param>
	static public void GetLeaderboardHighScore(string leaderboardId) {
		ArrayList args = new ArrayList{
			leaderboardId
		};
		
		SendMessage("getLeaderboardHighScore", args);
	}

	static public void GetLeaderboardHighScore(int leaderboardId) {
		GetLeaderboardHighScore(leaderboardId.ToString());
	}

	/// <summary>
	/// Get element for special achivement.
	/// </summary>
	/// <param name='achievementId'>
	/// String containing the achivement ID.
	/// </param>
	/// <param name='field'>
	/// Field.
	/// </param>
	static public void GetAchievementElementByIdAndField(string achievementId, string field) {
		ArrayList args = new ArrayList{
			achievementId,
			field
		};
		SendMessage("getAchievementByElementIdAndField",args);
	}

	static public void GetAchievementElementByIdAndField(int achievementId, string field) {
		GetAchievementElementByIdAndField(achievementId.ToString(), field);
	}

	/// <summary>
	/// Unlocks the achievement.
	/// </summary>
	/// <param name='achievementId'>
	/// Achievement identifier.
	/// </param>
	static public void UnlockAchievement(string achievementId)
	{
		ArrayList args = new ArrayList{
			achievementId
		};
		SendMessage("unlockAchievement",args);
	}

	static public void UnlockAchievement(long achievementId)
	{
		UnlockAchievement(achievementId.ToString());
	}
	
	/// <summary>
	/// Unlocks the achievement deferred.
	/// </summary>
	/// <remarks>
	/// Events : OnAchievementUnlocked
	/// </remarks>
	/// <param name='achievementId'>
	/// Achievement identifier.
	/// </param>
	static public void UnlockAchievementDeferred(string achievementId)
	{
		#if UNITY_ANDROID
			throwAndroidUnsupported("OpenFeint.cs", "UnlockAchievementDeferred");
		#else
			ArrayList args = new ArrayList{
				achievementId
			};
			SendMessage("unlockAchievementDeferred",args);
		#endif
	}

	static public void UnlockAchievementDeferred(long achievementId)
	{
		UnlockAchievementDeferred(achievementId.ToString());
	}
	
	/// <summary>
	/// Updates an achievement.
	/// </summary>
	/// <remarks>
	/// Events : OnAchievementUnlocked
	/// </remarks>
	/// <param name='achievementId'>
	/// Achievement identifier.
	/// </param>
	/// <param name='percentageComplete'>
	/// Percentage of the complete.
	/// </param>
	/// <param name='showNotification'>
	/// Whether to show notification.
	/// </param>
	static public void UpdateAchievement(string achievementId, double percentageComplete, bool showNotification)
	{
		#if UNITY_ANDROID
			throwAndroidUnsupported("OpenFeint.cs", "UpdateAchievement");
		#else
			ArrayList args = new ArrayList{
				achievementId,
				percentageComplete,
				showNotification
			};
			SendMessage("updateAchievement",args);
		#endif
	}

	static public void UpdateAchievement(long achievementId, double percentageComplete, bool showNotification)
	{
		UpdateAchievement(achievementId.ToString(),percentageComplete,showNotification);
	}

	/// <summary>
	/// Updates the achievement deferred.
	/// </summary>
	/// <remarks>
	/// Events : OnAchievementUnlocked
	/// </remarks>
	/// <param name='achievementId'>
	/// Achievement identifier.
	/// </param>
	/// <param name='percentageComplete'>
	/// Percentage of the complete.
	/// </param>
	/// <param name='showNotification'>
	/// Whether to show notification.
	/// </param>
	static public void UpdateAchievementDeferred(string achievementId, double percentageComplete, bool showNotification)
	{
		#if UNITY_ANDROID
			throwAndroidUnsupported("OpenFeint.cs", "UpdateAchievementDeferred");
		#else
			ArrayList args = new ArrayList{
				achievementId,
				percentageComplete,
				showNotification
			};
			SendMessage("updateAchievementDeferred", args);
		#endif
	}

	static public void UpdateAchievementDeferred(long achievementId, double percentageComplete, bool showNotification)
	{
		UpdateAchievementDeferred(achievementId.ToString(), percentageComplete, showNotification);
	}

	static public void SubmitDeferredAchievements()
	{
		#if UNITY_ANDROID
			throwAndroidUnsupported("OpenFeint.cs", "SubmitDeferredAchievements");
		#else
			SendMessage("submitDeferredAchievements", new ArrayList());
		#endif
	}
		
	/// <summary>
	/// Gets the achievement icon.
	/// </summary>
	/// <remarks>
	/// Events : OnAchievementUnlocked
	/// </remarks>
	/// <param name='achievementId'>
	/// Achievement identifier.
	/// </param>
	static public void GetAchievementIcon(string achievementId)
	{
		ArrayList args = new ArrayList{
			achievementId
		};
		SendMessage("getAchievementIcon",args);
	}

	static public void GetAchievementIcon(long achievementId)
	{
		GetAchievementIcon(achievementId.ToString());
	}	


	static public void SetAchievementSocialUrl(string url) 
	{
		#if UNITY_ANDROID
			throwAndroidUnsupported("OpenFeint.cs", "SetAchievementSocialUrl");
		#else
			ArrayList args = new ArrayList{
				url
			};
			SendMessage("setAchievementSocialUrl",args);
		#endif
	}

	/// <summary>
	/// Retrieve a list of achievement ids.
	/// </summary>
	static public void AchievementList()
	{
		SendMessage("achievementList", new ArrayList());
	}

	/// <summary>
	/// Retrieve a list of achievement properties.
	/// </summary>
	/// <param name='achievementId'>
	/// Achievement identifier.
	/// </param>
	static public void AchievementProperties(string achievementId)
	{
		ArrayList args = new ArrayList{
			achievementId
		};
		SendMessage("achievementProperties", args);
	}

	static public void AchievementProperties(long achievementId)
	{
		AchievementProperties(achievementId.ToString());
	}

	/// <summary>
	/// Get the challenge data that was sent with a challenge.
	/// </summary>
	/// <param name='playerPrefsKeys'>
	/// An array of PlayerPrefs keys where the challenge data for this challenge will be stored to.
	/// </param>
	static public void GetChallengeDataToPlayerPrefs(string[] playerPrefsKeys)
	{
		#if UNITY_ANDROID
			throwAndroidUnsupported("OpenFeint.cs", "GetChallengeDataToPlayerPrefs");
		#else
			ArrayList args = new ArrayList {
				playerPrefsKeys
			};
			SendMessage("getChallengeDataToPlayerPrefs", args);
		#endif
	}
		  
	/// <summary>
	/// Get the challenge data that was sent with a challenge.
	/// </summary>
	/// <param name='filePath'>
	/// he path to a file in the app's Documents directory that will hold the challenge data.
	/// </param>
	static public void GetChallengeDataToFile(string filePath)
	{
		#if UNITY_ANDROID
			throwAndroidUnsupported("OpenFeint.cs", "GetChallengeDataToFile");
		#else
			ArrayList args = new ArrayList {
				filePath
			};
			SendMessage("getChallengeDataToFile", args);
		#endif
	}

	/// <summary>
	/// Displays the modal for sending a challenge to another player.
	/// </summary>
	/// <param name='challengeId'>
	/// The ID of the challenge.
	/// </param>
	/// <param name='challengeText'>
	/// Displays what needs to be fulfilled to complete the challenge.
	/// </param>
	/// <param name='dataKeys'>
	/// An array of PlayerPrefs keys where the data for this challenge is stored. This will be packaged 
	/// up an sent as the challenge data from this player.
	/// </param>
	static public void DisplaySendChallengeModalWithPlayerPrefs(string challengeId, string challengeText, string[] dataKeys)
	{
		#if UNITY_ANDROID
			throwAndroidUnsupported("OpenFeint.cs", "DisplaySendChallengeModalWithPlayerPrefs");
		#else
			ArrayList args = new ArrayList {
				challengeId,
				challengeText,
				dataKeys
			};
			SendMessage("displaySendChallengeModalWithPlayerPrefs", args);
		#endif
	}

	static public void DisplaySendChallengeModalWithPlayerPrefs(long challengeId, string challengeText, string[] dataKeys)
	{
		DisplaySendChallengeModalWithPlayerPrefs(challengeId.ToString(), challengeText, dataKeys);
	}

	/// <summary>
	/// Displays the modal for sending a challenge to another player.
	/// </summary>
	/// <param name='challengeId'>
	/// he ID of the challenge.
	/// </param>
	/// <param name='challengeText'>
	/// CDisplays what needs to be fulfilled to complete the challenge.
	/// </param>
	/// <param name='dataFilePath'>
	/// The path to a file located in the app's Documents directory that contains the challenge data. 
	/// This is the preferred method for serialized XML data (Unity iPhone Advanced only).
	/// </param>
	static public void DisplaySendChallengeModalWithFile(string challengeId, string challengeText, string dataFilePath)
	{
		#if UNITY_ANDROID
			throwAndroidUnsupported("OpenFeint.cs", "DisplaySendChallengeModalWithFile");
		#else
			ArrayList args = new ArrayList {
				challengeId,
				challengeText,
				dataFilePath
			};
			
			SendMessage("displaySendChallengeModalWithFile", args);
		#endif
	}

	static public void DisplaySendChallengeModalWithFile(long challengeId, string challengeText, string dataFilePath)
	{
		DisplaySendChallengeModalWithFile(challengeId.ToString(), challengeText, dataFilePath);
	}

	/// <summary>
	/// Submits the result of a played challenge to the server. This should always be called
	/// after the challenge game play session is over, regardless of the result or the type
	/// of challenge. It should be called before DisplayChallengeCompletedModal.
	/// </summary>
	/// <remarks>
	/// Events : OnChallengeResultSubmitted
	/// </remarks>
	/// <param name='result'>
	/// The result of the challenge.
	/// </param>
	/// <param name='resultDescription'>
	/// A string that will be prefixed by either the recipient's name or "You", depending on 
	/// the player that is viewing the result. It should not contain whether the recipient won 
	/// or lost, but should contain the statistics of his attempt. (Ex. "beat 30 monsters" will 
	/// turn into "You beat 30 monsters")
	/// </param>
	static public void SubmitChallengeResult(Gree.Unity.CallbackFetcherOpenFeint.eChallengeResult result, string resultDescription)
	{
		ArrayList args = new ArrayList {
			result,
			resultDescription
		};
		
		SendMessage("submitChallengeResult", args);
	}

	/// <summary>
	/// Displays the modal for showing the result of the challenge and allows for the user
	/// to re-challenge (if it was a multi-attempt challenge). Call SubmitChallengeResult BEFORE
	/// calling this.
	/// </summary>
	/// <param name='result'>
	/// The result of the challenge.
	/// </param>
	/// <param name='resultDescription'>
	/// A string that will be prefixed by either the recipient's name or "You", depending on the player 
	/// that is viewing the result. It should not contain whether the recipient won or lost, but 
	/// should contain the statistics of his attempt. (Ex. "beat 30 monsters" will turn into 
	/// "You beat 30 monsters")
	/// </param>
	/// <param name='reChallengeDescription'>
	/// If the user decides to send out the result as a re-challenge, this will be the description 
	/// for the new challenge. It should display what needs to be fulfilled to complete the challenge. 
	/// Only used by multi-attempt challenges.
	/// </param>
	/// <param name='reChallengeDataKeys'>
	/// Tells the native code how/where to access the challenge data if the user decides to send out a 
	/// "re-challenge". Only used for multi-attempt challenges.
	/// </param>
	static public void DisplayChallengeCompletedModalWithPlayerPrefs(Gree.Unity.CallbackFetcherOpenFeint.eChallengeResult result,
		string resultDescription, string reChallengeDescription, string[] reChallengeDataKeys)
	{
		ArrayList args = new ArrayList {
			result,
			resultDescription,
			reChallengeDescription,
			reChallengeDataKeys
		};

		SendMessage("displayChallengeCompletedModalWithPlayerPrefs", args);
	}

	/// <summary>
	/// Displays the modal for showing the result of the challenge and allows for the user
	/// to re-challenge (if it was a multi-attempt challenge). Call SubmitChallengeResult BEFORE
	/// calling this.
	/// </summary>
	/// <param name='result'>
	/// The result of the challenge.
	/// </param>
	/// <param name='resultDescription'>
	/// RA string that will be prefixed by either the recipient's name or "You", depending on the 
	/// player that is viewing the result. It should not contain whether the recipient won or lost, 
	/// but should contain the statistics of his attempt. (Ex. "beat 30 monsters" will turn into 
	/// "You beat 30 monsters")
	/// </param>
	/// <param name='reChallengeDescription'>
	/// If the user decides to send out the result as a re-challenge, this will be the description 
	/// for the new challenge. It should display what needs to be fulfilled to complete the challenge. 
	/// Only used by multi-attempt challenges.
	/// </param>
	/// <param name='reChallengeDataFilePath'>
	/// Tells the native code how/where to access the challenge data if the user decides to send 
	/// out a "re-challenge". Only used for multi-attempt challenges.
	/// </param>
	static public void DisplayChallengeCompletedModalWithFile(Gree.Unity.CallbackFetcherOpenFeint.eChallengeResult result, string resultDescription,
		string reChallengeDescription, string reChallengeDataFilePath)
	{
		ArrayList args = new ArrayList {
			result,
			resultDescription,
			reChallengeDescription,
			reChallengeDataFilePath
		};
		
		SendMessage("displayChallengeCompletedModalWithFile", args);
	}

	/// <summary>
	/// Get a challenge defintion structure for a particular challenge ID.
	/// </summary>
	/// <param name='challengeId'>
	/// The ID of the challenge.
	/// </param>
	/// <param name='callback'>
	/// Called upon success or failure of the get. CANNOT be 'null'.
	/// </param>
	static public void GetChallengeDefinition(string challengeId, Gree.Unity.CallbackFetcherOpenFeint.ChallengeDefinitionResultDelegate callback)
	{
		#if UNITY_ANDROID
		throwAndroidUnsupported("OpenFeint.cs", "GetChallengeDefinition");
		#else
	
			ArrayList args = new ArrayList {
				challengeId
			};
			
			if(callback != null)
			{
				if(Gree.Unity.CallbackFetcherOpenFeint.challengeDefinitionResultCallback == null)
				{
					Gree.Unity.CallbackFetcherOpenFeint.challengeDefinitionResultCallback = callback;
					SendMessage("getChallengeDefinition", args);
				}
				else
				{
					Debug.LogError("OpenFeint error: Cannot call GetChallengeDefinition until previous call has returned success or failure.");
				}
			}
			else
			{
				Debug.LogError("OpenFeint error: No callback supplied to GetChallengeDefinition.");
			}
		#endif
	}

	static public void GetChallengeDefinition(long challengeId, Gree.Unity.CallbackFetcherOpenFeint.ChallengeDefinitionResultDelegate callback)
	{
		GetChallengeDefinition(challengeId.ToString(), callback);
	}
	
	/// <summary>
	/// Upload a data blob to the "cloud", which can be later retrieved on other devices.
	/// </summary>
	/// <remarks>
	/// Events : OnUploadedCloudBlob
	/// </remarks>
	/// <param name='blobKey'>
	/// The key of the blob you wish to upload to the cloud.
	/// </param>
	/// <param name='playerPrefsKeys'>
	/// n array of PlayerPrefs keys you wish to send to the cloud.
	/// </param>
	static public void UploadBlobFromPlayerPrefs(string blobKey, string[] playerPrefsKeys)
	{
		#if UNITY_ANDROID
			throwAndroidUnsupported("OpenFeint.cs", "UploadBlobFromPlayerPrefs");
		#else
			ArrayList args = new ArrayList {
				blobKey,
				playerPrefsKeys
			};
			
			SendMessage("uploadBlobFromPlayerPrefs", args);
		#endif
	}

	/// <summary>
	/// Upload a data blob to the "cloud", which can be later retrieved on other devices.
	/// </summary>
	/// <remarks>
	/// Events : OnUploadedCloudBlob
	/// </remarks>
	/// <param name='blobKey'>
	/// The key of the blob you wish to upload to the cloud.
	/// </param>
	/// <param name='filePath'>
	/// A path to a file in the app's Documents directory that you wish to upload to the cloud. 
	/// This is the preferred method if using serialized XML data (Unity iPhone Advanced only).
	/// </param>
	static public void UploadBlobFromFile(string blobKey, string filePath)
	{
		ArrayList args = new ArrayList {
			blobKey,
			filePath
		};
		
		SendMessage("uploadBlobFromFile", args);
	}

	/// <summary>
	/// Download a previously uploaded data blob from the "cloud".
	/// </summary>
	/// <remarks>
	/// Events : OnUploadedCloudBlob
	/// </remarks>
	/// <param name='blobKey'>
	/// The key of the blob you wish to download from the cloud.
	/// </param>
	/// <param name='playerPrefsKeys'>
	/// An array of PlayerPrefs keys that will be populated with the downloaded blob data.
	/// </param>
	/// <param name='callback'>
	/// Called whenever the action returns success or failure. CANNOT be 'null'.
	/// </param>
	static public void DownloadBlobToPlayerPrefs(string blobKey, string[] playerPrefsKeys, Gree.Unity.CallbackFetcherOpenFeint.DeferredResultDelegate callback)
	{
		#if UNITY_ANDROID
		throwAndroidUnsupported("OpenFeint.cs", "DownloadBlobToPlayerPrefs");
		#else
			ArrayList args = new ArrayList {
				blobKey,
				playerPrefsKeys
			};
			if(callback != null)
			{
				if(Gree.Unity.CallbackFetcherOpenFeint.downloadCloudBlobResultCallback == null)
				{
					Gree.Unity.CallbackFetcherOpenFeint.downloadCloudBlobResultCallback = callback;
					SendMessage("downloadBlobToPlayerPrefs", args);
				}
				else
				{
					Debug.LogError("OpenFeint error: Cannot call DownloadBlobToPlayerPrefsKeys until previous blob download has returned success or failure.");
				}
			}
			else
			{
				Debug.LogError("OpenFeint error: No callback supplied to DownloadBlobToPlayerPrefsKeys.");
			}
		#endif
	}

	/// <summary>
	/// Download a previously uploaded data blob from the "cloud".
	/// </summary>
	/// <param name='blobKey'>
	/// The key of the blob you wish to download from the cloud.
	/// </param>
	/// <param name='filePath'>
	/// A path to a file in the app's Documents directory that you wish to be populated 
	/// with the downloaded blob contents. This is the preferred method if using serialized 
	/// XML data (Unity iPhone Advanced only).
	/// </param>
	/// <param name='callback'>
	/// Called whenever the action returns success or failure. CANNOT be 'null'.
	/// </param>
	static public void DownloadBlobToFilePath(string blobKey, string filePath, Gree.Unity.CallbackFetcherOpenFeint.DeferredResultDelegate callback)
	{
		ArrayList args = new ArrayList {
			blobKey,
			filePath
		};

		if(callback != null)
		{
			if(Gree.Unity.CallbackFetcherOpenFeint.downloadCloudBlobResultCallback == null)
			{
				Gree.Unity.CallbackFetcherOpenFeint.downloadCloudBlobResultCallback = callback;

				SendMessage("downloadBlobToFile", args);
			}
			else
			{
				Debug.LogError("OpenFeint error: Cannot call DownloadBlobToFilePath until previous blob download has returned success or failure.");
			}
		}
		else
		{
			Debug.LogError("OpenFeint error: No callback supplied to DownloadBlobToFilePath.");
		}
	}

	/// <summary>
	/// Sets the dashboard OpenFeint orientation.
	/// </summary>
	/// <param name='orientation'>
	/// Orientation to set to.
	/// </param>
	static public void SetDashboardOrientation(DeviceOrientation orientation)
	{
		ArrayList args = new ArrayList {
			(int)orientation
		};
		
		SendMessage("setDashboardOrientation", args);
	}

	/// <summary>
	/// Present the OpenFeint user approval modal.
	/// </summary>
	/// <remarks>
	/// Events : OnFeintApprovalScreenClosed
	/// </remarks>
	static public void PresentUserFeintApprovalModal()
	{
		#if UNITY_ANDROID
			throwAndroidUnsupported("OpenFeint.cs", "PresentUserFeintApprovalModal");
		#else
			SendMessage("presentUserFeintApprovalModal", new ArrayList());
		#endif
	}

	/// <summary>
	/// Determine if the local user if following anyone.
	/// </summary>
	/// <param name='callback'>
	/// Called upon receiving results from the call. Second parameter is set to whether or not the 
	/// user is following anyone. CANNOT be 'null'.
	/// </param>
	static public void IsLocalUserFollowingAnyone(Gree.Unity.CallbackFetcherOpenFeint.FollowingResultDelegate callback)
	{
		if(callback != null)
		{
			if(Gree.Unity.CallbackFetcherOpenFeint.followingResultCallback == null)
			{
				Gree.Unity.CallbackFetcherOpenFeint.followingResultCallback = callback;
				SendMessage("isLocalUserFollowingAnyone", new ArrayList());
			}
			else
			{
				Debug.LogError("OpenFeint error: Cannot call IsLocalUserFollowingAnyone until previous call has returned success or failure.");
			}
		}
		else
		{
			Debug.LogError("OpenFeint error: No callback supplied to IsLocalUserFollowingAnyone.");
		}
	}

	static public void ClearCallbacks()
	{
		Gree.Unity.CallbackFetcherOpenFeint.followingResultCallback = null;
		Gree.Unity.CallbackFetcherOpenFeint.challengeDefinitionResultCallback = null;
		Gree.Unity.CallbackFetcherOpenFeint.downloadCloudBlobResultCallback = null;
	}


	// PROPERTIES //
	// These properties allow you to get (and sometimes set) current state
	// information from OpenFeint.
	//
	// Example:
	//		if(OpenFeint.isOnline)
	//		{
	//			Debug.Log("User is online!");
	//		}
	//

	/// <summary>
	/// Check whether or not the local user has approved use of OpenFeint.
	/// </summary>
	/// <value>
	/// <c>true</c> if user has approved feint; otherwise, <c>false</c>.
	/// </value>
	static public bool hasUserApprovedFeint
	{
		get { return (bool)SendMessage("hasUserApprovedFeint", new ArrayList()); }
	}
	
	/// <summary>
	/// If you have a custom intro flow, call this method if the user approves Feint.
	/// </summary>
	/// <param name='wantIntroFlow'>
	/// If we default intro flow should be displayed.
	/// </param>/
	static public void userApprovedFeint(bool wantIntroFlow) {
		ArrayList args = new ArrayList{
			wantIntroFlow
		};
		SendMessage("userApprovedFeint", args);
	}
	
	/// <summary>
	/// If you have a custom intro flow, call this method if the user declines Feint.
	/// </summary>
	static public void userDeclinedFeint() {
		SendMessage("userDeclinedFeint", new ArrayList());
	}
	

	/// <summary>
	/// Check whether or not the local user is online.
	/// </summary>
	/// <value>
	/// <c>true</c> if is online; otherwise, <c>false</c>.
	/// </value>
	static public bool isOnline
	{
		get { return (bool)SendMessage("isOnline", new ArrayList()); }
	}

	/// <summary>
	/// Get the user name of the local user.
	/// </summary>
	/// <value>
	/// The name of the local user.
	/// </value>
	static public string localUserName
	{
		get { return (string)SendMessage("localUserName", new ArrayList()); }
	}

	/// <summary>
	/// Get the user ID of the local user.
	/// </summary>
	/// <value>
	/// The local user ID.
	/// </value>
	static public string localUserID
	{
		get { return (string)SendMessage("localUserID", new ArrayList()); }
	}
	
	/// <summary>
	/// GSet/get whether or not we are allowing OpenFeint notifications.
	/// </summary>
	/// <value>
	/// <c>true</c> if allow notifications; otherwise, <c>false</c>.
	/// </value>
	static public bool allowNotifications
	{		
		get	{ 
			#if UNITY_ANDROID
				throwAndroidUnsupported("OpenFeint.cs", "allowNotifications");
				return false;
			#else
				return (bool)SendMessage("userAllowsNotifications", new ArrayList()); 
			#endif
		}
		set	{ 
			#if UNITY_ANDROID
				throwAndroidUnsupported("OpenFeint.cs", "allowNotifications");
			#else
				ArrayList args = new ArrayList {
					value
				};
				
				SendMessage("setUserAllowsNotifications", args);				
			#endif
		}
	}

	/// <summary>
	/// The number of unviewed challenges when the user logged in.
	/// </summary>
	/// <value>
	/// The number of unviewed challenges at login.
	/// </value>
	static public int numberOfUnviewedChallengesAtLogin
	{
		get	{
			#if UNITY_ANDROID
				throwAndroidUnsupported("OpenFeint.cs", "numberOfUnviewedChallengesAtLogin");
				return -1;
			#else
				return System.Convert.ToInt32(SendMessage("getNumberOfUnviewedChallenges", new ArrayList())); 
			#endif
		}
	}

	/// <summary>
	/// Gets the release version string.
	/// </summary>
	/// <value>
	/// The release version string.
	/// </value>
	static public string releaseVersionString
	{
		get		{ return (string)SendMessage("openFeintReleaseVersionString", new ArrayList()); }
	}

	/// <summary>
	/// Gets the list of leaderboard high scores.
	/// </summary>
	/// <returns>
	/// The list of leaderboard high scores.
	/// </returns>
	/// <param name='leaderboardId'>
	/// Leaderboard identifier as string.
	/// </param>/
	static public void GetLeaderboardHighScores(string leaderboardId) {
		#if UNITY_IPHONE
			throwIOSUnsupported("OpenFeint.cs", "getLeaderboardHighScores");
		#else
			ArrayList args = new ArrayList{
				leaderboardId
			};
			
			SendMessage("getLeaderboardHighScores", args);
		#endif
	}

	/// <summary>
	/// Gets the list of leaderboard high scores.
	/// </summary>
	/// <param name='leaderboardId'>
	/// Leaderboard identifier as integer.
	/// </param>/
	static public void GetLeaderboardHighScores(int leaderboardId) {
		GetLeaderboardHighScores(leaderboardId.ToString());
	}
}
