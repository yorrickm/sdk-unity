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
  protected IWebAPI.IShopActions shop_actions_;
  protected DataStore data_store_;
  protected ILogger logger_;

  public Shop( IWebAPI.IShopActions shop_actions, DataStore data_store, ILogger logger )
  {
		shop_actions_ = shop_actions;
		data_store_ = data_store;
		logger_ = logger;

		RoarManager.shopReadyEvent += () => cacheFromShop();
  }

  public void fetch(Roar.Callback callback) { data_store_.Shop_.fetch(callback); }
  public bool hasDataFromServer { get { return data_store_.Shop_.hasDataFromServer; } }

  public void buy( string shop_ikey, Roar.Callback callback ) { shopBuy( shop_ikey, callback ); }

  public ArrayList list() { return list(null); }
  public ArrayList list(Roar.Callback callback) 
  {
    if (callback!=null) callback( new Roar.CallbackInfo<object>( data_store_.Shop_.list() ) );
    return data_store_.Shop_.list();
  }

  // Returns the *object* associated with attribute `key`
  public object getShopItem( string ikey ) { return getShopItem(ikey,null); }
  public object getShopItem( string ikey, Roar.Callback callback )
  {
    if (callback!=null) callback( new Roar.CallbackInfo<object>( data_store_.Shop_._get(ikey) ) );
    return data_store_.Shop_._get(ikey); 
  }
		
  public void shopBuy( string shop_ikey, Roar.Callback cb )
  {
    var shop_item = data_store_.Shop_._get( shop_ikey);

    // Make the call if the item is in the shop
    if (shop_item==null)
    {
      logger_.DebugLog("[roar] -- Cannot find to purchase: "+ shop_ikey);
      return;
    }
    logger_.DebugLog ("trying to buy me a : "+Roar.Json.ObjectToJSON(shop_item) );
    string ikey = shop_item["ikey"] as string;
		
	Hashtable args = new Hashtable();
	args["shop_item_ikey"] = shop_ikey;

    shop_actions_.buy( args, new OnShopBuy(cb,this,ikey) );
  }
  
  protected class OnShopBuy : SimpleRequestCallback<IXMLNode>
  {
    Shop shop;
    string ikey;
    
    public OnShopBuy( Roar.Callback in_cb, Shop in_shop, string in_ikey) : base(in_cb)
    {
      shop = in_shop;
      ikey = in_ikey;
    }
    
    public override object onSuccess( CallbackInfo<IXMLNode> info )
    {
      IXMLNode result = info.data.GetNode("roar>0>shop>0>buy>0");	

      // Obtain the server id for the purchased item
      // NOTE: Assumes ONLY ONE item per "shopitem" entity

      IXMLNode cost = result.GetNode( "costs>0>cost>0" );
      IXMLNode item = result.GetNode( "modifiers>0>modifier>0" );


      string id = item.GetAttribute("item_id");	

      RoarManager.OnGoodBought(
        new RoarManager.PurchaseInfo(
          cost.GetAttribute("ikey"),             //currency_name
          int.Parse(cost.GetAttribute("value")), // item_price
          ikey,                                  // iitem_id
          1                                      //item_qty
          ));

      Hashtable data = new Hashtable();
      data["id"]=id;
      data["ikey"]=ikey;
      
      return data;
    }
  }

  // Builds a list of items to fetch from Server by comparing
  // what's in the Shop list and what's currently in the cache
  public bool cacheFromShop()
  {
    if (data_store_.Shop_.hasDataFromServer)
    {
      // Build sanitised ARRAY of ikeys from Shop.list()
      var l = data_store_.Shop_.list() as ArrayList;
      var ikeyList = new ArrayList();

      foreach( Hashtable v in l)
      {
         ikeyList.Add( v["ikey"] );
      } 
      
      return data_store_.Cache_.addToCache( ikeyList );
    }
    else return false;
  }
		

		
}
	
}