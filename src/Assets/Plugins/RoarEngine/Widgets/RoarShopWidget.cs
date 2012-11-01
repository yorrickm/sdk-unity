using UnityEngine;
using System.Collections;

public class RoarShopWidget : RoarUIWidget
{
	public enum WhenToFetch { OnEnable, Once, Occassionally, Manual };
	public WhenToFetch whenToFetch = WhenToFetch.Occassionally;
	public float howOftenToFetch = 60;
	
	private bool isFetching;
	private float whenLastFetched;
	private Roar.Components.IShop shop;
	
	protected override void OnEnable ()
	{
		base.OnEnable ();
		shop = DefaultRoar.Instance.Shop;
		if (shop != null)
		{
			if (whenToFetch == WhenToFetch.OnEnable 
			|| (whenToFetch == WhenToFetch.Once && !shop.HasDataFromServer)
			|| (whenToFetch == WhenToFetch.Occassionally && (whenLastFetched == 0 || Time.realtimeSinceStartup - whenLastFetched >= howOftenToFetch))
			)
			{
				Fetch();
			}
		}
		else if (Debug.isDebugBuild)
		{
			Debug.LogWarning("Shop data is null; unable to render shop widget");
		}
	}

	public void Fetch()
	{
		isFetching = true;
		shop.Fetch(OnRoarFetchShopComplete);
	}
	
	void OnRoarFetchShopComplete(Roar.CallbackInfo info)
	{
		whenLastFetched = Time.realtimeSinceStartup;
		isFetching = false;
	}
	
	protected override void DrawGUI(int windowId)
	{
	}
}
