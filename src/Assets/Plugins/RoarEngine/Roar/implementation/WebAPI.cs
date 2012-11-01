using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WebAPI : IWebAPI
{
	protected IRequestSender requestSender_;

	public WebAPI (IRequestSender requestSender)
	{
		requestSender_ = requestSender;

		admin_ = new AdminActions (requestSender);
		appstore_ = new AppstoreActions (requestSender);
		chrome_web_store_ = new Chrome_web_storeActions (requestSender);
		facebook_ = new FacebookActions (requestSender);
		friends_ = new FriendsActions (requestSender);
		google_ = new GoogleActions (requestSender);
		info_ = new InfoActions (requestSender);
		items_ = new ItemsActions (requestSender);
		leaderboards_ = new LeaderboardsActions (requestSender);
		mail_ = new MailActions (requestSender);
		shop_ = new ShopActions (requestSender);
		scripts_ = new ScriptsActions (requestSender);
		tasks_ = new TasksActions (requestSender);
		user_ = new UserActions (requestSender);
		urbanairship_ = new UrbanairshipActions (requestSender);
	}

	public override IAdminActions admin { get { return admin_; } }

	public AdminActions admin_;

	public override IAppstoreActions appstore { get { return appstore_; } }

	public AppstoreActions appstore_;

	public override IChrome_web_storeActions chrome_web_store { get { return chrome_web_store_; } }

	public Chrome_web_storeActions chrome_web_store_;

	public override IFacebookActions facebook { get { return facebook_; } }

	public FacebookActions facebook_;

	public override IFriendsActions friends { get { return friends_; } }

	public FriendsActions friends_;

	public override IGoogleActions google { get { return google_; } }

	public GoogleActions google_;

	public override IInfoActions info { get { return info_; } }

	public InfoActions info_;

	public override IItemsActions items { get { return items_; } }

	public ItemsActions items_;

	public override ILeaderboardsActions leaderboards { get { return leaderboards_; } }

	public LeaderboardsActions leaderboards_;

	public override IMailActions mail { get { return mail_; } }

	public MailActions mail_;

	public override IShopActions shop { get { return shop_; } }

	public ShopActions shop_;

	public override IScriptsActions scripts { get { return scripts_; } }

	public ScriptsActions scripts_;

	public override ITasksActions tasks { get { return tasks_; } }

	public TasksActions tasks_;

	public override IUserActions user { get { return user_; } }

	public UserActions user_;

	public override IUrbanairshipActions urbanairship { get { return urbanairship_; } }

	public UrbanairshipActions urbanairship_;



	public class APIBridge
	{
		protected IRequestSender api;

		public APIBridge (IRequestSender caller)
		{
			api = caller;
		}
	}

	public class AdminActions : APIBridge, IAdminActions
	{
		public AdminActions (IRequestSender caller) : base(caller)
		{
		}

		public void delete_player (Hashtable obj, IRequestCallback<IXMLNode> cb)
		{
			api.MakeCall ("admin/delete_player", obj, cb);
		}

		public void inrement_stat (Hashtable obj, IRequestCallback<IXMLNode> cb)
		{
			api.MakeCall ("admin/inrement_stat", obj, cb);
		}

		public void _set (Hashtable obj, IRequestCallback<IXMLNode> cb)
		{
			api.MakeCall ("admin/set", obj, cb);
		}

		public void set_custom (Hashtable obj, IRequestCallback<IXMLNode> cb)
		{
			api.MakeCall ("admin/set_custom", obj, cb);
		}

		public void view_player (Hashtable obj, IRequestCallback<IXMLNode> cb)
		{
			api.MakeCall ("admin/view_player", obj, cb);
		}

	}

	public class AppstoreActions : APIBridge, IAppstoreActions
	{
		public AppstoreActions (IRequestSender caller) : base(caller)
		{
		}

		public void buy (Hashtable obj, IRequestCallback<IXMLNode> cb)
		{
			api.MakeCall ("appstore/buy", obj, cb);
		}

		public void shop_list (Hashtable obj, IRequestCallback<IXMLNode> cb)
		{
			api.MakeCall ("appstore/shop_list", obj, cb);
		}

	}

	public class Chrome_web_storeActions : APIBridge, IChrome_web_storeActions
	{
		public Chrome_web_storeActions (IRequestSender caller) : base(caller)
		{
		}

		public void list (Hashtable obj, IRequestCallback<IXMLNode> cb)
		{
			api.MakeCall ("chrome_web_store/list", obj, cb);
		}

	}

	public class FacebookActions : APIBridge, IFacebookActions
	{
		public FacebookActions (IRequestSender caller) : base(caller)
		{
		}

		public void bind_signed (Hashtable obj, IRequestCallback<IXMLNode> cb)
		{
			api.MakeCall ("facebook/bind_signed", obj, cb);
		}

		public void create_oauth (Hashtable obj, IRequestCallback<IXMLNode> cb)
		{
			api.MakeCall ("facebook/create_oauth", obj, cb);
		}

		public void create_signed (Hashtable obj, IRequestCallback<IXMLNode> cb)
		{
			api.MakeCall ("facebook/create_signed", obj, cb);
		}

		public void fetch_oauth_token (Hashtable obj, IRequestCallback<IXMLNode> cb)
		{
			api.MakeCall ("facebook/fetch_oauth_token", obj, cb);
		}

		public void friends (Hashtable obj, IRequestCallback<IXMLNode> cb)
		{
			api.MakeCall ("facebook/friends", obj, cb);
		}

		public void login_oauth (Hashtable obj, IRequestCallback<IXMLNode> cb)
		{
			api.MakeCall ("facebook/login_oauth", obj, cb);
		}

		public void login_signed (Hashtable obj, IRequestCallback<IXMLNode> cb)
		{
			api.MakeCall ("facebook/login_signed", obj, cb);
		}

		public void shop_list (Hashtable obj, IRequestCallback<IXMLNode> cb)
		{
			api.MakeCall ("facebook/shop_list", obj, cb);
		}

	}

	public class FriendsActions : APIBridge, IFriendsActions
	{
		public FriendsActions (IRequestSender caller) : base(caller)
		{
		}

		public void accept (Hashtable obj, IRequestCallback<IXMLNode> cb)
		{
			api.MakeCall ("friends/accept", obj, cb);
		}

		public void decline (Hashtable obj, IRequestCallback<IXMLNode> cb)
		{
			api.MakeCall ("friends/decline", obj, cb);
		}

		public void invite (Hashtable obj, IRequestCallback<IXMLNode> cb)
		{
			api.MakeCall ("friends/invite", obj, cb);
		}

		public void invite_info (Hashtable obj, IRequestCallback<IXMLNode> cb)
		{
			api.MakeCall ("friends/info", obj, cb);
		}

		public void list (Hashtable obj, IRequestCallback<IXMLNode> cb)
		{
			api.MakeCall ("friends/list", obj, cb);
		}

		public void remove (Hashtable obj, IRequestCallback<IXMLNode> cb)
		{
			api.MakeCall ("friends/remove", obj, cb);
		}

	}

	public class GoogleActions : APIBridge, IGoogleActions
	{
		public GoogleActions (IRequestSender caller) : base(caller)
		{
		}

		public void bind_user (Hashtable obj, IRequestCallback<IXMLNode> cb)
		{
			api.MakeCall ("google/bind_user", obj, cb);
		}

		public void bind_user_token (Hashtable obj, IRequestCallback<IXMLNode> cb)
		{
			api.MakeCall ("google/bind_user_token", obj, cb);
		}

		public void create_user (Hashtable obj, IRequestCallback<IXMLNode> cb)
		{
			api.MakeCall ("google/create_user", obj, cb);
		}

		public void create_user_token (Hashtable obj, IRequestCallback<IXMLNode> cb)
		{
			api.MakeCall ("google/create_user_token", obj, cb);
		}

		public void friends (Hashtable obj, IRequestCallback<IXMLNode> cb)
		{
			api.MakeCall ("google/friends", obj, cb);
		}

		public void login_user (Hashtable obj, IRequestCallback<IXMLNode> cb)
		{
			api.MakeCall ("google/login_user", obj, cb);
		}

		public void login_user_token (Hashtable obj, IRequestCallback<IXMLNode> cb)
		{
			api.MakeCall ("google/login_user_token", obj, cb);
		}

	}

	public class InfoActions : APIBridge, IInfoActions
	{
		public InfoActions (IRequestSender caller) : base(caller)
		{
		}

		public void get_bulk_player_info (Hashtable obj, IRequestCallback<IXMLNode> cb)
		{
			api.MakeCall ("info/get_bulk_player_info", obj, cb);
		}

		public void ping (Hashtable obj, IRequestCallback<IXMLNode> cb)
		{
			api.MakeCall ("info/ping", null, cb);
		}

		public void user (Hashtable obj, IRequestCallback<IXMLNode> cb)
		{
			api.MakeCall ("info/user", obj, cb);
		}

		public void poll (Hashtable obj, IRequestCallback<IXMLNode> cb)
		{
			api.MakeCall ("info/poll", null, cb);
		}

	}

	public class ItemsActions : APIBridge, IItemsActions
	{
		public ItemsActions (IRequestSender caller) : base(caller)
		{
		}

		public void equip (Hashtable obj, IRequestCallback<IXMLNode> cb)
		{
			api.MakeCall ("items/equip", obj, cb);
		}

		public void list (Hashtable obj, IRequestCallback<IXMLNode> cb)
		{
			api.MakeCall ("items/list", null, cb);
		}

		public void sell (Hashtable obj, IRequestCallback<IXMLNode> cb)
		{
			api.MakeCall ("items/sell", obj, cb);
		}

		public void _set (Hashtable obj, IRequestCallback<IXMLNode> cb)
		{
			api.MakeCall ("items/set", obj, cb);
		}

		public void unequip (Hashtable obj, IRequestCallback<IXMLNode> cb)
		{
			api.MakeCall ("items/unequip", obj, cb);
		}

		public void use (Hashtable obj, IRequestCallback<IXMLNode> cb)
		{
			api.MakeCall ("items/use", obj, cb);
		}

		public void view (Hashtable obj, IRequestCallback<IXMLNode> cb)
		{
			api.MakeCall ("items/view", obj, cb);
		}

		public void view_all (Hashtable obj, IRequestCallback<IXMLNode> cb)
		{
			api.MakeCall ("items/view_all", obj, cb);
		}

	}

	public class LeaderboardsActions : APIBridge, ILeaderboardsActions
	{
		public LeaderboardsActions (IRequestSender caller) : base(caller)
		{
		}

		public void list (Hashtable obj, IRequestCallback<IXMLNode> cb)
		{
			api.MakeCall ("leaderboards/list", obj, cb);
		}

		public void view (Hashtable obj, IRequestCallback<IXMLNode> cb)
		{
			api.MakeCall ("leaderboards/view", obj, cb);
		}

	}

	public class MailActions : APIBridge, IMailActions
	{
		public MailActions (IRequestSender caller) : base(caller)
		{
		}

		public void accept (Hashtable obj, IRequestCallback<IXMLNode> cb)
		{
			api.MakeCall ("mail/accept", obj, cb);
		}

		public void send (Hashtable obj, IRequestCallback<IXMLNode> cb)
		{
			api.MakeCall ("mail/send", obj, cb);
		}

		public void what_can_i_accept (Hashtable obj, IRequestCallback<IXMLNode> cb)
		{
			api.MakeCall ("mail/what_can_i_accept", obj, cb);
		}

		public void what_can_i_send (Hashtable obj, IRequestCallback<IXMLNode> cb)
		{
			api.MakeCall ("mail/what_can_i_send", obj, cb);
		}

	}

	public class ShopActions : APIBridge, IShopActions
	{
		public ShopActions (IRequestSender caller) : base(caller)
		{
		}

		public void list (Hashtable obj, IRequestCallback<IXMLNode> cb)
		{
			api.MakeCall ("shop/list", null, cb);
		}

		public void buy (Hashtable obj, IRequestCallback<IXMLNode> cb)
		{
			api.MakeCall ("shop/buy", obj, cb);
		}

	}

	public class ScriptsActions : APIBridge, IScriptsActions
	{
		public ScriptsActions (IRequestSender caller) : base(caller)
		{
		}

		public void run (Hashtable obj, IRequestCallback<IXMLNode> cb)
		{
			api.MakeCall ("scripts/run", obj, cb);
		}

	}

	public class TasksActions : APIBridge, ITasksActions
	{
		public TasksActions (IRequestSender caller) : base(caller)
		{
		}

		public void list (Hashtable obj, IRequestCallback<IXMLNode> cb)
		{
			api.MakeCall ("tasks/list", null, cb);
		}

		public void start (Hashtable obj, IRequestCallback<IXMLNode> cb)
		{
			api.MakeCall ("tasks/start", obj, cb);
		}

	}

	public class UserActions : APIBridge, IUserActions
	{
		public UserActions (IRequestSender caller) : base(caller)
		{
		}

		public void achievements (Hashtable obj, IRequestCallback<IXMLNode> cb)
		{
			api.MakeCall ("user/achievements", null, cb);
		}

		public void change_name (Hashtable obj, IRequestCallback<IXMLNode> cb)
		{
			api.MakeCall ("user/change_name", obj, cb);
		}

		public void change_password (Hashtable obj, IRequestCallback<IXMLNode> cb)
		{
			api.MakeCall ("user/change_password", obj, cb);
		}

		public void create (Hashtable obj, IRequestCallback<IXMLNode> cb)
		{
			api.MakeCall ("user/create", obj, cb);
		}

		public void login (Hashtable obj, IRequestCallback<IXMLNode> cb)
		{
			api.MakeCall ("user/login", obj, cb);
		}

		public void login_facebook_oauth (Hashtable obj, IRequestCallback<IXMLNode> cb)
		{
			api.MakeCall ("facebook/login_oauth", obj, cb);
		}

		public void logout (Hashtable obj, IRequestCallback<IXMLNode> cb)
		{
			api.MakeCall ("user/logout", null, cb);
		}

		public void netdrive_save (Hashtable obj, IRequestCallback<IXMLNode> cb)
		{
			api.MakeCall ("user/netdrive_set", obj, cb);
		}

		public void netdrive_fetch (Hashtable obj, IRequestCallback<IXMLNode> cb)
		{
			api.MakeCall ("user/netdrive_get", obj, cb);
		}

		public void _set (Hashtable obj, IRequestCallback<IXMLNode> cb)
		{
			api.MakeCall ("user/set", obj, cb);
		}

		public void view (Hashtable obj, IRequestCallback<IXMLNode> cb)
		{
			api.MakeCall ("user/view", null, cb);
		}

	}

	public class UrbanairshipActions : APIBridge, IUrbanairshipActions
	{
		public UrbanairshipActions (IRequestSender caller) : base(caller)
		{
		}

		public void ios_register (Hashtable obj, IRequestCallback<IXMLNode> cb)
		{
			api.MakeCall ("urbanairship/ios_register", obj, cb);
		}

		public void push (Hashtable obj, IRequestCallback<IXMLNode> cb)
		{
			api.MakeCall ("urbanairship/push", obj, cb);
		}

	}


}

