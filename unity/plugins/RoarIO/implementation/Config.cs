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

namespace Roar.implementation
{

public class Config : Roar.IConfig
{
    public bool isDebug { get; set; }
    public string game { get; set; }
    public string auth_token { get; set; }
    public string roar_api_url { get; set; }

    public Config()
    {
       isDebug = true;
       game = "";
       auth_token = "";
       roar_api_url = "http://api.roar.io/";
       
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

