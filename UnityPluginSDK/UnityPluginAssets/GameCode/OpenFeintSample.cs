using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Gree.Unity;

public class OpenFeintSample : BaseUI 
{
	public GUISkin guiSkin;  

	public enum eOFTopic
	{
		User,
		Achievements,
		Highscore,
		Challenges,
		Utility
	}
	
	private string versionString;
  	
	/// <summary>
	/// Scroll position.
	/// </summary>
	private Vector2 scrollPos = new Vector2();	
	
	/// <summary>
	/// It contains current values for all input fields present in this screen.
	/// </summary>
	private Dictionary<string, object[,]> inputValuesDictionary = new Dictionary<string, object[,]>();

	// Default leaderboard id
	private static string DEFAULT_LEADERBOARD_ID = (string)Properties.ConfigurationProperties["MY_APP_DEFAULT_LEADERBOARD_ID"]; // "978776";
	private static string DEFAULT_ACHIEVEMENT_ID = (string)Properties.ConfigurationProperties["MY_APP_DEFAULT_ACHIEVEMENT_ID"]; // "1206612";
	private static string DEFAULT_PERCENTAGE_ACHIEVEMENT_ID = (string)Properties.ConfigurationProperties["MY_APP_DEFAULT_PERCENTAGE_ACHIEVEMENT_ID"]; // "1235182";
	private static string DEFAULT_SCORE_ID = (string)Properties.ConfigurationProperties["MY_APP_DEFAULT_SCORE_ID"]; //"978776-1";
	private static string DEFAULT_CHALLENGE_ID = (string)Properties.ConfigurationProperties["MY_APP_DEFAULT_CHALLENGE_ID"]; //"29223";
	
	
	private eOFTopic viewTopic = eOFTopic.User;
		
	// User
	// User online
	protected static IExecutor onlineExec = new OnlineStatusExecutor();
	protected static IExecutor localUsernameExec = new LocalUserNameExecutor();
	protected static IExecutor localUserIdExec = new LocalUserIdExecutor();
	protected static IExecutor approvedOFExec = new ApprovedOFExecutor();
	protected static IInputExecutor unlockAchvExec = new UnlockAchievementExecutor();
	protected static IInputExecutor updateAchvExec = new UpdateAchievementExecutor();
	protected static IInputExecutor getAchvElemExec = new GetAchievementElementExecutor();
	protected static IExecutor achvListExec = new AchievementListExecutor();
	protected static IInputExecutor achvPropertiesExec = new AchievementPropertiesExecutor();
	protected static IInputExecutor getAchvIconExec = new GetAchievementIconExecutor();
	protected static IInputExecutor submitHighscoreExec = new SubmitHighscoreExecutor();
	protected static IInputExecutor getHighscoreExec = new GetLeaderboardHighscoreExecutor();
	protected static IExecutor releaseVersionExec = new ReleaseVersionExecutor();
	protected static IInputExecutor setDashboardOrientationExec = new SetDashboardOrientationExecutor();
	protected static IExecutor launchDashboardExec = new LaunchDashboardExecutor();
	protected static IInputExecutor inGameNotifExec = new InGameNotificationExecutor();
	protected static IInputExecutor uploadBlobFromFileExec = new UploadBlobFromFileExecutor();
	protected static IInputExecutor downloadBlobToPathExec = new DownloadBlobToPathExecutor();
	protected static IInputExecutor downloadHighScorePayloadExec = new DownloadHighScorePayloadExecutor();

	protected static IInputExecutor writeChallengeExec = new WriteChallengeExecutor();
	protected static IInputExecutor displayChallengeExec = new DisplayChallengeExecutor();

	
	private Rect menuButtonRect0 = menuButtonRectForButton(0);
	private Rect menuButtonRect1 = menuButtonRectForButton(1);
	private Rect menuButtonRect2 = menuButtonRectForButton(2);
	private Rect menuButtonRect3 = menuButtonRectForButton(3);	
	private Rect menuButtonRect4 = menuButtonRectForButton(4);	
		
