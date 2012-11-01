
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;


public class RoarManager
{
	public class GoodInfo
	{
		public GoodInfo( string in_id, string in_ikey, string in_label )
		{
			id = in_id;
			ikey = in_ikey;
			label = in_label;
		}
		public string id;
		public string ikey;
		public string label;
	};

	public class PurchaseInfo
	{
		public PurchaseInfo( string in_currency, int in_price, string in_ikey, int in_quantity )
		{
			currency = in_currency;
			price = in_price;
			ikey = in_ikey;
			quantity = in_quantity;
		}
		public string currency;
		public int price;
		public string ikey;
		public int quantity;
	};

	public class CallInfo
	{
		public CallInfo( object xml_node, int code, string message, string call_id)
		{
		}
	};



	public static event Action<string> logInFailedEvent;
	public static void OnLogInFailed( string mesg ) { if(logInFailedEvent!=null) logInFailedEvent(mesg); }

	public static event Action<string> createUserFailedEvent;
	public static void OnCreateUserFailed( string mesg ) { if(createUserFailedEvent!=null) createUserFailedEvent(mesg); }

	public static event Action loggedOutEvent;
	public static void OnLoggedOut() { if(loggedOutEvent!=null) loggedOutEvent(); }

	public static event Action loggedInEvent;
	public static void OnLoggedIn() { if(loggedInEvent!=null) loggedInEvent(); }

	public static event Action createdUserEvent;
	public static void OnCreatedUser() { if(createdUserEvent!=null) createdUserEvent(); }

	public static event Action<GoodInfo> goodActivatedEvent;
	public static void OnGoodActivated( GoodInfo info ) { if(goodActivatedEvent!=null) goodActivatedEvent(info); }

	public static event Action<GoodInfo> goodDeactivatedEvent;
	public static void OnGoodDeactivated( GoodInfo info ) { if(goodDeactivatedEvent!=null) goodDeactivatedEvent(info); }

	public static event Action<GoodInfo> goodUsedEvent;
	public static void OnGoodUsed( GoodInfo info ) { if(goodUsedEvent!=null) goodUsedEvent(info); }

	public static event Action<GoodInfo> goodSoldEvent;
	public static void OnGoodSold( GoodInfo info ) { if(goodSoldEvent!=null) goodSoldEvent(info); }

	/**
	 * Fired when a shop item has been successfully purchased.
	 */
	public static event Action<PurchaseInfo> goodBoughtEvent;
	public static void OnGoodBought( PurchaseInfo info ) { if(goodBoughtEvent!=null) goodBoughtEvent(info); }

	public static event Action<IXMLNode> eventDoneEvent;
	public static void OnEventDone( IXMLNode eventInfo ) { if(eventDoneEvent!=null) eventDoneEvent(eventInfo); }

	public static event Action<string,string> dataLoadedEvent;
	public static void OnDataLoaded( string key, string value ) { if(dataLoadedEvent!=null) dataLoadedEvent(key, value); }

	public static event Action<string,string> dataSavedEvent;
	public static void OnDataSaved( string key, string value ) { if(dataSavedEvent!=null) dataSavedEvent(key, value); }

	public static event Action roarNetworkStartEvent;
	public static void OnRoarNetworkStart() { if(roarNetworkStartEvent!=null) roarNetworkStartEvent(); }

	public static event Action<string> roarNetworkEndEvent;
	public static void OnRoarNetworkEnd( string call_id ) { if(roarNetworkEndEvent!=null) roarNetworkEndEvent(call_id); }

	public static event Action<CallInfo> callCompleteEvent;
	public static void OnCallComplete( CallInfo info ) { if(callCompleteEvent!=null) callCompleteEvent(info); }

	/**
	 * @note The object is really an XML Node.
	 * @todo It's ugly to be using an implementation detail like this.
	 */
	public static event Action<object> roarServerAllEvent;
	public static void OnRoarServerAll( object info ) { if(roarServerAllEvent!=null) roarServerAllEvent(info); }

