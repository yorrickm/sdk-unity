using System.Collections;
using Roar.Components;
using UnityEngine;

namespace Roar.implementation.Components
{
	public class Leaderboards : ILeaderboards
	{
		protected DataStore dataStore;
		protected ILogger logger;

		public Leaderboards (DataStore dataStore, ILogger logger)
		{
			this.dataStore = dataStore;
			this.logger = logger;
		}

		public void Fetch (Roar.Callback callback)
		{
			dataStore.leaderboards.Fetch (callback);
		}

		public bool HasDataFromServer { get { return dataStore.leaderboards.HasDataFromServer; } }

		public ArrayList List ()
		{
			return List (null);
		}

		public ArrayList List (Roar.Callback callback)
		{
			if (callback != null)
				callback (new Roar.CallbackInfo<object> (dataStore.leaderboards.List ()));
			return dataStore.leaderboards.List ();
		}

		// Returns the leaderboard Hashtable associated with attribute `ikey`
		public Hashtable GetLeaderboard (string ikey)
		{
			return GetLeaderboard (ikey, null);
		}

		public Hashtable GetLeaderboard (string ikey, Roar.Callback callback)
		{
			if (callback != null)
				callback (new Roar.CallbackInfo<object> (dataStore.leaderboards.Get (ikey)));
			return dataStore.leaderboards.Get (ikey);
		}
	}
}