    //At this script initialization  
    public void initializeInterface()  
    {  		 
		InitializeWithConfiguration(true);
		
		Gree.Unity.CallbackFetcherOpenFeint.OnHighScoreSubmitted += new Gree.Unity.CallbackFetcherOpenFeint.DeferredResultEventHandler(HighScoreSubmitted);
		Gree.Unity.CallbackFetcherOpenFeint.OnLeaderboardHighScoreFetched += new Gree.Unity.CallbackFetcherOpenFeint.DeferredLeaderboardHighscore(HighScoreFetched);
		Gree.Unity.CallbackFetcherOpenFeint.OnAchievementElementLoaded += new Gree.Unity.CallbackFetcherOpenFeint.DeferredAchievementElementLoaded(AchievementLoaded);
		Gree.Unity.CallbackFetcherOpenFeint.OnAchievementListFetched += new Gree.Unity.CallbackFetcherOpenFeint.DeferredAchievementListLoaded(AchievementListLoaded);
		Gree.Unity.CallbackFetcherOpenFeint.OnAchievementPropertiesFetched += new Gree.Unity.CallbackFetcherOpenFeint.DeferredAchievementPropertiesLoaded(AchievementPropertiesLoaded);
		Gree.Unity.CallbackFetcherOpenFeint.OnUploadedCloudBlob += new Gree.Unity.CallbackFetcherOpenFeint.DeferredResultEventHandler(BlobUploaded);
		Gree.Unity.CallbackFetcherOpenFeint.OnAchievementIconLoaded += new Gree.Unity.CallbackFetcherOpenFeint.IconLoadedEventHandler(AchievementIconDownloaded);
		Gree.Unity.CallbackFetcherOpenFeint.OnAchievementIconLoadFailed += new Gree.Unity.CallbackFetcherOpenFeint.IconLoadFailedEventHandler(AchievementIconFailed);
		
		Gree.Unity.CallbackFetcherOpenFeint.OnStartedChallenge += new Gree.Unity.CallbackFetcherOpenFeint.StartedChallengeEventHandler(ReceivedNewChallenge);
			
		initializeInputValuesDictionary();
		
    }  
	
	private void initializeInputValuesDictionary() {
		inputValuesDictionary = new Dictionary<string, object[,]>();
		inputValuesDictionary.Add("submitHighscore", new object[,] { {"Highscore :","12500","textfield"}});
		inputValuesDictionary.Add("dashboardWithId", new object[,] { {"Leaderboard Id :",DEFAULT_LEADERBOARD_ID,"textfield"}});
		inputValuesDictionary.Add("inGameNotification", new object[,] { {"Notification to send :","This is a default notification man !","textfield"}});
		inputValuesDictionary.Add("getHighscore", new object[,] { {"Leaderboard Id :",DEFAULT_LEADERBOARD_ID,"textfield"}});
		inputValuesDictionary.Add("unlockAchievement", new object[,] { {"Achievement Id :",DEFAULT_ACHIEVEMENT_ID,"textfield"}});
		inputValuesDictionary.Add("updateAchievement", new object[,] { {"Achievement Id :",DEFAULT_PERCENTAGE_ACHIEVEMENT_ID,"textfield"}});
		inputValuesDictionary.Add("getAchievementElement", new object[,] { {"Achievement Id :",DEFAULT_ACHIEVEMENT_ID,"textfield"}, {"Field :","title","textfield"}});
		inputValuesDictionary.Add("setDashboardOrientation", new object[,] { {"Orientation :","1","textfield"}});
		inputValuesDictionary.Add("achievementProperties", new object[,] { {"Achievement Id :",DEFAULT_ACHIEVEMENT_ID,"textfield"}});
		inputValuesDictionary.Add("uploadBlobFromFile", new object[,] { {"Blob key :","TestBlob","textfield"}, {"File Path :","file:///mnt/sdcard/images/android.jpg","textfield"}});
		inputValuesDictionary.Add("downloadBlobToPath", new object[,] { {"Blob key :","TestBlob","textfield"}, {"File Path :","/mnt/sdcard/images/downloaded.jpg","textfield"}});
		inputValuesDictionary.Add("getAchievementIcon", new object[,] { {"Achievement Id :",DEFAULT_ACHIEVEMENT_ID,"textfield"}});
		inputValuesDictionary.Add("downloadHighscorePayload", new object[,] { {"Leaderboard Id :",DEFAULT_LEADERBOARD_ID,"textfield"}, {"Score Id :",DEFAULT_SCORE_ID,"textfield"}});
		inputValuesDictionary.Add("writeChallengeData", new object[,] { {"Challenge Key :","KeyString","textfield"}, {"Challenge Data :","DataString","textfield"}});
		inputValuesDictionary.Add("displayChallenge", new object[,] { {"Challenge Id :",DEFAULT_CHALLENGE_ID,"textfield"}, {"Challenge Key :","KeyString","textfield"}});
		
	}
	
