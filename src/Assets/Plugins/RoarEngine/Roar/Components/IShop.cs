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
	   * - fires the RoarManager#shopReadyEvent
	   * - sets #hasDataFromServer to true
	   *
	   * On failure:
	   * - invokes callback with error code and error message
	   *
	   * @param callback the callback function to be passed this function's result.
	   *
	   * @returns nothing - use a callback and/or subscribe to RoarManager events for results of non-blocking calls.
	   */
		void Fetch (Roar.Callback callback);

		/**
	   * Check whether shop information has been obtained from the server.
	   *
	   * @returns true if #fetch has completed execution.
	   */
		bool HasDataFromServer { get; }

		/**
	   * Checks on the server that purcahse requirements are met and, if they are, purchases an item, adding it to the user's
	   * inventory and deducting the cost from the appropriate user model statistic.
	   *
	   * On success:
	   * - invokes callback with parameter *Hastable data* containing the "id" of the purchased item and the "ikey" of the shop
	   * - fires the RoarManager#goodBoughtEvent
	   * - fires the RoarManager#roarServerInventoryChangedEvent
	   *
	   * On failure:
	   * - invokes callback with error code and error message
	   *
	   * @param shop_ikey the shop_ikey of the item to purchase.
	   * @param callback the callback function to be passed this function's result.
	   *
	   * @returns nothing - use a callback and/or subscribe to RoarManager events for results of non-blocking calls.
	   */
		void Buy (string shop_ikey, Roar.Callback callback);

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
		ArrayList List ();

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
		ArrayList List (Roar.Callback callback);

		/**
	   * Returns the shop item object for a given key.
	   *
	   * @param ikey the key that uniquely identifies a shop item.
	   *
	   * @returns the shop item Hashtable associated with the *ikey*
	   *          or null if the shop item does not exist in the data store.
	   */
		object GetShopItem (string ikey);

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
		object GetShopItem (string ikey, Roar.Callback callback);
	}

}
