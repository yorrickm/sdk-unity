using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(RoarLeaderboardsModule))]
public class RoarLeaderboardsModuleInspector : RoarModuleInspector
{
	private SerializedProperty whenToFetch;
	private SerializedProperty howOftenToFetch;

	private SerializedProperty scrollViewHeight;
	private SerializedProperty scrollBarPadding;
	private SerializedProperty leaderboardEntryWidth;
	private SerializedProperty leaderboardEntryHeight;
	private SerializedProperty leaderboardEntrySpacing;
	private SerializedProperty leaderboardEntryStyle;
	private SerializedProperty leaderboardTitleRect;
	private SerializedProperty leaderboardTitleOnRankingStyle;
	private SerializedProperty leaderboardTitlePadding;
	private SerializedProperty rankingEntryWidth;
	private SerializedProperty rankingEntryHeight;
	private SerializedProperty rankingEntrySpacing;
	private SerializedProperty rankingEntryPlayerRankStyle;
	private SerializedProperty rankingEntryPlayerNameStyle;
	private SerializedProperty rankingEntryPlayerScoreStyle;
	
	protected override void OnEnable()
	{
		base.OnEnable();
		showBoundsConstraintSettings = false;
		
		whenToFetch = serializedObject.FindProperty("whenToFetch");
		howOftenToFetch = serializedObject.FindProperty("howOftenToFetch");

		scrollViewHeight = serializedObject.FindProperty("scrollViewHeight");
		scrollBarPadding = serializedObject.FindProperty("scrollBarPadding");
		leaderboardEntryWidth = serializedObject.FindProperty("leaderboardEntryWidth");
		leaderboardEntryHeight = serializedObject.FindProperty("leaderboardEntryHeight");
		leaderboardEntrySpacing = serializedObject.FindProperty("leaderboardEntrySpacing");
		leaderboardEntryStyle = serializedObject.FindProperty("leaderboardEntryStyle");
		
		leaderboardTitleRect = serializedObject.FindProperty("leaderboardTitleRect");
		leaderboardTitleOnRankingStyle = serializedObject.FindProperty("leaderboardTitleOnRankingStyle");
		leaderboardTitlePadding = serializedObject.FindProperty("leaderboardTitlePadding");
		
		rankingEntryWidth = serializedObject.FindProperty("rankingEntryWidth");
		rankingEntryHeight = serializedObject.FindProperty("rankingEntryHeight");
		rankingEntrySpacing = serializedObject.FindProperty("rankingEntrySpacing");
		rankingEntryPlayerRankStyle = serializedObject.FindProperty("rankingEntryPlayerRankStyle");
		rankingEntryPlayerNameStyle = serializedObject.FindProperty("rankingEntryPlayerNameStyle");
		rankingEntryPlayerScoreStyle = serializedObject.FindProperty("rankingEntryPlayerScoreStyle");
	}

	protected override void DrawGUI()
	{
		base.DrawGUI();
		
		// data fetching
		Comment("How often to fetch leaderboard data from the server.");
		EditorGUILayout.PropertyField(whenToFetch);
		if (whenToFetch.enumValueIndex == 2)
			EditorGUILayout.PropertyField(howOftenToFetch, new GUIContent("How Often (seconds)"));

		Comment("Miscellaneous attributes.");
		EditorGUILayout.PropertyField(scrollViewHeight);
		EditorGUILayout.PropertyField(scrollBarPadding);
		EditorGUILayout.PropertyField(leaderboardEntryWidth);
		EditorGUILayout.PropertyField(leaderboardEntryHeight);
		EditorGUILayout.PropertyField(leaderboardEntrySpacing);
		EditorGUILayout.PropertyField(leaderboardEntryStyle);

		EditorGUILayout.PropertyField(leaderboardTitleRect);
		EditorGUILayout.PropertyField(leaderboardTitleOnRankingStyle);
		EditorGUILayout.PropertyField(leaderboardTitlePadding);
		
		EditorGUILayout.PropertyField(rankingEntryWidth);
		EditorGUILayout.PropertyField(rankingEntryHeight);
		EditorGUILayout.PropertyField(rankingEntrySpacing);
		EditorGUILayout.PropertyField(rankingEntryPlayerRankStyle);
		EditorGUILayout.PropertyField(rankingEntryPlayerNameStyle);
		EditorGUILayout.PropertyField(rankingEntryPlayerScoreStyle);
	}
}
