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
    
	static string codeParameter = null; //when using oauth this parameter is passed via a get parameter.
    static string oAuthToken = null;

    static IRoar roar;

    static void onOAuthTokenFetch(Roar.CallbackInfo info)
    {
        Debug.Log("onOAuthTokenFetch Called");
        Debug.Log(info);
        Debug.Log(info.code);
        Debug.Log(info.msg);
        Debug.Log(info.d);

        //need to add an event for oauth token verified and saved here. Once the fetch code works.
    }
    

	
	
	public static void Init( string applicationId )
	{
		Debug.Log("FacebookBinding.Init called with "+applicationId);
        //this application id can be derived either from a variable in the unity3d script (entered in the inspector)
        //or can be retrieved from the webapi since there is an option to enter it in the admin panel.
        
        roar = DefaultRoar.Instance;
	}

    static void onLogin(Roar.CallbackInfo info)
    {
        Debug.Log("facebook binding login");
        //Debug.Log(info.d.ToString());
        Debug.Log(info.msg.ToString());

    }

    //attempts to login to roar. If it fails it most likely means roar needs to create the player or something
    //worse has gone wrong.
    void AttemptLogin()
    {
        
        Debug.Log("attempting login");
        roar.LoginFacebookOAuth(oAuthToken, onLogin);
        
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
			//fire event that says we are redirecting to login/authorize.

			Debug.Log("redirecting because of a blank code");
			Application.OpenURL("https://graph.facebook.com/oauth/authorize?client_id="+applicationID+"&redirect_uri="+paras.Split(' ')[1]);
			
		}
		else
		{
			Debug.Log("got string para");
			Debug.Log("string is "+paras);
        	codeParameter = paras.Split(' ')[0];
            //FacebookManager.OnOAuthTokenReady();
            //AttemptLogin();
			// move on to fetch oauth.
			//Hashtable h = new Hashtable();
			//h.Add("code", codeParameter);
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
        roar = DefaultRoar.Instance;
        if (signedR == "")
        {
            //fire signed request event failed. go for the graph api method.
            RequestFacebookGetCode();

        }
        else
        {
            //fire events related to signed request or signed auth.
            oAuthToken = signedR;
            AttemptLogin();
            //roar.LoginFacebookOAuth(signedR);
        }
    }
	
	/**
   * Creates a user based on the facebook oauth with the requested username.
   * 
   *   
   * @param Requested name.
   **/
	public static void CreateFacebookOAuth(string requestedName)
    {
        roar = DefaultRoar.Instance;
        if(oAuthToken != null)
        {
            
            roar.CreateFacebookOAuthToken(requestedName, oAuthToken);
        }
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
	
	// Try to collect a signed_request and if that is unavailable fire the graph.authorize
	public static string GetAccessToken()
	{

        RequestFacebookSignedRequest();
		return "";
	}
    // This function should trigger the whole login sequence. 
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

