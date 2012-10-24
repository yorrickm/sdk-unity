using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RoarStatsModule : RoarModule
{
	public enum WhenToFetch { OnEnable, Once, Occassionally, Manual };
	public WhenToFetch whenToFetch = WhenToFetch.Occassionally;
	public float howOftenToFetch = 60;
	
	private bool isFetching;
	private float whenLastFetched;
	private ArrayList stats;
	
	private Roar.Components.IProperties properties;
	
	void OnEnable()
	{
		properties = DefaultRoar.Instance.Properties;
		
		if (whenToFetch == WhenToFetch.OnEnable 
		|| (whenToFetch == WhenToFetch.Once && !properties.HasDataFromServer)
		|| (whenToFetch == WhenToFetch.Occassionally && (whenLastFetched == 0 || Time.realtimeSinceStartup - whenLastFetched >= howOftenToFetch))
		)
		{
			Fetch();
		}
	}

	public void Fetch()
	{
		if (!properties.HasDataFromServer)
		{
			isFetching = true;
			properties.Fetch(OnRoarFetchPropertiesComplete);
		}
		else
		{
			whenLastFetched = Time.realtimeSinceStartup;
			stats = properties.List();

			foreach (Hashtable hashTable in stats)
			{
				foreach (DictionaryEntry kvp in hashTable)
				{
					Debug.Log(string.Format("{0} -> {1}", kvp.Key, kvp.Value));
				}
			}
		}
	}

	void OnRoarFetchPropertiesComplete(Roar.CallbackInfo info)
	{
		whenLastFetched = Time.realtimeSinceStartup;
		isFetching = false;
		stats = properties.List();
	}
		
	protected override void DrawGUI()
	{
		if (isFetching)
		{
			GUI.Label(new Rect(Screen.width/2f - 256,Screen.height/2f - 32,512,64), "Fetching stats...", "StatusNormal");
		}
		else
		{
			if (!properties.HasDataFromServer || stats == null || stats.Count == 0)
			{
				GUI.Label(new Rect(Screen.width/2f - 256,Screen.height/2f - 32,512,64), "No stats to display", "StatusNormal");
			}
			else
			{
				/*
				Rect entry = new Rect(0,0,512,32);
				foreach (Hashtable hashTable in stats)
				{
					foreach (DictionaryEntry kvp in hashTable)
					{
						Debug.Log(string.Format("{0} -> {1}", kvp.Key, kvp.Value));
					}
				}
				*/
			}
		}
	}
}
