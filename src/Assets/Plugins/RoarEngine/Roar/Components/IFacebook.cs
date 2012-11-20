using System.Collections;

namespace Roar.Components
{
	/**
	 * \brief Methods for creating, authenticating and logging out a User.
	 *
	 * @todo The naming of the members of this class seem a little odd.
	 **/
	public interface IFacebook
	{
        /**
         * Fetch a facebook OAuth token giving a code.
         *
         * On success:
         * - invokes callback with the facebook oAuth token, success code and success message
         *
         * On failure:
         * - invokes callback with empty data parameter, error code and error message
         *
         * @param code the code string from facebook.
         * @param cb the callback function to be passed the result of doLogin.
         **/
        void DoFetchFacebookOAuthToken(string code, Roar.Callback cb);

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
        void DoLoginFacebookOAuth(string oauth_token, Roar.Callback cb);

        /**
         * Login a player using Facebook Signed Request. Note this is for UnityPlayers
         * that are running in a facebook iframe only (Canvas app)
         *
         * On success:
         * - invokes callback with empty data parameter, success code and success message
         * - fires a RoarManager#loggedInEvent
         *
         * On failure:
         * - invokes callback with empty data parameter, error code and error message
         * - fires a RoarManager#logInFailedEvent containing a failure message
         *
         * @param signed request taken from facebook post
         * @param cb the callback function to be passed the result of doLogin.
         **/
        void DoLoginFacebookSignedReq(string signedReq, Roar.Callback cb);


        /**
         * Creates a new user with the given username and facebook signed auth, and logs
         * that player in. This will only work if you have a signed request verifying the current user. 
         * You will automatically get this from Facebook as a POST parameter if you are running an iframe app within Facebook.
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
         * @param the players facebook signed auth
         * @param cb the callback function to be passed the result of doCreate.
         */
        void DoCreateFacebookSignedReq(string name, string signedAuth, Roar.Callback cb);
	}
}
