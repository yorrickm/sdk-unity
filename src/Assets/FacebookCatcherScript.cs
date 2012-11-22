using UnityEngine;
using System.Collections;

public class FacebookCatcherScript : MonoBehaviour {

    
    string requestedName = "";
	static string debugStr = "";
	string userName ="";
	bool needsCreation = false;
	bool loginSuccessful = false;
	
	void RoarManager_logInFailedEvent(string obj)
    {
        Debug.Log("login failed, likely you haven't created a player" +obj);
        //creating player now.
		
		needsCreation = true;
    }
	
	// Use this for initialization
	void Start () {
        
		RoarManager.logInFailedEvent += new System.Action<string>(RoarManager_logInFailedEvent);

        RoarManager.loggedInEvent += new System.Action(RoarManager_loggedInEvent);
		
		
		
	}

    void RoarManager_loggedInEvent()
    {
        needsCreation = false;
        loginSuccessful = true;

    }
	
	static void onCreatedSigned( Roar.CallbackInfo cbI )
    {
        Debug.Log( "onCreatedSigned Called" );
        
        debugStr = cbI.msg;
    }
	
	static void onLoginSigned( Roar.CallbackInfo cbI )
    {
        Debug.Log( "onLoginSigned Called" );
        
        debugStr = cbI.msg+"logged in!";
    }
	

    void OnGUI()
    {
        IRoar roar = DefaultRoar.Instance;
		
		GUI.Label(new Rect(0, 0, 200, 60), debugStr);
        GUI.Label(new Rect(0, 0, 200, 130), "loggedIn"+loginSuccessful);
        GUILayout.BeginArea(new Rect(200, 200, 300, 500), "");

        //if (GUILayout.Button("check GET code"))
        //{
        //    FacebookBinding.RequestFacebookGetCode();

        //}

        if (GUILayout.Button("check SignedReq"))
        {
            FacebookBinding.RequestFacebookSignedRequest();
        }
		
		if(needsCreation)
		{


            GUILayout.BeginHorizontal();
            userName = GUILayout.TextField(userName);
            if (GUILayout.Button("Create Player"))
            {
				
				FacebookBinding.CreateFacebookOAuth(userName);
				//roar.WebAPI.facebook.create_signed(h,new );
            }
			
			GUILayout.EndHorizontal();
			
			
		}


        GUILayout.EndArea();
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
