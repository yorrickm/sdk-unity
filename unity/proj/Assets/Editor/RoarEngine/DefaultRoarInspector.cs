using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(DefaultRoar))]
public class DefaultRoarInspector : RoarInspector
{
	private SerializedProperty debug;
	private SerializedProperty appstoreSandbox;
	private SerializedProperty gameKey;
	private SerializedProperty xmlParser;
	
	protected override void OnEnable()
	{
		base.OnEnable();
		
		debug = serializedObject.FindProperty("debug");
		appstoreSandbox = serializedObject.FindProperty("appstoreSandbox");
		gameKey = serializedObject.FindProperty("gameKey");
		xmlParser = serializedObject.FindProperty("xmlParser");
	}
	
	protected override void DrawGUI()
	{
		// gameKey
		EditorGUILayout.Space();
		EditorGUILayout.HelpBox("The game key, as specified in the Roar dashboard", MessageType.Info);
		gameKey.stringValue = gameKey.stringValue.Trim();
		if (gameKey.stringValue.Length == 0)
			EditorGUILayout.HelpBox("Please specify your game key. It is required!", MessageType.Error);
		EditorGUILayout.PropertyField(gameKey);
		
		// debug
		EditorGUILayout.Space();
		EditorGUILayout.HelpBox("Enable debug output.", MessageType.Info);
		EditorGUILayout.PropertyField(debug);
		
		// appstoreSandbox
		EditorGUILayout.Space();
		EditorGUILayout.HelpBox("Define if the in-app purchases are executed in a sandbox.", MessageType.Info);
		EditorGUILayout.PropertyField(appstoreSandbox);
		
		// xmlType
		EditorGUILayout.Space();
		EditorGUILayout.HelpBox("The XML parser type. The lightweight parser is recommended.", MessageType.Info);
		EditorGUILayout.PropertyField(xmlParser);
	}
}
