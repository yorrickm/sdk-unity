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
		protected ILogger logger;
		protected DataStore dataStore;
		protected IWebAPI.IAppstoreActions actions;
		protected bool isSandbox;
		protected bool hasDataFromAppstore;
		protected bool isServerCalling;
		protected IDictionary<string, Hashtable> productsMap;
		protected IList<Hashtable> productsList;
		protected Roar.Callback purchaseCallback;

		public InAppPurchase (IWebAPI.IAppstoreActions actions, string nativeCallbackGameObject, ILogger logger, bool isSandbox)
		{
			this.logger = logger;
			this.actions = actions;
			this.isSandbox = isSandbox;
			hasDataFromAppstore = false;
			isServerCalling = false;
			productsMap = new Dictionary<string, Hashtable> ();
			productsList = new List<Hashtable> ();
    		#if UNITY_IOS && !UNITY_EDITOR
      		_StoreKitInit(nativeCallbackGameObject);
    		#else
			logger.DebugLog (string.Format ("Can't call _StoreKitInit({0}) from Unity Editor", nativeCallbackGameObject));
    		#endif
		}

		/**
	     * Fetch has two stages:
	     * 1. Retrieve the appstore product ids from roar.
	     * 2. Use the product ids to retrieve the product details from the appstore.
	     **/
		public void Fetch (Roar.Callback callback)
		{
			if (isServerCalling) {
				return;
			}
			isServerCalling = false;
			productsMap.Clear ();
			productsList.Clear ();
			Hashtable args = new Hashtable ();
			actions.shop_list (args, new AppstoreListCallback (callback, this));
		}

		class AppstoreListCallback : SimpleRequestCallback<IXMLNode>
		{
			InAppPurchase appstore;

			public override void Prologue ()
			{
				// Reset this function call
				appstore.isServerCalling = false;
			}

			public AppstoreListCallback (Roar.Callback in_cb, InAppPurchase in_appstore) : base(in_cb)
			{
				appstore = in_appstore;
			}

			public override object OnSuccess (CallbackInfo<IXMLNode> info)
			{
				appstore.logger.DebugLog (string.Format ("onAppstoreList.onSuccess() called with: {0}", info.data.DebugAsString ()));
				ArrayList productIdentifiersList = appstore.GetProductIdentifiers (info.data);
				string combinedProductIdentifiers = string.Join (",", (string[])productIdentifiersList.ToArray (typeof(string)));
        		#if UNITY_IOS && !UNITY_EDITOR
        		_StoreKitRequestProductData(combinedProductIdentifiers);
        		#else
				appstore.logger.DebugLog (string.Format ("Can't call _StoreKitRequestProductData({0}) from Unity Editor", combinedProductIdentifiers));
        		#endif
				return productIdentifiersList;
			}
		}

		public IList<Hashtable> List ()
		{
			return  productsList;
		}

		public Hashtable GetShopItem (string productIdentifier)
		{
			return productsMap [productIdentifier];
		}

		protected ArrayList GetProductIdentifiers (IXMLNode data)
		{
			ArrayList productIdentifiers = new ArrayList ();
			// extract the product identifiers from the xml
			string path = "roar>0>appstore>0>shop_list>0>shopitem";
			List<IXMLNode> products = data.GetNodeList (path);
			if (products == null) {
				logger.DebugLog (string.Format ("data.GetNodeList('{0}') return null", path));
				return productIdentifiers;
			}
			foreach (IXMLNode product in products) {
				string pid = product.GetAttribute ("product_identifier");
				if (!string.IsNullOrEmpty (pid)) {
					productIdentifiers.Add (pid);
				}
			}
			return productIdentifiers;
		}

		public bool HasDataFromServer { get { return hasDataFromAppstore; } }

		protected void ValidateReceipt (string receiptId, Roar.Callback callback)
		{

			Hashtable args = new Hashtable ();
			args ["receipt"] = receiptId;
			args ["sandbox"] = Convert.ToString (isSandbox);

			actions.buy (args, new OnReceiptValidation (callback, this, receiptId));
		}

		class OnReceiptValidation : SimpleRequestCallback<IXMLNode>
		{
			InAppPurchase appstore;
			string receiptId;

			public OnReceiptValidation (Roar.Callback in_cb, InAppPurchase in_appstore, string in_receiptId) : base(in_cb)
			{
				appstore = in_appstore;
				receiptId = in_receiptId;
			}

			public override object OnSuccess (CallbackInfo<IXMLNode> info)
			{
				appstore.logger.DebugLog (string.Format ("onReceiptValidation() called with: {0}", info.data.DebugAsString ()));
				return receiptId;
			}
		}

		public void Purchase (string productId, Roar.Callback cb)
		{
			purchaseCallback = cb;
    		#if UNITY_IOS && !UNITY_EDITOR
      		_StoreKitPurchase(productId);
    		#else
			logger.DebugLog (string.Format ("Can't call _StoreKitPurchase({0}) from Unity Editor", productId));
   			#endif
		}

		public void Purchase (string productId, int quantity, Roar.Callback cb)
		{
			purchaseCallback = cb;
    		#if UNITY_IOS && !UNITY_EDITOR
      		_StoreKitPurchaseQuantity(productId, quantity);
    		#else
			logger.DebugLog (string.Format ("Can't call _StoreKitPurchase({0}) from Unity Editor", productId));
    		#endif
		}

		public bool PurchasesEnabled ()
		{
    		#if UNITY_IOS && !UNITY_EDITOR
      		return _StoreKitPurchasesEnabled();
    		#else
			logger.DebugLog (string.Format ("Can't call _StoreKitPurchasesEnabled() from Unity Editor"));
			return false;
    		#endif
		}

		// Store Kit wrapper to roar Unity client communication methods

		public void OnProductData (string productDataXml)
		{

			hasDataFromAppstore = true;
			productsList.Clear ();
			productsMap.Clear ();
			logger.DebugLog (string.Format ("OnProductData() called with: {0}", productDataXml));
			IXMLNode root = IXMLNodeFactory.instance.Create (productDataXml);
			IXMLNode appstoreNode = root.GetFirstChild ("appstore");
			IEnumerable<IXMLNode> children = appstoreNode.Children;

			if (children != null) {
				foreach (IXMLNode shopItemXml in children) {
					string pid = shopItemXml.GetAttribute ("product_identifier");
					Hashtable shopItemHashtable = new Hashtable ();
					foreach (KeyValuePair<string, string> attribute in shopItemXml.Attributes) {
						logger.DebugLog (string.Format ("Adding product {0} property {1}:{2}", pid, attribute.Key, attribute.Value));
						shopItemHashtable [attribute.Key] = attribute.Value;
					}
					logger.DebugLog (string.Format ("Adding {0} to productsList_:", pid));
					logger.DebugLog (Roar.Json.HashToJSON (shopItemHashtable));
					productsList.Add (shopItemHashtable);
					productsMap.Add (pid, shopItemHashtable);
				}
			} else {
				logger.DebugLog ("No products passed to OnProductData()");
			}
		}

		public void OnInvalidProductId (string invalidProductId)
		{
			logger.DebugLog (string.Format ("OnInvalidProductId() called with: {0}", invalidProductId));
		}

		public void OnPurchaseComplete (string purchaseXml)
		{
			logger.DebugLog (string.Format ("OnPurchaseComplete() called with: {0}", purchaseXml));
			IXMLNode root = IXMLNodeFactory.instance.Create (purchaseXml);
			IXMLNode purchaseNode = root.GetFirstChild ("shop_item_purchase_success");
			string transactionIdentifier = purchaseNode.GetAttribute ("transaction_identifier");
			ValidateReceipt (transactionIdentifier, purchaseCallback);
		}

		public void OnPurchaseCancelled (string productIdentifier)
		{
			logger.DebugLog (string.Format ("OnPurchaseCancelled() called with: {0}", productIdentifier));
			if (purchaseCallback != null) {
				purchaseCallback (new CallbackInfo<string> (productIdentifier, IWebAPI.DISALLOWED, "Purchase cancelled by user"));
			}
		}

		public void OnPurchaseFailed (string errorXml)
		{
			logger.DebugLog (string.Format ("OnPurchaseFailed() called with: {0}", errorXml));
			if (purchaseCallback != null) {
				purchaseCallback (new CallbackInfo<string> (errorXml, IWebAPI.UNKNOWN_ERR, "Purchase failed"));
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
