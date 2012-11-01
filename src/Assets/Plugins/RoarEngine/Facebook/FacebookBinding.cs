using UnityEngine;
using System.Collections;


/**
 * This class is a fake! you should replace it with a real version that does whatyou need.
 * It's modeled closely on the prime31 Social Networking plugins interface.
 */

public class FacebookBinding : MonoBehaviour
{
	static bool isLoggedIn = false;

	public static void Init( string applicationId )
	{
		Debug.Log("FacebookBinding.Init called with "+applicationId);
	}

	public static string GetAccessToken()
	{
		if( ! isLoggedIn )
		{
			Debug.LogError("FacebookBinding.getAccessToken used when not logged in");
			return "invalid";
		}

		Debug.Log ("FacebookBinding.getAccessToken called" );
		return "abefbedb123123b123abda_facebook_";
	}

	public static void Login()
	{
		isLoggedIn = true;
		Debug.Log ("FacebookBinding.login called");
		FacebookManager.OnLogin();
	}

	public static void Logout()
	{
		isLoggedIn = false;
		Debug.Log ("FacebookBinding.logout called");
		FacebookManager.OnLogout();
	}

}

