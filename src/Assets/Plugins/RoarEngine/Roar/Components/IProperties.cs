using System.Collections;

namespace Roar.Components
{

	/**
   * \brief IProperties is an interface for retrieving the properties of the authenticated user.
   *
   * Properties can include user attributes, resources and currencies for a given user, along with other core user information.
   *
   * You can get the list of all properties via the #list function,
   * a single property via the #getProperty function or if you only want
   * the value of a property - you can use the #getValue function.
   *
   * \code
   * Hashtable userHealth = Properties.getProperty("health");
   *
   * string currentValue = userHealth["value"];
   *        // or
   *        currentValue = Properties.getValue("health");
   *
   * string maxHealth = userHealth["max"];
   *
   * \endcode
   *
   **/
	public interface IProperties
	{

		/**
	     * Fetch player information from the server.
	     *
	     * On success:
	     * - invokes callback with parameter *Hastable data* containing the properties for the user
	     * - fires the RoarManager#propertiesReadyEvent
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
	     * Check whether any user properties data has been obtained from the server.
	     *
	     * @returns true if #fetch has completed execution.
	     **/
		bool HasDataFromServer { get; }

		/**
	     * Get a list of all the property objects for the authenticated user.
	     *
	     * @returns A list of Hashtables for each user property.
	     *
	     * @note This does _not_ make a server call. It requires the user properties to
	     *       have already been fetched via a call to #fetch. If this function
	     *       is called prior to the successful completion of a #fetch call,
	     *       it will return an empty array.
	     **/
		ArrayList List ();

		/**
	     * Get a list of all the property objects for the authenticated user.
	     *
	     * On success:
	     * - invokes callback with parameter *data* containing the list of Hashtable user properties
	     *
	     * On failure:
	     * - returns an empty list
	     *
	     * @param callback the callback function to be passed this function's result.
	     *
	     * @returns A list of Hashtables for each user property.
	     *
	     * @note This does _not_ make a server call. It requires the user properties to
	     *       have already been fetched via a call to #fetch. If this function
	     *       is called prior to the successful completion of a #fetch call,
	     *       it will return an empty array.
	     **/
		ArrayList List (Roar.Callback callback);

		/**
	     * Returns the property object for a given key.
	     *
	     * @param key the key that uniquely identifies a property.
	     *
	     * @returns the property Hashtable associated with the *key*
	     *          or null if the property does not exist in the data store.
	     **/
		object GetProperty (string key);

		/**
	     * Returns the property object for a given key.
	     *
	     * On success:
	     * - invokes callback with parameter *data* containing the Hashtable user property
	     *
	     * On failure:
	     * - invokes callback with parameter *data* equaling null if user property does not exist
	     *
	     *
	     * @param key the key that uniquely identifies a property.
	     * @param callback the callback function to be passed this function's result.
	     *
	     * @returns the property Hashtable associated with the *key*
	     *          or null if the property does not exist in the data store.
	     **/
		object GetProperty (string key, Roar.Callback callback);

		/**
	     * Returns the *value* attribute of a property object.
	     *
	     * @param ikey the key that uniquely identifies a property.
	     *
	     * @return the *value* attribute of a property object
	     *         or null if the user property does not exist in the data store.
	     */
		string GetValue (string ikey);

		/**
	     * Returns the *value* attribute of a property object.
	     *
	     * On success:
	     * - invokes callback with parameter *data* containing the user property value string
	     *
	     * On failure:
	     * - invokes callback with parameter *data* equaling null if user property does not exist
	     *
	     * @param the key that uniquely identifies a property.
	     * @param callback the callback function to be passed this function's result.
	     *
	     * @return the *value* attribute of a property object
	     *         or null if the user property does not exist in the data store.
	     */
		string GetValue (string ikey, Roar.Callback callback);
	}
}
