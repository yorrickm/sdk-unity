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

public abstract class IWebAPI
{
	public abstract IAdminActions admin { get; }
	public abstract IAppstoreActions appstore { get; }
	public abstract IChrome_web_storeActions chrome_web_store { get; }
	public abstract IFacebookActions facebook { get; }
	public abstract IFriendsActions friends { get; }
	public abstract IGoogleActions google { get; }
	public abstract IInfoActions info { get; }
	public abstract IItemsActions items { get; }
	public abstract ILeaderboardsActions leaderboards { get; }
	public abstract IMailActions mail { get; }
	public abstract IShopActions shop { get; }
	public abstract IScriptsActions scripts { get; }
	public abstract ITasksActions tasks { get; }
	public abstract IUserActions user { get; }
	public abstract IUrbanairshipActions urbanairship { get; }


	public const int UNKNOWN_ERR  = 0;    // Default unspecified error (parse manually)
	public const int UNAUTHORIZED = 1;    // Auth token is no longer valid. Relogin.
	public const int BAD_INPUTS   = 2;    // Incorrect parameters passed to Roar
	public const int DISALLOWED   = 3;    // Action was not allowed (but otherwise successful)
	public const int FATAL_ERROR  = 4;    // Server died somehow (sad/bad/etc)
	public const int AWESOME      = 11;   // Turn it up.
	public const int OK           = 200;  // Everything ok - proceed


	public interface IAdminActions
	{
		void delete_player( Hashtable obj, IRequestCallback<IXMLNode> cb);
		void inrement_stat( Hashtable obj, IRequestCallback<IXMLNode> cb);
		void _set( Hashtable obj, IRequestCallback<IXMLNode> cb);
		void set_custom( Hashtable obj, IRequestCallback<IXMLNode> cb);
		void view_player( Hashtable obj, IRequestCallback<IXMLNode> cb);
	}

	public interface IAppstoreActions
	{
		void buy( Hashtable obj, IRequestCallback<IXMLNode> cb);
		void shop_list( Hashtable obj, IRequestCallback<IXMLNode> cb);
	}

	public interface IChrome_web_storeActions
	{
		void list( Hashtable obj, IRequestCallback<IXMLNode> cb);
	}

	public interface IFacebookActions
	{
		void bind_signed( Hashtable obj, IRequestCallback<IXMLNode> cb);
		void create_oauth( Hashtable obj, IRequestCallback<IXMLNode> cb);
		void create_signed( Hashtable obj, IRequestCallback<IXMLNode> cb);
		void fetch_oauth_token( Hashtable obj, IRequestCallback<IXMLNode> cb);
		void friends( Hashtable obj, IRequestCallback<IXMLNode> cb);
		void login_oauth( Hashtable obj, IRequestCallback<IXMLNode> cb);
		void login_signed( Hashtable obj, IRequestCallback<IXMLNode> cb);
		void shop_list( Hashtable obj, IRequestCallback<IXMLNode> cb);
	}

	public interface IFriendsActions
	{
		void accept( Hashtable obj, IRequestCallback<IXMLNode> cb);
		void decline( Hashtable obj, IRequestCallback<IXMLNode> cb);
		void invite( Hashtable obj, IRequestCallback<IXMLNode> cb);
		void invite_info( Hashtable obj, IRequestCallback<IXMLNode> cb);
		void list( Hashtable obj, IRequestCallback<IXMLNode> cb);
		void remove( Hashtable obj, IRequestCallback<IXMLNode> cb);
	}

	public interface IGoogleActions
	{
		void bind_user( Hashtable obj, IRequestCallback<IXMLNode> cb);
		void bind_user_token( Hashtable obj, IRequestCallback<IXMLNode> cb);
		void create_user( Hashtable obj, IRequestCallback<IXMLNode> cb);
		void create_user_token( Hashtable obj, IRequestCallback<IXMLNode> cb);
		void friends( Hashtable obj, IRequestCallback<IXMLNode> cb);
		void login_user( Hashtable obj, IRequestCallback<IXMLNode> cb);
		void login_user_token( Hashtable obj, IRequestCallback<IXMLNode> cb);
	}

	public interface IInfoActions
	{
		void get_bulk_player_info( Hashtable obj, IRequestCallback<IXMLNode> cb);
		void ping( Hashtable obj, IRequestCallback<IXMLNode> cb);
		void user( Hashtable obj, IRequestCallback<IXMLNode> cb);
		void poll( Hashtable obj, IRequestCallback<IXMLNode> cb);
	}

	public interface IItemsActions
	{
		void equip( Hashtable obj, IRequestCallback<IXMLNode> cb);
		void list( Hashtable obj, IRequestCallback<IXMLNode> cb);
		void sell( Hashtable obj, IRequestCallback<IXMLNode> cb);
		void _set( Hashtable obj, IRequestCallback<IXMLNode> cb);
		void unequip( Hashtable obj, IRequestCallback<IXMLNode> cb);
		void use( Hashtable obj, IRequestCallback<IXMLNode> cb);
		void view( Hashtable obj, IRequestCallback<IXMLNode> cb);
		void view_all( Hashtable obj, IRequestCallback<IXMLNode> cb);
	}

	public interface ILeaderboardsActions
	{
		void list( Hashtable obj, IRequestCallback<IXMLNode> cb);
		void view( Hashtable obj, IRequestCallback<IXMLNode> cb);
	}

	public interface IMailActions
	{
		void accept( Hashtable obj, IRequestCallback<IXMLNode> cb);
		void send( Hashtable obj, IRequestCallback<IXMLNode> cb);
		void what_can_i_accept( Hashtable obj, IRequestCallback<IXMLNode> cb);
		void what_can_i_send( Hashtable obj, IRequestCallback<IXMLNode> cb);
	}

	public interface IShopActions
	{
		void list( Hashtable obj, IRequestCallback<IXMLNode> cb);
		void buy( Hashtable obj, IRequestCallback<IXMLNode> cb);
	}

	public interface IScriptsActions
	{
		void run( Hashtable obj, IRequestCallback<IXMLNode> cb);
	}

	public interface ITasksActions
	{
		void list( Hashtable obj, IRequestCallback<IXMLNode> cb);
		void start( Hashtable obj, IRequestCallback<IXMLNode> cb);
	}

	public interface IUserActions
	{
		void achievements( Hashtable obj, IRequestCallback<IXMLNode> cb);
		void change_name( Hashtable obj, IRequestCallback<IXMLNode> cb);
		void change_password( Hashtable obj, IRequestCallback<IXMLNode> cb);
		void create( Hashtable obj, IRequestCallback<IXMLNode> cb);
		void login( Hashtable obj, IRequestCallback<IXMLNode> cb);
		void login_facebook_oauth( Hashtable obj, IRequestCallback<IXMLNode> cb);
		void logout( Hashtable obj, IRequestCallback<IXMLNode> cb);
		void netdrive_save( Hashtable obj, IRequestCallback<IXMLNode> cb);
		void netdrive_fetch( Hashtable obj, IRequestCallback<IXMLNode> cb);
		void _set( Hashtable obj, IRequestCallback<IXMLNode> cb);
		void view( Hashtable obj, IRequestCallback<IXMLNode> cb);
	}

	public interface IUrbanairshipActions
	{
		void ios_register( Hashtable obj, IRequestCallback<IXMLNode> cb);
		void push( Hashtable obj, IRequestCallback<IXMLNode> cb);
	}

}

