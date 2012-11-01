using System.Collections;

namespace Roar.Components
{
	/**
	 * \brief Methods for creating, authenticating and logging out a User.
	 *
	 * @todo The naming of the members of this class seem a little odd.
	 **/
	public interface IUser
	{
		/**
		 * Creates a new user with the given username and password, and logs
		 * that player in.
		 *
		 * On success:
		 * - fires a RoarManager#createdUserEvent
		 * - automatically calls doLogin()
		 *
		 * On failure:
		 * - invokes callback with empty data parameter, error code and error message
		 * - fires a RoarManager#createUserFailedEvent containing a failure message
		 *
		 * @param name the players username
		 * @param hash the players password
		 * @param cb the callback function to be passed the result of doCreate.
		 */
		void DoCreate( string name, string hash, Roar.Callback cb );

		/**
		 * Login a player.
		 *
		 * Requests an authentication token from the server for the player,
		 * which is used to validate subsequent requests.
		 *
		 * On success:
		 * - invokes callback with empty data parameter, success code and success message
		 * - fires a RoarManager#loggedInEvent
		 *
		 * On failure:
		 * - invokes callback with empty data parameter, error code and error message
		 * - fires a RoarManager#logInFailedEvent containing a failure message
		 *
		 * @param name the players username
		 * @param hash the players password
		 * @param cb the callback function to be passed the result of doLogin.
		 **/
		void DoLogin( string name, string hash, Roar.Callback cb );

		/**
		 * Login a player using Facebook OAuth.
		 *
		 * On success:
		 * - invokes callback with empty data parameter, success code and success message
		 * - fires a RoarManager#loggedInEvent
		 *
		 * On failure:
		 * - invokes callback with empty data parameter, error code and error message
		 * - fires a RoarManager#logInFailedEvent containing a failure message
		 *
		 * @param oauth_token the OAuth token.
		 * @param cb the callback function to be passed the result of doLogin.
		 **/
		void DoLoginFacebookOAuth( string oauth_token, Roar.Callback cb );

		/**
		 * Logs out a user.
		 * Clears the authentication token for a user. Must re-login to authenticate.
		 *
		 * On success:
		 * - fires a RoarManager#loggedOutEvent
		 *
		 * On failure:
		 * - invokes callback with empty data parameter, error code and error message
		 *
		 * @param the callback function to be passed the result of doLoginFacebookOAuth.
		 **/
		void DoLogout( Roar.Callback cb );
	}
}
