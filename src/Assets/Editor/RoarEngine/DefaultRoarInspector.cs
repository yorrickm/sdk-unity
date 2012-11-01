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
	private SerializedProperty defaultGUISkin;
	
	protected override void OnEnable()
	{
		base.OnEnable();
		
		debug = serializedObject.FindProperty("debug");
		appstoreSandbox = serializedObject.FindProperty("appstoreSandbox");
		gameKey = serializedObject.FindProperty("gameKey");
		xmlParser = serializedObject.FindProperty("xmlParser");
		defaultGUISkin = serializedObject.FindProperty("defaultGUISkin");
	}
	
	protected override void DrawGUI()
	{
		// gameKey
		Comment("The game key, as specified in the Roar dashboard");
		gameKey.stringValue = gameKey.stringValue.Trim();
		if (gameKey.stringValue.Length == 0)
			EditorGUILayout.HelpBox("Please specify your game key. It is required!", MessageType.Error);
		EditorGUILayout.PropertyField(gameKey);
		
		// debug
		Comment("Enable debug output.");
		EditorGUILayout.PropertyField(debug);
		
		// appstoreSandbox
		Comment("Define if the in-app purchases are executed in a sandbox.");
		EditorGUILayout.PropertyField(appstoreSandbox);
		
		// xmlType
		Comment("The XML parser type. The lightweight parser is recommended.");
		EditorGUILayout.PropertyField(xmlParser);

		// defaultGUISkin
		Comment("The default GUI skin to use for Roar components.");
		EditorGUILayout.PropertyField(defaultGUISkin);
	}
}
