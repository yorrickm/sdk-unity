using UnityEngine;
using System.Collections;

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
		|| (whenToFetch == WhenToFetch.Once && !properties.hasDataFromServer)
		|| (whenToFetch == WhenToFetch.Occassionally && (whenLastFetched == 0 || Time.realtimeSinceStartup - whenLastFetched >= howOftenToFetch))
		)
		{
			Fetch();
		}
	}

	public void Fetch()
	{
		isFetching = true;
		properties.fetch(OnRoarFetchPropertiesComplete);
	}

	void OnRoarFetchPropertiesComplete(Roar.CallbackInfo info)
	{
		whenLastFetched = Time.realtimeSinceStartup;
		isFetching = false;
		stats = properties.list();
	}
		
	protected override void DrawGUI()
	{
		// TEMP
		if (isFetching)
		{
			GUI.Label(new Rect(Screen.width/2f - 256,Screen.height/2f - 32,512,64), "Fetching stats...", skin.FindStyle("StatusNormal"));
		}
		else
		{
			if (!properties.hasDataFromServer || stats == null || stats.Count == 0)
				GUI.Label(new Rect(Screen.width/2f - 256,Screen.height/2f - 32,512,64), "No stats to display", skin.FindStyle("StatusNormal"));
		}
		// /TEMP
	}
}
