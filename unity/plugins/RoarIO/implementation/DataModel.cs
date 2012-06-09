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
  public string _name;
  public Hashtable attributes = new Hashtable();

  private Hashtable _previousAttributes = new Hashtable();
  private bool _hasChanged = false;
  private string _serverDataAPI;
  private string _node;

  private bool _isServerCalling = false;
  public bool hasDataFromServer = false;

  protected DC.IXmlToHashtable xmlParser_;
  protected IRequestSender api_;
  protected Roar.ILogger logger_;

  public DataModel( string name, string url, string node, ArrayList conditions, DC.IXmlToHashtable xmlParser, IRequestSender api, Roar.ILogger logger )
  {
    _name = name;
    _serverDataAPI = url;
    _node = node;
    xmlParser_ = xmlParser;
    api_ = api;
    logger_=logger;
  }

  // Return code for calls attempting to access/modify Model data
  // if none is present
  private void onNoData() { onNoData( null ); }
  private void onNoData( string key )
  {
    string msg = "No data intialised for Model: " + _name;
    if (key!=null) msg  += " (Invalid access for \""+key+"\")";

    logger_.DebugLog( "[roar] -- "+msg );
  }

  // Removes all attributes from the model
  public void clear( bool silent = false )
  { 
    attributes = new Hashtable();
    
    // Set internal changed flag
    this._hasChanged = true;

    if ( !silent ) { RoarIOManager.OnComponentChange( _name ); }
  }


  // Internal call to retrieve model data from server and pass back
  // to `callback`. `params` is optional obj to pass to RoarAPI call.
  // `persistModel` optional can prevent Model data clearing.
  public bool fetch( Roar.Callback cb ) { return fetch( cb, null, false ); }
  public bool fetch( Roar.Callback cb, Hashtable p ) { return fetch( cb, p, false ); }
  public bool fetch( Roar.Callback cb, Hashtable p, bool persist ) 
  {
    // Bail out if call for this Model is already underway
    if (this._isServerCalling) return false;

    // Reset the internal register
    if (!persist) attributes = new Hashtable();

    // Using direct call (serverDataAPI url) rather than API mapping
    // - Unity doesn't easily support functions as strings: func['sub']['mo']()
    api_.make_call( _serverDataAPI, p, new OnFetch(cb,this) );

    this._isServerCalling = true;
    return true;
  }

  private class OnFetch : SimpleRequestCallback<IXMLNode>
  {
    protected DataModel model;
    public OnFetch( Roar.Callback in_cb, DataModel in_model ) : base(in_cb)
    {
      model = in_model;
    }
    
    public override void prologue()
    {
      // Reset this function call
      model._isServerCalling = false;
    }
  
    public override object onSuccess( Roar.CallbackInfo<IXMLNode> info )
    {
      model.logger_.DebugLog ("onFetch got given: "+info.data.DebugAsString() );

      // First process the data for Model use
      string[] t = model._serverDataAPI.Split('/');
      if( t.Length != 2 ) throw new System.ArgumentException("Invalid url format - must be abc/def");
      string path = "roar>0>"+t[0]+">0>"+t[1]+">0>"+model._node;
      List<IXMLNode> nn = info.data.GetNodeList(path);
      if(nn==null)
      {
        model.logger_.DebugLog ( string.Format("Unable to get node\nFor path = {0}\nXML = {1}", path, info.data.DebugAsString()) );
      }
      else
      {
        model._processData( nn );
      }

      return model.attributes;
    }
  }

  // Preps the data from server and places it within the Model
  private void _processData( List<IXMLNode> d )
  {
    Hashtable _o = new Hashtable();

    if (d==null) logger_.DebugLog("[roar] -- No data to process!");
    else
    {
      for (var i=0; i<d.Count; i++)
      {
        string key = xmlParser_.GetKey(d[i]);
        if(key==null)
        {
          logger_.DebugLog( string.Format ("no key found for {0}", d[i].DebugAsString() ) );
          continue;
        }
        Hashtable hh = xmlParser_.BuildHashtable(d[i]);
        if( _o.ContainsKey(key) )
        {
          logger_.DebugLog ("Duplicate key found");
        }
        else
        {
          _o[key] = hh;
        }
      }
    }

    // Flag server cache called
    // Must do before `set()` to flag before change events are fired
    this.hasDataFromServer = true;

    // Update the Model
    this._set( _o );

    logger_.DebugLog ("Setting the model in "+_name+" to : "+Roar.Json.ObjectToJSON(_o) );
    logger_.DebugLog("[roar] -- Data Loaded: " + _name);

    // Broadcast data ready event
    RoarIOManager.OnComponentReady(this._name);
  }


  // Shallow clone object
  public static Hashtable _clone( Hashtable obj )
  {
    if (obj==null) return null;

    Hashtable copy = new Hashtable();
    foreach (DictionaryEntry prop in obj)
    {
      copy[ prop.Key ] = prop.Value;
    }

    return copy;
  }


  // Have to prefix 'set' as '_set' due to Unity function name restrictions
  public DataModel _set( Hashtable data ) { return _set(data,false); }
  public DataModel _set( Hashtable data, bool silent )
  {
    // Setup temporary copy of attributes to be assigned
    // to the previousAttributes register if a change occurs
    var prev = _clone( this.attributes );

    foreach (DictionaryEntry prop in data)
    {
      this.attributes[ prop.Key ] = prop.Value;

      // Set internal changed flag
      this._hasChanged = true;

      // Broadcasts an attribute specific change event of the form:
      // **change:attribute_name**
      if (!silent) { RoarIOManager.OnComponentChange(this._name); }
    }

    // Broadcasts a `change` event if the model changed
    if (hasChanged() && !silent) 
    { 
      this._previousAttributes = prev;
      this.change();
    }

    return this;
  }


  // Removes an attribute from the data model
  // and fires a change event unless `silent` is passed as an option
  public void unset( string key ) { unset(key,false); }
  public void unset( string key, bool silent )
  {
    // Setup temporary copy of attributes to be assigned
    // to the previousAttributes register if a change occurs
    var prev = _clone( this.attributes );

    // Check that server data is present
    if ( !this.hasDataFromServer ) { this.onNoData( key ); return; }

    if (this.attributes[key]!=null)
    {
      // Remove the specific element
      this.attributes.Remove( key );

      this._hasChanged = true;
      // Broadcasts an attribute specific change event of the form:
      // **change:attribute_name**
      if (!silent) { RoarIOManager.OnComponentChange(this._name); }
    }

    // Broadcasts a `change` event if the model changed
    if (hasChanged() && !silent) 
    {
      this._previousAttributes = prev;
      this.change();
    }
  }

  // Returns the value of a given data key (usually an object)
  // Using '_get' due to Unity restrictions on function names
  public Hashtable _get( string key )
  {
    // Check that server data is present
    if ( !this.hasDataFromServer ) { this.onNoData( key ); return null; }

    if (this.attributes[key]!=null) { return this.attributes[key] as Hashtable; }
    logger_.DebugLog("[roar] -- No property found: "+key);
    return null;
  }

  // Returns the embedded value within an object attribute
  public string getValue( string key )
  {
    var o = this._get(key);
    if (o!=null) return o["value"] as string;
    else return null;
  }

  // Returns an array of all the elements in this.attributes
  public ArrayList list()
  {
    var l = new ArrayList();
    
    // Check that server data is present
    if ( !this.hasDataFromServer ) { this.onNoData(); return l; }

    foreach (DictionaryEntry prop in this.attributes)
    {
      l.Add( prop.Value );
    }
    return l;
  }

  // Returns the object of an attribute key from the PREVIOUS register
  public Hashtable previous( string key )
  {
    // Check that server data is present
    if ( !this.hasDataFromServer ) { this.onNoData( key ); return null; }

    if (this._previousAttributes[key]!=null) return this._previousAttributes[key] as Hashtable;
    else return null;
  }

  // Checks whether element `key` is present in the
  // list of ikeys in the Model. Optional `number` to search, default 1
  // Returns true if player has equal or greater number, false if not, and
  // null for an invalid query.
  public bool has( string key ) { return has( key, 1); }
  public bool has( string key, int number )
  {
    // Fire warning *only* if no data intitialised, but continue
    if ( !this.hasDataFromServer ) { this.onNoData( key ); return false; }

    int count = 0;
    foreach (DictionaryEntry i in this.attributes)
    {
      // Search `ikey`, `id` and `shop_ikey` keys and increment counter if found
      if ( (i.Value as Hashtable)["ikey"] as string == key) count++;
      else if ( (i.Value as Hashtable)["id"] as string == key) count++;
      else if ( (i.Value as Hashtable)["shop_ikey"] as string == key) count++;
    }

    if (count >= number) return true;
    else { return false; }
  }

  // Similar to Model.has(), but returns the number of elements in the
  // Model of id or ikey `key`.
  public int quantity( string key )
  {
    // Fire warning *only* if no data initialised, but continue
    if ( !this.hasDataFromServer ) { this.onNoData( key ); return 0; }

    int count = 0;
    foreach (DictionaryEntry i in this.attributes)
    {
      // Search `ikey`, `id` and `shop_ikey` keys and increment counter if found
      if ( (i.Value as Hashtable)["ikey"] as string == key) count++;
      else if ( (i.Value as Hashtable)["id"] as string == key) count++;
      else if ( (i.Value as Hashtable)["shop_ikey"] as string == key) count++;
    }

    return count;
  }

  // Flag to indicate whether the model has changed since last "change" event
  public bool hasChanged()
  {
    return this._hasChanged;
  }

  // Manually fires a "change" event on this model
  public void change()
  {
    RoarIOManager.OnComponentChange(this._name);
    this._hasChanged = false;
  }
}
