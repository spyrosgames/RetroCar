using UnityEngine;
using UnityEditor;

using System.IO;
using System.Collections;


public class OpenFeintSettingsWizard : ScriptableWizard
{
	// The settings that can be set by the wizard.
	public string appDisplayName;
	public string appShortDisplayName;
	public string clientId;
	public string productKey;
	public string secretKey;
	public DeviceOrientation initialDashboardOrientation = DeviceOrientation.LandscapeLeft;
	public bool allowNotifications = true;
	public ENotificationPosition notificationPosition = ENotificationPosition.ENotificationPosition_TOP;
	public bool allowStrongerAttemptForChallenges;
	public bool enablePushNotifications;
	public bool disableUserGeneratedContent;
	public bool promptToPostAchievementUnlock;
	public bool alwaysAskForApprovalInDebug;
	
	// Writes out the settings to a header file that will be included with the Xcode project upon build.
	private void OnWizardCreate()
	{
		string settingsFile = OFEditorIntegration.kSettingsFile;

		StreamWriter writer = null;
		try
		{
			writer = new StreamWriter(settingsFile);
			
			if(writer != null)
			{
				writer.WriteLine("#define UOF_APP_DISPLAY_NAME @\"" + appDisplayName + "\"");
				writer.WriteLine("#define UOF_APP_SHORT_DISPLAY_NAME @\"" + appShortDisplayName + "\"");
				writer.WriteLine("#define UOF_CLIENT_ID @\"" + clientId + "\"");
				writer.WriteLine("#define UOF_PRODUCT_KEY @\"" + productKey + "\"");
				writer.WriteLine("#define UOF_SECRET_KEY @\"" + secretKey + "\"");
				
				writer.WriteLine("#define UOF_INITIAL_DASHBOARD_ORIENTATION " + (int)initialDashboardOrientation);
				writer.WriteLine("#define UOF_ALLOW_NOTIFICATIONS " + ((allowNotifications) ? "YES" : "NO"));
				writer.WriteLine("#define UOF_NOTIFICATION_POSITION " + (int)notificationPosition);
				writer.WriteLine("#define UOF_ALLOW_STRONGER_ATTEMPT_FOR_CHALLENGES " + ((allowStrongerAttemptForChallenges) ? "1" : "0"));
				writer.WriteLine("#define UOF_ENABLE_PUSH_NOTIFICATIONS " + ((enablePushNotifications) ? "YES" : "NO"));
				writer.WriteLine("#define UOF_DISABLE_USER_GENERATED_CONTENT " + ((disableUserGeneratedContent) ? "YES" : "NO"));
				writer.WriteLine("#define UOF_PROMPT_TO_POST_ACHIEVEMENT_UNLOCK " + ((promptToPostAchievementUnlock) ? "YES" : "NO"));
				writer.WriteLine("#define UOF_ALWAYS_ASK_FOR_APPROVAL_IN_DEBUG " + ((alwaysAskForApprovalInDebug) ? "YES" : "NO"));
			}
		}
		finally
		{
			if(writer != null)
			{
				writer.Close();
				
				// Make sure we refresh so that the file shows up in Unity.
				AssetDatabase.Refresh();
			}
		}
	}
	
	private void OnWizardUpdate()
	{
		helpString = "Set your application's OpenFeint initialization settings here. This will create a header file which will be added to the Xcode project to provide OpenFeint with the proper settings for startup.";
	}
}