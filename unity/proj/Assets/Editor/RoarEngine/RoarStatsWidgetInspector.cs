using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(RoarStatsWidget))]
public class RoarStatsWidgetInspector : RoarUIWidgetInspector
{
	private SerializedProperty autoCalculateContentBounds;
	private SerializedProperty defaultValueFormat;
	private SerializedProperty statsToDisplay;
	
	protected override void OnEnable ()
	{
		base.OnEnable ();
		
		autoCalculateContentBounds = serializedObject.FindProperty("autoCalculateContentBounds");
		defaultValueFormat = serializedObject.FindProperty("defaultValueFormat");
		statsToDisplay = serializedObject.FindProperty("statsToDisplay");
	}
	
	protected override void DrawGUI()
	{
		base.DrawGUI();

		Comment("Player stats to display.");
		if (useScrollView.boolValue)
		{
			EditorGUILayout.PropertyField(autoCalculateContentBounds, new GUIContent("Calculate Content Bounds"));
		}
		EditorGUILayout.PropertyField(defaultValueFormat);
		EditorGUILayout.Space();
		
		if (GUILayout.Button("Add Stat To Display"))
		{
			statsToDisplay.arraySize++;

			string path = string.Format("statsToDisplay.Array.data[{0}]", statsToDisplay.arraySize - 1);
			SerializedProperty enabled = serializedObject.FindProperty(string.Format("{0}.{1}", path, "enabled"));
			SerializedProperty key = serializedObject.FindProperty(string.Format("{0}.{1}", path, "key"));
			SerializedProperty title = serializedObject.FindProperty(string.Format("{0}.{1}", path, "title"));
			SerializedProperty titleStyle = serializedObject.FindProperty(string.Format("{0}.{1}", path, "titleStyle"));
			SerializedProperty valueFormat = serializedObject.FindProperty(string.Format("{0}.{1}", path, "valueFormat"));
			SerializedProperty valueStyle = serializedObject.FindProperty(string.Format("{0}.{1}", path, "valueStyle"));
			SerializedProperty valueType = serializedObject.FindProperty(string.Format("{0}.{1}", path, "valueType"));
			
			// default values
			enabled.boolValue = true;
			key.stringValue = string.Empty;
			title.stringValue = string.Empty;
			titleStyle.stringValue = "StatsTitleStyle";
			valueFormat.stringValue = string.Empty;
			valueStyle.stringValue = "StatsValueStyle";
			valueType.enumValueIndex = 1; // String
			
			serializedObject.ApplyModifiedProperties();
		}
		EditorGUILayout.Space();
		int statToRemove = -1;
		for (int i=0; i<statsToDisplay.arraySize; i++)
		{			
			string path = string.Format("statsToDisplay.Array.data[{0}]", i);
			SerializedProperty enabled = serializedObject.FindProperty(string.Format("{0}.{1}", path, "enabled"));
			SerializedProperty key = serializedObject.FindProperty(string.Format("{0}.{1}", path, "key"));
			SerializedProperty bounds = serializedObject.FindProperty(string.Format("{0}.{1}", path, "bounds"));
			SerializedProperty title = serializedObject.FindProperty(string.Format("{0}.{1}", path, "title"));
			SerializedProperty titleStyle = serializedObject.FindProperty(string.Format("{0}.{1}", path, "titleStyle"));
			SerializedProperty valueFormat = serializedObject.FindProperty(string.Format("{0}.{1}", path, "valueFormat"));
			SerializedProperty valueStyle = serializedObject.FindProperty(string.Format("{0}.{1}", path, "valueStyle"));
			//SerializedProperty valueType = serializedObject.FindProperty(string.Format("{0}.{1}", path, "valueType"));
			
			GUILayout.BeginVertical("box");
			{
				EditorGUILayout.PropertyField(enabled);
				EditorGUILayout.PropertyField(key);
				if (key.stringValue == string.Empty)
				{
					EditorGUILayout.HelpBox("Please supply a key.", MessageType.Warning);
				}
				EditorGUILayout.PropertyField(bounds);
				if (bounds.rectValue.width <= 0 || bounds.rectValue.height <= 0)
				{
					EditorGUILayout.HelpBox("Since the bounds width and/or height is 0, the stat will not be visible.", MessageType.Warning);
				}
				EditorGUILayout.PropertyField(title);
				EditorGUILayout.PropertyField(titleStyle);
				//EditorGUILayout.PropertyField(valueType);
				EditorGUILayout.PropertyField(valueStyle);
				EditorGUILayout.PropertyField(valueFormat);
				if (GUILayout.Button("Remove"))
				{
					if (EditorUtility.DisplayDialog("Remove Displayed Stat", "Are you sure that you want to remove this displayed stat?", "OK", "Cancel"))
					{
						statToRemove = i;
					}
				}
			}
			GUILayout.EndVertical();
		}
		if (statToRemove > -1)
		{
			for (int i=statToRemove; i<statsToDisplay.arraySize-1; i++)
				statsToDisplay.MoveArrayElement(i+1, i);
			statsToDisplay.arraySize--;
		}
	}
}
