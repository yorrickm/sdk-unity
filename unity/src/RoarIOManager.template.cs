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

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;


public class RoarIOManager
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


<%
  _.each( data.events.concat( data.server_events ), function(e,i,l) {
  var action_type = "Action";
  var args_with_type = "";
  var args = "";
  _.each( e.args, function(a,j,l) {
    args += a.name;
  } );
  if( e.args.length > 0 )
  {
    action_type = "Action<" +  _.map( e.args, function(x){ return x.type; } ).join("," ) + ">";
    args_with_type = " " + _.map( e.args, function(x) { return x.type+" "+x.name }). join(", ")+ " ";
    args = _.map( e.args, function(x) { return x.name }). join(", ");
  }
  var event_name = e.name + "Event";
%><% if (e.notes) { print("\n\t/**\n"); _.each(e.notes, function(x) { print("\t * "+x+".\n"); }); print("\t */"); } %>
	public static event <%= action_type %> <%= event_name %>;
	public static void On<%= capitalizeFirst(e.name) %>(<%= args_with_type %>) { if(<%= event_name %>!=null) <%= event_name %>(<%= args %>); }
<% } ) %>

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
<%
  _.each( data.server_events, function(e,i,l) {
      print("\t\t\tcase \""+e.server_name+"\":\n");
      print("\t\t\t\tOn"+capitalizeFirst(e.name)+"(info);\n");
      print("\t\t\t\tbreak;\n");
    } );
%>
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
<%
  _.each( data.components, function(e,i,l) {
      print("\t\tcase \""+e+"\":\n");
      print("\t\t\tOn"+capitalizeFirst(e)+"Change();\n");
      print("\t\t\tbreak;\n");
    } );
%>
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
<%
  _.each( data.components, function(e,i,l) {
      print("\t\tcase \""+e+"\":\n");
      print("\t\t\tOn"+capitalizeFirst(e)+"Ready();\n");
      print("\t\t\tbreak;\n");
    } );
%>
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
