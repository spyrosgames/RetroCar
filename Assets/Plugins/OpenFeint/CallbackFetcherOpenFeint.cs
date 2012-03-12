using UnityEngine;
using System;
using System.Collections;
namespace Gree.Unity
{
    /// <summary>
    /// Callback fetcher for OpenFeint events/methods.
    /// </summary>
	public class CallbackFetcherOpenFeint : CallbackFetcherBase
    {
        #region Initialization
        public CallbackFetcherOpenFeint()
            : base()
        {
            initializeCallbacks();
        }

        /// <summary>
        /// Initializes the callbacks for every method to register.
        /// </summary>
		private void initializeCallbacks()
        {
            // OpenFeint
            registeredCallbacks.Add("PauseAudio", new PauseAudio());
            registeredCallbacks.Add("TriggerGeneralEvent", new TriggerGeneralEvent());
            registeredCallbacks.Add("TriggerDeferredResultEvent", new TriggerDeferredResultEvent());
            registeredCallbacks.Add("TriggerNotificationEvent", new TriggerNotificationEvent());
            registeredCallbacks.Add("TriggerUserLogInEvent", new TriggerUserLogInEvent());
            registeredCallbacks.Add("TriggerStartedChallengeEvent", new TriggerStartedChallengeEvent());
            registeredCallbacks.Add("TriggerAchievementIconLoaded", new TriggerAchievementIconLoaded());
            registeredCallbacks.Add("TriggerAchievementIconLoadFailed", new TriggerAchievementIconLoadFailed());
            registeredCallbacks.Add("TriggerFollowingResultCallback", new TriggerFollowingResultCallback());
            registeredCallbacks.Add("TriggerChallengeDefinitionResultCallback", new TriggerChallengeDefinitionResultCallback());
            registeredCallbacks.Add("TriggerDownloadCloudBlobResultCallback", new TriggerDownloadCloudBlobResultCallback());
//			this is the highscore table            
//          registeredCallbacks.Add("TriggerDeferredHighscoreResultEvent", new TriggerDeferredHighscoreResultEvent());
            registeredCallbacks.Add("TriggerHighScoreDownloadFailed", new TriggerHighScoreDownloadFailed());
			registeredCallbacks.Add("TriggerDownloadHighScorePayloadResultCallback", new TriggerDownloadHighScorePayloadCallback());
			registeredCallbacks.Add("TriggerDeferredAchievementElementCallback", new TriggerDeferredAchievementElementCallback());
			registeredCallbacks.Add("TriggerDeferredAchievementListCallback", new TriggerDeferredAchievementListCallback());
			registeredCallbacks.Add("TriggerDeferredAchievementPropertiesCallback", new TriggerDeferredAchievementPropertiesCallback());
			registeredCallbacks.Add("TriggerDeferredLeaderboardHighscoreCallback", new TriggerDeferredLeaderboardHighscoreCallback());

        }
		
		public override Hashtable callBackHandlersList() {
			
			Hashtable _callbackHandlers = new Hashtable();
			_callbackHandlers.Add ("PauseAudio", this);
			_callbackHandlers.Add ("TriggerGeneralEvent", this);
			_callbackHandlers.Add ("TriggerDeferredResultEvent", this);
			_callbackHandlers.Add ("TriggerNotificationEvent", this);
			_callbackHandlers.Add ("TriggerUserLogInEvent", this);
			_callbackHandlers.Add ("TriggerStartedChallengeEvent", this);
			_callbackHandlers.Add ("TriggerAchievementIconLoaded", this);
			_callbackHandlers.Add ("TriggerAchievementIconLoadFailed", this);
			_callbackHandlers.Add ("TriggerFollowingResultCallback", this);
			_callbackHandlers.Add ("TriggerChallengeDefinitionResultCallback", this);
			_callbackHandlers.Add ("TriggerDownloadCloudBlobResultCallback", this);
//			_callbackHandlers.Add ("TriggerDeferredHighscoreResultEvent", this);
			_callbackHandlers.Add ("TriggerHighScoreDownloadFailed", this);
			_callbackHandlers.Add ("TriggerDownloadHighScorePayloadResultCallback", this);
			_callbackHandlers.Add ("TriggerDeferredAchievementElementCallback", this);
			_callbackHandlers.Add ("TriggerDeferredAchievementListCallback", this);
			_callbackHandlers.Add ("TriggerDeferredAchievementPropertiesCallback", this);
			_callbackHandlers.Add ("TriggerDeferredLeaderboardHighscoreCallback", this);
			return _callbackHandlers;
		}

