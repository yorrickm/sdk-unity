using System.Collections;
using Roar.Components;

namespace Roar.implementation.Components
{
	public class Actions : IActions
	{
		protected DataStore dataStore;
		protected IWebAPI.ITasksActions taskActions;

		public Actions (IWebAPI.ITasksActions taskActions, DataStore dataStore)
		{
			this.taskActions = taskActions;
			this.dataStore = dataStore;
		}

		public bool HasDataFromServer { get { return dataStore.actions.HasDataFromServer; } }

		public void Fetch (Roar.Callback callback)
		{
			dataStore.actions.Fetch (callback);
		}

		public ArrayList List ()
		{
			return List (null);
		}

		public ArrayList List (Roar.Callback callback)
		{
			ArrayList listResult = dataStore.actions.List ();
			if (callback != null)
				callback (new Roar.CallbackInfo<object> (listResult));
			return listResult;
		}

		public void Execute (string ikey, Roar.Callback callback)
		{

			Hashtable args = new Hashtable ();
			args ["task_ikey"] = ikey;

			taskActions.start (args, new OnActionsDo (callback, this));
		}
		class OnActionsDo : SimpleRequestCallback<IXMLNode>
		{
			//Actions actions;

			public OnActionsDo (Roar.Callback in_cb, Actions in_actions) : base(in_cb)
			{
				//actions = in_actions;
			}

			public override object OnSuccess (CallbackInfo<IXMLNode> info)
			{
				// Event complete info (task_complete) is sent in a <server> chunk
				// (backend quirk related to potentially asynchronous tasks)
				// In this case its ALWAYS a synchronous call, so we KNOW the data will
				// be available - data is formatted in WebAPI Class.
				//var eventData = d["server"] as Hashtable;
				IXMLNode eventData = info.data.GetFirstChild ("server");

				RoarManager.OnEventDone (eventData);

				return eventData;
			}
		}
	}

}
