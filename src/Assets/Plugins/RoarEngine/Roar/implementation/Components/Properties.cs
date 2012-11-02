using System.Collections;
using Roar.Components;

namespace Roar.implementation.Components
{
	public class Properties : IProperties
	{
		protected DataStore dataStore;

		public Properties (DataStore dataStore)
		{
			this.dataStore = dataStore;
			RoarManager.roarServerUpdateEvent += this.OnUpdate;
		}

		public void Fetch (Roar.Callback callback)
		{
			dataStore.properties.Fetch (callback);
		}

		public bool HasDataFromServer { get { return dataStore.properties.HasDataFromServer; } }

		public ArrayList List ()
		{
			return List (null);
		}

		public ArrayList List (Roar.Callback callback)
		{
			if (callback != null)
				callback (new Roar.CallbackInfo<object> (dataStore.properties.List ()));
			return dataStore.properties.List ();
		}

		// Returns the *object* associated with attribute `key`
		public object GetProperty (string key)
		{
			return GetProperty (key, null);
		}

		public object GetProperty (string key, Roar.Callback callback)
		{
			if (callback != null)
				callback (new Roar.CallbackInfo<object> (dataStore.properties.Get (key)));
			return dataStore.properties.Get (key);
		}

		// Returns the *value* of attribute `key`
		public string GetValue (string ikey)
		{
			return GetValue (ikey, null);
		}

		public string GetValue (string ikey, Roar.Callback callback)
		{
			if (callback != null)
				callback (new Roar.CallbackInfo<object> (dataStore.properties.GetValue (ikey)));
			return dataStore.properties.GetValue (ikey);
		}

		protected void OnUpdate (IXMLNode update)
		{
			//Since you can get change events from login calls, when the Properties object is not yet setup we need to be careful here:
			if (! HasDataFromServer)
				return;

			//var d = event['data'] as Hashtable;

			var v = GetProperty (update.GetAttribute ("ikey")) as Hashtable;
			if (v != null) {
				v ["value"] = update.GetAttribute ("value");
			}
		}

	}
}