	Rect menuRect = new Rect(5.0f, backButtonSquareDimension + titleBarHeight + groupHeightSeparator, menuWindowWidth, 350.0f);
	Rect testRect = testWindowViewRect;	
	Rect backButtonRect = new Rect (0, titleBarHeight + 5.0f, backButtonSquareDimension, backButtonSquareDimension);
		
	public bool DrawGUI()
	{
		DrawTitleBar("Unity Bridge Test App", null, "OF ver. " + versionString);
		if (GUI.Button(backButtonRect, "Back")) {
			return true;
		}

		menuRect = GUI.Window(0, menuRect, DrawMenu, "Menu");
	
		testRect = GUI.Window(1, testRect, DrawScrollView, viewTopicTitle);
		return false;
	}
	

	private void DrawMenu(int windowID) {

		if (GUI.Button(menuButtonRect0, "User")) {
			scrollPos = Vector2.zero;
			viewTopic = eOFTopic.User;
		}

		if (GUI.Button(menuButtonRect1, "Achievements")) {
			scrollPos = Vector2.zero;
			viewTopic = eOFTopic.Achievements;
		}

		if (GUI.Button(menuButtonRect2, "Highscore")) {
			scrollPos = Vector2.zero;
			viewTopic = eOFTopic.Highscore;
		}
		
		if (GUI.Button(menuButtonRect3, "Challenges")) {
			scrollPos = Vector2.zero;
			viewTopic = eOFTopic.Challenges;
		}

		if (GUI.Button(menuButtonRect4, "Utility")) {
			scrollPos = Vector2.zero;
			viewTopic = eOFTopic.Utility;
		}
	}

	private void DrawScrollView(int windowID) {
		switch (viewTopic) {
			case eOFTopic.User:
				viewTopicTitle = "User";
				scrollPos = scrollView(scrollPos, 4, scrollViewSize);
				scrollViewSize = DrawUserGUI();
				break;
			case eOFTopic.Achievements:
				viewTopicTitle = "Achievements";
				scrollPos = scrollView(scrollPos, 6, scrollViewSize);
				scrollViewSize = DrawAchievementsGUI();
				break;
			case eOFTopic.Highscore:
				viewTopicTitle = "Highscores";
				scrollPos = scrollView(scrollPos, 3, scrollViewSize);
				scrollViewSize = DrawHighscoreGUI();
				break;
			case eOFTopic.Challenges:
				viewTopicTitle = "Challenges";
				scrollPos = scrollView(scrollPos, 2, scrollViewSize);
				scrollViewSize = DrawChallengesGUI();
				break;
			case eOFTopic.Utility:
				viewTopicTitle = "Utility";
				scrollPos = scrollView(scrollPos, 6, scrollViewSize);
				scrollViewSize = DrawUtilityGUI();
				break;
		}

		// End scroll view
		GUI.EndScrollView();

	}
	
	private float DrawUserGUI() {
		float groupStartY = 0f + groupHeightSeparator;
		
		groupStartY += drawGroup("Online Status", "Check Status", groupStartY, onlineExec);
		groupStartY += drawGroup("Local User Name", "Fetch Name", groupStartY, localUsernameExec);
		groupStartY += drawGroup("Local User Id", "Fetch User Id", groupStartY, localUserIdExec);
		groupStartY += drawGroup("Approved Open Feint ?", "Is Approved ?", groupStartY, approvedOFExec);
		return groupStartY;
	}


	private float DrawAchievementsGUI() {
		float groupStartY = 0f + groupHeightSeparator;
		
		groupStartY += drawGroupWithInput("Unlock Achievement", "Unlock It!", groupStartY, unlockAchvExec, inputValuesDictionary["unlockAchievement"]);
		groupStartY += drawGroupWithInput("Update Achievement", "Update It!", groupStartY, updateAchvExec, inputValuesDictionary["updateAchievement"]);
		groupStartY += drawGroupWithInput("Achievement Element", "Get Element", groupStartY, getAchvElemExec, inputValuesDictionary["getAchievementElement"]);
		groupStartY += drawGroup("Achievements", "Get List", groupStartY, achvListExec);
		groupStartY += drawGroupWithInput("Achievement Properties", "Get List", groupStartY, achvPropertiesExec, inputValuesDictionary["achievementProperties"]);
		groupStartY += drawGroupWithInput("Get Achievement Icon", "Get Icon", groupStartY, getAchvIconExec, inputValuesDictionary["getAchievementIcon"]);
		return groupStartY;
	}

