using System.Collections;
using Roar.Components;
using UnityEngine;

namespace Roar.implementation.Components
{
	public class Achievements : IAchievements
	{
		protected DataStore dataStore;
		protected ILogger logger;

		public Achievements (DataStore dataStore, ILogger logger)
		{
			this.dataStore = dataStore;
			this.logger = logger;
		}

		public void Fetch (Roar.Callback callback)
		{
			dataStore.achievements.Fetch (callback);
		}

		public bool HasDataFromServer { get { return dataStore.achievements.HasDataFromServer; } }

		public ArrayList List ()
		{
			return List (null);
		}

		public ArrayList List (Roar.Callback callback)
		{
			if (callback != null)
				callback (new Roar.CallbackInfo<object> (dataStore.achievements.List ()));
			return dataStore.achievements.List ();
		}

		// Returns the achievement Hashtable associated with attribute `ikey`
		public Hashtable GetAchievement (string ikey)
		{
			return GetAchievement (ikey, null);
		}

		public Hashtable GetAchievement (string ikey, Roar.Callback callback)
		{
			if (callback != null)
				callback (new Roar.CallbackInfo<object> (dataStore.achievements.Get (ikey)));
			return dataStore.achievements.Get (ikey);
		}
	}
}
