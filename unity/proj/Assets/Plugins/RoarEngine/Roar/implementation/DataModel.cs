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
using System.Collections.Generic;
using UnityEngine;
using DC = Roar.implementation.DataConversion;

public class DataModel
{
	public string name;
	public Hashtable attributes = new Hashtable ();
	private Hashtable previousAttributes = new Hashtable ();
	private bool hasChanged = false;
	private string serverDataAPI;
	private string node;
	private bool isServerCalling = false;
	public bool HasDataFromServer { get; set; }
	protected DC.IXmlToHashtable xmlParser;
	protected IRequestSender api;
	protected Roar.ILogger logger;

	public DataModel (string name, string url, string node, ArrayList conditions, DC.IXmlToHashtable xmlParser, IRequestSender api, Roar.ILogger logger)
	{
		this.name = name;
		serverDataAPI = url;
		this.node = node;
		this.xmlParser = xmlParser;
		this.api = api;
		this.logger = logger;
	}

	// Return code for calls attempting to access/modify Model data
	// if none is present
	private void OnNoData ()
	{
		OnNoData (null);
	}

	private void OnNoData (string key)
	{
		string msg = "No data intialised for Model: " + name;
		if (key != null)
			msg += " (Invalid access for \"" + key + "\")";

		logger.DebugLog ("[roar] -- " + msg);
	}

	// Removes all attributes from the model
	public void Clear (bool silent = false)
	{ 
		attributes = new Hashtable ();
    
		// Set internal changed flag
		this.hasChanged = true;

		if (!silent) {
			RoarManager.OnComponentChange (name);
		}
	}


	// Internal call to retrieve model data from server and pass back
	// to `callback`. `params` is optional obj to pass to RoarAPI call.
	// `persistModel` optional can prevent Model data clearing.
	public bool Fetch (Roar.Callback cb)
	{
		return Fetch (cb, null, false);
	}

	public bool Fetch (Roar.Callback cb, Hashtable p)
	{
		return Fetch (cb, p, false);
	}

	public bool Fetch (Roar.Callback cb, Hashtable p, bool persist)
	{
		// Bail out if call for this Model is already underway
		if (this.isServerCalling)
			return false;

		// Reset the internal register
		if (!persist)
			attributes = new Hashtable ();

		// Using direct call (serverDataAPI url) rather than API mapping
		// - Unity doesn't easily support functions as strings: func['sub']['mo']()
		api.MakeCall (serverDataAPI, p, new OnFetch (cb, this));

		this.isServerCalling = true;
		return true;
	}

	private class OnFetch : SimpleRequestCallback<IXMLNode>
	{
		protected DataModel model;

		public OnFetch (Roar.Callback in_cb, DataModel in_model) : base(in_cb)
		{
			model = in_model;
		}
    
		public override void Prologue ()
		{
			// Reset this function call
			model.isServerCalling = false;
		}
  
		public override object OnSuccess (Roar.CallbackInfo<IXMLNode> info)
		{
			model.logger.DebugLog ("onFetch got given: " + info.data.DebugAsString ());

			// First process the data for Model use
			string[] t = model.serverDataAPI.Split ('/');
			if (t.Length != 2)
				throw new System.ArgumentException ("Invalid url format - must be abc/def");
			string path = "roar>0>" + t [0] + ">0>" + t [1] + ">0>" + model.node;
			List<IXMLNode> nn = info.data.GetNodeList (path);
			if (nn == null) {
				model.logger.DebugLog (string.Format ("Unable to get node\nFor path = {0}\nXML = {1}", path, info.data.DebugAsString ()));
			} else {
				model.ProcessData (nn);
			}

			return model.attributes;
		}
	}

	// Preps the data from server and places it within the Model
	private void ProcessData (List<IXMLNode> d)
	{
		Hashtable o = new Hashtable ();

		if (d == null)
			logger.DebugLog ("[roar] -- No data to process!");
		else {
			for (var i=0; i<d.Count; i++) {
				string key = xmlParser.GetKey (d [i]);
				if (key == null) {
					logger.DebugLog (string.Format ("no key found for {0}", d [i].DebugAsString ()));
					continue;
				}
				Hashtable hh = xmlParser.BuildHashtable (d [i]);
				if (o.ContainsKey (key)) {
					logger.DebugLog ("Duplicate key found");
				} else {
					o [key] = hh;
				}
			}
		}

		// Flag server cache called
		// Must do before `set()` to flag before change events are fired
		HasDataFromServer = true;

		// Update the Model
		this.Set (o);

		logger.DebugLog ("Setting the model in " + name + " to : " + Roar.Json.ObjectToJSON (o));
		logger.DebugLog ("[roar] -- Data Loaded: " + name);

		// Broadcast data ready event
		RoarManager.OnComponentReady (this.name);
	}


	// Shallow clone object
	public static Hashtable Clone (Hashtable obj)
	{
		if (obj == null)
			return null;

		Hashtable copy = new Hashtable ();
		foreach (DictionaryEntry prop in obj) {
			copy [prop.Key] = prop.Value;
		}

		return copy;
	}


