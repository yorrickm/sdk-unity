using System.Collections;
using Roar.Components;
using UnityEngine;

namespace Roar.implementation.Components
{
	public class Ranking : IRanking
	{
		protected string boardId;
		protected int page = 1;

		protected DataStore dataStore;
		protected ILogger logger;

		public Ranking(string boardId, DataStore dataStore, ILogger logger)
		{
			this.boardId = boardId;
			this.dataStore = dataStore;
			this.logger = logger;
		}

		public int Page
		{
			set { page = value; }
		}

		public void Fetch(Roar.Callback callback)
		{
			Hashtable data = new Hashtable();
			data.Add("board_id", boardId);
			data.Add("page", page.ToString());
			dataStore.ranking.Fetch(callback, data);
		}

		public bool HasDataFromServer { get { return dataStore.ranking.HasDataFromServer; } }

		public ArrayList List() { return List(null); }
		public ArrayList List( Roar.Callback callback)
		{
			if (callback!=null) callback( new Roar.CallbackInfo<object>( dataStore.ranking.List() ) );
			return dataStore.ranking.List();
		}

		// Returns the ranking Hashtable associated with attribute `ikey`
		public Hashtable GetEntry( string ikey ) { return GetEntry(ikey,null); }
		public Hashtable GetEntry( string ikey, Roar.Callback callback )
		{
			if (callback!=null) callback( new Roar.CallbackInfo<object>( dataStore.ranking.Get(ikey) ) );
			return dataStore.ranking.Get(ikey);
		}
	}
}
