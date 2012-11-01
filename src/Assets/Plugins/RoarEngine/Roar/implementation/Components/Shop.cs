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
using Roar.Components;
using UnityEngine;

namespace Roar.implementation.Components
{
	public class Shop : IShop
	{
		protected IWebAPI.IShopActions shopActions;
		protected DataStore dataStore;
		protected ILogger logger;

		public Shop (IWebAPI.IShopActions shopActions, DataStore dataStore, ILogger logger)
		{
			this.shopActions = shopActions;
			this.dataStore = dataStore;
			this.logger = logger;

			RoarManager.shopReadyEvent += () => CacheFromShop ();
		}

		public void Fetch (Roar.Callback callback)
		{
			dataStore.shop.Fetch (callback);
		}

		public bool HasDataFromServer { get { return dataStore.shop.HasDataFromServer; } }

		public void Buy (string shop_ikey, Roar.Callback callback)
		{
			ShopBuy (shop_ikey, callback);
		}

		public ArrayList List ()
		{
			return List (null);
		}

		public ArrayList List (Roar.Callback callback)
		{
			if (callback != null)
				callback (new Roar.CallbackInfo<object> (dataStore.shop.List ()));
			return dataStore.shop.List ();
		}

		// Returns the *object* associated with attribute `key`
		public object GetShopItem (string ikey)
		{
			return GetShopItem (ikey, null);
		}

		public object GetShopItem (string ikey, Roar.Callback callback)
		{
			if (callback != null)
				callback (new Roar.CallbackInfo<object> (dataStore.shop.Get (ikey)));
			return dataStore.shop.Get (ikey); 
		}
		
		public void ShopBuy (string shop_ikey, Roar.Callback cb)
		{
			var shop_item = dataStore.shop.Get (shop_ikey);

			// Make the call if the item is in the shop
			if (shop_item == null) {
				logger.DebugLog ("[roar] -- Cannot find to purchase: " + shop_ikey);
				return;
			}
			logger.DebugLog ("trying to buy me a : " + Roar.Json.ObjectToJSON (shop_item));
			string ikey = shop_item ["ikey"] as string;
		
			Hashtable args = new Hashtable ();
			args ["shop_item_ikey"] = shop_ikey;

			shopActions.buy (args, new ShopBuyCallback (cb, this, ikey));
		}
  
		protected class ShopBuyCallback : SimpleRequestCallback<IXMLNode>
		{
			//Shop shop;
			string ikey;
    
			public ShopBuyCallback (Roar.Callback in_cb, Shop in_shop, string in_ikey) : base(in_cb)
			{
				//shop = in_shop;
				ikey = in_ikey;
			}
    
			public override object OnSuccess (CallbackInfo<IXMLNode> info)
			{
				IXMLNode result = info.data.GetNode ("roar>0>shop>0>buy>0");	

				// Obtain the server id for the purchased item
				// NOTE: Assumes ONLY ONE item per "shopitem" entity

				IXMLNode cost = result.GetNode ("costs>0>cost>0");
				IXMLNode item = result.GetNode ("modifiers>0>modifier>0");


				string id = item.GetAttribute ("item_id");	

				RoarManager.OnGoodBought (
					new RoarManager.PurchaseInfo (
						cost.GetAttribute ("ikey"), //currency_name
						int.Parse (cost.GetAttribute ("value")), // item_price
						ikey, // iitem_id
						1                                      //item_qty
				));

				Hashtable data = new Hashtable ();
				data ["id"] = id;
				data ["ikey"] = ikey;
      
				return data;
			}
		}

		// Builds a list of items to fetch from Server by comparing
		// what's in the Shop list and what's currently in the cache
		public bool CacheFromShop ()
		{
			if (dataStore.shop.HasDataFromServer) {
				// Build sanitised ARRAY of ikeys from Shop.list()
				var l = dataStore.shop.List () as ArrayList;
				var ikeyList = new ArrayList ();

				foreach (Hashtable v in l) {
					ikeyList.Add (v ["ikey"]);
				} 
      
				return dataStore.cache.AddToCache (ikeyList);
			} else
				return false;
		}
		

		
	}
	
}