	// Have to prefix 'set' as '_set' due to Unity function name restrictions
	public DataModel Set (Hashtable data)
	{
		return Set (data, false);
	}

	public DataModel Set (Hashtable data, bool silent)
	{
		// Setup temporary copy of attributes to be assigned
		// to the previousAttributes register if a change occurs
		var prev = Clone (this.attributes);

		foreach (DictionaryEntry prop in data) {
			this.attributes [prop.Key] = prop.Value;

			// Set internal changed flag
			this.hasChanged = true;

			// Broadcasts an attribute specific change event of the form:
			// **change:attribute_name**
			if (!silent) {
				RoarManager.OnComponentChange (this.name);
			}
		}

		// Broadcasts a `change` event if the model changed
		if (HasChanged && !silent) { 
			this.previousAttributes = prev;
			this.Change ();
		}

		return this;
	}


	// Removes an attribute from the data model
	// and fires a change event unless `silent` is passed as an option
	public void Unset (string key)
	{
		Unset (key, false);
	}

	public void Unset (string key, bool silent)
	{
		// Setup temporary copy of attributes to be assigned
		// to the previousAttributes register if a change occurs
		var prev = Clone (this.attributes);

		// Check that server data is present
		if (!HasDataFromServer) {
			this.OnNoData (key);
			return;
		}

		if (this.attributes [key] != null) {
			// Remove the specific element
			this.attributes.Remove (key);

			this.hasChanged = true;
			// Broadcasts an attribute specific change event of the form:
			// **change:attribute_name**
			if (!silent) {
				RoarManager.OnComponentChange (this.name);
			}
		}

		// Broadcasts a `change` event if the model changed
		if (HasChanged && !silent) {
			this.previousAttributes = prev;
			this.Change ();
		}
	}

	// Returns the value of a given data key (usually an object)
	// Using '_get' due to Unity restrictions on function names
	public Hashtable Get (string key)
	{
		// Check that server data is present
		if (!HasDataFromServer) {
			this.OnNoData (key);
			return null;
		}

		if (this.attributes [key] != null) {
			return this.attributes [key] as Hashtable;
		}
		logger.DebugLog ("[roar] -- No property found: " + key);
		return null;
	}

	// Returns the embedded value within an object attribute
	public string GetValue (string key)
	{
		var o = this.Get (key);
		if (o != null)
			return o ["value"] as string;
		else
			return null;
	}

	// Returns an array of all the elements in this.attributes
	public ArrayList List ()
	{
		var l = new ArrayList ();
    
		// Check that server data is present
		if (!HasDataFromServer) {
			this.OnNoData ();
			return l;
		}

		foreach (DictionaryEntry prop in this.attributes) {
			l.Add (prop.Value);
		}
		return l;
	}

	// Returns the object of an attribute key from the PREVIOUS register
	public Hashtable Previous (string key)
	{
		// Check that server data is present
		if (!HasDataFromServer) {
			this.OnNoData (key);
			return null;
		}

		if (this.previousAttributes [key] != null)
			return this.previousAttributes [key] as Hashtable;
		else
			return null;
	}

	// Checks whether element `key` is present in the
	// list of ikeys in the Model. Optional `number` to search, default 1
	// Returns true if player has equal or greater number, false if not, and
	// null for an invalid query.
	public bool Has (string key)
	{
		return Has (key, 1);
	}

	public bool Has (string key, int number)
	{
		// Fire warning *only* if no data intitialised, but continue
		if (!HasDataFromServer) {
			this.OnNoData (key);
			return false;
		}

		int count = 0;
		foreach (DictionaryEntry i in this.attributes) {
			// Search `ikey`, `id` and `shop_ikey` keys and increment counter if found
			if ((i.Value as Hashtable) ["ikey"] as string == key)
				count++;
			else if ((i.Value as Hashtable) ["id"] as string == key)
				count++;
			else if ((i.Value as Hashtable) ["shop_ikey"] as string == key)
				count++;
		}

		if (count >= number)
			return true;
		else {
			return false;
		}
	}

	// Similar to Model.Has(), but returns the number of elements in the
	// Model of id or ikey `key`.
	public int Quantity (string key)
	{
		// Fire warning *only* if no data initialised, but continue
		if (!HasDataFromServer) {
			this.OnNoData (key);
			return 0;
		}

		int count = 0;
		foreach (DictionaryEntry i in this.attributes) {
			// Search `ikey`, `id` and `shop_ikey` keys and increment counter if found
			if ((i.Value as Hashtable) ["ikey"] as string == key)
				count++;
			else if ((i.Value as Hashtable) ["id"] as string == key)
				count++;
			else if ((i.Value as Hashtable) ["shop_ikey"] as string == key)
				count++;
		}

		return count;
	}

	// Flag to indicate whether the model has changed since last "change" event
	public bool HasChanged
	{
		get
		{
			return hasChanged;
		}
	}

	// Manually fires a "change" event on this model
	public void Change ()
	{
		RoarManager.OnComponentChange (this.name);
		this.hasChanged = false;
	}
}
