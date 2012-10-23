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
using Roar.Components;
using UnityEngine;

namespace Roar.implementation.Components
{
	public class Ranking : IRanking
	{
		protected string boardId;
		protected int page_ = 1;
		
		protected DataStore data_store_;
		protected ILogger logger_;
		
		public Ranking(string boardId, DataStore data_store, ILogger logger)
		{
			this.boardId = boardId;
			data_store_ = data_store;
			logger_ = logger;
		}
		
		public int page
		{
			set { page_ = value; }
		}
		
		public void fetch(Roar.Callback callback)
		{ 
			Hashtable data = new Hashtable();
			data.Add("board_id", boardId);
			data.Add("page", page_.ToString());
			data_store_.Ranking_.fetch(callback, data);
		}
		
		public bool hasDataFromServer { get { return data_store_.Ranking_.hasDataFromServer; } }
		
		public ArrayList list() { return list(null); }
		public ArrayList list( Roar.Callback callback) 
		{
			if (callback!=null) callback( new Roar.CallbackInfo<object>( data_store_.Ranking_.list() ) );
			return data_store_.Ranking_.list();
		}
		
		// Returns the ranking Hashtable associated with attribute `ikey`
		public Hashtable getEntry( string ikey ) { return getEntry(ikey,null); }
		public Hashtable getEntry( string ikey, Roar.Callback callback )
		{
			if (callback!=null) callback( new Roar.CallbackInfo<object>( data_store_.Ranking_._get(ikey) ) );
			return data_store_.Ranking_._get(ikey);
		}
	}
}
