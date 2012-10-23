/*
Copyright (c) 2012, Run With Robots
All rights reserved.

Redistribution and use in source and binary forms, with or without
modification, are permitted provided that the following conditions are met:
    * Redistributions of source code must retain the above copyright
      notice, this list of conditions and the following disclaimer.
    * Redistributions in binary form must reproduce the above copyright
      notice, this list of conditions and the following disclaimer in the
      documentation and/or other materials provided with the distribution.
    * Neither the name of the roar.io library nor the
      names of its contributors may be used to endorse or promote products
      derived from this software without specific prior written permission.

THIS SOFTWARE IS PROVIDED BY RUN WITH ROBOTS ''AS IS'' AND ANY
EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
DISCLAIMED. IN NO EVENT SHALL MICHAEL ANDERSON BE LIABLE FOR ANY
DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
(INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
(INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*/
using System.Collections;
using UnityEngine;

using Roar.Components;

namespace Roar.implementation.Components
{
	public class User : IUser
	{
		protected DataStore dataStore;
		IWebAPI.IUserActions userActions;
		ILogger logger;

		public User (IWebAPI.IUserActions userActions, DataStore dataStore, ILogger logger)
		{
			this.userActions = userActions;
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


		// ---- Access Methods ----
		// ------------------------

		public void DoLogin (string name, string hash, Roar.Callback cb)
		{
			if (name == "" || hash == "") {
				logger.DebugLog ("[roar] -- Must specify username and password for login");
				return;
			}

			Hashtable args = new Hashtable ();
			args ["name"] = name;
			args ["hash"] = hash;
			userActions.login (args, new LoginCallback (cb, this));
		}
  
		protected class LoginCallback : SimpleRequestCallback<IXMLNode>
		{
			protected User user;

			public LoginCallback (Roar.Callback in_cb, User in_user) : base(in_cb)
			{
				user = in_user;
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

			userActions.login_facebook_oauth (args, new LoginFacebookOAuthCallback (cb, this));
		}
		class LoginFacebookOAuthCallback : SimpleRequestCallback<IXMLNode>
		{
			protected User user;

			public LoginFacebookOAuthCallback (Roar.Callback in_cb, User in_user) : base( in_cb )
			{
				user = in_user;
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


		public void DoLogout (Roar.Callback cb)
		{
			userActions.logout (null, new LogoutCallback (cb, this));
		}
  
		protected class LogoutCallback : SimpleRequestCallback<IXMLNode>
		{
			protected User user;

			public LogoutCallback (Roar.Callback in_cb, User in_user) : base(in_cb)
			{
				user = in_user;
				cb = in_cb;
			}

			public override object OnSuccess (CallbackInfo<IXMLNode> info)
			{
				RoarManager.OnLoggedOut ();
				return null;
			}

		};


		public void DoCreate (string name, string hash, Roar.Callback cb)
		{
			if (name == "" || hash == "") {
				logger.DebugLog ("[roar] -- Must specify username and password for login");
				return;
			}
			Hashtable args = new Hashtable ();
			args ["name"] = name;
			args ["hash"] = hash;

			userActions.create (args, new CreateCallback (cb, this));
		}
		protected class CreateCallback : SimpleRequestCallback<IXMLNode>
		{
			protected User user;

			public CreateCallback (Roar.Callback in_cb, User in_user) : base(in_cb)
			{
				user = in_user;
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
