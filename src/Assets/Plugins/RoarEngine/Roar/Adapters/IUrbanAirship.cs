using System;

namespace Roar.Adapters
{
	public interface IUrbanAirship
	{
		/**
	     * Sends a mobile notification to the given user.
	     *
	     * This will only work if the target user has enabled notifications for
	     * your application on their device.
	     */
		void SendPushNotification (string message, string targetUserID);
	}
}

