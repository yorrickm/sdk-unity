using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(RoarLoginUI))]
public class RoarLoginUIInspector : RoarUIInspector
{
	private SerializedProperty statusWidth;
	private SerializedProperty statusHeight;
	private SerializedProperty statusNormalStyle;
	private SerializedProperty statusErrorStyle;

	private SerializedProperty textfieldWidth;
	private SerializedProperty textfieldHeight;
	private SerializedProperty textFieldSpacing;
	
	private SerializedProperty buttonWidth;
	private SerializedProperty buttonHeight;
	private SerializedProperty spacingAboveButtons;
	private SerializedProperty spacingBetweenButtons;

	private SerializedProperty verticalOffset;
	
	protected override void OnEnable()
	{
		base.OnEnable();

		statusWidth = serializedObject.FindProperty("statusWidth");
		statusHeight = serializedObject.FindProperty("statusHeight");
		statusNormalStyle = serializedObject.FindProperty("statusNormalStyle");
		statusErrorStyle = serializedObject.FindProperty("statusErrorStyle");
		
		textfieldWidth = serializedObject.FindProperty("textfieldWidth");
		textfieldHeight = serializedObject.FindProperty("textfieldHeight");
		textFieldSpacing = serializedObject.FindProperty("textFieldSpacing");

		buttonWidth = serializedObject.FindProperty("buttonWidth");
		buttonHeight = serializedObject.FindProperty("buttonHeight");
		spacingAboveButtons = serializedObject.FindProperty("spacingAboveButtons");
		spacingBetweenButtons = serializedObject.FindProperty("spacingBetweenButtons");

		verticalOffset = serializedObject.FindProperty("verticalOffset");
	}

	protected override void DrawGUI()
	{
		base.DrawGUI();
		
		// status
		Comment("Status field attributes.");
		EditorGUILayout.PropertyField(statusWidth);
		EditorGUILayout.PropertyField(statusHeight);
		EditorGUILayout.PropertyField(statusNormalStyle);
		EditorGUILayout.PropertyField(statusErrorStyle);
		
		// text field properties
		Comment("Text field attributes.");
		EditorGUILayout.PropertyField(textfieldWidth);
		EditorGUILayout.PropertyField(textfieldHeight);
		EditorGUILayout.PropertyField(textFieldSpacing);

		// button properties
		Comment("Button attributes.");
		EditorGUILayout.PropertyField(buttonWidth);
		EditorGUILayout.PropertyField(buttonHeight);
		EditorGUILayout.PropertyField(spacingAboveButtons);
		EditorGUILayout.PropertyField(spacingBetweenButtons);
		
		Comment("Miscellaneous attributes.");
		EditorGUILayout.PropertyField(verticalOffset);
	}
}
