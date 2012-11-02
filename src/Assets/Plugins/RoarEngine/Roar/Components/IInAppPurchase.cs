using System.Collections;
using System.Collections.Generic;

namespace Roar.Components
{
	/**
   * \brief IInAppPurchase is an interface for buying virtual items via the Apple AppStore.
   *
   * @note The #list, #Purchase and #getShopItem functions can only be called after the
   * shop items have been fetched from the server via a call to #fetch.
   * @note once #fetch has received and processed shop items from the server, the #hasDataFromServer
   * property will return true and calls to #buy, #list and #getShopItem will be functional.
   */
	public interface IInAppPurchase
	{
		/**
	     * Fetch appstore product keys from the roar server.
	     * followed by product details from the appstore.
	     **/
		void Fetch (Roar.Callback callback);

		/**
	     * Check whether appstore product keys have been obtained from the roar server
	     * AND product details have been retrieved from the appstore.
	     *
	     * @returns true if #fetch has completed execution.
	     **/
		bool HasDataFromServer { get; }

		IList<Hashtable> List ();

		Hashtable GetShopItem (string productIdentifier);

		void Purchase (string productId, Roar.Callback cb);

		void Purchase (string productId, int quantity, Roar.Callback cb);

		bool PurchasesEnabled ();

		void OnProductData (string productDataXml);

		void OnInvalidProductId (string invalidProductId);

		void OnPurchaseComplete (string purchaseXml);

		void OnPurchaseCancelled (string productIdentifier);

		void OnPurchaseFailed (string errorXml);
	}
}
