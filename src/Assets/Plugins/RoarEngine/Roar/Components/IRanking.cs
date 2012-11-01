using System.Collections;

namespace Roar.Components
{

	/**
   * \brief IRanking is an interface for listing leaderboard rankings.
   **/
	public interface IRanking
	{
		int Page { set; }

		/**
	     * Fetch ranking information from the server.
	     *
	     * On success:
	     * - invokes callback with parameter *Hastable data* containing the ranking
	     * - fires the RoarManager#rankingReadyEvent
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
	     * Check whether any ranking data has been obtained from the server.
	     *
	     * @returns true if #fetch has completed execution.
	     **/
		bool HasDataFromServer { get; }

		/**
	     * Get a list of all the ranking objects for the authenticated user.
	     *
	     * @returns A list of Hashtables for each ranking.
	     *
	     * @note This does _not_ make a server call. It requires the leaderboards to
	     *       have already been fetched via a call to #fetch. If this function
	     *       is called prior to the successful completion of a #fetch call,
	     *       it will return an empty array.
	     **/
		ArrayList List ();

		/**
	     * Get a list of all the ranking objects for the authenticated user.
	     *
	     * On success:
	     * - invokes callback with parameter *data* containing the list of Hashtable ranking
	     *
	     * On failure:
	     * - returns an empty list
	     *
	     * @param callback the callback function to be passed this function's result.
	     *
	     * @returns A list of Hashtables for each ranking.
	     *
	     * @note This does _not_ make a server call. It requires the user achievements to
	     *       have already been fetched via a call to #fetch. If this function
	     *       is called prior to the successful completion of a #fetch call,
	     *       it will return an empty array.
	     **/
		ArrayList List (Roar.Callback callback);


		/**
	     * Returns the ranking object for a given key.
	     *
	     * @param ikey the key that uniquely identifies a ranking.
	     *
	     * @returns the property Hashtable associated with the *key*
	     *          or null if the ranking does not exist in the data store.
	     **/
		Hashtable GetEntry (string ikey);

		/**
	     * Returns the ranking object for a given key.
	     *
	     * On success:
	     * - invokes callback with parameter *data* containing the ranking Hashtable
	     *
	     * On failure:
	     * - invokes callback with parameter *data* equalling null if ranking does not exist
	     *
	     * @param ikey the key that uniquely identifies a ranking.
	     * @param callback the callback function to be passed this function's result.
	     *
	     * @returns the achievement Hashtable associated with the *ikey*
	     *          or null if the ranking does not exist in the data store.
	     **/
		Hashtable GetEntry (string ikey, Roar.Callback callback);
	}
}
