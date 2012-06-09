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
using System;
using System.Collections;
using System.Collections.Generic;
using Roar.Components;
using UnityEngine;
using System.Runtime.InteropServices;


namespace Roar.implementation.Components
{
  public class InAppPurchase : IInAppPurchase
  {
    protected ILogger logger_;
    protected DataStore data_store_;
    protected IWebAPI.IAppstoreActions actions_;
    protected bool sandbox_;
    protected bool hasDataFromAppstore_;
    protected bool isServerCalling_;
    protected IDictionary<string, Hashtable> productsMap_;
    protected IList<Hashtable> productsList_;
    protected Roar.Callback purchaseCallback;

    public InAppPurchase (IWebAPI.IAppstoreActions actions, string nativeCallbackGameObject, ILogger logger, bool sandbox)
    {
      logger_ = logger;
      actions_ = actions;
      sandbox_ = sandbox;
      hasDataFromAppstore_ = false;
      isServerCalling_ = false;
      productsMap_ = new Dictionary<string, Hashtable>();
      productsList_ = new List<Hashtable>();
    #if UNITY_IOS && !UNITY_EDITOR
      _StoreKitInit(nativeCallbackGameObject);
    #else
      logger_.DebugLog(string.Format("Can't call _StoreKitInit({0}) from Unity Editor", nativeCallbackGameObject));
    #endif
    }
    
    /**
     * Fetch has two stages:
     * 1. Retrieve the appstore product ids from roar.
     * 2. Use the product ids to retrieve the product details from the appstore.
     **/
    public void fetch(Roar.Callback callback) {
      if(isServerCalling_) {
        return;
      }
      isServerCalling_ = false;
	    productsMap_.Clear();
      productsList_.Clear();
      Hashtable args = new Hashtable();
      actions_.shop_list(args, new onAppstoreList(callback, this));
    }

    class onAppstoreList : SimpleRequestCallback<IXMLNode>
    {
      InAppPurchase appstore;

      public override void prologue()
      {
        // Reset this function call
        appstore.isServerCalling_ = false;
      }

      public onAppstoreList( Roar.Callback in_cb, InAppPurchase in_appstore) : base(in_cb)
      {
        appstore = in_appstore;
      }

      public override object onSuccess( CallbackInfo<IXMLNode> info )
      {
        appstore.logger_.DebugLog(string.Format("onAppstoreList.onSuccess() called with: {0}", info.data.DebugAsString()));
        ArrayList productIdentifiersList = appstore.getProductIdentifiers(info.data);
        string combinedProductIdentifiers = string.Join(",", (string[]) productIdentifiersList.ToArray( typeof( string ) ));
        #if UNITY_IOS && !UNITY_EDITOR
        _StoreKitRequestProductData(combinedProductIdentifiers);
        #else
        appstore.logger_.DebugLog(string.Format("Can't call _StoreKitRequestProductData({0}) from Unity Editor", combinedProductIdentifiers));
        #endif
        return productIdentifiersList;
      }
    }
    
    public IList<Hashtable> list() {
      return  productsList_;
    }
    
    public Hashtable getShopItem( string productIdentifier ) {
      return productsMap_[productIdentifier];
    }
    
    protected ArrayList getProductIdentifiers(IXMLNode data) {
      ArrayList productIdentifiers = new ArrayList();
      // extract the product identifiers from the xml
      string path = "roar>0>appstore>0>shop_list>0>shopitem";
      List<IXMLNode> products = data.GetNodeList(path);
      if(products == null) {
        logger_.DebugLog(string.Format("data.GetNodeList('{0}') return null", path));
        return productIdentifiers;
      }
      foreach(IXMLNode product in products) {
        string pid = product.GetAttribute("product_identifier");
        if(!string.IsNullOrEmpty(pid)) {
          productIdentifiers.Add(pid);
        }
      }
      return productIdentifiers;
    }

    public bool hasDataFromServer { get { return hasDataFromAppstore_; } }
    
    protected void ValidateReceipt(string receiptId, Roar.Callback callback) {
      
      Hashtable args = new Hashtable();
      args["receipt"] = receiptId;
      args["sandbox"] = Convert.ToString(sandbox_);
      
      actions_.buy(args, new OnReceiptValidation(callback, this, receiptId));
    }

    class OnReceiptValidation : SimpleRequestCallback<IXMLNode>
    {
      InAppPurchase appstore;
      string receiptId;

      public OnReceiptValidation( Roar.Callback in_cb, InAppPurchase in_appstore, string in_receiptId) : base(in_cb)
      {
        appstore = in_appstore;
        receiptId = in_receiptId;
      }

      public override object onSuccess( CallbackInfo<IXMLNode> info )
      {
        appstore.logger_.DebugLog(string.Format("onReceiptValidation() called with: {0}", info.data.DebugAsString()));
        return receiptId;
      }
    }
    