        #endregion

        #region Declarations
        // The following enumerations correspond to enumerations found in the OpenFeint
        // source code.
        
		/// <summary>
		/// Corresponds to OFNotificationCategory in OFNotificationDelegate.h
		/// </summary>
        public enum eNotificationCategory
        {
            Foreground = 1,
            Login,
            Challenge,
            HighScore,
            Leaderboard,
            Achievement,
            SocialNotification,
            Presence,
        }
		
		/// <summary>
		/// Corresponds to OFNotificationType in OFNotificationDelegate.h
		/// </summary>
        public enum eNotificationType
        {
            None = 0,
            Submitting,
            Downloading,
            Error,
            Success,
            NewResources,
            UserPresenceOnline,
            UserPresenceOffline,
            NewMessage,
        }

        /// <summary>
        /// Corresponds to OFChallengeResult in OFChallengeToUser.h
        /// </summary>
		public enum eChallengeResult
        {
            Incomplete,
            RecipientWon,
            RecipientLost,
            Tie,
        }


        // STRUCTURES //

        public struct ChallengeDefinition
        {
            public string title;
            public long challengeId;
            public string iconUrl;
            public bool multiAttempt;
        }

        // OpenFeint (continuing)
        //-----------------------
        //
        // Event handler delegates.
        public delegate void GeneralEventHandler();
        public delegate void DeferredResultEventHandler( bool result );
        public delegate void NotificationEventHandler( string text, eNotificationCategory category, eNotificationType type );
        public delegate void UserLogInEventHandler( string userId );
        public delegate void StartedChallengeEventHandler( ChallengeDefinition definition );
        public delegate void IconLoadEventHandler( string achievementId, string filePath );

		// Deprecated
        public delegate void IconLoadedEventHandler( string filePath );
		// Deprecated
        public delegate void IconLoadFailedEventHandler( string achievementId );
// this is the highscore table         
//		public delegate void DeferredHighScoreDelegate( bool result, double score);
		public delegate void DeferredAchievementElementLoaded( bool result, string achievement);		
		
        // Callback delegates.
        public delegate void FollowingResultDelegate( bool success, bool isFollowingAnyone );
        public delegate void ChallengeDefinitionResultDelegate( bool success, ChallengeDefinition definition );
        public delegate void DeferredResultDelegate( bool success );
		public delegate void DeferredAchievementListLoaded( ArrayList list );
		public delegate void DeferredAchievementPropertiesLoaded( Hashtable properties );
		public delegate void DeferredLeaderboardHighscore(bool result, double highScore, string formattedHighScore);	

        // OpenFeint (continuing)
        //----------------------- 
        //
		/// <summary>
		/// Sent when an OpenFeint notification should appear on screen. This is
        /// sent regardless of whether or not notifications are allowed. If
        /// notifications are not allowed, use this event to determine when you
        /// should display your own message.
		/// </summary>
        static public event NotificationEventHandler OnNotification;

        // Sent upon success or failure of a high score submission.
		/// <summary>
		/// Sent upon success or failure of a high score submission.
		/// </summary>
        static public event DeferredResultEventHandler OnHighScoreSubmitted;

		/// <summary>
        /// Sent upon success or failure of an achievement progression update. If the
        /// achievement has already been unlocked locally, this event will not
        /// be sent.
		/// </summary>
        static public event DeferredResultEventHandler OnAchievementUpdated;

