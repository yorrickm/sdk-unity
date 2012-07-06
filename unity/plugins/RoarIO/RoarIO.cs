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
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Logger : Roar.ILogger
{
  public void DebugLog( string k )
  {
    Debug.Log (k);
  }
}

/**
 * Implementation of the IRoarIO interface.
 * This is the class you need to drag onto your unity empty to start using the
 * RoarIO framework. However once that is done you should only use the object
 * through the IRoarIO interface. That is your unity scripts should look
 * something like this
 *
 *    var roar_:IRoarIO
 *    function Awake()
 *    {
 *      roar_ = GetComponent(RoarIO) as IRoarIO
 *    }
 *
 * Further documentation about how you can use the RoarIO object
 * can be found in the IRoarIO class.
 */

public class RoarIO : MonoBehaviour, IRoarIO, IUnityObject
{

  //These are purely to enable the values to show up in the unity UI.
  public bool debug = true;
  public bool appstoreSandbox = true;
  public string gameKey="";

  public Roar.IConfig Config { get { return Config_; } }
  protected Roar.IConfig Config_;

  public IWebAPI WebAPI { get { return WebAPI_; } }
  protected IWebAPI WebAPI_;

  public Roar.Components.IUser User { get { return User_; } }
  protected Roar.Components.IUser User_;

  public Roar.Components.IProperties Properties { get { return Properties_; } }
  protected Roar.Components.IProperties Properties_;

  public Roar.Components.IData Data { get { return Data_; } }
  protected Roar.Components.IData Data_;

  public Roar.Components.IInventory Inventory { get { return Inventory_; } }
  protected Roar.Components.IInventory Inventory_ = null;

  public Roar.Components.IShop Shop { get { return Shop_; } }
  protected Roar.Components.IShop Shop_;

  public Roar.Components.IActions Actions { get { return Actions_; } }
  protected Roar.Components.IActions Actions_;
 
  public Roar.Components.IAchievements Achievements { get { return Achievements_; } }
  protected Roar.Components.IAchievements Achievements_;
  
  public Roar.Components.IGifts Gifts { get { return Gifts_; } }
  protected Roar.Components.IGifts Gifts_;
  
  public Roar.Components.IInAppPurchase Appstore { get{ return Appstore_;} }
  protected Roar.implementation.Components.InAppPurchase Appstore_;
  
  public Roar.Adapters.IUrbanAirship UrbanAirship { get{ return UrbanAirship_;} }
  protected Roar.implementation.Adapters.UrbanAirship UrbanAirship_;

  public string AuthToken { get { return Config_.auth_token; } }

  /**
   * Called by unity when everything is ready to go.
   * We use this rather than the constructor as its what unity suggests.
   */
  public void Awake()
  {
    Config_ = new Roar.implementation.Config();
    
     // Apply public settings 
    Config.game = gameKey;
    Config.isDebug = debug;
    
    Logger logger = new Logger();

    RequestSender api = new RequestSender(Config_,this,logger);
    Roar.implementation.DataStore data_store = new Roar.implementation.DataStore(api, logger);
    WebAPI_ = new global::WebAPI(api);
    User_ = new Roar.implementation.Components.User(WebAPI_.user,data_store, logger);
    Properties_ = new Roar.implementation.Components.Properties( data_store );
    Inventory_ = new Roar.implementation.Components.Inventory( WebAPI_.items, data_store, logger);
    Data_ = new Roar.implementation.Components.Data( WebAPI_.user, data_store, logger);
    Shop_ = new Roar.implementation.Components.Shop( WebAPI_.shop, data_store, logger );
    Actions_ = new Roar.implementation.Components.Actions( WebAPI_.tasks, data_store );

    Appstore_ = new Roar.implementation.Components.InAppPurchase( WebAPI_.appstore, "Roar", logger, appstoreSandbox );
    
    UrbanAirship_ = new Roar.implementation.Adapters.UrbanAirship(WebAPI_);

    DontDestroyOnLoad( this );

  }

  public void Start()
  {
    if(UrbanAirship_!=null) UrbanAirship_.OnStart();
  }

  public void OnUpdate()
  {
    if(UrbanAirship_!=null) UrbanAirship_.OnUpdate();
  }

  string _version="1.0.0";

  public string version( Roar.Callback callback = null )
  {
    if(callback!=null) callback( new Roar.CallbackInfo<object>( _version ) );
    return _version;
  }

  public void login( string username, string password, Roar.Callback callback=null )
  {
    User.doLogin(username,password,callback);
  }

  public void login_facebook_oauth( string oauth_token, Roar.Callback callback=null )
  {
    User.doLoginFacebookOAuth(oauth_token,callback);
  }

  public void logout( Roar.Callback callback=null )
  {
    User.doLogout(callback);
  }

  public void create( string username, string password, Roar.Callback callback=null )
  {
    User.doCreate(username,password,callback);
  }


  public string whoami( Roar.Callback callback=null )
  {
    if (callback!=null) callback( new Roar.CallbackInfo<object>(Properties.getValue( "name" )) );
    return Properties.getValue( "name" );
  }

  public bool isDebug{ get { return Config.isDebug; } }
  
  public void doCoroutine( IEnumerator method )
  {
     this.StartCoroutine(method);
  }
 
  
  #region EXTERNAL CALLBACKS
  void OnAppstoreProductData(string productDataXml) {
    Appstore.OnProductData(productDataXml);
  }
  void OnAppstoreRequestProductDataInvalidProductId(string invalidProductId) {
    Appstore.OnInvalidProductId(invalidProductId);
  }
  void OnAppstoreProductPurchaseComplete(string purchaseXml) {
    Appstore.OnPurchaseComplete(purchaseXml);
  }
  void OnAppstoreProductPurchaseCancelled(string productIdentifier) {
    Appstore.OnPurchaseCancelled(productIdentifier);
  }
  void OnAppstoreProductPurchaseFailed(string errorXml) {
    Appstore.OnPurchaseFailed(errorXml);
  }
  #endregion
}
