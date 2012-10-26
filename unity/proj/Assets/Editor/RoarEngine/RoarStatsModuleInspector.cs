using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(RoarStatsModule))]
public class RoarStatsModuleInspector : RoarModuleInspector
{
	private SerializedProperty whenToFetch;
	private SerializedProperty howOftenToFetch;

	protected override void OnEnable()
	{
		base.OnEnable();

		whenToFetch = serializedObject.FindProperty("whenToFetch");
		howOftenToFetch = serializedObject.FindProperty("howOftenToFetch");
	}

	protected override void DrawGUI()
	{
		base.DrawGUI();
		
		// data fetching
		Comment("How often to fetch player statistics from the server.");
		EditorGUILayout.PropertyField(whenToFetch);
		if (whenToFetch.enumValueIndex == 2)
			EditorGUILayout.PropertyField(howOftenToFetch, new GUIContent("How Often (seconds)"));
	}
}
