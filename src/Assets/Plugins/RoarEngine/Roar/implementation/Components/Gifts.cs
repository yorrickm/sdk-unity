using System.Collections;
using Roar.Components;
using UnityEngine;

namespace Roar.implementation.Components
{
	public class Gifts : IGifts
	{
		protected DataStore dataStore;
		protected ILogger logger;

		public Gifts (DataStore dataStore, ILogger logger)
		{
			this.dataStore = dataStore;
			this.logger = logger;
		}

		public void Fetch (Roar.Callback callback)
		{
			dataStore.gifts.Fetch (callback);
		}

		public bool HasDataFromServer { get { return dataStore.gifts.HasDataFromServer; } }

		public ArrayList List ()
		{
			return List (null);
		}

		public ArrayList List (Roar.Callback callback)
		{
			if (callback != null)
				callback (new Roar.CallbackInfo<object> (dataStore.gifts.List ()));
			return dataStore.gifts.List ();
		}

		// Returns the gift Hashtable associated with attribute `id`
		public Hashtable GetGift (string id)
		{
			return GetGift (id, null);
		}

		public Hashtable GetGift (string id, Roar.Callback callback)
		{
			if (callback != null)
				callback (new Roar.CallbackInfo<object> (dataStore.gifts.Get (id)));
			return dataStore.gifts.Get (id);
		}
	}
}
