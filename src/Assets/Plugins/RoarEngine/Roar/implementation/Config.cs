using System.Collections;

namespace Roar.implementation
{

public class Config : Roar.IConfig
{
    public bool IsDebug { get; set; }
    public string Game { get; set; }
    public string AuthToken { get; set; }
    public string RoarAPIUrl { get; set; }

    public Config()
    {
       IsDebug = true;
       Game = "";
       AuthToken = "";
       RoarAPIUrl = "https://api.roar.io/";

// TODO : These exist in javascript version but are not yet used here!
//    props["apiOnly"]=false;
//    props["applifierAppID"]=null;
//    props["applifierBarType"]="leaderboard";
//    props["autoLogin"]=false;
//
//    // Automatic load data flags
//    props["autoLoadPlayerData"]=true;
//    props["autoLoadProperties"]=false;
//    props["autoLoadInventory"]=false;
//    props["autoLoadBadges"]=false;
//    props["autoLoadFriends"]=false;
//    props["autoLoadShops"]=false;
//    props["autoLoadActions"]=false;
//    props["autoLoadGifts"]=false;
//
//    props["clariticsAPIKey"]=null;
//    props["connectionTimeout"]=5000;
//    props["platform"]="web";
//    props["pollInterval"]=0;
//    props["propertyNotices"]=new ArrayList();
//    props["superrewardsAppID"]=null;
//    props["uiDimensions"]= new Hashtable();
//	(props["uiDimensions"] as Hashtable)["height"]=320;
//	(props["uiDimensions"] as Hashtable)["width"]=480 ;
//
//	// 3rd Party Adapters
//	props["urbanAirshipEnabled"]=false;
//
    }
}

}

