////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
/// 
///  Copyright 2010 Aurora Feint, Inc.
/// 
///  Licensed under the Apache License, Version 2.0 (the "License");
///  you may not use this file except in compliance with the License.
///  You may obtain a copy of the License at
///  
///      http://www.apache.org/licenses/LICENSE-2.0
///      
///  Unless required by applicable law or agreed to in writing, software
///  distributed under the License is distributed on an "AS IS" BASIS,
///  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
///  See the License for the specific language governing permissions and
///  limitations under the License.
/// 
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

//
// OFEditorIntegration.cs
//
// Integrates certain OpenFeint features into the Unity editor.
//
// This code was contributed to OpenFeint by Tiny Tim Games (www.tinytimgames.com).
// It is thanks to awesome developers like these that OpenFeint can keep rocking!
//

using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Diagnostics;
using System.IO;

public class OFEditorIntegration
{
	public static readonly string kSettingsFile = Application.dataPath + "/Plugins/iOS/OpenFeintSettingsDefines.h";
	
	// SetApplicationSettings //
	[MenuItem("OpenFeint/Set Application Initialization Settings...")]
	public static void SetApplicationSettings()
	{
		// Show the OpenFeint settings wizard.
		OpenFeintSettingsWizard newWizard = ScriptableWizard.DisplayWizard(
		"Set Application Settings for OpenFeint",
			typeof(OpenFeintSettingsWizard), "Apply") as OpenFeintSettingsWizard;
			
		// Initialize the settings from the file.
		string settingsFile = kSettingsFile;
		if(System.IO.File.Exists(settingsFile))
		{
			StreamReader reader = null;
			try
			{
				reader = new StreamReader(settingsFile);
				
				if(reader != null)
				{
					for(int i = 0; i < 5; i++)
					{
						string s = reader.ReadLine();
						string[] x = s.Split('\"');
						
						switch(i)
						{
							// #define UOF_APP_DISPLAY_NAME @"x"
							case 0:	newWizard.appDisplayName = x[1]; break;
							
							// #define UOF_APP_SHORT_DISPLAY_NAME @"x"
							case 1: newWizard.appShortDisplayName = x[1]; break;
							
							// #define UOF_CLIENT_ID @"x"
							case 2: newWizard.clientId = x[1]; break;
							
							// #define UOF_PRODUCT_KEY @"x"
							case 3: newWizard.productKey = x[1]; break;
							
							// #define UOF_SECRET_KEY @"x"
							case 4: newWizard.secretKey = x[1]; break;
						}
					}
					
					for(int i = 0; i < 7; i++)
					{
						string s = reader.ReadLine();
						string[] x = s.Split(' ');
						
						switch(i)
						{
							// #define UOF_INITIAL_DASHBOARD_ORIENTATION x
							case 0: newWizard.initialDashboardOrientation = (DeviceOrientation)int.Parse(x[2]); break;
							
							// #define UOF_ALLOW_NOTIFICATIONS x
							case 1: newWizard.allowNotifications = (x[2] == "YES") ? true : false; break;
							
							// #define UOF_INVERT_NOTIFICATIONS x
							case 2: newWizard.notificationPosition = (ENotificationPosition)int.Parse(x[2]); break;
							
							// #define UOF_ALLOW_STRONGER_ATTEMPT_FOR_CHALLENGES x
							case 3: newWizard.allowStrongerAttemptForChallenges = (x[2] == "1") ? true : false; break;
							
							// #define UOF_ENABLE_PUSH_NOTIFICATIONS x
							case 4: newWizard.enablePushNotifications = (x[2] == "YES") ? true : false; break;
							
							// #define UOF_DISABLE_USER_GENERATED_CONTENT x
							case 5: newWizard.disableUserGeneratedContent = (x[2] == "YES") ? true : false; break;
							
							// #define UOF_PROMPT_TO_POST_ACHIEVEMENT_UNLOCK x
							case 6: newWizard.promptToPostAchievementUnlock = (x[2] == "YES") ? true : false; break;
							
							// #define UOF_ALWAYS_ASK_FOR_APPROVAL_IN_DEBUG
							case 7: newWizard.alwaysAskForApprovalInDebug = (x[2] == "YES") ? true : false; break;
						}
					}
				}
			}
			finally
			{
				if(reader != null)
				{
					reader.Close();
				}
			}
		}
	}
	
	// OpenDeveloperDashboard //
	[MenuItem("OpenFeint/Developer Dashboard")]
	public static void OpenDeveloperDashboard()
	{
		int clientId = -1;
		string settingsFile = kSettingsFile;
		
		// Determine if the settings file exists.
		if(System.IO.File.Exists(settingsFile))
		{
			StreamReader reader = null;
			try
			{
				reader = new StreamReader(settingsFile);
				
				if(reader != null)
				{
					// Skip two lines to get to the proper define.
					reader.ReadLine();
					reader.ReadLine();
					
					// Read in the value of the client ID.
					string s = reader.ReadLine();
					string[] x = s.Split('\"');
					clientId = int.Parse(x[1]);
				}
			}
			finally
			{
				if(reader != null)
				{
					reader.Close();
				}
			}
		}
		
		if(clientId != -1)
		{
			// Open the OpenFeint Developer Dashboard to the proper client application.
			Application.OpenURL("https://api.openfeint.com/?client_application_id=" + clientId);
		}
		else
		{
			// No client ID was found, so just open up the OpenFeint Developer Dashboard to the default page.
			Application.OpenURL("https://api.openfeint.com");
		}
	}
	
	// OpenDeveloperSupportCenter //
	[MenuItem("OpenFeint/Developer Support Center")]
	public static void OpenDeveloperSupportCenter()
	{
		// This opens the OpenFeint Developer Support Center webpage.
		Application.OpenURL("http://www.openfeint.com/ofdeveloper");
	}
}
