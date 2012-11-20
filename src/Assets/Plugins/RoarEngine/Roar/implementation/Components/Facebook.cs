using System.Collections;
using UnityEngine;

using Roar.Components;

namespace Roar.implementation.Components
{
	public class Facebook : IFacebook
	{
		protected DataStore dataStore;
		IWebAPI.IFacebookActions facebook;
		ILogger logger;

		public Facebook (IWebAPI.IFacebookActions facebookActions, DataStore dataStore, ILogger logger)
		{
			this.facebook = facebookActions;
			this.dataStore = dataStore;
			this.logger = logger;

			// -- Event Watchers
			// Flush models on logout
			RoarManager.loggedOutEvent += () => {
				dataStore.Clear (true); };

			// Watch for initial inventory ready event, then watch for any
			// subsequent `change` events
			RoarManager.inventoryReadyEvent += () => CacheFromInventory ();
		}

        protected class FacebookCreateCallback : SimpleRequestCallback<IXMLNode>
		{
			protected Facebook facebook;

			public FacebookCreateCallback (Roar.Callback in_cb, Facebook in_facebook) : base(in_cb)
			{
				facebook = in_facebook;
			}

			public override void OnFailure (CallbackInfo<IXMLNode> info)
			{
				RoarManager.OnCreateUserFailed (info.msg);
			}

			public override object OnSuccess (CallbackInfo<IXMLNode> info)
			{
				RoarManager.OnCreatedUser ();
				RoarManager.OnLoggedIn ();
				return null;
			}
		}


		// ---- Access Methods ----
		// ------------------------

		
		protected class LoginCallback : SimpleRequestCallback<IXMLNode>
		{
			protected Facebook facebook;

			public LoginCallback (Roar.Callback in_cb, Facebook in_facebook) : base(in_cb)
			{
				facebook = in_facebook;
			}

			public override void OnFailure (CallbackInfo<IXMLNode> info)
			{
				RoarManager.OnLogInFailed (info.msg);
			}

			public override object OnSuccess (CallbackInfo<IXMLNode> info)
			{
				RoarManager.OnLoggedIn ();
				// @todo Perform auto loading of game and player data
				return null;
			}
		}

		public void DoLoginFacebookOAuth (string oauth_token, Roar.Callback cb)
		{
			if (oauth_token == "") {
				logger.DebugLog ("[roar] -- Must specify oauth_token for facebook login");
				return;
			}

			Hashtable args = new Hashtable ();
			args ["oauth_token"] = oauth_token;

			facebook.login_facebook_oauth (args, new LoginFacebookOAuthCallback (cb, this));
		}
		
		public void DoLoginFacebookSignedReq (string signedReq, Roar.Callback cb)
		{
			if (signedReq == "") {
				logger.DebugLog ("[roar] -- Must specify signedReq for facebook login");
				return;
			}

			Hashtable args = new Hashtable ();
			args ["signed_request"] = signedReq;

			facebook.login_signed (args, new LoginCallback (cb, this));
		}

        public void DoFetchFacebookOAuthToken (string code, Roar.Callback cb)
		{
			Hashtable args = new Hashtable ();
			args ["code"] = code;
			facebook.fetch_facebook_oauth(args, new FetchFacebookOAuthTokenCallback (cb, this));
		}


        class FetchFacebookOAuthTokenCallback : SimpleRequestCallback<IXMLNode>
        {
            protected Facebook facebook;

            public FetchFacebookOAuthTokenCallback(Roar.Callback in_cb, Facebook in_facebook)
                : base(in_cb)
            {
                facebook = in_facebook;
            }

            public override void OnFailure(CallbackInfo<IXMLNode> info)
            {
                Debug.Log("OAuth Fetch Failed " + info.msg);
            }

            public override object OnSuccess(CallbackInfo<IXMLNode> info)
            {
                Debug.Log("oauth successful "+info.data+info.msg);
                //RoarManager.OnLoggedIn();
                // @todo Perform auto loading of game and player data
                return null;
            }
        }


		class LoginFacebookOAuthCallback : SimpleRequestCallback<IXMLNode>
		{
			protected Facebook facebook;

			public LoginFacebookOAuthCallback (Roar.Callback in_cb, Facebook in_facebook) : base( in_cb )
			{
				facebook = in_facebook;
			}

			public override void OnFailure (CallbackInfo<IXMLNode> info)
			{
				RoarManager.OnLogInFailed (info.msg);
			}

			public override object OnSuccess (CallbackInfo<IXMLNode> info)
			{
				RoarManager.OnLoggedIn ();
				// @todo Perform auto loading of game and player data
				return null;
			}
		}


		
		
		public void DoCreateFacebookSignedReq (string name, string signedReq, Roar.Callback cb)
		{
			if (name == "" || signedReq == "") {
				logger.DebugLog ("[roar] -- Must specify username and signed req for creation");
				return;
			}
			
			Hashtable args = new Hashtable ();
			args ["name"] = name;
			args ["signed_request"] = signedReq;

			facebook.create_signed (args, new FacebookCreateCallback (cb, this));
		}
		

		//TODO: not sure this belongs in this class!
		public void CacheFromInventory (Roar.Callback cb=null)
		{
			if (! dataStore.inventory.HasDataFromServer)
				return;

			// Build sanitised ARRAY of ikeys from Inventory.list()
			var l = dataStore.inventory.List ();
			var ikeyList = new ArrayList ();
			for (int i=0; i<l.Count; i++)
				ikeyList.Add ((l [i] as Hashtable) ["ikey"]);

			var toCache = dataStore.cache.ItemsNotInCache (ikeyList) as ArrayList;

			// Build sanitised Hashtable of ikeys from Inventory
			// No need to call server as information is already present
			Hashtable cacheData = new Hashtable ();
			for (int i=0; i<toCache.Count; i++) {
				for (int k=0; k<l.Count; k++) {
					// If the Inventory ikey matches a value in the
					// list of items to cache, add it to our `cacheData` obj
					if ((l [k] as Hashtable) ["ikey"] == toCache [i])
						cacheData [toCache [i]] = l [k];
				}
			}

			// Enable update of cache if it hasn't been initialised yet
			dataStore.cache.HasDataFromServer = true;
			dataStore.cache.Set (cacheData);
		}

	}

}
