using UnityEngine;
using System.Collections;

public class FacebookCatcherScript : MonoBehaviour {

    
    string requestedName = "";
	static string debugStr = "";
	
	

	// Use this for initialization
	void Start () {
        
		
		
	
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
        
		GUI.Label(new Rect(0, 0, 200, 60), debugStr);
        GUILayout.BeginArea(new Rect(200, 200, 300, 500), "");

        if (GUILayout.Button("check GET code"))
        {
            FacebookBinding.RequestFacebookGetCode();

        }

        if (GUILayout.Button("check SignedReq"))
        {
            FacebookBinding.RequestFacebookSignedRequest();
			
        }
        if (FacebookBinding.GetSignedRequestString() != null)
        {
            GUILayout.Label("Got Req:"+ FacebookBinding.GetSignedRequestString());
            GUILayout.BeginHorizontal();
            requestedName = GUILayout.TextField(requestedName);
            if (GUILayout.Button("Create Player"))
            {
                 IRoar roar = DefaultRoar.Instance;
                 Hashtable h = new Hashtable();
				h.Add("signed_request", FacebookBinding.GetSignedRequestString());
				h.Add("name", requestedName);
				roar.CreateFacebookSignedReq(requestedName, FacebookBinding.GetSignedRequestString(), onCreatedSigned);
				//roar.WebAPI.facebook.create_signed(h,new );
            }
			
			GUILayout.EndHorizontal();
            
			if (GUILayout.Button("Login"))
            {
                 IRoar roar = DefaultRoar.Instance;
				roar.LoginFacebookSignedReq(FacebookBinding.GetSignedRequestString(), onLoginSigned);
				//roar.WebAPI.facebook.create_signed(h,new );
            }
			
        }
        

        GUILayout.EndArea();
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