	/**
	 * @todo These should be generated for each component.
	 */
	public static event Action xxxChangeEvent;
	public static void OnXxxChange() { if(xxxChangeEvent!=null) xxxChangeEvent(); }

	/**
	 * Fired when the data have been retrieved from the server.
	 */
	public static event Action propertiesReadyEvent;
	public static void OnPropertiesReady() { if(propertiesReadyEvent!=null) propertiesReadyEvent(); }

	/**
	 * Fired when the data have been retrieved from the server.
	 */
	public static event Action leaderboardsReadyEvent;
	public static void OnLeaderboardsReady() { if(leaderboardsReadyEvent!=null) leaderboardsReadyEvent(); }

	/**
	 * Fired when the data have been retrieved from the server.
	 */
	public static event Action rankingReadyEvent;
	public static void OnRankingReady() { if(rankingReadyEvent!=null) rankingReadyEvent(); }

	/**
	 * Fired when the data changes.
	 */
	public static event Action propertiesChangeEvent;
	public static void OnPropertiesChange() { if(propertiesChangeEvent!=null) propertiesChangeEvent(); }

	/**
	 * Fired when the data have been retrieved from the server.
	 */
	public static event Action shopReadyEvent;
	public static void OnShopReady() { if(shopReadyEvent!=null) shopReadyEvent(); }

	/**
	 * Fired when the data have been retrieved from the server.
	 */
	public static event Action leaderboardsChangeEvent;
	public static void OnLeaderboardsChange() { if(leaderboardsChangeEvent!=null) leaderboardsChangeEvent(); }

	/**
	 * Fired when the data have been retrieved from the server.
	 */
	public static event Action rankingChangeEvent;
	public static void OnRankingChange() { if(rankingChangeEvent!=null) rankingChangeEvent(); }

	/**
	 * Fired when the data changes.
	 */
	public static event Action shopChangeEvent;
	public static void OnShopChange() { if(shopChangeEvent!=null) shopChangeEvent(); }

	/**
	 * Fired when the data have been retrieved from the server.
	 */
	public static event Action inventoryReadyEvent;
	public static void OnInventoryReady() { if(inventoryReadyEvent!=null) inventoryReadyEvent(); }

	/**
	 * Fired when the data changes.
	 */
	public static event Action inventoryChangeEvent;
	public static void OnInventoryChange() { if(inventoryChangeEvent!=null) inventoryChangeEvent(); }

	/**
	 * Fired when the data have been retrieved from the server.
	 */
	public static event Action cacheReadyEvent;
	public static void OnCacheReady() { if(cacheReadyEvent!=null) cacheReadyEvent(); }

	/**
	 * Fired when the data changes.
	 */
	public static event Action cacheChangeEvent;
	public static void OnCacheChange() { if(cacheChangeEvent!=null) cacheChangeEvent(); }

	/**
	 * Fired when the data have been retrieved from the server.
	 */
	public static event Action tasksReadyEvent;
	public static void OnTasksReady() { if(tasksReadyEvent!=null) tasksReadyEvent(); }

	/**
	 * Fired when the data changes.
	 */
	public static event Action tasksChangeEvent;
	public static void OnTasksChange() { if(tasksChangeEvent!=null) tasksChangeEvent(); }

	/**
	 * @todo Ugly to be using a hash here.
	 * @todo Implement more server update functions.
	 */
	public static event Action<IXMLNode> roarServerUpdateEvent;
	public static void OnRoarServerUpdate( IXMLNode info ) { if(roarServerUpdateEvent!=null) roarServerUpdateEvent(info); }

	/**
	 * @todo Ugly to be using a hash here.
	 * @todo Implement more server update functions.
	 */
	public static event Action<IXMLNode> roarServerItemUseEvent;
	public static void OnRoarServerItemUse( IXMLNode info ) { if(roarServerItemUseEvent!=null) roarServerItemUseEvent(info); }

	/**
	 * @todo Ugly to be using a hash here.
	 * @todo Implement more server update functions.
	 */
	public static event Action<IXMLNode> roarServerItemLoseEvent;
	public static void OnRoarServerItemLose( IXMLNode info ) { if(roarServerItemLoseEvent!=null) roarServerItemLoseEvent(info); }