	private float DrawHighscoreGUI() {
		float groupStartY = 0f + groupHeightSeparator;
		
		groupStartY += drawGroupWithInput("Submit high score", "Submit", groupStartY, submitHighscoreExec, inputValuesDictionary["submitHighscore"]);
		groupStartY += drawGroupWithInput("Get Leaderboard Highscore", "Get Score", groupStartY, getHighscoreExec, inputValuesDictionary["getHighscore"]);
		groupStartY += drawGroupWithInput("Download High Score Payload", "Download", groupStartY, downloadHighScorePayloadExec, inputValuesDictionary["downloadHighscorePayload"]);
		return groupStartY;
	}
	
	private float DrawChallengesGUI() {
		float groupStartY = 0f + groupHeightSeparator;

		groupStartY += drawGroupWithInput("Write Challenge", "Submit", groupStartY, writeChallengeExec, inputValuesDictionary["writeChallengeData"]);
		groupStartY += drawGroupWithInput("Display Challenge", "Display", groupStartY, displayChallengeExec, inputValuesDictionary["displayChallenge"]);
		return groupStartY;
	}
	

	private float DrawUtilityGUI() {
		float groupStartY = 0f + groupHeightSeparator;
		
		groupStartY += drawGroup("Release Version", "Fetch Release Version", groupStartY, releaseVersionExec);
		groupStartY += drawGroupWithInput("Dashboard Orientation\n(1,2,3,4)", "Set", groupStartY, setDashboardOrientationExec, inputValuesDictionary["setDashboardOrientation"]);
		groupStartY += drawGroup("Launch Dashboard", "Launch", groupStartY, launchDashboardExec);
		groupStartY += drawGroupWithInput("In Game Notification", "Send Notification", groupStartY, inGameNotifExec, inputValuesDictionary["inGameNotification"]);
		groupStartY += drawGroupWithInput("Upload Blob from File", "Upload", groupStartY, uploadBlobFromFileExec, inputValuesDictionary["uploadBlobFromFile"]);
		groupStartY += drawGroupWithInput("Download Blob to Path", "Download", groupStartY, downloadBlobToPathExec, inputValuesDictionary["downloadBlobToPath"]);
		return groupStartY;
	}
	

	private void HighScoreFetched(bool result, double score, string formattedHighScore) {
		if (result)
			OpenFeint.InGameNotification("Raw Highscore is: " + score + " Formatted: " + formattedHighScore);
		else
			OpenFeint.InGameNotification("Call to fetch high score failed !");		
	}
	
	private void HighScoreSubmitted(bool result) {
		if (result)
			OpenFeint.InGameNotification("High Score submitted successfully.");
		else
			OpenFeint.InGameNotification("High Score failed submission.");		
	}
	
	
	private void AchievementLoaded(bool result, string achievement) {
		if (result)
			OpenFeint.InGameNotification("Achievement element loaded : " + achievement);
		else
			OpenFeint.InGameNotification("Call to fetch achievement element failed !");		
	}

	private void AchievementListLoaded(ArrayList achievementsIds) {
		OpenFeint.InGameNotification("Fetched : " + achievementsIds.Count + " achievements !");		
	}
	
	private void AchievementPropertiesLoaded(Hashtable achvProperties) {
		OpenFeint.InGameNotification("Fetched : " + achvProperties.Count + " achievement properties !");		
	}

	private void BlobUploaded(bool result) {
		if (result)
			OpenFeint.InGameNotification("Blob successfully uploaded !");
		else
			OpenFeint.InGameNotification("Failed to upload Blob !");		
	}

	private void AchievementIconDownloaded(string filePath) {
		if (filePath != null && !"".Equals(filePath))
			OpenFeint.InGameNotification("Achievement Icon file path : " + filePath);
		else
			OpenFeint.InGameNotification("Get Achievement Icon failed !");
	}
	
	private void AchievementIconFailed(string achievementId) {
		OpenFeint.InGameNotification("Get Achievement Icon failed with error");
	}
	
	private void ReceivedNewChallenge(Gree.Unity.CallbackFetcherOpenFeint.ChallengeDefinition challengeDefinition)
	{
		OpenFeint.InGameNotification("Accepted Challenge with title: " + challengeDefinition.title);
	}

