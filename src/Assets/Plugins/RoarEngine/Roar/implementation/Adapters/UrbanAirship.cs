using UnityEngine;
using System.Collections;
using System;

using Roar.Adapters;

namespace Roar.implementation.Adapters
{
	/**
	 * The interface to UrbanAirship notifications.
	 *
	 * The call sequence is a little messy here.
	 *
	 *     Roar.Start -> UrbanAirship.OnStart -> NotificationServices.RegisterForRemoteNotificationTypes (Apple)
	 *
	 * Then we wait and check for registration to complete:
	 *
	 *     Roar.Update -> UrbanAirship.OnUpdate -> remoteRegistrationSucceeded Event -> registerWithUrbanAirship (Roar Server) -> handleUASIOSRegister Callback.
	 */

	public class UrbanAirship : IUrbanAirship
	{
		public UrbanAirship( IWebAPI webAPI )
		{
			this.webAPI = webAPI;
		}

		protected IWebAPI webAPI;

		#if !UNITY_IOS
		public void OnStart() {
		}
		public void OnUpdate() {
		}
		public void SendPushNotification( string message, string targetUserID ) {
		}
		#else

		// Flag if the Device token has been sent to UA *this* session
		protected bool hasTokenBeenSentToUA = false;
		protected bool firedNotificationEvent = false;

		// How many remote notifications have we received this session
		protected int lastRemoteNotificationCount = 0;

		// --------
		// Fired when remote notifications are successfully registered for
		public static event Action remoteRegistrationSucceeded;

		// Fired when remote notification registration fails
		// @note This is currently never triggered!
		public static event Action<string> remoteRegistrationFailed;

		// Fired when UrbanAirship registration succeeds
		public static event Action urbanAirshipRegistrationSucceeded;

		// Fired when UrbanAirship registration fails
		public static event Action<string> urbanAirshipRegistrationFailed;

		// Fired when a remote notification is received or game was launched from a remote notification
		// public static event Action<Hashtable> remoteNotificationReceived;
		// --------



		public void OnStart()
		{
			// Need to call register on EVERY start of the application
            //
			// At some time in the future this call will cause NotificationServices.deviceToken to be set to
		    // a usefull value. Since we dont know when this will happen we have to watch for it inside the OnUpdate
		    // function
			NotificationServices.RegisterForRemoteNotificationTypes( RemoteNotificationType.Alert |
					RemoteNotificationType.Badge |
					RemoteNotificationType.Sound );

			remoteRegistrationSucceeded += registerWithUrbanAirship;
		}

		// Called onve we have a valid NotificationServices.deviceToken . We pass that value
		// on to the roar server in the correct form.
		void registerWithUrbanAirship()
		{
			byte[] token = NotificationServices.deviceToken;

			// Assuming we have a device token, proceed...
			if (token == null) return;

			// Swaps out XX-YY-ZZ to the required format XXYYZZ
			string formatToken = System.BitConverter.ToString( token ).Replace( "-", "" );

			Hashtable post = new Hashtable();
			post["device_token"] = formatToken;

			// Send registration token to UrbanAirship
			webAPI.urbanairship.ios_register( post, new HandleUASIOSRegister(null,this) );
		}

		// Called with the response from the call to roar to register the deviceToken.
		class HandleUASIOSRegister : SimpleRequestCallback<IXMLNode>
		{
			protected UrbanAirship urbanAirship;
			public HandleUASIOSRegister( Roar.Callback in_cb, UrbanAirship in_urbanAirship) : base(in_cb)
			{
				urbanAirship = in_urbanAirship;
			}

			public override void onFailure( CallbackInfo<IXMLNode> info )
			{
				if( urbanAirshipRegistrationFailed!=null) urbanAirshipRegistrationFailed(info.msg);
			}

			public override object onSuccess( CallbackInfo<IXMLNode> info )
			{
				urbanAirship.hasTokenBeenSentToUA = true;
				if( UrbanAirship.urbanAirshipRegistrationSucceeded!=null) UrbanAirship.urbanAirshipRegistrationSucceeded();
				return null;
			}
		}


		public void OnUpdate()
		{
			//Check whether registration has returned from Apple.
			if( NotificationServices.deviceToken == null ) return;

			if( ! firedNotificationEvent )
			{
				firedNotificationEvent = true;
				if( remoteRegistrationSucceeded!=null) remoteRegistrationSucceeded();
			}

			pollForNewRemoteNotifications();
			return;
		}



		// Expose this to developers to enable them to wire push notices on ANYthing...
		// @todo This should accept a callback so that if roar goes bad they can catch it.
		public void SendPushNotification( string message, string targetUserID )
		{
			// Only allow send if this device has been registered
			if (!hasTokenBeenSentToUA) return;

			Hashtable post = new Hashtable();
			post["message"] = message;
			post["roar_id"] = targetUserID;

			webAPI.urbanairship.push( post, null );
		}


		protected void pollForNewRemoteNotifications()
		{
			if (NotificationServices.remoteNotificationCount > lastRemoteNotificationCount)
			{
				lastRemoteNotificationCount = NotificationServices.remoteNotificationCount;

				// @TODO: Fire event on new remote Notification/s
				// string tokenText = ( NotificationServices.remoteNotifications[0] as RemoteNotification).alertBody;
			}
		}
		#endif
	}
}