	/**
	 * @todo Ugly to be using a hash here.
	 * @todo Implement more server update functions.
	 */
	public static event Action<IXMLNode> roarServerInventoryChangedEvent;
	public static void OnRoarServerInventoryChanged( IXMLNode info ) { if(roarServerInventoryChangedEvent!=null) roarServerInventoryChangedEvent(info); }

	/**
	 * @todo Ugly to be using a hash here.
	 * @todo Implement more server update functions.
	 */
	public static event Action<IXMLNode> roarServerRegenEvent;
	public static void OnRoarServerRegen( IXMLNode info ) { if(roarServerRegenEvent!=null) roarServerRegenEvent(info); }

	/**
	 * @todo Ugly to be using a hash here.
	 * @todo Implement more server update functions.
	 */
	public static event Action<IXMLNode> roarServerItemAddEvent;
	public static void OnRoarServerItemAdd( IXMLNode info ) { if(roarServerItemAddEvent!=null) roarServerItemAddEvent(info); }

	/**
	 * @todo Ugly to be using a hash here.
	 * @todo Implement more server update functions.
	 */
	public static event Action<IXMLNode> roarServerTaskCompleteEvent;
	public static void OnRoarServerTaskComplete( IXMLNode info ) { if(roarServerTaskCompleteEvent!=null) roarServerTaskCompleteEvent(info); }


  	/**
  	 * Fire the correct event for a server chunk.
	 *
  	 * @param key the event name.
  	 * @param info the event info.
  	 **/
	public static void OnServerEvent( string key, IXMLNode info )
	{
		switch(key)
		{
			case "update":
				OnRoarServerUpdate(info);
				break;
			case "item_use":
				OnRoarServerItemUse(info);
				break;
			case "item_lose":
				OnRoarServerItemLose(info);
				break;
			case "inventory_changed":
				OnRoarServerInventoryChanged(info);
				break;
			case "regen":
				OnRoarServerRegen(info);
				break;
			case "item_add":
				OnRoarServerItemAdd(info);
				break;
			case "task_complete":
				OnRoarServerTaskComplete(info);
				break;

			default:
				Debug.Log("Server event "+key+" not yet implemented");
				break;
		}
	}

	/**
	 * Fire the correct event for a component change.
	 *
	 * @param name The name of the event.
	 */
	public static void OnComponentChange( string name )
	{
		switch(name)
		{
		case "properties":
			OnPropertiesChange();
			break;
		case "shop":
			OnShopChange();
			break;
		case "inventory":
			OnInventoryChange();
			break;
		case "cache":
			OnCacheChange();
			break;
		case "tasks":
			OnTasksChange();
			break;
		case "leaderboards":
			OnLeaderboardsChange();
			break;
		case "ranking":
			OnRankingChange();
			break;
		default:
			Debug.Log ("Component change event for "+name+" not yet implemented");
			break;
		}
	}

	/**
	 * Fire the correct event for a component ready.
	 *
	 * @param name The name of the event.
	 */
	public static void OnComponentReady( string name )
	{
		switch(name)
		{
		case "properties":
			OnPropertiesReady();
			break;
		case "shop":
			OnShopReady();
			break;
		case "inventory":
			OnInventoryReady();
			break;
		case "cache":
			OnCacheReady();
			break;
		case "tasks":
			OnTasksReady();
			break;
		case "leaderboards":
			OnLeaderboardsReady();
			break;
		case "ranking":
			OnRankingReady();
			break;
		default:
			Debug.Log ("Component ready event for "+name+" not yet implemented");
			break;
		}
	}

	/**
	 * Fire off the events for all the contained server events.
	 */
	public static void NotifyOfServerChanges( IXMLNode node )
	{
		if( node == null ) return;
		OnRoarServerAll( node );
		foreach( IXMLNode nn in node.Children )
		{
			OnServerEvent( nn.Name, nn );
		}
	}

}
