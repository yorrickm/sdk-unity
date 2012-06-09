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
	 * \brief Methods for creating, authenticating and logging out a User.
	 **/
	public interface IUser
	{
		/**
		 * Creates a new user with the given username and password, and logs
		 * that player in.
		 *
		 * On success:
		 * - fires a RoarIOManager#createdUserEvent
		 * - automatically calls doLogin()
		 *
		 * On failure:
		 * - invokes callback with empty data parameter, error code and error message
		 * - fires a RoarIOManager#createUserFailedEvent containing a failure message
		 *
		 * @param name the players username
		 * @param hash the players password
		 * @param cb the callback function to be passed the result of doCreate.
		 */
		void doCreate( string name, string hash, Roar.Callback cb );

		/**
		 * Login a player.
		 *
		 * Requests an authentication token from the server for the player,
		 * which is used to validate subsequent requests.
		 *
		 * On success:
		 * - invokes callback with empty data parameter, success code and success message
		 * - fires a RoarIOManager#loggedInEvent
		 *
		 * On failure:
		 * - invokes callback with empty data parameter, error code and error message
		 * - fires a RoarIOManager#logInFailedEvent containing a failure message
		 *
		 * @param name the players username
		 * @param hash the players password
		 * @param cb the callback function to be passed the result of doLogin.
		 **/
		void doLogin( string name, string hash, Roar.Callback cb );

		/**
		 * Login a player using Facebook OAuth.
		 *
		 * On success:
		 * - invokes callback with empty data parameter, success code and success message
		 * - fires a RoarIOManager#loggedInEvent
		 *
		 * On failure:
		 * - invokes callback with empty data parameter, error code and error message
		 * - fires a RoarIOManager#logInFailedEvent containing a failure message
		 *
		 * @param oauth_token the OAuth token.
		 * @param cb the callback function to be passed the result of doLogin.
		 **/
		void doLoginFacebookOAuth( string oauth_token, Roar.Callback cb );

		/**
		 * Logs out a user.
		 * Clears the authentication token for a user. Must re-login to authenticate.
		 *
		 * On success:
		 * - fires a RoarIOManager#loggedOutEvent
		 *
		 * On failure:
		 * - invokes callback with empty data parameter, error code and error message
		 *
		 * @param the callback function to be passed the result of doLoginFacebookOAuth.
		 **/
		void doLogout( Roar.Callback cb );
	}
}