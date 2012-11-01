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
