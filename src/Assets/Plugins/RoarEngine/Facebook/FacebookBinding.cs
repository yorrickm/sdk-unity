using UnityEngine;
using System.Collections;


/**
 * This class is a fake! you should replace it with a real version that does whatyou need.
 * It's modeled closely on the prime31 Social Networking plugins interface.
 */

public class FacebookBinding : MonoBehaviour
{
	public string applicationID;
	static bool isLoggedIn = false;
    static bool isAuthorized = false;
    static string signedRequestString = null;
	static string codeParameter = null; //when using oauth this parameter is passed via a get parameter.

    static IRoar roar;

    static void onOAuthTokenFetch(Roar.CallbackInfo info)
    {
        Debug.Log("onOAuthTokenFetch Called");
        Debug.Log(info);
        Debug.Log(info.code);
        Debug.Log(info.msg);
        Debug.Log(info.d);
    }
    

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
   * Function that is called from javascript and is handed the facebook code parameter. Call graph.authorize with this.
   * 
   *   
   * @param code is the get parameter picked up from facebook 'GET'. Can be null.
   **/
	void CatchCodeGetPara(string paras)
    {
		roar = DefaultRoar.Instance;
		if(paras.Split(' ')[0] == "")
		{
			
			//Invoke redirect with authorization.
			
			Debug.Log("redirecting because of a blank code");
			Application.OpenURL("https://graph.facebook.com/oauth/authorize?client_id="+applicationID+"&redirect_uri="+paras.Split(' ')[1]);
			
		}
		else
		{
			Debug.Log("got string para");
			Debug.Log("string is "+paras);
        	codeParameter = paras.Split(' ')[0];
			
			// move on to fetch oauth.
			Hashtable h = new Hashtable();
			h.Add("code", codeParameter);
            roar.FetchFacebookOAuthToken(codeParameter, onOAuthTokenFetch);
		
		}
		
		
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
	
	/**
   * Function that is called to ask the hosting browser to pass the get parameter code give by facebook
   * If no get parameter code is available will return blank and a graph authorization url will have to be requested.
   *   
   * 
   **/
	public static void RequestFacebookGetCode()
	{
		Application.ExternalCall("returnCodeIfAvailable");
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

