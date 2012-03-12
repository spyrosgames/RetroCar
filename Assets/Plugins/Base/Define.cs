using UnityEngine;
using System.Collections;

namespace Gree.Unity {
	public class Define {
		// DeployTarget
		public enum DeployTarget {
			PRODUCTION,
    		DEVELOPMENT,
    		STAGING,
		}
		public static DeployTarget GetDeployTargetFromString(string target) {
			return (DeployTarget)System.Enum.Parse(DeployTarget.PRODUCTION.GetType(), target);
		}
		
		// HTTP_STATUS
		public const int HTTP_STATUS_CODE_EXCEPTION = 0; 				///< Socket library catched exception. included send&recieve timeout.
		public const int HTTP_STATUS_CODE_OK = 200; 					///< OK
		public const int HTTP_STATUS_CODE_BAD_REQUEST = 400;			///< Bad request
		public const int HTTP_STATUS_CODE_UNAUTHORIZED = 401;			///< OAuth authentication failed.
		public const int HTTP_STATUS_CODE_METHOD_NOT_ALLOWED = 405;		///< The method used to access the API is not allowed.
		public const int HTTP_STATUS_CODE_INTERNAL_SERVER_ERROR = 500;	///< Game platform internal trouble
		public const int HTTP_STATUS_CODE_SERVER_UNAVAIABLE = 503;		///< Maintenance time.
		
		// ERROR_CODE
		public const int ERROR_CODE_UNKNOWN = 0;						///< Unknow error_code (as default.)
		public const int ERROR_CODE_FRIEND_CODE_ALREADY_IN_USE = 12402; ///< User friend code already in use.
		public const int ERROR_CODE_FRIEND_CODE_UNUPDATEABLE = 12403;   ///< User friend code unupdateable.
	}
}