    public void Purchase(string productId, Roar.Callback cb) {
      purchaseCallback = cb;
    #if UNITY_IOS && !UNITY_EDITOR
      _StoreKitPurchase(productId);
    #else
      logger_.DebugLog(string.Format("Can't call _StoreKitPurchase({0}) from Unity Editor", productId));
    #endif
    }
    
    public void Purchase(string productId, int quantity, Roar.Callback cb) {
      purchaseCallback = cb;
    #if UNITY_IOS && !UNITY_EDITOR
      _StoreKitPurchaseQuantity(productId, quantity);
    #else
      logger_.DebugLog(string.Format("Can't call _StoreKitPurchase({0}) from Unity Editor", productId));
    #endif
    }

    public bool PurchasesEnabled() {
    #if UNITY_IOS && !UNITY_EDITOR
      return _StoreKitPurchasesEnabled();
    #else
      logger_.DebugLog(string.Format("Can't call _StoreKitPurchasesEnabled() from Unity Editor"));
      return false;
    #endif
    }

    // Store Kit wrapper to roar Unity client communication methods
    
    public void OnProductData(string productDataXml) {
	
      hasDataFromAppstore_ = true;
      productsList_.Clear();
      productsMap_.Clear();
      logger_.DebugLog(string.Format("OnProductData() called with: {0}", productDataXml));
      IXMLNode root = IXMLNodeFactory.instance.Create(productDataXml);
      IXMLNode appstoreNode = root.GetFirstChild("appstore");
      IEnumerable<IXMLNode> children = appstoreNode.Children;
			
      if(children != null) {
        foreach(IXMLNode shopItemXml in children) {
		  string pid = shopItemXml.GetAttribute("product_identifier");
          Hashtable shopItemHashtable = new Hashtable();
          foreach(KeyValuePair<string, string> attribute in shopItemXml.Attributes) {
			logger_.DebugLog(string.Format ("Adding product {0} property {1}:{2}", pid, attribute.Key, attribute.Value));
            shopItemHashtable[attribute.Key] = attribute.Value;
          }
		  logger_.DebugLog(string.Format ("Adding {0} to productsList_:", pid));
		  logger_.DebugLog(Roar.Json.HashToJSON(shopItemHashtable));
          productsList_.Add(shopItemHashtable);
          productsMap_.Add(pid, shopItemHashtable);
        }
      } else {
	    logger_.DebugLog("No products passed to OnProductData()");
	  }
    }
    
    public void OnInvalidProductId(string invalidProductId) {
      logger_.DebugLog(string.Format("OnInvalidProductId() called with: {0}", invalidProductId));
    }

    public void OnPurchaseComplete(string purchaseXml) {
      logger_.DebugLog(string.Format("OnPurchaseComplete() called with: {0}", purchaseXml));
      IXMLNode root = IXMLNodeFactory.instance.Create(purchaseXml);
      IXMLNode purchaseNode = root.GetFirstChild("shop_item_purchase_success");
      string transactionIdentifier = purchaseNode.GetAttribute("transaction_identifier");
      ValidateReceipt(transactionIdentifier, purchaseCallback);
    }
    
    public void OnPurchaseCancelled(string productIdentifier) {
      logger_.DebugLog(string.Format("OnPurchaseCancelled() called with: {0}", productIdentifier));
      if(purchaseCallback!=null) {
        purchaseCallback(new CallbackInfo<string>(productIdentifier, IWebAPI.DISALLOWED, "Purchase cancelled by user"));
      }
    }
    
    public void OnPurchaseFailed(string errorXml) {
      logger_.DebugLog(string.Format("OnPurchaseFailed() called with: {0}", errorXml));
      if(purchaseCallback!=null) {
        purchaseCallback(new CallbackInfo<string>(errorXml, IWebAPI.UNKNOWN_ERR, "Purchase failed"));
      }
    }
    
    // roar Unity client to Store Kit wrapper communication methods
    
    #region DllImports
    #if UNITY_IOS
        [DllImport ("__Internal", CharSet = CharSet.Auto)]
        static extern void _StoreKitInit(string unityGameObject);
        [DllImport ("__Internal", CharSet = CharSet.Auto)]
        static extern bool _StoreKitPurchasesEnabled();
        [DllImport ("__Internal", CharSet = CharSet.Auto)]
        static extern void _StoreKitRequestProductData(string productIdentifiers);
        [DllImport ("__Internal", CharSet = CharSet.Auto)]
        static extern void _StoreKitPurchase(string productIdentifier);
        [DllImport ("__Internal", CharSet = CharSet.Auto)]
        static extern void _StoreKitPurchaseQuantity(string productIdentifier, int quantity);
    #endif
    #endregion
  }
}
