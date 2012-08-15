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
using System;
using System.Collections;

namespace Roar
{
	
  public abstract class CallbackInfo
  {
	public abstract object d { get; }

	protected CallbackInfo(int in_code, string in_msg )
	{
		code = in_code;
		msg = in_msg;
	}

	public int code;
	public string msg;
  }

  public class CallbackInfo<T> : CallbackInfo
  {
    public override object d { get { return data; } }
    public T data;
  
    public CallbackInfo( T in_data, int in_code=IWebAPI.OK, string in_msg="" ) : base( in_code, in_msg )
    {
      data = in_data;
    }
  };

  /**
   * Many roar.io functions take a callback function.  Often this callback is
   * optional, but if you wish to use one it is always a #Roar.Callback type.
   * You might not need one if you choose to catch the results of the call using
   * the events in #RoarManager.
   *
   * The Hashtable returned will usually contain three parameters:
   *
   *   + code : an int corresponding to the values in #IWebAPI
   *   + msg : a string message, often empty on success, but containing more
   *     details in the case of an error
   *   + data : an object with the results of the call.
   *
   * The only place you might need to provide a function of a different signature
   * is when using the events specified in #RoarManager. These events accept a
   * function that corresponds to the data available to the event. See the
   * individual events for details.
   */
  //TODO: Can we unify this callback with the IRequestCallback class?
  public delegate void Callback( CallbackInfo h );
}

/**
 * The public facing container for Roar functionality.
 *
 * You get a real instance of this interface by binding the Roar script to an
 * object in your game.
 *
 * This class provides several utility functions for common tasks, and  several
 * lower-level components for more detailed operations.
 */
public interface IRoar
{
  /**
   * Get a configuration object that lets you configure how various
   * aspects of roar.io behave.
   */
  Roar.IConfig Config { get; }

  /**
   * Low level access to the entire roar api.
   *
   * @note The callbacks used by the #IWebAPI are slightly different from the Callbacks used by 
   * the other functions in #IRoar .
   */
   IWebAPI WebAPI { get; }

  /**
   * Get access to the players properties and stats.
   */
  Roar.Components.IProperties Properties { get; }

  /**
   * Get access to the players inventory functions.
   */
  Roar.Components.IInventory Inventory { get; }

  /**
   * Get access to the shop functions.
   */
  Roar.Components.IShop Shop { get; }

  /**
   * Get access to the tasks/actions functions.
   */
  Roar.Components.IActions Actions { get; }
  
  /**
   * Get access to the achievements functions.
   */
  Roar.Components.IAchievements Achievements { get; }
  
  /**
   * Get access to the gifts functions.
   */
  Roar.Components.IGifts Gifts { get; }
  
  Roar.Components.IInAppPurchase Appstore { get; }

  /**
   * Methods for notifications.
   */
  Roar.Adapters.IUrbanAirship UrbanAirship { get; }

  /**
   * The roar authentication token.
   *
   * You usually would not need this, unless you are making some direct
   * calls to the roar servers, but can be usefull for debugging.
   */
  string AuthToken { get; }

  /**
   * Returns a string representing the current version of the roar.io API
   * that you are using.
   *
   * The callback, if provided, is called with the arguments
   *
   *     {"data":"version string", "code":IWebAPI.OK, "msg":null}
   */
  string version( Roar.Callback callback = null );

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
  void login( string username, string password, Roar.Callback callback=null );

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
  void login_facebook_oauth( string oauth_token, Roar.Callback callback=null );

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
  void logout( Roar.Callback callback=null );

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
   **/
  void create( string username, string password, Roar.Callback callback=null );

  /**
   * @todo Document me!
   */
  string whoami( Roar.Callback callback=null );
}