	public static void BlobDownloaded(bool result) {
		if (result)
			OpenFeint.InGameNotification("Blob successfully downloaded !");
		else
			OpenFeint.InGameNotification("Failed to download Blob !");		
	}

	private void InitializeWithConfiguration(bool initSuccess) {
		Debug.Log("Properties Loaded");
		versionString = OpenFeint.releaseVersionString;
	}

	/// <summary>
	/// It launches OF dashboard with an id.
	/// </summary>
	private class LaunchDashboardWithIdExecutor : IInputExecutor {
		
		public void execute(object[] input) {
			OpenFeint.LaunchDashboardWithLeaderboardID((string) input[0]);		
		}
		
	}
	
	/// <summary>
	/// It launches OF dashboard.
	/// </summary>
	private class LaunchDashboardExecutor : IExecutor {
		
		public void execute() {
			OpenFeint.LaunchDashboard();		
		}
		
	}

	/// <summary>
	/// It submits a given high score.
	/// </summary>
	private class SubmitHighscoreExecutor : IInputExecutor {
		
		public void execute(object[] input) {
			if(OpenFeint.isOnline) {
				OpenFeint.SubmitHighScore(long.Parse((string) input[0]), "HighScore : " + (string) input[0], DEFAULT_LEADERBOARD_ID, false);
			
				OpenFeint.InGameNotification("Failed to download Blob !");
			} else {
				OpenFeint.InGameNotification("The user is offline !");
			}
		}
		
	}

	/// <summary>
	/// It shows a game notification with no image.
	/// </summary>
	private class InGameNotificationExecutor : IInputExecutor {
		
		public void execute(object[] input) {
			OpenFeint.InGameNotification("Notification : " + (string) input[0]);
		}
		
	}

	/// <summary>
	/// It Fetches high score for a leaderboard.
	/// </summary>
	private class GetLeaderboardHighscoreExecutor : IInputExecutor {
		
		public void execute(object[] input) {
			if(OpenFeint.isOnline)
				OpenFeint.GetLeaderboardHighScore((string) input[0]);
			else
				OpenFeint.InGameNotification("The user is offline !");
		}
		
	}
	
	/// <summary>
	/// It unlocks an achievement.
	/// </summary>
	private class UnlockAchievementExecutor : IInputExecutor {
		
		public void execute(object[] input) {
			if(OpenFeint.isOnline) {
				OpenFeint.UnlockAchievement((string) input[0]);
				OpenFeint.InGameNotification("Achievement : " + (string) input[0] + " unlocked !");
			} else {
				OpenFeint.InGameNotification("The user is offline !");
			}
		}
		
	}

	/// <summary>
	/// Updates an achievement.
	/// </summary>
	private class UpdateAchievementExecutor : IInputExecutor {
		
		public void execute(object[] input) {
			if(OpenFeint.isOnline) {
				System.Random r = new System.Random();
				OpenFeint.UpdateAchievement((string) input[0], r.Next(1, 99), true);
				OpenFeint.InGameNotification("Achievement : " + (string) input[0] + " updated !");
			} else {
				OpenFeint.InGameNotification("The user is offline !");
			}
		}
		
	}

	/// <summary>
	/// Gets an achievement element by id and field.
	/// </summary>
	private class GetAchievementElementExecutor : IInputExecutor {
		
		public void execute(object[] input) {
			if(OpenFeint.isOnline)
				OpenFeint.GetAchievementElementByIdAndField((string) input[0], (string) input[1]);
			else
				OpenFeint.InGameNotification("The user is offline !");
		}
		
	}

	/// <summary>
	/// It checks if user status is online or not.
	/// </summary>
	private class OnlineStatusExecutor : IExecutor {
		
		public void execute() {
			if(OpenFeint.isOnline)
				OpenFeint.InGameNotification("The user is online !");
			else
				OpenFeint.InGameNotification("The user is offline !");
		}
		
	}
	
	/// <summary>
	/// It fetches local user name.
	/// </summary>
	private class LocalUserNameExecutor : IExecutor {
		
		public void execute() {
			if(OpenFeint.isOnline)
				OpenFeint.InGameNotification("User local name is : " + OpenFeint.localUserName);
			else
				OpenFeint.InGameNotification("The user is offline !");
		}
		
	}

	/// <summary>
	/// It fetches local user id.
	/// </summary>
	private class LocalUserIdExecutor : IExecutor {
		
