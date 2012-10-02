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
using DC = Roar.implementation.DataConversion;
using System.Collections;
namespace Roar.implementation
{
	public class ItemCache : DataModel
	{
		public ItemCache ( string name, string url, string node, ArrayList conditions, DC.IXmlToHashtable xmlParser, IRequestSender api, Roar.ILogger logger )
		: base(name,url,node,conditions,xmlParser,api,logger)
		{
		}
		
		/**
	    * Fetches details about `items` array and adds to item Cache Model
	    */
	  public bool addToCache( ArrayList items, Roar.Callback cb=null )
	  {
	    ArrayList batch = itemsNotInCache( items );
	
	    // Make the call if there are new items to fetch,
	    // passing the `batch` list and persisting the Model data (adding)  
	    // Returns `true` if items are to be added, `false` if nothing to add
	    if (batch.Count>0)
	    {
	      var keysAsJSON = Roar.Json.ArrayToJSON(batch) as string;
	      Hashtable args = new Hashtable();
	      args["item_ikeys"] = keysAsJSON;
	      fetch( cb, args, true);
	      return true;
	    }
	    else return false;
	  }
		
		/**
	   * Takes an array of items and returns an new array of any that are 
	   * NOT currently in cache.
	   */
	  public ArrayList itemsNotInCache( ArrayList items )
	  {
	    // First build a list of "new" items to add to cache
	    if (!hasDataFromServer){ return items.Clone() as ArrayList; }
	
	    var batch = new ArrayList();
	    for (int i=0;i<items.Count;i++)
	      if (!has( (items[i] as string) )) batch.Add( items[i] );
	
	    return batch;
	  }
	}
	

}

