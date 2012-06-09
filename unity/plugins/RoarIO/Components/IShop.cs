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

namespace Roar.Components
{

/**
 * \brief IShop is an interface for buying virtual items.
 *
 * A shop is composed of a list of shop items, a shop item has 3 core properties:
 * - costs
 * - requirements
 * - modifiers
 *
 * It also has an ikey, label, description and optional tags list.
 *
 * You can get the list of all shop items via the #list function,
 * and can purchase the shop item using the #buy function.
 *
 * A shop item can be accessed via its Hashtable interface:
 *
 * \code
 * Hashtable item = Shop.getShopItem("an_item_key");
 *
 * string itemKey = item["ikey"];
 * string itemLabel = item["label"];
 * string itemDescription = item["description"];
 *
 * Hashtable cost = item["costs"][0];
 * string costType = cost["type"];
 * string costKey = cost["ikey"];
 * string costValue = cost["value"];
 * string costStatus = cost["ok"];
 * string costReason = cost["reason"];
 *
 * Hashtable modifier = item["modifiers"][0];
 * string modifierKey = modifier["ikey"]
 *
 * Hashtable tag = item["tags"][0];
 * string tagValue = tag["value"];
 *
 * \endcode
 *
 * @note The #buy, #list and #getShopItem functions can only be called after the
 * shop items have been fetched from the server via a call to #fetch.
 * @note once #fetch has received and processed shop items from the server, the #hasDataFromServer
 * property will return true and calls to #buy, #list and #getShopItem will be functional.
 */
public interface IShop
{
  /**
   * Fetch shop information from the server.
   *
   * On success:
   * - invokes callback with parameter *Hastable data* containing the data for the shop.
   * - fires the RoarIOManager#shopReadyEvent
   * - sets #hasDataFromServer to true
   *
   * On failure:
   * - invokes callback with error code and error message
   *
   * @param callback the callback function to be passed this function's result.
   *
   * @returns nothing - use a callback and/or subscribe to RoarIOManager events for results of non-blocking calls.
   */
  void fetch( Roar.Callback callback );

  /**
   * Check whether shop information has been obtained from the server.
   *
   * @returns true if #fetch has completed execution.
   */
  bool hasDataFromServer { get; }

  /**
   * Checks on the server that purcahse requirements are met and, if they are, purchases an item, adding it to the user's
   * inventory and deducting the cost from the appropriate user model statistic.
   *
   * On success:
   * - invokes callback with parameter *Hastable data* containing the "id" of the purchased item and the "ikey" of the shop
   * - fires the RoarIOManager#goodBoughtEvent
   * - fires the RoarIOManager#roarServerInventoryChangedEvent
   *
   * On failure:
   * - invokes callback with error code and error message
   *
   * @param shop_ikey the shop_ikey of the item to purchase.
   * @param callback the callback function to be passed this function's result.
   *
   * @returns nothing - use a callback and/or subscribe to RoarIOManager events for results of non-blocking calls.
   */
  void buy( string shop_ikey, Roar.Callback callback );

  /**
   * Get a list of all the available items in the shop.
   *
   * @returns A list of Hashtables for each shop item.
   *
   * @note This does _not_ make a server call. It requires the shop data to
   *       have already been fetched via a call to #fetch. If this function
   *       is called prior to the successful completion of a #fetch call,
   *       it will return an empty array.
   */
  ArrayList list();

  /**
   * Get a list of all the available items in the shop.
   *
   * On success:
   * - invokes callback with parameter *data* containing the list of Hashtable shop items
   *
   * On failure:
   * - returns an empty list
   *
   * @param callback the callback function to be passed this function's result.
   *
   * @returns A list of Hashtables for each shop item.
   *
   * @note This does _not_ make a server call. It requires the shop data to
   *       have already been fetched via a call to #fetch. If this function
   *       is called prior to the successful completion of a #fetch call,
   *       it will return an empty array.
   */
  ArrayList list(Roar.Callback callback); 

  /**
   * Returns the shop item object for a given key.
   *
   * @param ikey the key that uniquely identifies a shop item.
   *
   * @returns the shop item Hashtable associated with the *ikey*
   *          or null if the shop item does not exist in the data store.
   */
  object getShopItem( string ikey );

  /**
   * Returns the shop item object for a given key.
   *
   * On success:
   * - invokes callback with parameter *data* containing the Hashtable shop item
   *
   * On failure:
   * - invokes callback with parameter *data* equaling null if shop item does not exist.
   *
   *
   * @param ikey the key that uniquely identifies a shop item.
   * @param callback the callback function to be passed this function's result.
   *
   * @returns the shop item Hashtable associated with the *ikey*
   *          or null if the shop item does not exist in the data store.
   */
  object getShopItem( string ikey, Roar.Callback callback );
}

}
