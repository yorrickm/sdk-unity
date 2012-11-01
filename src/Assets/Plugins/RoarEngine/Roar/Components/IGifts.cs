using System.Collections;

namespace Roar.Components
{
	/**
   * \brief IGifts is an interface for listing all of the items belonging to a user that can be gifted to other users.
   *
   * You can get the list of all giftable items via the #list function or
   * a single giftable item via the #getGift function.
   *
   * A gift is represented in a Hashtable using the following keys:
   *
   * \code
   *
   *  - id: "3467"
   *  - type: "gift"
   *  - label: "a label"
   *  - requirements : list of requirements to send this give
   *  - costs : list of costs for this gift
   *  - on_accept : list of actions to execute on accept
   *  - on_give : list of actions to execute on give
   *  - tags
   *
   * \endcode
   *
   * @todo: are all mailable items gifts, or only those of type:'gift'?
   **/
	public interface IGifts
	{

		/**
	     * Fetch gifts information from the server.
	     *
	     * On success:
	     * - invokes callback with parameter *Hastable data* containing the gifts for the user
	     * - fires the RoarManager#giftsReadyEvent
	     * - sets #hasDataFromServer to true
	     *
	     * On failure:
	     * - invokes callback with error code and error message
	     *
	     * @param callback the callback function to be passed this function's result.
	     *
	     * @returns nothing - use a callback and/or subscribe to RoarManager events for results of non-blocking calls.
	     **/
		void Fetch (Roar.Callback callback);

		/**
	     * Check whether any user gifts data has been obtained from the server.
	     *
	     * @returns true if #fetch has completed execution.
	     **/
		bool HasDataFromServer { get; }

		/**
	     * Get a list of all the gift objects for the authenticated user.
	     *
	     * @returns A list of Hashtables for each gift.
	     *
	     * @note This does _not_ make a server call. It requires the gifts to
	     *       have already been fetched via a call to #fetch. If this function
	     *       is called prior to the successful completion of a #fetch call,
	     *       it will return an empty array.
	     **/
		ArrayList List ();

		/**
	     * Get a list of all the giftable items for the authenticated user.
	     *
	     * On success:
	     * - invokes callback with parameter *data* containing the list of gift Hashtables
	     *
	     * On failure:
	     * - returns an empty list
	     *
	     * @param callback the callback function to be passed this function's result.
	     *
	     * @returns A list of Hashtables for each giftable items.
	     *
	     * @note This does _not_ make a server call. It requires the gifts to
	     *       have already been fetched via a call to #fetch. If this function
	     *       is called prior to the successful completion of a #fetch call,
	     *       it will return an empty array.
	     **/
		ArrayList List (Roar.Callback callback);


		/**
	     * Returns the gift object for a given key.
	     *
	     * @param id the key that uniquely identifies a gift.
	     *
	     * @returns the gift Hashtable associated with the *id*
	     *          or null if the gift does not exist in the data store.
	     **/
		Hashtable GetGift (string id);

		/**
	     * Returns the gift object for a given id.
	     *
	     * On success:
	     * - invokes callback with parameter *data* containing the gift Hashtable
	     *
	     * On failure:
	     * - invokes callback with parameter *data* equalling null if gift does not exist
	     *
	     * @param id the key that uniquely identifies a gift.
	     * @param callback the callback function to be passed this function's result.
	     *
	     * @returns the gift Hashtable associated with the *id*
	     *          or null if the gift does not exist in the data store.
	     **/
		Hashtable GetGift (string id, Roar.Callback callback);
	}
}
