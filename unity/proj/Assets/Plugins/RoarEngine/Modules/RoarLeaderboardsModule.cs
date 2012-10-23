using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RoarLeaderboardsModule : RoarModule
{
	public enum WhenToFetch { OnEnable, Once, Occassionally, Manual };
	public WhenToFetch whenToFetch = WhenToFetch.OnEnable;
	public float howOftenToFetch = 60;
	
	public float scrollViewHeight = 448;
	public float scrollBarPadding = 4;
	
	public float leaderboardEntryWidth = 512;
	public float leaderboardEntryHeight = 64;
	public float leaderboardEntrySpacing = 2;
	public string leaderboardEntryStyle = "LeaderboardEntryButton";
	
	public Rect leaderboardTitleRect;
	public string leaderboardTitleOnRankingStyle = "LeaderboardTitleOnRanking";
	public float leaderboardTitlePadding = 8;
	public float rankingEntryWidth = 512;
	public float rankingEntryHeight = 64;
	public float rankingEntrySpacing = 2;
	public string rankingEntryPlayerRankStyle = "LeaderboardRankingPlayerRank";
	public string rankingEntryPlayerNameStyle = "LeaderboardRankingPlayerName";
	public string rankingEntryPlayerScoreStyle = "LeaderboardRankingPlayerScore";
	
	public string rankingNavigatePageValueStyle = "LabelPageValue";
	public string rankingNavigateLeftButtonStyle = "ButtonNavigatePageLeft";
	public string rankingNavigateRightButtonStyle = "ButtonNavigatePageRight";
	
	private enum Mode { Leaderboards, Rankings };
	private Mode mode = Mode.Leaderboards;
	private bool isFetching;
	private float whenLastFetched;
	private ArrayList leaderboardsRaw;
		// each leaderboard has this data:
		// <board board_id="int" ikey="string" resource_id="int" label="string" />	
	private Roar.Components.ILeaderboards boards;	
	private List<Leaderboard> leaderboards;	
	private Leaderboard activeLeaderboard;
	private float verticalScrollbarWidth;
	private Vector3 scrollViewPosition = Vector3.zero;
	
	[System.Serializable]
	public class Leaderboard
	{
		public string id;
		public string key;
		public string label;

		public float whenLastFetched;
		public Roar.Components.IRanking ranking;
		public ArrayList rankingRaw;
		public int offset;
		public int numResults;
		public int page;
		public bool lowIsHigh;
		
		public List<Rank> ranks;
		
		public bool HasPrevious
		{
			get { return ranks != null && page > 1; }
		}
		
		public bool HasNext
		{
			get { return ranks != null && ranks.Count > 0 && ranks.Count == numResults; }
		}
	}
	[System.Serializable]
	public class Rank
	{
		public int rank;
		public string playerId;
		public string playerName;
		public string value;
	}
	
	protected override void Awake ()
	{
		base.Awake();
		
		GUIStyle verticalScrollBar = skin.FindStyle("verticalscrollbar");
		verticalScrollbarWidth = verticalScrollBar.fixedWidth;
		
		// leaderboard rankings title alignment adjustments
		switch (horizontalContentAlignment)
		{
		case AlignmentHorizontal.Center:
			leaderboardTitleRect.x += (Screen.width - leaderboardTitleRect.width)/2;
			break;
		case AlignmentHorizontal.Right:
			leaderboardTitleRect.x += Screen.width - leaderboardTitleRect.width;
			break;
		}
		leaderboardTitleRect.x += horizontalContentOffset;
		leaderboardTitleRect.y += verticalContentOffset;
	}
	
	void OnEnable()
	{
		boards = roar.Leaderboards;
		mode = Mode.Leaderboards;
		
		if (whenToFetch == WhenToFetch.OnEnable 
		|| (whenToFetch == WhenToFetch.Once && !boards.HasDataFromServer)
		|| (whenToFetch == WhenToFetch.Occassionally && (whenLastFetched == 0 || Time.realtimeSinceStartup - whenLastFetched >= howOftenToFetch))
		)
		{
			FetchLeaderboards();
		}
	}
	
	private Mode CurrentMode
	{
		get { return mode; }
		set
		{
			mode = value;
			scrollViewPosition = Vector3.zero;
		}
	}
	
	protected override void DrawGUI()
	{
		switch (mode)
		{
		case Mode.Leaderboards:
			DrawLeaderboardsGUI();
			break;
		case Mode.Rankings:
			DrawRankingsGUI();
			break;
		}
	}
	
#if UNITY_EDITOR
	void Update()
	{
		if (CurrentMode == Mode.Rankings && Input.GetKeyUp(KeyCode.Escape))
		{
			CurrentMode = Mode.Leaderboards;
		}
	}
#endif

	#region Utility
	public override void ResetToDefaultConfiguration()
	{
		base.ResetToDefaultConfiguration();
		horizontalContentAlignment = AlignmentHorizontal.Center;
		horizontalContentOffset = 0;
		verticalContentAlignment = AlignmentVertical.Top;
		verticalContentOffset = 148;
		backgroundType = RoarModule.BackgroundType.ExtentedImage;
		backgroundImageStyle = "RoundedBackground";
		backgroundColor = new Color32(199,199,199,192);
		extendedBackgroundWidth = 564;
		extendedBackgroundHeight = 532;
		whenToFetch = WhenToFetch.OnEnable;
		scrollViewHeight = 448;
		scrollBarPadding = 4;
		leaderboardEntryWidth = 512;
		leaderboardEntryHeight = 64;
		leaderboardEntrySpacing = 2;
		leaderboardEntryStyle = "LeaderboardEntryButton";
		leaderboardTitleRect = new Rect(0,0,512,24);
		leaderboardTitleOnRankingStyle = "LeaderboardTitleOnRanking";
		leaderboardTitlePadding = 8;
		leaderboardEntryWidth = 512;
		leaderboardEntryHeight = 64;
		rankingEntrySpacing = 2;
		rankingEntryPlayerRankStyle = "LeaderboardRankingPlayerRank";
		rankingEntryPlayerNameStyle = "LeaderboardRankingPlayerName";
		rankingEntryPlayerScoreStyle = "LeaderboardRankingPlayerScore";
	}
	#endregion
	
	#region Leaderboards
	
	public void FetchLeaderboards()
	{
		isFetching = true;
		boards.Fetch(OnRoarFetchLeaderboardsComplete);
	}
	
	void OnRoarFetchLeaderboardsComplete(Roar.CallbackInfo info)
	{
		whenLastFetched = Time.realtimeSinceStartup;
		isFetching = false;
		leaderboardsRaw = boards.List();
		
		leaderboards = new List<Leaderboard>(leaderboardsRaw.Count);
		Leaderboard leaderboard;
		foreach (Hashtable data in leaderboardsRaw)
		{
			leaderboard = new Leaderboard();
			leaderboard.id = data["board_id"] as string;
			leaderboard.key = data["ikey"] as string;
			leaderboard.label = data["label"] as string;
			if (leaderboard.label.Length == 0)
				leaderboard.label = string.Format("Leaderboard{0}", leaderboard.id);
			leaderboard.ranking = new Roar.implementation.Components.Ranking(leaderboard.id, roar.DataStore, roar.Logger);
			
			leaderboards.Add(leaderboard);
		}
	}
	
	void DrawLeaderboardsGUI()
	{
		if (isFetching)
		{
			GUI.Label(new Rect(Screen.width/2f - 256,Screen.height/2f - 32,512,64), "Fetching leaderboards...", "StatusNormal");
		}
		else
		{
			if (!boards.HasDataFromServer || leaderboardsRaw == null || leaderboardsRaw.Count == 0)
			{
				GUI.Label(new Rect(Screen.width/2f - 256,Screen.height/2f - 32,512,64), "No leaderboards to display", "StatusNormal");
			}
			else
			{				
				GUI.Label(leaderboardTitleRect, "Leaderboards", leaderboardTitleOnRankingStyle);
				float spacing = leaderboardTitleRect.height + leaderboardTitlePadding;
				
				Rect entry;				
				float totalHeight = (leaderboardEntryHeight + leaderboardEntrySpacing) * leaderboards.Count;
				if (totalHeight > scrollViewHeight)
				{
					entry = new Rect((Screen.width-leaderboardEntryWidth)/2 + horizontalContentOffset,verticalContentOffset + spacing,leaderboardEntryWidth + verticalScrollbarWidth + scrollBarPadding,scrollViewHeight);
					scrollViewPosition = GUI.BeginScrollView(entry, scrollViewPosition, new Rect(0,0,leaderboardEntryWidth, totalHeight));
					entry = new Rect(0,0,leaderboardEntryWidth,leaderboardEntryHeight);
				}
				else
				{
					entry = new Rect((Screen.width-leaderboardEntryWidth)/2 + horizontalContentOffset,verticalContentOffset + spacing,leaderboardEntryWidth,leaderboardEntryHeight);
				}
				
				foreach (Leaderboard leaderboard in leaderboards)
				{
					if (GUI.Button(entry, leaderboard.label, leaderboardEntryStyle))
					{
						if ((whenToFetch == WhenToFetch.OnEnable)
						||  (whenToFetch == WhenToFetch.Once && !leaderboard.ranking.HasDataFromServer)
						||  (whenToFetch == WhenToFetch.Occassionally && (leaderboard.whenLastFetched == 0 || Time.realtimeSinceStartup - leaderboard.whenLastFetched >= howOftenToFetch))
						)
						{
							FetchRankings(leaderboard, 1);
							CurrentMode = Mode.Rankings;
						}
					}
					entry.y += leaderboardEntryHeight + leaderboardEntrySpacing;
				}
				
				if (totalHeight > scrollViewHeight)
				{
					GUI.EndScrollView();
				}
			}
		}
	}
	
	#endregion
	
	#region Rankings
	
	public void FetchRankings(Leaderboard leaderboard, int page)
	{
		isFetching = true;
		activeLeaderboard = leaderboard;
		leaderboard.ranking.Page = page;
		leaderboard.ranking.Fetch(OnRoarFetchRankingsComplete);
	}

	void OnRoarFetchRankingsComplete(Roar.CallbackInfo info)
	{
		activeLeaderboard.whenLastFetched = Time.realtimeSinceStartup;
		isFetching = false;
		activeLeaderboard.rankingRaw = activeLeaderboard.ranking.List();
		foreach (Hashtable data in activeLeaderboard.rankingRaw)
		{
			foreach (DictionaryEntry datum in data)
			{
				//Debug.Log(string.Format("{0} => {1}", entry.Key, entry.Value));
				if ((string)datum.Key == "entries")
				{
					ArrayList entries = (ArrayList)datum.Value;
					activeLeaderboard.ranks = new List<Rank>(entries.Count);
					Rank rank;
					foreach (Hashtable entry in entries)
					{
						rank = new Rank();
						foreach (DictionaryEntry kv in entry)
						{
							//Debug.Log(string.Format("{0} => {1}", kv.Key, kv.Value));
							string k = kv.Key as string;
							if (k == "rank")
							{
								rank.rank = System.Convert.ToInt32(kv.Value);
							}
							else if (k == "player_id")
							{
								rank.playerId = kv.Value as string;
							}
							else if (k == "player_name")
							{
								rank.playerName = kv.Value as string;
							}
							else if (k == "value")
							{
								rank.value = kv.Value as string;
							}
						}
						activeLeaderboard.ranks.Add(rank);
					}
				}
				else if ((string)datum.Key == "properties")
				{
					Hashtable properties = (Hashtable)datum.Value;
					if (properties.ContainsKey("page"))
						activeLeaderboard.page = System.Convert.ToInt32(properties["page"]);
					if (properties.ContainsKey("num_results"))
						activeLeaderboard.numResults = System.Convert.ToInt32(properties["num_results"]);
					if (properties.ContainsKey("offset"))
						activeLeaderboard.offset = System.Convert.ToInt32(properties["offset"]);
					if (properties.ContainsKey("low_is_high"))
						activeLeaderboard.lowIsHigh = System.Convert.ToBoolean(properties["low_is_high"]);
				}
			}
		}
	}
	
	void DrawRankingsGUI()
	{
		if (isFetching)
		{
			GUI.Label(new Rect(Screen.width/2f - 256,Screen.height/2f - 32,512,64), "Fetching rankings...", "StatusNormal");
		}
		else if (activeLeaderboard != null)
		{
			if (!activeLeaderboard.ranking.HasDataFromServer || activeLeaderboard.ranks == null || activeLeaderboard.ranks.Count == 0)
			{
				GUI.Label(new Rect(Screen.width/2f - 256,Screen.height/2f - 32,512,64), "No rankings to display", "StatusNormal");
			}
			else
			{
				GUI.Label(leaderboardTitleRect, activeLeaderboard.label, leaderboardTitleOnRankingStyle);
				float spacing = leaderboardTitleRect.height + leaderboardTitlePadding;
				
				Rect entry;				
				float totalHeight = (rankingEntryHeight + rankingEntrySpacing) * activeLeaderboard.ranks.Count;
				if (totalHeight > scrollViewHeight)
				{
					entry = new Rect((Screen.width-rankingEntryWidth)/2 + horizontalContentOffset, verticalContentOffset + spacing, rankingEntryWidth + verticalScrollbarWidth + scrollBarPadding,scrollViewHeight);
					scrollViewPosition = GUI.BeginScrollView(entry, scrollViewPosition, new Rect(0,0,rankingEntryWidth, totalHeight));
					entry = new Rect(0,0,rankingEntryWidth,rankingEntryHeight);
				}
				else
				{
					entry = new Rect((Screen.width-rankingEntryWidth)/2 + horizontalContentOffset,verticalContentOffset + spacing,rankingEntryWidth,rankingEntryHeight);
				}
				
				foreach (Rank rank in  activeLeaderboard.ranks)
				{
					GUIRanking(entry, rank);
					entry.y += rankingEntryHeight + rankingEntrySpacing;
				}

				if (totalHeight > scrollViewHeight)
				{
					GUI.EndScrollView();
				}
				
				Rect pageNavigatorRect;
				if (backgroundType == RoarModule.BackgroundType.ExtentedImage)
				{
					pageNavigatorRect = new Rect((Screen.width-rankingEntryWidth)/2 + horizontalContentOffset, extendedBackgroundHeight + spacing + verticalContentOffset,rankingEntryWidth,rankingEntryHeight);
				}
				else
				{
					pageNavigatorRect = new Rect((Screen.width-rankingEntryWidth)/2 + horizontalContentOffset, scrollViewHeight + spacing + verticalContentOffset,rankingEntryWidth,rankingEntryHeight);
				}
				GUIPageNavigator(pageNavigatorRect);
			}
		}
		else
		{
			CurrentMode = Mode.Leaderboards;
		}
	}
	
	void GUIRanking(Rect rect, Rank rank)
	{
		GUI.BeginGroup(rect);
		rect.x = 0;
		rect.y = 0;
		GUI.Box(rect, string.Empty);
		
		GUI.Label(rect, rank.rank.ToString(), rankingEntryPlayerRankStyle);
		GUI.Label(rect, rank.playerName, rankingEntryPlayerNameStyle);
		GUI.Label(rect, rank.value, rankingEntryPlayerScoreStyle);
		
		GUI.EndGroup();
	}
	
	void GUIPageNavigator(Rect rect)
	{
		GUIStyle navigateButtonStyle;
		float w = rect.width;
		//float h = rect.height;

		GUI.BeginGroup(rect);
		rect.x = 0;
		rect.y = 0;
		
		navigateButtonStyle = skin.FindStyle(rankingNavigateLeftButtonStyle);
		rect.width = navigateButtonStyle.fixedWidth;
		rect.height = navigateButtonStyle.fixedHeight;
		if (activeLeaderboard.HasPrevious)
		{
			if (GUI.Button(rect, string.Empty, rankingNavigateLeftButtonStyle))
			{
				FetchRankings(activeLeaderboard, activeLeaderboard.page + 1);
			}
		}
		
		rect.width = w;
		if (activeLeaderboard.HasPrevious || activeLeaderboard.HasNext)
			GUI.Label(rect, activeLeaderboard.page.ToString(), rankingNavigatePageValueStyle);

		navigateButtonStyle = skin.FindStyle(rankingNavigateRightButtonStyle);
		rect.width = navigateButtonStyle.fixedWidth;
		rect.height = navigateButtonStyle.fixedHeight;
		rect.x = w - rect.width;
		if (activeLeaderboard.HasNext)
		{
			if (GUI.Button(rect, string.Empty, rankingNavigateRightButtonStyle))
			{
				FetchRankings(activeLeaderboard, activeLeaderboard.page - 1);
			}
		}
		
		GUI.EndGroup();
	}
	
	#endregion
}
