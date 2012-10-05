using UnityEngine;
using UnityEditor;
using System.Collections;

public abstract class RoarUIInspector : RoarInspector
{
	protected SerializedProperty customGUISkin;
	protected SerializedProperty constrainToBounds;
	protected SerializedProperty bounds;
	
	// background properties
	protected SerializedProperty backgroundType;
	protected SerializedProperty backgroundColor;
	protected SerializedProperty backgroundImage;
	protected SerializedProperty backgroundStyle;
	protected SerializedProperty extendedBackgroundWidth;
	protected SerializedProperty extendedBackgroundHeight;

	protected override void OnEnable()
	{
		base.OnEnable();
		customGUISkin = serializedObject.FindProperty("customGUISkin");
		constrainToBounds = serializedObject.FindProperty("constrainToBounds");
		bounds = serializedObject.FindProperty("bounds");
		backgroundType = serializedObject.FindProperty("backgroundType");
		backgroundColor = serializedObject.FindProperty("backgroundColor");
		backgroundImage = serializedObject.FindProperty("backgroundImage");
		backgroundStyle = serializedObject.FindProperty("backgroundStyle");
		extendedBackgroundWidth = serializedObject.FindProperty("extendedBackgroundWidth");
		extendedBackgroundHeight = serializedObject.FindProperty("extendedBackgroundHeight");
	}

	protected override void DrawGUI()
	{
		// customGUISkin
		EditorGUILayout.Space();
		Comment("You can specify custom skin for the user interface. Otherwise, the default skin will be used.");
		EditorGUILayout.PropertyField(customGUISkin);
		
		// constrainToBounds
		bool isController = (this is RoarUIControllerInspector);
		if (!isController)
		{
			Comment("UI boundary constraint.");
			EditorGUILayout.PropertyField(constrainToBounds);
			if (constrainToBounds.boolValue)
			{
				EditorGUILayout.PropertyField(bounds);
				Rect rect = bounds.rectValue;
				if (rect.width < 0) rect.width = 0;
				if (rect.height < 0) rect.height = 0;
				bounds.rectValue = rect;
			}
		}
		
		// background
		EditorGUILayout.Space();
		Comment("Background properties.");
		EditorGUILayout.PropertyField(backgroundType);
		switch (backgroundType.enumValueIndex)
		{
		case 1: // SolidColor
			EditorGUILayout.PropertyField(backgroundColor);
			backgroundImage.objectReferenceValue = null;
			break;
		case 2: // Image
			EditorGUILayout.PropertyField(backgroundImage);
			if (backgroundImage.objectReferenceValue == null)
				EditorGUILayout.HelpBox("Please set a background image.", MessageType.Warning);
			break;
		case 3: // ExtentedImage
			EditorGUILayout.PropertyField(backgroundStyle);
			EditorGUILayout.PropertyField(backgroundColor);
			EditorGUILayout.PropertyField(extendedBackgroundWidth, new GUIContent("Width"));
			EditorGUILayout.PropertyField(extendedBackgroundHeight, new GUIContent("Height"));
			backgroundImage.objectReferenceValue = null;
			break;
		default:
			backgroundImage.objectReferenceValue = null;
			break;
		}
	}
}
