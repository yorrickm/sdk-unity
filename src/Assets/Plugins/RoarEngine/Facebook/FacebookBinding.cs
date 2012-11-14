using UnityEngine;
using System.Collections;


/**
 * This class is a fake! you should replace it with a real version that does whatyou need.
 * It's modeled closely on the prime31 Social Networking plugins interface.
 */

public class FacebookBinding : MonoBehaviour
{
	static bool isLoggedIn = false;
    static bool isAuthorized = false;
    static string signedRequestString = null;

    static IRoar roar;


	public static string GetSignedRequestString()
	{
		return signedRequestString;	
		
	}
	public static void Init( string applicationId )
	{
		Debug.Log("FacebookBinding.Init called with "+applicationId);
        //this application id can be derived either from a variable in the unity3d script (entered in the inspector)
        //or can be retrieved from the webapi since there is an option to enter it in the admin panel.
        
        roar = DefaultRoar.Instance;

        
            
	}
	
	/**
   * Function that is called from javascript and is passed the signedRequest string
   * 
   *   
   * @param signedRequest is the actual signed request picked up from facebook 'POST'
   **/
	void CatchFacebookRequest(string signedR)
    {
        signedRequestString = signedR;

    }
	
	/**
   * Function that is called to tell the hosting iframe to passback the signed request string if available.
   * Must be called before using the signed request string. 
   *   
   * 
   **/
	public static void RequestFacebookSignedRequest()
	{
		Application.ExternalCall("sendSignedRequest");
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

