using DC = Roar.implementation.DataConversion;
using System.Collections;

namespace Roar.implementation
{
	public class ItemCache : DataModel
	{
		public ItemCache (string name, string url, string node, ArrayList conditions, DC.IXmlToHashtable xmlParser, IRequestSender api, Roar.ILogger logger)
		: base(name,url,node,conditions,xmlParser,api,logger)
		{
		}

		/**
	    * Fetches details about `items` array and adds to item Cache Model
	    */
		public bool AddToCache (ArrayList items, Roar.Callback cb=null)
		{
			ArrayList batch = ItemsNotInCache (items);

			// Make the call if there are new items to fetch,
			// passing the `batch` list and persisting the Model data (adding)
			// Returns `true` if items are to be added, `false` if nothing to add
			if (batch.Count > 0) {
				var keysAsJSON = Roar.Json.ArrayToJSON (batch) as string;
				Hashtable args = new Hashtable ();
				args ["item_ikeys"] = keysAsJSON;
				Fetch (cb, args, true);
				return true;
			} else
				return false;
		}

		/**
	   * Takes an array of items and returns an new array of any that are
	   * NOT currently in cache.
	   */
		public ArrayList ItemsNotInCache (ArrayList items)
		{
			// First build a list of "new" items to add to cache
			if (!HasDataFromServer) {
				return items.Clone () as ArrayList;
			}

			var batch = new ArrayList ();
			for (int i=0; i<items.Count; i++)
				if (!Has ((items [i] as string)))
					batch.Add (items [i]);

			return batch;
		}
	}


}