		/// <summary>
        /// Sent upon success or failure of an achievement unlock. If the
        /// achievement has already been unlocked locally, this event will not
        /// be sent.
		/// </summary>
        static public event DeferredResultEventHandler OnAchievementUnlocked;

        /// <summary>
        /// Sent when the user starts a challenge. You will need to subsequently
        /// call one of the GetChallengeData method to get the challenge data that
        /// was sent with the challenge.
        /// </summary>
		static public event StartedChallengeEventHandler OnStartedChallenge;

		/// <summary>
		/// Sent when the user restarts a challenge.
		/// </summary>
        static public event GeneralEventHandler OnRestartedChallenge;

        /// <summary>
        /// Sent when the user restarts a challenge that he is trying to create.
        /// This message is only sent if stronger attempts are enabled for
        /// challenges and if the challenge was a multi-attempt challenge.
        /// </summary>
		static public event GeneralEventHandler OnRestartedCreateChallenge;

        /// <summary>
		/// Sent when the completed challenge screen is closed.
        /// </summary>
        static public event GeneralEventHandler OnCompletedChallengeScreenClosed;

        /// <summary>
		/// Sent when the send challenge screen is closed.
        /// </summary>
        static public event GeneralEventHandler OnSendChallengeScreenClosed;

		/// <summary>
        /// Sent when the user sends a challenge to another user.
		/// </summary>
        static public event GeneralEventHandler OnUserSentChallenges;

        /// <summary>
		/// Sent upon success or failure of a challenge result submission.
        /// </summary>
        static public event DeferredResultEventHandler OnChallengeResultSubmitted;

		/// <summary>
		/// Send upon success or failure of uploading a blob to the cloud.
		/// </summary>
        static public event DeferredResultEventHandler OnUploadedCloudBlob;

		/// <summary>
        /// Sent when a user successfully connects to the OpenFeint servers.
		/// </summary>
        static public event UserLogInEventHandler OnUserLoggedInToOF;

        /// <summary>
        /// Sent when a user explicitly logs out from OpenFeint (though does not
        /// get sent when user switches accounts; only OnUserLoggedInToOF gets sent
        /// in that instance).
        /// </summary>
		static public event UserLogInEventHandler OnUserLoggedOutOfOF;
		
		/// <summary>
        /// Sent when an Icon successfully is written to local storage or
		/// sent when an Icon retrieval fails.
		/// </summary>
		static public event IconLoadEventHandler OnAchievementIconLoad;	

		/// <summary>
        /// Sent when an Icon successfully is written to local storage (Deprecated)
		/// </summary>
        static public event IconLoadedEventHandler OnAchievementIconLoaded;

		/// <summary>
        /// Sent when an Icon retrieval fails.  (Deprecated)
		/// </summary>
        static public event IconLoadFailedEventHandler OnAchievementIconLoadFailed;

		static public event DeferredAchievementElementLoaded OnAchievementElementLoaded;
		
		static public event DeferredAchievementListLoaded OnAchievementListFetched;
		
		static public event DeferredAchievementPropertiesLoaded OnAchievementPropertiesFetched;
		
		static public event DeferredLeaderboardHighscore OnLeaderboardHighScoreFetched;
		
        #endregion

        #region OpenFeint CallBacks
        private class PauseAudio : ICallback
        {
            public void execute( ArrayList args )
            {
                bool pauseToggle = System.Convert.ToBoolean(args[0]);
                AudioListener.pause = pauseToggle;
            }
        }

        private class TriggerGeneralEvent : ICallback
        {

            public void execute( ArrayList args )
            {
                int eventIndex = System.Convert.ToInt32(args[0]);
                switch ( eventIndex )
                {
                    case 0: if ( OnRestartedChallenge != null ) OnRestartedChallenge(); break;
                    case 1: if ( OnRestartedCreateChallenge != null ) OnRestartedCreateChallenge(); break;
                    case 2: if ( OnCompletedChallengeScreenClosed != null ) OnCompletedChallengeScreenClosed(); break;
                    case 3: if ( OnSendChallengeScreenClosed != null ) OnSendChallengeScreenClosed(); break;
                    case 4: if ( OnUserSentChallenges != null ) OnUserSentChallenges(); break;
                }
            }
        }


