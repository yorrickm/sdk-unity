using System;
using System.Collections;

namespace UnityEngine
{
	public class Debug
	{
		public static void Log( string k )
		{
			Console.WriteLine(k);
		}
		public static void LogError( string k )
		{
			Console.WriteLine("ERROR : "+ k);
		}
	}

	public class MonoBehaviour
	{
		public void StartCoroutine( IEnumerator e )
		{
		}

		public void DontDestroyOnLoad( MonoBehaviour m )
		{
		}
	}

	public class WWWForm
	{
		public void AddField(string K, string V)
		{
		}
	}

	public class WWW
	{
		public WWW( string url, WWWForm post_params )
		{
			text = "unimplemented stub!";
		}

		public string text;
	}

	public class RemoteNotificationType
	{
		public static int Alert = 1;
		public static int Badge = 2;
		public static int Sound = 4;
	}

	public class NotificationServices
	{
		public static void RegisterForRemoteNotificationTypes( int x )
		{
		}
		public static byte[] deviceToken;
		public static int remoteNotificationCount;
	}
}