		public void execute() {
			if(OpenFeint.isOnline)
				OpenFeint.InGameNotification("User local id is : " + OpenFeint.localUserID);
			else
				OpenFeint.InGameNotification("The user is offline !");
		}
		
	}

	/// <summary>
	/// It fetches release version.
	/// </summary>
	private class ReleaseVersionExecutor : IExecutor {
		
		public void execute() {
			OpenFeint.InGameNotification("Release version is : " + OpenFeint.releaseVersionString);
		}
		
	}

	/// <summary>
	/// It checks if user has approved OpenFeint.
	/// </summary>
	private class ApprovedOFExecutor : IExecutor {
		
		public void execute() {
			if (OpenFeint.hasUserApprovedFeint)
				OpenFeint.InGameNotification("User has approved OpenFeint !");
			else
				OpenFeint.InGameNotification("User has not approved OpenFeint !");
		}
		
	}

	/// <summary>
	/// Sets dashborad orientation.
	/// </summary>
	private class SetDashboardOrientationExecutor : IInputExecutor {
		
		public void execute(object[] input) {
			OpenFeint.SetDashboardOrientation((DeviceOrientation)System.Convert.ToInt32(input[0]));
		}
		
	}

	/// <summary>
	/// It fetches achievements ids.
	/// </summary>
	private class AchievementListExecutor : IExecutor {
		
		public void execute() {
			if(OpenFeint.isOnline)
				OpenFeint.AchievementList();
			else
				OpenFeint.InGameNotification("The user is offline !");
		}
		
	}
	
	/// <summary>
	/// It fetches achievement properties.
	/// </summary>
	private class AchievementPropertiesExecutor : IInputExecutor {
		
		public void execute(object[] input) {
			if(OpenFeint.isOnline)
				OpenFeint.AchievementProperties((string) input[0]);
			else
				OpenFeint.InGameNotification("The user is offline !");
		}
		
	}
	
	/// <summary>
	/// It uploads blob from file.
	/// </summary>
	private class UploadBlobFromFileExecutor : IInputExecutor {
		
		public void execute(object[] input) {
			if(OpenFeint.isOnline)
				OpenFeint.UploadBlobFromFile((string) input[0], (string) input[1]);
			else
				OpenFeint.InGameNotification("The user is offline !");
		}
		
	}
	
	/// <summary>
	/// It downloads blob to file path.
	/// </summary>
	private class DownloadBlobToPathExecutor : IInputExecutor {
		
		public void execute(object[] input) {
			if(OpenFeint.isOnline)
				OpenFeint.DownloadBlobToFilePath((string) input[0], (string) input[1], BlobDownloaded);
			else
				OpenFeint.InGameNotification("The user is offline !");
		}
		
	}
	
	/// <summary>
	/// It gets achievement icon and gets its file path.
	/// </summary>
	private class GetAchievementIconExecutor : IInputExecutor {
		
		public void execute(object[] input) {
			if(OpenFeint.isOnline)
				OpenFeint.GetAchievementIcon((string) input[0]);
			else
				OpenFeint.InGameNotification("The user is offline !");
		}
		
	}
	
	/// <summary>
	/// It downloads the highscore payload for a leaderboard and score
	/// </summary>
	private class DownloadHighScorePayloadExecutor : IInputExecutor {
	
		public void execute(object[] input) {
			if(OpenFeint.isOnline)
				OpenFeint.DownloadHighScorePayload((string) input[0], (string) input[1]);
			else
				OpenFeint.InGameNotification("The user is offline !");
		}
	
	}

	/// <summary>
	/// Writes challenge data to PlayerPrefs
	/// </summary>
	private class WriteChallengeExecutor : IInputExecutor {
	
		public void execute(object[] input) {
			if(OpenFeint.isOnline)
				PlayerPrefs.SetString((string) input[0], (string) input[1]);
			else
				OpenFeint.InGameNotification("The user is offline !");
		}
	
	}

	/// <summary>
	/// Displays the send challenge modal for a given id and keys
	/// </summary>
	private class DisplayChallengeExecutor : IInputExecutor {
	
		public void execute(object[] input) {
			if(OpenFeint.isOnline) {
				string[] dataKeys = new string[1];
				dataKeys[0] = (string) input[1];
				OpenFeint.DisplaySendChallengeModalWithPlayerPrefs((string) input[0], "Challenge description text", dataKeys);
			} else {
				OpenFeint.InGameNotification("The user is offline !");
			}
		}
	
	}
	
	
}