        private class TriggerDeferredResultEvent : ICallback
        {

            public void execute( ArrayList args )
            {
                int eventIndex = System.Convert.ToInt32(args[0]);
                bool result = System.Convert.ToBoolean(args[1]);
                switch ( eventIndex )
                {
                    case 0: if ( OnHighScoreSubmitted != null ) OnHighScoreSubmitted(result); break;
                    case 1: if ( OnAchievementUpdated != null ) OnAchievementUpdated(result); break;
                    case 2: if ( OnAchievementUnlocked != null ) OnAchievementUnlocked(result); break;
                    case 3: if ( OnChallengeResultSubmitted != null ) OnChallengeResultSubmitted(result); break;
                    case 4: if ( OnUploadedCloudBlob != null ) OnUploadedCloudBlob(result); break;
                }
            }
        }

        private class TriggerNotificationEvent : ICallback
        {

            public void execute( ArrayList args )
            {
                string text = (string)args[0];
                int category = System.Convert.ToInt32(args[1]);
                int type = System.Convert.ToInt32(args[2]);
                if ( OnNotification != null )
                {
                    OnNotification(text, (eNotificationCategory)category, (eNotificationType)type);
                }
            }
        }

        private class TriggerUserLogInEvent : ICallback
        {
            public void execute( ArrayList args )
            {
                int eventIndex = System.Convert.ToInt32(args[0]);
                string userId = (string)args[1];

                switch ( eventIndex )
                {
                    case 0: if ( OnUserLoggedInToOF != null ) OnUserLoggedInToOF(userId); break;
                    case 1: if ( OnUserLoggedOutOfOF != null ) OnUserLoggedOutOfOF(userId); break;
                }
            }
        }

        private class TriggerStartedChallengeEvent : ICallback
        {
            public void execute( ArrayList args )
            {
                string title = (string)args[0];
                long id = System.Convert.ToInt64(args[1]);
                string iconUrl = (string)args[2];
                bool multiAttempt = System.Convert.ToBoolean(args[3]);

                if ( OnStartedChallenge != null )
                {
                    ChallengeDefinition definition;
                    definition.title = title;
                    definition.challengeId = id;
                    definition.iconUrl = iconUrl;
                    definition.multiAttempt = multiAttempt;

                    OnStartedChallenge(definition);
                }
            }
        }

        private class TriggerAchievementIconLoad : ICallback
        {
            public void execute( ArrayList args )
            {
                if ( OnAchievementIconLoad != null ) {
					string achievementId = (string)args[0];
					string filePath = null;
					if (args[1] == null) {
                		filePath = (string)args[1];
					}
                    OnAchievementIconLoad(achievementId, filePath);
                }
            }
        }

        private class TriggerAchievementIconLoaded : ICallback
        {
            public void execute( ArrayList args )
            {
                string filePath = (string)args[0];

                if ( OnAchievementIconLoaded != null )
                {
                    OnAchievementIconLoaded(filePath);
                }
            }
        }

        private class TriggerAchievementIconLoadFailed : ICallback
        {
            public void execute( ArrayList args )
            {
                string achievementId = (string)args[0];

                if ( OnAchievementIconLoadFailed != null )
                {
                    OnAchievementIconLoadFailed(achievementId);
                }
            }
        }


        // Methods called from Objective-C to trigger callbacks.
        private class TriggerFollowingResultCallback : ICallback
        {
            public void execute( ArrayList args )
            {
                bool success = System.Convert.ToBoolean(args[0]);
                bool result = System.Convert.ToBoolean(args[1]);
                if ( followingResultCallback != null )
                {
                    followingResultCallback(success, result);
                    followingResultCallback = null;
                }
            }
        }

        private class TriggerChallengeDefinitionResultCallback : ICallback
        {

            public void execute( ArrayList args )
            {
                bool success = System.Convert.ToBoolean(args[1]);
                if ( challengeDefinitionResultCallback != null )
                {
                    ChallengeDefinition definition;
					definition.title = "";
					definition.challengeId = 0;
					definition.iconUrl = "";
					definition.multiAttempt = false;
                    Hashtable challlengeDefinitionValues;
                    if (success) 
                    {
                    	challlengeDefinitionValues = (Hashtable)args[0];

						definition.title = (string)challlengeDefinitionValues["title"];
						definition.challengeId = (long)challlengeDefinitionValues["challengeId"];
						definition.iconUrl = (string)challlengeDefinitionValues["iconUrl"];
						definition.multiAttempt = (bool)challlengeDefinitionValues["multiAttempt"];
					}
                    challengeDefinitionResultCallback(success, definition);
                    challengeDefinitionResultCallback = null;
                }
            }
        }

        private class TriggerDownloadCloudBlobResultCallback : ICallback
        {
            public void execute( ArrayList args )
            {
                bool success = System.Convert.ToBoolean(args[0]);
                if ( downloadCloudBlobResultCallback != null )
                {
                    downloadCloudBlobResultCallback(success);
                }
            }
        }

/*      This is the highscore table  
		private class TriggerDeferredHighscoreResultEvent : ICallback
        {

            public void execute( ArrayList args )
            {
                bool success = System.Convert.ToBoolean(args[0]);
				double score = -1;
				if (OnLeaderboardHighScoreFetched != null) {
					if (success) {
						score = System.Convert.ToInt64(args[1]);
					}
					OnLeaderboardHighScoreFetched(success, score);
				}
						
            }
        }
*/

		private class TriggerDeferredLeaderboardHighscoreCallback : ICallback
		{

		    public void execute( ArrayList args )
		    {
				if (OnLeaderboardHighScoreFetched != null) {                
					bool success = System.Convert.ToBoolean(args[0]);
					Int64 score = System.Convert.ToInt64(args[1]);
					string formattedScore = (string)args[2];						
					OnLeaderboardHighScoreFetched(success, score, formattedScore);
				}
				
		    }
		}

        private class TriggerDeferredAchievementElementCallback : ICallback
        {

            public void execute( ArrayList args )
            {
                bool success = System.Convert.ToBoolean(args[0]);
				string achievementPropertyValue = "";
				if (OnAchievementElementLoaded != null) {
					if (success) {
						achievementPropertyValue = System.Convert.ToString(args[1]);
					}
					OnAchievementElementLoaded(success, achievementPropertyValue);
				}
						
            }
        }

        private class TriggerDeferredAchievementListCallback : ICallback
        {

            public void execute( ArrayList args )
            {
				if (OnAchievementListFetched != null) {
					OnAchievementListFetched(args);
				}
            }
        }

        private class TriggerDeferredAchievementPropertiesCallback : ICallback
        {

            public void execute( ArrayList args )
            {
				Hashtable properties = new Hashtable();
				if (OnAchievementPropertiesFetched != null) {
					if (args.Count > 0) {
						properties = (Hashtable)args[0];
					}
					OnAchievementPropertiesFetched(properties);
				}
            }
        }

		private class TriggerHighScoreDownloadFailed : ICallback
        {

            public void execute( ArrayList args )
            {
                //bool success = System.Convert.ToBoolean(args[0]);
            }
        }
		
        private class TriggerDownloadHighScorePayloadCallback : ICallback
        {
            public void execute( ArrayList args )
            {
                bool success = System.Convert.ToBoolean(args[0]);
                if ( downloadHighScorePayloadResultCallback != null )
                {
                    downloadHighScorePayloadResultCallback(success);
                }
            }
        }	
        			
        // Stored callbacks.
        static public FollowingResultDelegate followingResultCallback;
        static public ChallengeDefinitionResultDelegate challengeDefinitionResultCallback;
        static public DeferredResultDelegate downloadCloudBlobResultCallback;
		static public DeferredResultDelegate downloadHighScorePayloadResultCallback;
		
        #endregion
    }
}
